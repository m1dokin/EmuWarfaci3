using System;
using DedicatedPoolServer.Model;
using MasterServer.Common;
using MasterServer.ServerInfo;
using NLog;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000608 RID: 1544
	[RoomExtension]
	internal class ServerExtension : RoomExtensionBase
	{
		// Token: 0x06002104 RID: 8452 RVA: 0x000880A1 File Offset: 0x000864A1
		public ServerExtension(IGameRoomServer gameRoomServer, IServerInfo serverInfo)
		{
			this.m_gameRoomServer = gameRoomServer;
			this.m_serverInfo = serverInfo;
		}

		// Token: 0x14000084 RID: 132
		// (add) Token: 0x06002105 RID: 8453 RVA: 0x000880B8 File Offset: 0x000864B8
		// (remove) Token: 0x06002106 RID: 8454 RVA: 0x000880F0 File Offset: 0x000864F0
		public event ServerExtension.TrOnServerBoundDeleg tr_server_bound;

		// Token: 0x14000085 RID: 133
		// (add) Token: 0x06002107 RID: 8455 RVA: 0x00088128 File Offset: 0x00086528
		// (remove) Token: 0x06002108 RID: 8456 RVA: 0x00088160 File Offset: 0x00086560
		public event ServerExtension.TrOnServerBindFailDeleg tr_server_bind_failed;

		// Token: 0x14000086 RID: 134
		// (add) Token: 0x06002109 RID: 8457 RVA: 0x00088198 File Offset: 0x00086598
		// (remove) Token: 0x0600210A RID: 8458 RVA: 0x000881D0 File Offset: 0x000865D0
		public event ServerExtension.TrOnServerUnboundDeleg tr_server_unbound;

		// Token: 0x14000087 RID: 135
		// (add) Token: 0x0600210B RID: 8459 RVA: 0x00088208 File Offset: 0x00086608
		// (remove) Token: 0x0600210C RID: 8460 RVA: 0x00088240 File Offset: 0x00086640
		public event ServerExtension.TrOnServerChangedDeleg tr_server_changed;

		// Token: 0x14000088 RID: 136
		// (add) Token: 0x0600210D RID: 8461 RVA: 0x00088278 File Offset: 0x00086678
		// (remove) Token: 0x0600210E RID: 8462 RVA: 0x000882B0 File Offset: 0x000866B0
		public event ServerExtension.OnServerBoundDeleg ServerBound;

		// Token: 0x14000089 RID: 137
		// (add) Token: 0x0600210F RID: 8463 RVA: 0x000882E8 File Offset: 0x000866E8
		// (remove) Token: 0x06002110 RID: 8464 RVA: 0x00088320 File Offset: 0x00086720
		public event ServerExtension.OnServerUnboundDeleg ServerUnbound;

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06002111 RID: 8465 RVA: 0x00088356 File Offset: 0x00086756
		public string ServerID
		{
			get
			{
				return base.Room.GetState<ServerState>(AccessMode.ReadOnly).Server.ServerID;
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06002112 RID: 8466 RVA: 0x0008836E File Offset: 0x0008676E
		public int Port
		{
			get
			{
				return base.Room.GetState<ServerState>(AccessMode.ReadOnly).Server.Port;
			}
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06002113 RID: 8467 RVA: 0x00088386 File Offset: 0x00086786
		public string Host
		{
			get
			{
				return base.Room.GetState<ServerState>(AccessMode.ReadOnly).Server.Hostname;
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06002114 RID: 8468 RVA: 0x0008839E File Offset: 0x0008679E
		public string ServerOnlineID
		{
			get
			{
				return base.Room.GetState<ServerState>(AccessMode.ReadOnly).Server.OnlineID;
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06002115 RID: 8469 RVA: 0x000883B6 File Offset: 0x000867B6
		public EGameServerStatus Status
		{
			get
			{
				return base.Room.GetState<ServerState>(AccessMode.ReadOnly).Server.Status;
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06002116 RID: 8470 RVA: 0x000883D0 File Offset: 0x000867D0
		public bool GameRunning
		{
			get
			{
				EGameServerStatus status = base.Room.GetState<ServerState>(AccessMode.ReadOnly).Server.Status;
				return status == EGameServerStatus.Waiting || status == EGameServerStatus.Playing;
			}
		}

		// Token: 0x06002117 RID: 8471 RVA: 0x00088402 File Offset: 0x00086802
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.tr_player_add_check += this.OnCanJoinPlayer;
		}

		// Token: 0x06002118 RID: 8472 RVA: 0x00088424 File Offset: 0x00086824
		public override void Close()
		{
			base.Close();
			base.Room.tr_player_add_check -= this.OnCanJoinPlayer;
			ServerState state = base.Room.GetState<ServerState>(AccessMode.ReadOnly);
			if (!string.IsNullOrEmpty(state.Server.ServerID))
			{
				this.UnbindServer(false);
			}
		}

		// Token: 0x06002119 RID: 8473 RVA: 0x00088478 File Offset: 0x00086878
		protected override void OnDisposing()
		{
			this.tr_server_bound = null;
			this.tr_server_bind_failed = null;
			this.tr_server_unbound = null;
			this.tr_server_changed = null;
			this.ServerBound = null;
			this.ServerUnbound = null;
			if (!string.IsNullOrEmpty(this.m_serverForRelease))
			{
				this.m_gameRoomServer.ReleaseServer(base.Room.ID, this.m_serverForRelease);
			}
			base.OnDisposing();
		}

		// Token: 0x0600211A RID: 8474 RVA: 0x000884E4 File Offset: 0x000868E4
		private GameRoomRetCode OnCanJoinPlayer(RoomPlayer player)
		{
			if (base.Room.PlayerCount == 0)
			{
				return GameRoomRetCode.OK;
			}
			string usersBuildType = base.Room.GetUsersBuildType();
			if (usersBuildType != string.Empty && player.BuildType != string.Empty && player.BuildType != usersBuildType)
			{
				return GameRoomRetCode.BUILD_TYPE_MISMATCH;
			}
			return GameRoomRetCode.OK;
		}

		// Token: 0x0600211B RID: 8475 RVA: 0x0008854C File Offset: 0x0008694C
		public GameRoomRetCode RequestServer(string server_id, string session_id)
		{
			if (this.m_server_requested)
			{
				return GameRoomRetCode.ERROR;
			}
			if (!this.m_gameRoomServer.RequestServer(base.Room.ID, session_id, server_id))
			{
				ServerExtension.Log.Warn("Failed to request server to player for room {0}", base.Room.ID);
				return GameRoomRetCode.ERROR;
			}
			this.m_server_requested = true;
			return GameRoomRetCode.OK;
		}

		// Token: 0x0600211C RID: 8476 RVA: 0x000885A8 File Offset: 0x000869A8
		public bool BindServer(ServerEntity server)
		{
			this.m_server_requested = false;
			ServerState state = base.Room.GetState<ServerState>(AccessMode.ReadWrite);
			state.Server = server;
			ServerExtension.Log.Debug<ulong, string>("Room {0} server bound: {1}", base.Room.ID, server.ServerID);
			if (this.tr_server_bound != null)
			{
				this.tr_server_bound(state.Server);
			}
			return true;
		}

		// Token: 0x0600211D RID: 8477 RVA: 0x00088610 File Offset: 0x00086A10
		public void BindServerFailed()
		{
			this.m_server_requested = false;
			ServerState state = base.Room.GetState<ServerState>(AccessMode.ReadWrite);
			state.Server.Status = EGameServerStatus.Failed;
			if (this.tr_server_bind_failed != null)
			{
				this.tr_server_bind_failed(state.Server);
			}
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x0008865C File Offset: 0x00086A5C
		public void UnbindServer(bool isServerDeleted)
		{
			ServerState state = base.Room.GetState<ServerState>(AccessMode.ReadWrite);
			ServerEntity server = state.Server;
			if (string.IsNullOrEmpty(server.ServerID))
			{
				return;
			}
			state.Invalidate();
			ServerExtension.Log.Debug<ulong, string>("Room {0} server unbound: {1}", base.Room.ID, server.ServerID);
			if (!isServerDeleted)
			{
				this.m_serverForRelease = server.ServerID;
			}
			if (this.tr_server_unbound != null)
			{
				this.tr_server_unbound(server, isServerDeleted);
			}
		}

		// Token: 0x0600211F RID: 8479 RVA: 0x000886E0 File Offset: 0x00086AE0
		public bool Update()
		{
			ServerState state = base.Room.GetState<ServerState>(AccessMode.ReadWrite);
			if (string.IsNullOrEmpty(state.Server.ServerID))
			{
				return false;
			}
			ServerExtension.Log.Debug("Room {0} updating server status", base.Room.ID);
			ServerEntity serverEntity;
			if (!this.m_serverInfo.GetServer(state.Server.ServerID, true, out serverEntity))
			{
				ServerExtension.Log.Debug("Room {0} server lost", base.Room.ID);
				this.UnbindServer(true);
				return false;
			}
			ServerExtension.Log.Debug<ServerEntity>("[ServerExtension] Server entity updated:\n{0}", serverEntity);
			state.Server = serverEntity;
			this.OnServerInfoChanged(state);
			if (this.tr_server_changed != null)
			{
				this.tr_server_changed(state.Server);
			}
			return true;
		}

		// Token: 0x06002120 RID: 8480 RVA: 0x000887A8 File Offset: 0x00086BA8
		private void OnServerInfoChanged(ServerState state)
		{
			if (state.Server.Status == EGameServerStatus.Free || state.Server.Status == EGameServerStatus.Finished || state.Server.Status == EGameServerStatus.Quiting)
			{
				this.m_gameRoomServer.ReleaseServer(base.Room.ID, state.Server.ServerID);
			}
		}

		// Token: 0x06002121 RID: 8481 RVA: 0x0008880C File Offset: 0x00086C0C
		public override void GetStateUpdateRecepients(RoomUpdate.Context ctx, Set<string> recepients)
		{
			if (ctx.target != RoomUpdate.Target.Server)
			{
				return;
			}
			ServerState serverState = (ServerState)ctx.new_state;
			ServerState serverState2 = (ServerState)ctx.old_state;
			if (!string.IsNullOrEmpty(serverState.Server.OnlineID) && string.Compare(serverState.Server.OnlineID, serverState2.Server.OnlineID) == 0)
			{
				recepients.Add(serverState.Server.OnlineID);
			}
		}

		// Token: 0x06002122 RID: 8482 RVA: 0x00088884 File Offset: 0x00086C84
		public override void PostStateChanged(IRoomState new_state, IRoomState old_state)
		{
			ServerState serverState = (ServerState)old_state;
			ServerState serverState2 = (ServerState)new_state;
			if (serverState2.Server.ServerID != serverState.Server.ServerID)
			{
				if (!string.IsNullOrEmpty(serverState.Server.ServerID) && this.ServerUnbound != null)
				{
					this.ServerUnbound(base.Room, serverState.Server);
				}
				if (!string.IsNullOrEmpty(serverState2.Server.ServerID) && this.ServerBound != null)
				{
					this.ServerBound(base.Room, serverState2.Server);
				}
			}
		}

		// Token: 0x0400101B RID: 4123
		private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

		// Token: 0x0400101C RID: 4124
		private IGameRoomServer m_gameRoomServer;

		// Token: 0x0400101D RID: 4125
		private IServerInfo m_serverInfo;

		// Token: 0x0400101E RID: 4126
		private bool m_server_requested;

		// Token: 0x0400101F RID: 4127
		private string m_serverForRelease;

		// Token: 0x02000609 RID: 1545
		// (Invoke) Token: 0x06002125 RID: 8485
		public delegate void TrOnServerBoundDeleg(ServerEntity server);

		// Token: 0x0200060A RID: 1546
		// (Invoke) Token: 0x06002129 RID: 8489
		public delegate void TrOnServerBindFailDeleg(ServerEntity server);

		// Token: 0x0200060B RID: 1547
		// (Invoke) Token: 0x0600212D RID: 8493
		public delegate void TrOnServerUnboundDeleg(ServerEntity server, bool isServerDeleted);

		// Token: 0x0200060C RID: 1548
		// (Invoke) Token: 0x06002131 RID: 8497
		public delegate void TrOnServerChangedDeleg(ServerEntity server);

		// Token: 0x0200060D RID: 1549
		// (Invoke) Token: 0x06002135 RID: 8501
		public delegate void OnServerBoundDeleg(IGameRoom room, ServerEntity server);

		// Token: 0x0200060E RID: 1550
		// (Invoke) Token: 0x06002139 RID: 8505
		public delegate void OnServerUnboundDeleg(IGameRoom room, ServerEntity server);
	}
}
