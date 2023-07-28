using System;
using System.Collections.Generic;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;
using Ninject;

namespace MasterServer.XMPP
{
	// Token: 0x02000812 RID: 2066
	[Service]
	[Singleton]
	internal class ServerPresenceNotifier : ServiceModule, IServerPresenceNotifier
	{
		// Token: 0x06002A57 RID: 10839 RVA: 0x000B6888 File Offset: 0x000B4C88
		public ServerPresenceNotifier(IOnlineClient onlineClient, IUserRepository userRepository, IRankSystem rankSystem, IGameRoomManager gameRoomManager, [Named("NonReentrant")] ITimerFactory timerFactory, IQueryManager queryManager, IQoSQueue qosQueue)
		{
			this.PresenceEnabled = true;
			this.m_onlineClient = onlineClient;
			this.m_userRepository = userRepository;
			this.m_rankSystem = rankSystem;
			this.m_gameRoomManager = gameRoomManager;
			this.m_timerFactory = timerFactory;
			this.m_queryManager = queryManager;
			this.m_qosQueue = qosQueue;
			this.LoadStats = new ServerLoadStats();
			ConfigSection section = Resources.QoSSettings.GetSection("limits");
			section.Get("max_load_threshold", out this.m_max_load_threshold);
			section.Get("cooldown_threshold", out this.m_cooldown_threshold);
			section.Get("max_online_users", out this.m_max_online_users);
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_onlineClient.ConnectionStateChanged += this.OnConnectionStateChanged;
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06002A58 RID: 10840 RVA: 0x000B6959 File Offset: 0x000B4D59
		// (set) Token: 0x06002A59 RID: 10841 RVA: 0x000B6961 File Offset: 0x000B4D61
		public ServerLoadStats LoadStats { get; private set; }

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06002A5A RID: 10842 RVA: 0x000B696A File Offset: 0x000B4D6A
		// (set) Token: 0x06002A5B RID: 10843 RVA: 0x000B6972 File Offset: 0x000B4D72
		public bool PresenceEnabled { get; set; }

		// Token: 0x06002A5C RID: 10844 RVA: 0x000B697C File Offset: 0x000B4D7C
		private void OnConnectionStateChanged(EConnectionState prev, EConnectionState current)
		{
			if (current == EConnectionState.Connected)
			{
				object querySync = this.m_querySync;
				lock (querySync)
				{
					this.StartTracking();
					this.SendPresence();
				}
			}
			else if (current == EConnectionState.Disconnecting)
			{
				object querySync2 = this.m_querySync;
				lock (querySync2)
				{
					this.SendShutdownPresence();
					this.StopTracking();
				}
			}
			else if (current == EConnectionState.Disconnected)
			{
				object querySync3 = this.m_querySync;
				lock (querySync3)
				{
					this.StopTracking();
				}
			}
		}

		// Token: 0x06002A5D RID: 10845 RVA: 0x000B6A54 File Offset: 0x000B4E54
		private void StartTracking()
		{
			this.StopTracking();
			if (Resources.Channel != Resources.ChannelType.Service)
			{
				this.m_eventCount = 0;
				this.m_lastSend = DateTime.MinValue;
				this.m_toJid = "k01." + this.m_onlineClient.XmppHost;
				this.m_in_cooldown = false;
				this.m_userRepository.UserLoggedIn += this.OnUserLoggedIn;
				this.m_userRepository.UserLoggedOut += this.OnUserLoggedOut;
				this.m_timer = this.m_timerFactory.CreateTimer(new TimerCallback(this.OnTickPresence), null, ServerPresenceNotifier.PRESENCE_TICK_TIMEOUT);
			}
		}

		// Token: 0x06002A5E RID: 10846 RVA: 0x000B6AFC File Offset: 0x000B4EFC
		private void StopTracking()
		{
			if (Resources.Channel != Resources.ChannelType.Service)
			{
				if (this.m_timer != null)
				{
					this.m_timer.Dispose();
					this.m_timer = null;
				}
				this.m_userRepository.UserLoggedIn -= this.OnUserLoggedIn;
				this.m_userRepository.UserLoggedOut -= this.OnUserLoggedOut;
			}
		}

		// Token: 0x06002A5F RID: 10847 RVA: 0x000B6B60 File Offset: 0x000B4F60
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (string.Equals(args.Name, "max_load_threshold", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_max_load_threshold = float.Parse(args.sValue);
				this.SendPresence();
			}
			else if (string.Equals(args.Name, "cooldown_threshold", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_cooldown_threshold = float.Parse(args.sValue);
				this.m_in_cooldown = false;
				this.SendPresence();
			}
			else if (string.Equals(args.Name, "max_online_users", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_max_online_users = args.iValue;
				this.SendPresence();
			}
		}

		// Token: 0x06002A60 RID: 10848 RVA: 0x000B6C00 File Offset: 0x000B5000
		public void SendPresence()
		{
			if (Resources.Channel == Resources.ChannelType.Service || !this.PresenceEnabled)
			{
				return;
			}
			this.m_lastSend = DateTime.Now;
			this.m_eventCount = 0;
			Resources.ChannelRankGroup channelRankGroup = this.m_rankSystem.ChannelRankGroup;
			ServerLoadStats serverLoadStats = this.CalculateLoadStats();
			this.m_queryManager.Request("masterserver_presence", this.m_toJid, new object[]
			{
				channelRankGroup,
				"available",
				serverLoadStats
			});
		}

		// Token: 0x06002A61 RID: 10849 RVA: 0x000B6C7C File Offset: 0x000B507C
		public void SendShutdownPresence()
		{
			Resources.ChannelRankGroup channelRankGroup = this.m_rankSystem.ChannelRankGroup;
			this.m_queryManager.Request("masterserver_presence", this.m_toJid, new object[]
			{
				channelRankGroup,
				"shutdown",
				new ServerLoadStats()
			});
		}

		// Token: 0x06002A62 RID: 10850 RVA: 0x000B6CCC File Offset: 0x000B50CC
		private void OnTickPresence(object dummy)
		{
			if (DateTime.Now - this.m_lastSend >= ServerPresenceNotifier.PRESENCE_SEND_INTERVAL)
			{
				object querySync = this.m_querySync;
				lock (querySync)
				{
					if (this.m_timer != null)
					{
						this.SendPresence();
					}
				}
			}
		}

		// Token: 0x06002A63 RID: 10851 RVA: 0x000B6D40 File Offset: 0x000B5140
		private void OnUserLoggedIn(UserInfo.User user, ELoginType loginType)
		{
			this.OnPopulationChanged();
		}

		// Token: 0x06002A64 RID: 10852 RVA: 0x000B6D48 File Offset: 0x000B5148
		private void OnUserLoggedOut(UserInfo.User user, ELogoutType logoutType)
		{
			this.OnPopulationChanged();
		}

		// Token: 0x06002A65 RID: 10853 RVA: 0x000B6D50 File Offset: 0x000B5150
		private void OnPopulationChanged()
		{
			object querySync = this.m_querySync;
			lock (querySync)
			{
				this.m_eventCount++;
				if (this.m_eventCount >= 25)
				{
					this.SendPresence();
				}
			}
		}

		// Token: 0x06002A66 RID: 10854 RVA: 0x000B6DB4 File Offset: 0x000B51B4
		private ServerLoadStats CalculateLoadStats()
		{
			int onlineUsersCount = this.m_userRepository.GetOnlineUsersCount();
			int num = onlineUsersCount + OnlineUsersShaper.GetLoginsInProgress(this.m_qosQueue);
			float num2;
			float item;
			float item2;
			float item3;
			if (this.m_max_online_users == 0)
			{
				num2 = ServerPresenceNotifier.MAX_LOAD;
				item = 1f;
				item2 = 1f;
				item3 = 1f;
			}
			else
			{
				float num3 = (float)this.m_max_online_users * this.m_max_load_threshold;
				float num4 = (float)this.m_max_online_users * this.m_cooldown_threshold;
				if (!this.m_in_cooldown && (float)num >= num3)
				{
					this.m_in_cooldown = true;
				}
				if (this.m_in_cooldown && (float)num < num4)
				{
					this.m_in_cooldown = false;
				}
				num2 = (float)num / num3;
				if (this.m_in_cooldown)
				{
					num2 += 1f;
				}
				num2 = Math.Min(num2, ServerPresenceNotifier.MAX_LOAD);
				int num5 = this.m_gameRoomManager.TotalPlayersCount((IGameRoom r) => r.Type.IsPvpAutoStartMode());
				item = Math.Min((float)num5 / num3, 1f);
				int num6 = this.m_gameRoomManager.TotalPlayersCount(delegate(IGameRoom room)
				{
					bool isSurvivalAutostart = false;
					room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
					{
						isSurvivalAutostart = (r.MissionType.IsSurvival() && r.Type.IsAutoStartMode());
					});
					return isSurvivalAutostart;
				});
				item2 = Math.Min((float)num6 / num3, 1f);
				int num7 = this.m_gameRoomManager.TotalPlayersCount((IGameRoom r) => r.Type.IsPveAutoStartMode()) - num6;
				item3 = Math.Min((float)num7 / num3, 1f);
			}
			List<Tuple<string, float>> loadStats = new List<Tuple<string, float>>
			{
				new Tuple<string, float>("quick_play", item),
				new Tuple<string, float>("survival", item2),
				new Tuple<string, float>("pve", item3)
			};
			ServerLoadStats serverLoadStats = new ServerLoadStats(onlineUsersCount, num2, loadStats, this.m_userRepository.GetOnlineUsersCountPerRegion());
			this.LoadStats = serverLoadStats;
			return serverLoadStats;
		}

		// Token: 0x04001699 RID: 5785
		private static readonly TimeSpan PRESENCE_TICK_TIMEOUT = new TimeSpan(0, 0, 5);

		// Token: 0x0400169A RID: 5786
		private static readonly TimeSpan PRESENCE_SEND_INTERVAL = new TimeSpan(0, 0, 30);

		// Token: 0x0400169B RID: 5787
		private const int BUSY_EVENT_NUM = 25;

		// Token: 0x0400169C RID: 5788
		public static float MAX_LOAD = 2f;

		// Token: 0x0400169D RID: 5789
		private float m_max_load_threshold;

		// Token: 0x0400169E RID: 5790
		private float m_cooldown_threshold;

		// Token: 0x0400169F RID: 5791
		private bool m_in_cooldown;

		// Token: 0x040016A0 RID: 5792
		private int m_max_online_users;

		// Token: 0x040016A1 RID: 5793
		private string m_toJid;

		// Token: 0x040016A2 RID: 5794
		private DateTime m_lastSend;

		// Token: 0x040016A3 RID: 5795
		private volatile int m_eventCount;

		// Token: 0x040016A5 RID: 5797
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x040016A6 RID: 5798
		private readonly IUserRepository m_userRepository;

		// Token: 0x040016A7 RID: 5799
		private readonly IRankSystem m_rankSystem;

		// Token: 0x040016A8 RID: 5800
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x040016A9 RID: 5801
		private readonly ITimerFactory m_timerFactory;

		// Token: 0x040016AA RID: 5802
		private readonly IQueryManager m_queryManager;

		// Token: 0x040016AB RID: 5803
		private readonly IQoSQueue m_qosQueue;

		// Token: 0x040016AC RID: 5804
		private ITimer m_timer;

		// Token: 0x040016AD RID: 5805
		private readonly object m_querySync = new object();
	}
}
