using System;
using System.Collections.Generic;
using System.Threading;
using MasterServer.Core;
using MasterServer.Core.Timers;
using MasterServer.Database;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.Users;
using OLAPHypervisor;

namespace MasterServer.Telemetry
{
	// Token: 0x02000733 RID: 1843
	internal class UserPresenceTracker : IDisposable
	{
		// Token: 0x06002626 RID: 9766 RVA: 0x000A15E0 File Offset: 0x0009F9E0
		public UserPresenceTracker(TelemetryService service, IUserRepository userRepository, IPlayerStatsService playerStatsService, IServerRepository serverRepositry, IDALService dal)
		{
			this.m_dal = dal;
			this.m_serverRepository = serverRepositry;
			this.m_service = service;
			this.m_userRepository = userRepository;
			this.m_playerStatsService = playerStatsService;
			this.m_userPresence = new Dictionary<ulong, UserPresenceTracker.UserPresence>();
			this.m_pendings = new Dictionary<ulong, TimeSpan>();
			this.m_userRepository.UserLoggedIn += this.OnUserLoggedIn;
			this.m_userRepository.UserLoggedOut += this.OnUserLoggedOut;
			this.m_srv_online_schedule = Resources.ModuleSettings.Get("OnlineUsersSchedule");
			this.m_service.DeferredStream.RegisterMeasureCallback(this.m_srv_online_schedule, new MeasureCallback(this.ReportServerOnline));
			int num = int.Parse(Resources.ModuleSettings.Get("UserPresenceInterval"));
			this.m_userTimer = new SafeTimer(new TimerCallback(this.TickUserPresence), this, UserPresenceTracker.TICK_TIMEOUT, UserPresenceTracker.TICK_TIMEOUT);
		}

		// Token: 0x06002627 RID: 9767 RVA: 0x000A16CC File Offset: 0x0009FACC
		public void Dispose()
		{
			if (this.m_service.DeferredStream != null)
			{
				this.m_service.DeferredStream.UnregisterMeasureCallback(new MeasureCallback(this.ReportServerOnline));
			}
			this.m_userTimer.Dispose();
			this.m_userRepository.UserLoggedIn -= this.OnUserLoggedIn;
			this.m_userRepository.UserLoggedOut -= this.OnUserLoggedOut;
		}

		// Token: 0x06002628 RID: 9768 RVA: 0x000A1740 File Offset: 0x0009FB40
		private Measure? Report(ulong profileId, TimeSpan time)
		{
			if (!this.m_service.CheckMode(TelemetryMode.Presence))
			{
				return null;
			}
			return new Measure?(this.m_service.MakeMeasure(time.Ticks / 1000000L, new object[]
			{
				"stat",
				"player_online_time",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname,
				"profile",
				profileId
			}));
		}

		// Token: 0x06002629 RID: 9769 RVA: 0x000A17D0 File Offset: 0x0009FBD0
		private void TickUserPresence(object dummy)
		{
			object userPresence = this.m_userPresence;
			lock (userPresence)
			{
				DateTime now = DateTime.Now;
				List<Measure> list = new List<Measure>();
				foreach (KeyValuePair<ulong, UserPresenceTracker.UserPresence> keyValuePair in this.m_userPresence)
				{
					ulong key = keyValuePair.Key;
					UserPresenceTracker.UserPresence value = keyValuePair.Value;
					Measure? measure = this.Report(key, now - value.last_tick);
					if (measure != null)
					{
						list.Add(measure.Value);
					}
					value.last_tick = now;
				}
				foreach (KeyValuePair<ulong, TimeSpan> keyValuePair2 in this.m_pendings)
				{
					ulong key2 = keyValuePair2.Key;
					TimeSpan value2 = keyValuePair2.Value;
					Measure? measure2 = this.Report(key2, value2);
					if (measure2 != null)
					{
						list.Add(measure2.Value);
					}
				}
				this.m_service.AddMeasure(list);
				this.m_playerStatsService.UpdatePlayerStats(list);
				this.m_pendings.Clear();
			}
		}

		// Token: 0x0600262A RID: 9770 RVA: 0x000A1970 File Offset: 0x0009FD70
		private void OnUserLoggedIn(UserInfo.User user, ELoginType loginType)
		{
			object userPresence = this.m_userPresence;
			lock (userPresence)
			{
				this.m_userPresence[user.ProfileID] = new UserPresenceTracker.UserPresence
				{
					last_tick = DateTime.Now
				};
			}
		}

		// Token: 0x0600262B RID: 9771 RVA: 0x000A19D0 File Offset: 0x0009FDD0
		private void OnUserLoggedOut(UserInfo.User user, ELogoutType logout_type)
		{
			object userPresence = this.m_userPresence;
			lock (userPresence)
			{
				UserPresenceTracker.UserPresence userPresence2;
				if (!this.m_userPresence.TryGetValue(user.ProfileID, out userPresence2))
				{
					return;
				}
				this.m_userPresence.Remove(user.ProfileID);
				TimeSpan zero;
				if (!this.m_pendings.TryGetValue(user.ProfileID, out zero))
				{
					zero = TimeSpan.Zero;
				}
				this.m_pendings[user.ProfileID] = zero + (DateTime.Now - userPresence2.last_tick);
			}
			this.WriteLastSeenDate(user);
		}

		// Token: 0x0600262C RID: 9772 RVA: 0x000A1A88 File Offset: 0x0009FE88
		private void WriteLastSeenDate(UserInfo.User user)
		{
			this.m_dal.ProfileSystem.UpdateLastSeenDate(user.ProfileID);
		}

		// Token: 0x0600262D RID: 9773 RVA: 0x000A1AA0 File Offset: 0x0009FEA0
		private void ReportServerOnline(List<Measure> outMeasures, DateTime forDate)
		{
			if (!this.m_service.CheckMode(TelemetryMode.Realm))
			{
				return;
			}
			object userPresence = this.m_userPresence;
			lock (userPresence)
			{
				outMeasures.Add(this.m_service.MakeMeasure((long)this.m_userPresence.Count, new object[]
				{
					"stat",
					"srv_online_users",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname
				}));
				outMeasures.Add(this.m_service.MakeMeasure((long)this.m_userPresence.Count, new object[]
				{
					"stat",
					"srv_online_users_max",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname
				}));
			}
		}

		// Token: 0x04001398 RID: 5016
		private static TimeSpan TICK_TIMEOUT = new TimeSpan(0, 10, 0);

		// Token: 0x04001399 RID: 5017
		private readonly TelemetryService m_service;

		// Token: 0x0400139A RID: 5018
		private readonly IUserRepository m_userRepository;

		// Token: 0x0400139B RID: 5019
		private readonly IPlayerStatsService m_playerStatsService;

		// Token: 0x0400139C RID: 5020
		private readonly IDALService m_dal;

		// Token: 0x0400139D RID: 5021
		private readonly IServerRepository m_serverRepository;

		// Token: 0x0400139E RID: 5022
		private SafeTimer m_userTimer;

		// Token: 0x0400139F RID: 5023
		private Dictionary<ulong, UserPresenceTracker.UserPresence> m_userPresence;

		// Token: 0x040013A0 RID: 5024
		private Dictionary<ulong, TimeSpan> m_pendings;

		// Token: 0x040013A1 RID: 5025
		private string m_srv_online_schedule;

		// Token: 0x02000734 RID: 1844
		private class UserPresence
		{
			// Token: 0x040013A2 RID: 5026
			public DateTime last_tick;
		}
	}
}
