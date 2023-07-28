using System;
using System.Xml;
using DedicatedPoolServer.Model;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.ServerInfo;
using Util.Common;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000611 RID: 1553
	[RoomExtension]
	internal class SessionExtension : RoomExtensionBase
	{
		// Token: 0x0600213E RID: 8510 RVA: 0x0008896A File Offset: 0x00086D6A
		public SessionExtension(IQueryManager queryManager)
		{
			this.m_queryManager = queryManager;
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x0600213F RID: 8511 RVA: 0x00088979 File Offset: 0x00086D79
		public string SessionID
		{
			get
			{
				return base.Room.GetState<SessionState>(AccessMode.ReadOnly).SessionID;
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06002140 RID: 8512 RVA: 0x0008898C File Offset: 0x00086D8C
		public SessionStatus Status
		{
			get
			{
				return base.Room.GetState<SessionState>(AccessMode.ReadOnly).Status;
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06002141 RID: 8513 RVA: 0x000889A0 File Offset: 0x00086DA0
		public bool Started
		{
			get
			{
				SessionStatus status = this.Status;
				return status == SessionStatus.Starting || status == SessionStatus.Running || status == SessionStatus.Finishing;
			}
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06002142 RID: 8514 RVA: 0x000889C9 File Offset: 0x00086DC9
		// (set) Token: 0x06002143 RID: 8515 RVA: 0x000889DC File Offset: 0x00086DDC
		public float GameProgress
		{
			get
			{
				return base.Room.GetState<SessionState>(AccessMode.ReadOnly).GameProgress;
			}
			set
			{
				base.Room.GetState<SessionState>(AccessMode.ReadWrite).GameProgress = value;
			}
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06002144 RID: 8516 RVA: 0x000889F0 File Offset: 0x00086DF0
		public DateTime SessionStartTime
		{
			get
			{
				return base.Room.GetState<SessionState>(AccessMode.ReadOnly).SessionStartTime;
			}
		}

		// Token: 0x1400008A RID: 138
		// (add) Token: 0x06002145 RID: 8517 RVA: 0x00088A04 File Offset: 0x00086E04
		// (remove) Token: 0x06002146 RID: 8518 RVA: 0x00088A3C File Offset: 0x00086E3C
		internal event SessionExtension.TrOnSessionCanStart tr_session_can_start;

		// Token: 0x1400008B RID: 139
		// (add) Token: 0x06002147 RID: 8519 RVA: 0x00088A74 File Offset: 0x00086E74
		// (remove) Token: 0x06002148 RID: 8520 RVA: 0x00088AAC File Offset: 0x00086EAC
		internal event SessionExtension.TrOnSessionStarting tr_session_starting;

		// Token: 0x1400008C RID: 140
		// (add) Token: 0x06002149 RID: 8521 RVA: 0x00088AE4 File Offset: 0x00086EE4
		// (remove) Token: 0x0600214A RID: 8522 RVA: 0x00088B1C File Offset: 0x00086F1C
		internal event SessionExtension.TrOnSessionStartFailed tr_session_start_failed;

		// Token: 0x1400008D RID: 141
		// (add) Token: 0x0600214B RID: 8523 RVA: 0x00088B54 File Offset: 0x00086F54
		// (remove) Token: 0x0600214C RID: 8524 RVA: 0x00088B8C File Offset: 0x00086F8C
		internal event SessionExtension.TrOnSessionStarted tr_session_started;

		// Token: 0x1400008E RID: 142
		// (add) Token: 0x0600214D RID: 8525 RVA: 0x00088BC4 File Offset: 0x00086FC4
		// (remove) Token: 0x0600214E RID: 8526 RVA: 0x00088BFC File Offset: 0x00086FFC
		internal event SessionExtension.TrOnSessionEnded tr_session_ended;

		// Token: 0x1400008F RID: 143
		// (add) Token: 0x0600214F RID: 8527 RVA: 0x00088C34 File Offset: 0x00087034
		// (remove) Token: 0x06002150 RID: 8528 RVA: 0x00088C6C File Offset: 0x0008706C
		public event SessionExtension.OnSessionStartedDeleg SessionStarted;

		// Token: 0x14000090 RID: 144
		// (add) Token: 0x06002151 RID: 8529 RVA: 0x00088CA4 File Offset: 0x000870A4
		// (remove) Token: 0x06002152 RID: 8530 RVA: 0x00088CDC File Offset: 0x000870DC
		public event SessionExtension.OnSessionEndedDeleg SessionEnded;

		// Token: 0x06002153 RID: 8531 RVA: 0x00088D14 File Offset: 0x00087114
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			extension.tr_server_bound += this.TrServerBound;
			extension.tr_server_unbound += this.TrServerUnbound;
			extension.tr_server_bind_failed += this.TrServerBindFailed;
			extension.tr_server_changed += this.TrServerChanged;
		}

		// Token: 0x06002154 RID: 8532 RVA: 0x00088D7C File Offset: 0x0008717C
		protected override void OnDisposing()
		{
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			extension.tr_server_bound -= this.TrServerBound;
			extension.tr_server_unbound -= this.TrServerUnbound;
			extension.tr_server_bind_failed -= this.TrServerBindFailed;
			extension.tr_server_changed -= this.TrServerChanged;
			this.tr_session_can_start = null;
			this.tr_session_starting = null;
			this.tr_session_start_failed = null;
			this.tr_session_started = null;
			this.tr_session_ended = null;
			this.SessionStarted = null;
			this.SessionEnded = null;
			base.OnDisposing();
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x00088E14 File Offset: 0x00087214
		private string GenerateSessionID()
		{
			ulong num = (ulong)((long)Resources.ServerID);
			num = (num << 16) + (ulong)((ushort)base.Room.ID);
			num = (num << 8) + (ulong)((byte)(++this.m_session_counter));
			return ((num << 32) + (ulong)((uint)TimeUtils.LocalTimeToUTCTimestamp(DateTime.Now))).ToString();
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x00088E74 File Offset: 0x00087274
		private GameRoomRetCode CanStartSession()
		{
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadWrite);
			if (!state.CanStart)
			{
				return GameRoomRetCode.NOT_READY;
			}
			foreach (Delegate @delegate in this.tr_session_can_start.GetInvocationList())
			{
				GameRoomRetCode gameRoomRetCode = ((SessionExtension.TrOnSessionCanStart)@delegate)();
				if (gameRoomRetCode != GameRoomRetCode.OK)
				{
					return gameRoomRetCode;
				}
			}
			return GameRoomRetCode.OK;
		}

		// Token: 0x06002157 RID: 8535 RVA: 0x00088ED8 File Offset: 0x000872D8
		public GameRoomRetCode StartSession(string server_id)
		{
			GameRoomRetCode gameRoomRetCode = this.CanStartSession();
			if (gameRoomRetCode != GameRoomRetCode.OK)
			{
				return gameRoomRetCode;
			}
			SessionState state = base.Room.GetState<SessionState>(AccessMode.ReadWrite);
			state.Clear();
			state.Status = SessionStatus.Starting;
			state.SessionStartTime = DateTime.UtcNow;
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			gameRoomRetCode = extension.RequestServer(server_id, this.GenerateSessionID());
			if (gameRoomRetCode != GameRoomRetCode.OK)
			{
				state.Status = SessionStatus.Failed;
				if (this.tr_session_start_failed != null)
				{
					this.tr_session_start_failed();
				}
				return gameRoomRetCode;
			}
			if (this.tr_session_starting != null)
			{
				this.tr_session_starting();
			}
			return GameRoomRetCode.OK;
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x00088F70 File Offset: 0x00087370
		public void StopSession()
		{
			if (!this.Started)
			{
				return;
			}
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			this.m_queryManager.Request("stop_game_session", extension.ServerOnlineID, new object[]
			{
				this.SessionID
			});
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x00088FBC File Offset: 0x000873BC
		public void PauseSession()
		{
			if (!this.Started)
			{
				return;
			}
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			this.m_queryManager.Request("pause_game_session", extension.ServerOnlineID, new object[]
			{
				this.SessionID
			});
		}

		// Token: 0x0600215A RID: 8538 RVA: 0x00089008 File Offset: 0x00087408
		public void ResumeSession()
		{
			if (!this.Started)
			{
				return;
			}
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			this.m_queryManager.Request("resume_game_session", extension.ServerOnlineID, new object[]
			{
				this.SessionID
			});
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x00089054 File Offset: 0x00087454
		public void RewardsReceived(string session_id)
		{
			SessionState state = base.Room.GetState<SessionState>(AccessMode.ReadWrite);
			if (string.IsNullOrEmpty(state.SessionID) || state.SessionID != session_id)
			{
				throw new ApplicationException("Invalid session");
			}
			if (state.RewardsProcessed)
			{
				throw new ApplicationException("Rewards already processed");
			}
			state.RewardsProcessed = true;
			Log.Info<ulong, string>("Room {0} session '{1}' rewards processed", base.Room.ID, session_id);
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x000890D0 File Offset: 0x000874D0
		private void TrServerBound(ServerEntity server)
		{
			SessionState state = base.Room.GetState<SessionState>(AccessMode.ReadWrite);
			state.Status = SessionStatus.Running;
			state.SessionID = server.SessionID;
			Log.Info<ulong, string>("Room {0} starts session '{1}'", base.Room.ID, state.SessionID);
			if (this.tr_session_started != null)
			{
				this.tr_session_started(state.SessionID);
			}
		}

		// Token: 0x0600215D RID: 8541 RVA: 0x00089134 File Offset: 0x00087534
		private void TrServerBindFailed(ServerEntity server)
		{
			SessionState state = base.Room.GetState<SessionState>(AccessMode.ReadWrite);
			state.Status = SessionStatus.Failed;
			if (this.tr_session_start_failed != null)
			{
				this.tr_session_start_failed();
			}
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x0008916C File Offset: 0x0008756C
		private void TrServerChanged(ServerEntity server)
		{
			SessionState state = base.Room.GetState<SessionState>(AccessMode.ReadWrite);
			if (state.Status == SessionStatus.Running && (server.Status == EGameServerStatus.PostGame || server.Status == EGameServerStatus.Finished || server.Status == EGameServerStatus.Quiting))
			{
				state.Status = SessionStatus.Finishing;
			}
		}

		// Token: 0x0600215F RID: 8543 RVA: 0x000891BD File Offset: 0x000875BD
		private void TrServerUnbound(ServerEntity server, bool isDeleted)
		{
			this.ProcessSessionEnd(isDeleted);
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x000891C8 File Offset: 0x000875C8
		private void ProcessSessionEnd(bool abnormal)
		{
			SessionState state = base.Room.GetState<SessionState>(AccessMode.ReadWrite);
			string sessionID = state.SessionID;
			state.Clear();
			state.EndedAbnormally = abnormal;
			Log.Info<ulong, string>("Room {0} session ended '{1}'", base.Room.ID, sessionID);
			if (this.tr_session_ended != null)
			{
				this.tr_session_ended(sessionID, abnormal);
			}
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x00089224 File Offset: 0x00087624
		public override XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			SessionState state = (SessionState)ctx.new_state;
			XmlElement xmlElement = ctx.factory.CreateElement("session");
			SessionExtension.SerializeSessionState(xmlElement, state, true);
			return xmlElement;
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x00089258 File Offset: 0x00087658
		public static void SerializeSessionState(XmlElement el, SessionState state, bool include_id)
		{
			if (include_id)
			{
				el.SetAttribute("id", state.SessionID);
			}
			string name = "status";
			int status = (int)state.Status;
			el.SetAttribute(name, status.ToString());
			el.SetAttribute("game_progress", state.GameProgress.ToString());
			el.SetAttribute("start_time", TimeUtils.LocalTimeToUTCTimestamp(state.SessionStartTime).ToString());
			el.SetAttribute("team1_start_score", state.Team1StartScore.ToString());
			el.SetAttribute("team2_start_score", state.Team2StartScore.ToString());
		}

		// Token: 0x06002163 RID: 8547 RVA: 0x00089314 File Offset: 0x00087714
		public override void PostStateChanged(IRoomState new_state, IRoomState old_state)
		{
			SessionState sessionState = (SessionState)old_state;
			SessionState sessionState2 = (SessionState)new_state;
			if (this.SessionStarted != null && !string.IsNullOrEmpty(sessionState2.SessionID) && string.IsNullOrEmpty(sessionState.SessionID))
			{
				this.SessionStarted(base.Room, sessionState2.SessionID);
			}
			if (this.SessionEnded != null && string.IsNullOrEmpty(sessionState2.SessionID) && !string.IsNullOrEmpty(sessionState.SessionID))
			{
				this.SessionEnded(base.Room, sessionState.SessionID, sessionState2.EndedAbnormally);
			}
		}

		// Token: 0x04001034 RID: 4148
		private int m_session_counter;

		// Token: 0x04001035 RID: 4149
		private readonly IQueryManager m_queryManager;

		// Token: 0x02000612 RID: 1554
		// (Invoke) Token: 0x06002165 RID: 8549
		internal delegate GameRoomRetCode TrOnSessionCanStart();

		// Token: 0x02000613 RID: 1555
		// (Invoke) Token: 0x06002169 RID: 8553
		internal delegate void TrOnSessionStarting();

		// Token: 0x02000614 RID: 1556
		// (Invoke) Token: 0x0600216D RID: 8557
		internal delegate void TrOnSessionStartFailed();

		// Token: 0x02000615 RID: 1557
		// (Invoke) Token: 0x06002171 RID: 8561
		internal delegate void TrOnSessionStarted(string session_id);

		// Token: 0x02000616 RID: 1558
		// (Invoke) Token: 0x06002175 RID: 8565
		internal delegate void TrOnSessionEnded(string session_id, bool abnormal);

		// Token: 0x02000617 RID: 1559
		// (Invoke) Token: 0x06002179 RID: 8569
		public delegate void OnSessionStartedDeleg(IGameRoom room, string session_id);

		// Token: 0x02000618 RID: 1560
		// (Invoke) Token: 0x0600217D RID: 8573
		public delegate void OnSessionEndedDeleg(IGameRoom room, string session_id, bool abnormal);
	}
}
