using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Timers;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x0200051A RID: 1306
	[RoomExtension]
	internal class AutoStartExtension : RoomExtensionBase
	{
		// Token: 0x06001C5E RID: 7262 RVA: 0x00071EA3 File Offset: 0x000702A3
		public AutoStartExtension(IGameModesSystem gameModesSystem)
		{
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06001C5F RID: 7263 RVA: 0x00071EB2 File Offset: 0x000702B2
		// (set) Token: 0x06001C60 RID: 7264 RVA: 0x00071EBA File Offset: 0x000702BA
		private TimeSpan IntermissionTimeout { get; set; }

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06001C61 RID: 7265 RVA: 0x00071EC3 File Offset: 0x000702C3
		// (set) Token: 0x06001C62 RID: 7266 RVA: 0x00071ECB File Offset: 0x000702CB
		private TimeSpan PostSessionTimeout { get; set; }

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06001C63 RID: 7267 RVA: 0x00071ED4 File Offset: 0x000702D4
		// (set) Token: 0x06001C64 RID: 7268 RVA: 0x00071EDC File Offset: 0x000702DC
		private IAutoStartState State { get; set; }

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06001C65 RID: 7269 RVA: 0x00071EE5 File Offset: 0x000702E5
		// (set) Token: 0x06001C66 RID: 7270 RVA: 0x00071EED File Offset: 0x000702ED
		private TimeSpan JoinedIntermissionTimeout { get; set; }

		// Token: 0x06001C67 RID: 7271 RVA: 0x00071EF8 File Offset: 0x000702F8
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			this.State = new AutoStartExtension.WaitingPlayersState(this);
			base.Room.tr_player_added += this.trOnPlayerAdded;
			base.Room.tr_player_removed += this.OnTrPlayerRemoved;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended += this.trOnSessionEnded;
			extension.tr_session_start_failed += this.trOnStartFailed;
			extension.tr_session_started += this.trOnSessionStarted;
			MissionExtension extension2 = base.Room.GetExtension<MissionExtension>();
			extension2.TrSetMissionInfoEnded += this.OnTrSetMissionInfoEnded;
		}

		// Token: 0x06001C68 RID: 7272 RVA: 0x00071FA8 File Offset: 0x000703A8
		public override void Close()
		{
			this.State.OnExitState();
			base.Room.tr_player_added -= this.trOnPlayerAdded;
			base.Room.tr_player_removed -= this.OnTrPlayerRemoved;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended -= this.trOnSessionEnded;
			extension.tr_session_start_failed -= this.trOnStartFailed;
			extension.tr_session_started -= this.trOnSessionStarted;
			MissionExtension extension2 = base.Room.GetExtension<MissionExtension>();
			extension2.TrSetMissionInfoEnded -= this.OnTrSetMissionInfoEnded;
			base.Close();
		}

		// Token: 0x06001C69 RID: 7273 RVA: 0x00072054 File Offset: 0x00070454
		public override XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			AutoStart autoStart = (AutoStart)ctx.new_state;
			XmlElement xmlElement = ctx.factory.CreateElement("auto_start");
			this.SerializeAutoStart(xmlElement, autoStart);
			return xmlElement;
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x00072087 File Offset: 0x00070487
		public void TriggerManualStart()
		{
			this.State.OnManualStart();
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x00072094 File Offset: 0x00070494
		private void trOnPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			this.State.OnPlayerAdded(profileId);
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x000720A2 File Offset: 0x000704A2
		private void OnTrPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			this.State.OnPlayerRemoved(player);
		}

		// Token: 0x06001C6D RID: 7277 RVA: 0x000720B0 File Offset: 0x000704B0
		private void trOnSessionEnded(string session_id, bool abnormal)
		{
			this.State.OnSessionEnded(session_id);
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x000720BE File Offset: 0x000704BE
		private void trOnStartFailed()
		{
			this.State.OnSessionStartFailed();
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x000720CB File Offset: 0x000704CB
		private void trOnSessionStarted(string session_id)
		{
			this.State.OnSessionStarted(session_id);
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x000720DC File Offset: 0x000704DC
		private void OnTrSetMissionInfoEnded(MissionContext mission)
		{
			GameModeSetting gameModeSetting = this.m_gameModesSystem.GetGameModeSetting(mission);
			int num;
			gameModeSetting.GetSetting(base.Room.Type, ERoomSetting.AUTOSTART_INTERMISSION_TIMEOUT_SEC, out num);
			this.IntermissionTimeout = TimeSpan.FromSeconds((double)num);
			gameModeSetting.GetSetting(base.Room.Type, ERoomSetting.AUTOSTART_POST_SESSION_TIMEOUT_SEC, out num);
			this.PostSessionTimeout = TimeSpan.FromSeconds((double)num);
			gameModeSetting.GetSetting(base.Room.Type, ERoomSetting.AUTOSTART_JOINED_INTERMISSION_TIMEOUT_SEC, out num);
			this.JoinedIntermissionTimeout = TimeSpan.FromSeconds((double)num);
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x0007215C File Offset: 0x0007055C
		private void SerializeAutoStart(XmlElement el, AutoStart autoStart)
		{
			el.SetAttribute("auto_start_timeout", (!autoStart.IsIntermission) ? "0" : "1");
			el.SetAttribute("auto_start_timeout_left", Math.Max((int)(autoStart.IntermissionEnd - DateTime.UtcNow).TotalSeconds, 0).ToString());
			el.SetAttribute("can_manual_start", (!autoStart.CanManualStart) ? "0" : "1");
			el.SetAttribute("joined_intermission_timeout", this.JoinedIntermissionTimeout.TotalSeconds.ToString());
		}

		// Token: 0x04000D93 RID: 3475
		private readonly IGameModesSystem m_gameModesSystem;

		// Token: 0x0200051B RID: 1307
		private class BaseState : IAutoStartState
		{
			// Token: 0x06001C72 RID: 7282 RVA: 0x00072212 File Offset: 0x00070612
			protected BaseState(AutoStartExtension owner)
			{
				this.m_owner = owner;
			}

			// Token: 0x06001C73 RID: 7283 RVA: 0x00072224 File Offset: 0x00070624
			public virtual void OnPlayerAdded(ulong profileID)
			{
				RoomPlayer player = this.m_owner.Room.GetPlayer(profileID);
				player.RoomStatus = RoomPlayer.EStatus.Ready;
			}

			// Token: 0x06001C74 RID: 7284 RVA: 0x0007224A File Offset: 0x0007064A
			public virtual void OnPlayerRemoved(RoomPlayer player)
			{
			}

			// Token: 0x06001C75 RID: 7285 RVA: 0x0007224C File Offset: 0x0007064C
			public virtual void OnSessionStarted(string session_id)
			{
			}

			// Token: 0x06001C76 RID: 7286 RVA: 0x0007224E File Offset: 0x0007064E
			public virtual void OnSessionEnded(string session_id)
			{
			}

			// Token: 0x06001C77 RID: 7287 RVA: 0x00072250 File Offset: 0x00070650
			public virtual void OnSessionStartFailed()
			{
			}

			// Token: 0x06001C78 RID: 7288 RVA: 0x00072252 File Offset: 0x00070652
			public virtual void OnManualStart()
			{
			}

			// Token: 0x06001C79 RID: 7289 RVA: 0x00072254 File Offset: 0x00070654
			public virtual void OnEnterState()
			{
				Log.Info<ulong, string>("[AutoStart] {0} on enter state '{1}'", this.m_owner.Room.ID, base.GetType().Name);
			}

			// Token: 0x06001C7A RID: 7290 RVA: 0x0007227B File Offset: 0x0007067B
			public virtual void OnExitState()
			{
				Log.Info<ulong, string>("[AutoStart] {0} on exit state '{1}'", this.m_owner.Room.ID, base.GetType().Name);
			}

			// Token: 0x06001C7B RID: 7291 RVA: 0x000722A2 File Offset: 0x000706A2
			protected void DoTransition(IAutoStartState newState)
			{
				this.m_owner.State.OnExitState();
				this.m_owner.State = newState;
				this.m_owner.State.OnEnterState();
			}

			// Token: 0x06001C7C RID: 7292 RVA: 0x000722D0 File Offset: 0x000706D0
			protected bool IsEnoughPlayers()
			{
				bool flag = this.m_owner.Room.PlayerCount >= this.m_owner.Room.MinReadyPlayers;
				MissionState state = this.m_owner.Room.GetState<MissionState>(AccessMode.ReadOnly);
				if (!state.Mission.noTeamsMode && flag)
				{
					TeamExtension extension = this.m_owner.Room.GetExtension<TeamExtension>();
					flag = extension.IsEnoughPlayersToStart();
				}
				return flag;
			}

			// Token: 0x04000D94 RID: 3476
			protected readonly AutoStartExtension m_owner;
		}

		// Token: 0x0200051C RID: 1308
		private class WaitingPlayersState : AutoStartExtension.BaseState
		{
			// Token: 0x06001C7D RID: 7293 RVA: 0x00072344 File Offset: 0x00070744
			public WaitingPlayersState(AutoStartExtension owner) : base(owner)
			{
			}

			// Token: 0x06001C7E RID: 7294 RVA: 0x00072350 File Offset: 0x00070750
			public override void OnEnterState()
			{
				base.OnEnterState();
				this.m_owner.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					AutoStart state = r.GetState<AutoStart>(AccessMode.ReadWrite);
					state.IsIntermission = false;
					CoreState state2 = r.GetState<CoreState>(AccessMode.ReadWrite);
					state2.CanStart = false;
				});
				AutoStartExtension.BaseState newStateTr = this.GetNewStateTr();
				if (newStateTr != null)
				{
					base.DoTransition(newStateTr);
				}
			}

			// Token: 0x06001C7F RID: 7295 RVA: 0x000723A8 File Offset: 0x000707A8
			public override void OnPlayerAdded(ulong player)
			{
				base.OnPlayerAdded(player);
				AutoStartExtension.BaseState newState = this.GetNewState();
				if (newState != null)
				{
					base.DoTransition(newState);
				}
			}

			// Token: 0x06001C80 RID: 7296 RVA: 0x000723D0 File Offset: 0x000707D0
			public override void OnPlayerRemoved(RoomPlayer player)
			{
				base.OnPlayerRemoved(player);
				AutoStartExtension.BaseState newState = this.GetNewState();
				if (newState != null)
				{
					base.DoTransition(newState);
				}
			}

			// Token: 0x06001C81 RID: 7297 RVA: 0x000723F8 File Offset: 0x000707F8
			private AutoStartExtension.BaseState GetNewStateTr()
			{
				AutoStartExtension.BaseState newState = null;
				this.m_owner.Room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					newState = this.GetNewState();
				});
				return newState;
			}

			// Token: 0x06001C82 RID: 7298 RVA: 0x0007243C File Offset: 0x0007083C
			private AutoStartExtension.BaseState GetNewState()
			{
				bool flag = base.IsEnoughPlayers();
				if (!flag)
				{
					return null;
				}
				if (!this.m_owner.Room.GetState<AutoStart>(AccessMode.ReadOnly).CanManualStart)
				{
					return new AutoStartExtension.IntermissionState(this.m_owner);
				}
				return new AutoStartExtension.ManualStartState(this.m_owner);
			}
		}

		// Token: 0x0200051D RID: 1309
		private class ManualStartState : AutoStartExtension.BaseState
		{
			// Token: 0x06001C84 RID: 7300 RVA: 0x000724D2 File Offset: 0x000708D2
			public ManualStartState(AutoStartExtension owner) : base(owner)
			{
			}

			// Token: 0x06001C85 RID: 7301 RVA: 0x000724DB File Offset: 0x000708DB
			public override void OnManualStart()
			{
				base.DoTransition(new AutoStartExtension.IntermissionState(this.m_owner));
			}
		}

		// Token: 0x0200051E RID: 1310
		private class IntermissionState : AutoStartExtension.BaseState
		{
			// Token: 0x06001C86 RID: 7302 RVA: 0x000724EE File Offset: 0x000708EE
			public IntermissionState(AutoStartExtension owner) : base(owner)
			{
			}

			// Token: 0x06001C87 RID: 7303 RVA: 0x000724F8 File Offset: 0x000708F8
			public override void OnEnterState()
			{
				base.OnEnterState();
				try
				{
					this.m_owner.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						AutoStart state = r.GetState<AutoStart>(AccessMode.ReadWrite);
						state.IntermissionEnd = DateTime.UtcNow + this.m_owner.IntermissionTimeout;
						state.IsIntermission = true;
						CoreState state2 = r.GetState<CoreState>(AccessMode.ReadWrite);
						state2.CanStart = false;
						this.m_intermissionTimer = new SafeTimer(delegate(object _)
						{
							base.DoTransition(new AutoStartExtension.StartingSessionState(this.m_owner));
						}, null, this.m_owner.IntermissionTimeout, TimeSpan.Zero);
					});
				}
				catch (RoomClosedException ex)
				{
					Log.Info(ex.Message);
				}
			}

			// Token: 0x06001C88 RID: 7304 RVA: 0x00072550 File Offset: 0x00070950
			public override void OnExitState()
			{
				if (this.m_intermissionTimer != null)
				{
					this.m_intermissionTimer.Dispose();
					this.m_intermissionTimer = null;
				}
				base.OnExitState();
			}

			// Token: 0x04000D96 RID: 3478
			private SafeTimer m_intermissionTimer;
		}

		// Token: 0x0200051F RID: 1311
		private class StartingSessionState : AutoStartExtension.BaseState
		{
			// Token: 0x06001C8B RID: 7307 RVA: 0x000725F9 File Offset: 0x000709F9
			public StartingSessionState(AutoStartExtension owner) : base(owner)
			{
			}

			// Token: 0x06001C8C RID: 7308 RVA: 0x00072604 File Offset: 0x00070A04
			public override void OnEnterState()
			{
				base.OnEnterState();
				GameRoomRetCode ret = GameRoomRetCode.ERROR;
				try
				{
					this.m_owner.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						if (!this.IsEnoughPlayers())
						{
							return;
						}
						CoreState state = r.GetState<CoreState>(AccessMode.ReadWrite);
						state.CanStart = true;
						SessionExtension extension = r.GetExtension<SessionExtension>();
						ret = extension.StartSession(string.Empty);
					});
					if (ret != GameRoomRetCode.OK)
					{
						base.DoTransition(new AutoStartExtension.WaitingPlayersState(this.m_owner));
					}
				}
				catch (RoomClosedException ex)
				{
					Log.Info(ex.Message);
				}
			}

			// Token: 0x06001C8D RID: 7309 RVA: 0x0007268C File Offset: 0x00070A8C
			public override void OnSessionStartFailed()
			{
				base.OnSessionStartFailed();
				base.DoTransition(new AutoStartExtension.WaitingPlayersState(this.m_owner));
			}

			// Token: 0x06001C8E RID: 7310 RVA: 0x000726A5 File Offset: 0x00070AA5
			public override void OnSessionStarted(string session_id)
			{
				base.OnSessionStarted(session_id);
				base.DoTransition(new AutoStartExtension.SessionState(this.m_owner));
			}
		}

		// Token: 0x02000520 RID: 1312
		private class SessionState : AutoStartExtension.BaseState
		{
			// Token: 0x06001C8F RID: 7311 RVA: 0x0007270D File Offset: 0x00070B0D
			public SessionState(AutoStartExtension owner) : base(owner)
			{
			}

			// Token: 0x06001C90 RID: 7312 RVA: 0x00072718 File Offset: 0x00070B18
			public override void OnEnterState()
			{
				base.OnEnterState();
				AutoStart state = this.m_owner.Room.GetState<AutoStart>(AccessMode.ReadWrite);
				state.IsIntermission = false;
			}

			// Token: 0x06001C91 RID: 7313 RVA: 0x00072744 File Offset: 0x00070B44
			public override void OnSessionEnded(string session_id)
			{
				base.OnSessionEnded(session_id);
				base.DoTransition(new AutoStartExtension.PostSessionState(this.m_owner));
			}

			// Token: 0x06001C92 RID: 7314 RVA: 0x0007275E File Offset: 0x00070B5E
			public override void OnSessionStartFailed()
			{
				base.OnSessionStartFailed();
				base.DoTransition(new AutoStartExtension.WaitingPlayersState(this.m_owner));
			}
		}

		// Token: 0x02000521 RID: 1313
		private class PostSessionState : AutoStartExtension.BaseState
		{
			// Token: 0x06001C93 RID: 7315 RVA: 0x00072777 File Offset: 0x00070B77
			public PostSessionState(AutoStartExtension owner) : base(owner)
			{
			}

			// Token: 0x06001C94 RID: 7316 RVA: 0x00072780 File Offset: 0x00070B80
			public override void OnEnterState()
			{
				base.OnEnterState();
				this.m_owner.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					CoreState state = r.GetState<CoreState>(AccessMode.ReadWrite);
					state.CanStart = false;
					bool canManualStart = r.GetState<AutoStart>(AccessMode.ReadOnly).CanManualStart;
					this.m_waitingTimer = new SafeTimer(delegate(object _)
					{
						if (!canManualStart)
						{
							this.DoTransition(new AutoStartExtension.IntermissionState(this.m_owner));
						}
						else
						{
							this.DoTransition(new AutoStartExtension.ManualStartState(this.m_owner));
						}
					}, null, this.m_owner.PostSessionTimeout, TimeSpan.Zero);
				});
			}

			// Token: 0x06001C95 RID: 7317 RVA: 0x000727A5 File Offset: 0x00070BA5
			public override void OnExitState()
			{
				if (this.m_waitingTimer != null)
				{
					this.m_waitingTimer.Dispose();
					this.m_waitingTimer = null;
				}
				base.OnExitState();
			}

			// Token: 0x04000D97 RID: 3479
			private SafeTimer m_waitingTimer;
		}
	}
}
