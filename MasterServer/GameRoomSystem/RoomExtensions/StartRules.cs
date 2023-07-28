using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000619 RID: 1561
	[RoomExtension]
	internal class StartRules : RoomExtensionBase
	{
		// Token: 0x06002181 RID: 8577 RVA: 0x000893CC File Offset: 0x000877CC
		public override void Close()
		{
			base.Close();
			this.ResetTimer();
		}

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06002182 RID: 8578 RVA: 0x000893DC File Offset: 0x000877DC
		public TimeSpan MasterIdleWarningTimeout
		{
			get
			{
				int seconds;
				Resources.ModuleSettings.GetSection("GameRoom").Get("MasterIdleWarning", out seconds);
				return new TimeSpan(0, 0, seconds);
			}
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06002183 RID: 8579 RVA: 0x0008940C File Offset: 0x0008780C
		public TimeSpan MasterIdleChownTimeout
		{
			get
			{
				int seconds;
				Resources.ModuleSettings.GetSection("GameRoom").Get("MasterIdleChown", out seconds);
				return new TimeSpan(0, 0, seconds);
			}
		}

		// Token: 0x06002184 RID: 8580 RVA: 0x0008943C File Offset: 0x0008783C
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.tr_player_added += this.trOnPlayerAdded;
			base.Room.tr_player_removed += this.trOnPlayerRemoved;
			base.Room.tr_players_changed += this.trOnPlayersChanged;
			RoomMasterExtension extension = base.Room.GetExtension<RoomMasterExtension>();
			extension.tr_master_changed += this.trOnMasterChanged;
			SessionExtension extension2 = base.Room.GetExtension<SessionExtension>();
			extension2.tr_session_ended += this.trOnSessionEnded;
		}

		// Token: 0x06002185 RID: 8581 RVA: 0x000894D4 File Offset: 0x000878D4
		protected override void OnDisposing()
		{
			base.Room.tr_player_added -= this.trOnPlayerAdded;
			base.Room.tr_player_removed -= this.trOnPlayerRemoved;
			base.Room.tr_players_changed -= this.trOnPlayersChanged;
			RoomMasterExtension extension = base.Room.GetExtension<RoomMasterExtension>();
			extension.tr_master_changed -= this.trOnMasterChanged;
			SessionExtension extension2 = base.Room.GetExtension<SessionExtension>();
			extension2.tr_session_ended -= this.trOnSessionEnded;
			base.OnDisposing();
		}

		// Token: 0x06002186 RID: 8582 RVA: 0x00089568 File Offset: 0x00087968
		private void CheckAllReady()
		{
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadWrite);
			MissionState state2 = base.Room.GetState<MissionState>(AccessMode.ReadOnly);
			RoomMasterState rm_state = base.Room.GetState<RoomMasterState>(AccessMode.ReadOnly);
			TeamExtension extension = base.Room.GetExtension<TeamExtension>();
			int num = base.Room.Players.Count((RoomPlayer player) => player.RoomStatus == RoomPlayer.EStatus.Ready || player.ProfileID == rm_state.RoomMaster);
			bool flag = num == base.Room.PlayerCount;
			bool flag2 = num >= Math.Min(state.MinReadyPlayers, base.Room.MaxPlayers);
			bool flag3 = state2.Mission.noTeamsMode || extension.IsEnoughPlayersToStart();
			bool flag4 = flag2 && flag3;
			if (flag4 != state.CanStart)
			{
				Log.Verbose(Log.Group.GameRoom, "Room {0} is {1} to start session", new object[]
				{
					base.Room.ID,
					(!flag4) ? "NOT READY" : "READY"
				});
				state.CanStart = flag4;
			}
			this.ResetTimer();
			SessionExtension extension2 = base.Room.GetExtension<SessionExtension>();
			if (state.CanStart && state.Players.Count > 1 && flag && !extension2.Started)
			{
				this.StartTimer(new Action(this.WarnOwner), this.MasterIdleWarningTimeout);
			}
		}

		// Token: 0x06002187 RID: 8583 RVA: 0x000896D8 File Offset: 0x00087AD8
		private void WarnOwner()
		{
			RoomMasterState state = base.Room.GetState<RoomMasterState>(AccessMode.ReadOnly);
			RoomPlayer player = base.Room.GetPlayer(state.RoomMaster);
			if (player != null)
			{
				TimeSpan delay = this.MasterIdleChownTimeout - this.MasterIdleWarningTimeout;
				Log.Verbose(Log.Group.GameRoom, "Warning room {0} master for not starting the game", new object[]
				{
					base.Room.ID
				});
				QueryManager.RequestSt("gameroom_loosemaster", player.OnlineID, new object[]
				{
					(int)delay.TotalSeconds
				});
				this.ResetTimer();
				this.StartTimer(new Action(this.ChangeOwner), delay);
			}
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x00089780 File Offset: 0x00087B80
		private void ChangeOwner()
		{
			Log.Verbose(Log.Group.GameRoom, "Initiating room {0} master rotation by timeout", new object[]
			{
				base.Room.ID
			});
			RoomMasterExtension extension = base.Room.GetExtension<RoomMasterExtension>();
			extension.ChooseNewMaster();
		}

		// Token: 0x06002189 RID: 8585 RVA: 0x000897C4 File Offset: 0x00087BC4
		private void TimerTick(object state)
		{
			KeyValuePair<int, Action> s = (KeyValuePair<int, Action>)state;
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_timer == null)
				{
					return;
				}
			}
			try
			{
				base.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					if (s.Key != this.m_check_id)
					{
						return;
					}
					if (!r.CanStart || r.GetExtension<SessionExtension>().Started)
					{
						return;
					}
					s.Value();
				});
			}
			catch (RoomClosedException p)
			{
				Log.Info<RoomClosedException>("{0}", p);
			}
		}

		// Token: 0x0600218A RID: 8586 RVA: 0x00089864 File Offset: 0x00087C64
		private void ResetTimer()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_timer != null)
				{
					this.m_timer.Dispose();
					this.m_timer = null;
				}
			}
		}

		// Token: 0x0600218B RID: 8587 RVA: 0x000898C0 File Offset: 0x00087CC0
		private void StartTimer(Action clb, TimeSpan delay)
		{
			KeyValuePair<int, Action> keyValuePair = new KeyValuePair<int, Action>(++this.m_check_id, clb);
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_timer = new SafeTimer(new TimerCallback(this.TimerTick), keyValuePair, delay, new TimeSpan(-1L));
			}
		}

		// Token: 0x0600218C RID: 8588 RVA: 0x0008993C File Offset: 0x00087D3C
		private void trOnPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			this.CheckAllReady();
		}

		// Token: 0x0600218D RID: 8589 RVA: 0x00089944 File Offset: 0x00087D44
		private void trOnPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			this.CheckAllReady();
		}

		// Token: 0x0600218E RID: 8590 RVA: 0x0008994C File Offset: 0x00087D4C
		private void trOnPlayersChanged()
		{
			this.CheckAllReady();
		}

		// Token: 0x0600218F RID: 8591 RVA: 0x00089954 File Offset: 0x00087D54
		private void trOnMasterChanged(ulong old_master, ulong new_master)
		{
			this.CheckAllReady();
		}

		// Token: 0x06002190 RID: 8592 RVA: 0x0008995C File Offset: 0x00087D5C
		private void trOnSessionEnded(string session_id, bool abnormal)
		{
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadWrite);
			foreach (RoomPlayer roomPlayer in state.Players.Values)
			{
				if (roomPlayer.RoomStatus != RoomPlayer.EStatus.CantBeReady)
				{
					roomPlayer.RoomStatus = RoomPlayer.EStatus.NotReady;
				}
			}
		}

		// Token: 0x0400103D RID: 4157
		private readonly object m_lock = new object();

		// Token: 0x0400103E RID: 4158
		private int m_check_id;

		// Token: 0x0400103F RID: 4159
		private SafeTimer m_timer;
	}
}
