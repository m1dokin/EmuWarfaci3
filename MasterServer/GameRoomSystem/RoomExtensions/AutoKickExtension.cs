using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Timers;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005F2 RID: 1522
	[RoomExtension]
	internal class AutoKickExtension : RoomExtensionBase
	{
		// Token: 0x06002056 RID: 8278 RVA: 0x00082CE0 File Offset: 0x000810E0
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			this.m_idleTimeout = TimeSpan.FromSeconds(Math.Max(this.m_checkIdleTimeout.TotalSeconds, (double)int.Parse(section.Get("PlayerIdleTimeoutSec"))));
			this.m_kickIdleTimer = new SafeTimer(new TimerCallback(this.KickIdlePlayers), null, this.m_checkIdleTimeout, this.m_checkIdleTimeout);
			base.Room.tr_player_removed += this.OnPlayerRemoved;
			base.Room.tr_player_status += this.OnPlayerStatus;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended += this.OnSessionFinished;
		}

		// Token: 0x06002057 RID: 8279 RVA: 0x00082DA0 File Offset: 0x000811A0
		public override void Close()
		{
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended -= this.OnSessionFinished;
			base.Room.tr_player_removed -= this.OnPlayerRemoved;
			base.Room.tr_player_status -= this.OnPlayerStatus;
			this.StopTimer();
			base.Close();
		}

		// Token: 0x06002058 RID: 8280 RVA: 0x00082E05 File Offset: 0x00081205
		public void PauseSession()
		{
			this.StopTimer();
			this.m_sessionPausedTime = DateTime.Now;
		}

		// Token: 0x06002059 RID: 8281 RVA: 0x00082E18 File Offset: 0x00081218
		public void ResumeSession()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_sessionPauseLength = DateTime.Now - this.m_sessionPausedTime;
				Dictionary<ulong, DateTime> dictionary = new Dictionary<ulong, DateTime>();
				foreach (KeyValuePair<ulong, DateTime> keyValuePair in this.m_idlePlayers)
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value.Add(this.m_sessionPauseLength);
				}
				Interlocked.Exchange<Dictionary<ulong, DateTime>>(ref this.m_idlePlayers, dictionary);
				this.m_kickIdleTimer = new SafeTimer(new TimerCallback(this.KickIdlePlayers), null, this.m_checkIdleTimeout, this.m_checkIdleTimeout);
			}
		}

		// Token: 0x0600205A RID: 8282 RVA: 0x00082F0C File Offset: 0x0008130C
		private void KickIdlePlayers(object dummy)
		{
			DateTime now = DateTime.Now;
			List<ulong> toKick = new List<ulong>();
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_kickIdleTimer == null)
				{
					return;
				}
				toKick.AddRange(from kv in this.m_idlePlayers
				where now - kv.Value >= this.m_idleTimeout
				select kv.Key);
				foreach (ulong key in toKick)
				{
					this.m_idlePlayers.Remove(key);
				}
			}
			try
			{
				if (toKick.Any<ulong>())
				{
					base.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						foreach (ulong num in toKick)
						{
							r.RemovePlayer(num, GameRoomPlayerRemoveReason.KickTimeout);
							Log.Verbose(Log.Group.GameRoom, "Player {0} was kicked from room {1} for inactivity", new object[]
							{
								num,
								r.ID
							});
						}
					});
				}
			}
			catch (RoomClosedException p)
			{
				Log.Info<RoomClosedException>("{0}", p);
			}
		}

		// Token: 0x0600205B RID: 8283 RVA: 0x00083060 File Offset: 0x00081460
		private void OnPlayerStatus(ulong profileId, UserStatus old_status, UserStatus new_status)
		{
			base.Room.CheckAccessMode(AccessMode.ReadWrite);
			if (UserStatuses.IsAway(old_status) != UserStatuses.IsAway(new_status))
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					bool flag2 = this.m_idlePlayers.ContainsKey(profileId);
					if (!flag2 && UserStatuses.IsAway(new_status) && (UserStatuses.Check(new_status, UserStatus.InGameRoom) || UserStatuses.Check(new_status, UserStatus.InGame)))
					{
						this.m_idlePlayers.Add(profileId, DateTime.Now);
					}
					else if (flag2 && !UserStatuses.IsAway(new_status))
					{
						this.m_idlePlayers.Remove(profileId);
					}
				}
			}
		}

		// Token: 0x0600205C RID: 8284 RVA: 0x00083128 File Offset: 0x00081528
		private void OnPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_idlePlayers.Remove(player.ProfileID);
			}
		}

		// Token: 0x0600205D RID: 8285 RVA: 0x00083178 File Offset: 0x00081578
		private void OnSessionFinished(string sessionId, bool abnormal)
		{
			this.m_sessionPauseLength = TimeSpan.Zero;
			this.m_sessionPausedTime = DateTime.MinValue;
		}

		// Token: 0x0600205E RID: 8286 RVA: 0x00083190 File Offset: 0x00081590
		private void StopTimer()
		{
			if (this.m_kickIdleTimer != null)
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					if (this.m_kickIdleTimer != null)
					{
						this.m_kickIdleTimer.Dispose();
						this.m_kickIdleTimer = null;
					}
				}
			}
		}

		// Token: 0x04000FD4 RID: 4052
		private readonly object m_lock = new object();

		// Token: 0x04000FD5 RID: 4053
		private Dictionary<ulong, DateTime> m_idlePlayers = new Dictionary<ulong, DateTime>();

		// Token: 0x04000FD6 RID: 4054
		private TimeSpan m_checkIdleTimeout = TimeSpan.FromSeconds(5.0);

		// Token: 0x04000FD7 RID: 4055
		private TimeSpan m_idleTimeout;

		// Token: 0x04000FD8 RID: 4056
		private SafeTimer m_kickIdleTimer;

		// Token: 0x04000FD9 RID: 4057
		private TimeSpan m_sessionPauseLength;

		// Token: 0x04000FDA RID: 4058
		private DateTime m_sessionPausedTime;
	}
}
