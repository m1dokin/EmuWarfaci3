using System;
using System.Collections.Generic;
using MasterServer.ServerInfo;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004AF RID: 1199
	[RoomExtension]
	internal class DebugRoomExtension : RoomExtensionBase
	{
		// Token: 0x06001971 RID: 6513 RVA: 0x000675A8 File Offset: 0x000659A8
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			this.m_eventsEnabled = new Dictionary<string, bool>();
			base.Room.tr_player_added += this.OnTrPlayerAdded;
			base.Room.tr_player_add_check += this.OnTrPlayerAddCheck;
			base.Room.tr_player_joined_session += this.OnTrPlayerJoinedSession;
			base.Room.tr_player_removed += this.OnTrPlayerRemoved;
			base.Room.tr_player_status += this.OnTrPlayerStatus;
			base.Room.tr_players_changed += this.OnTrPlayersChanged;
			base.Room.PlayerAdded += this.OnPlayerAdded;
			base.Room.PlayerRemoved += this.OnPlayerRemoved;
			base.Room.PlayerChanged += this.OnPlayerChanged;
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			extension.ServerBound += this.RoomServerBound;
			extension.ServerUnbound += this.RoomServerUnbound;
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x000676C8 File Offset: 0x00065AC8
		public override void Close()
		{
			base.Close();
			base.Room.tr_player_added -= this.OnTrPlayerAdded;
			base.Room.tr_player_add_check -= this.OnTrPlayerAddCheck;
			base.Room.tr_player_joined_session -= this.OnTrPlayerJoinedSession;
			base.Room.tr_player_removed -= this.OnTrPlayerRemoved;
			base.Room.tr_player_status -= this.OnTrPlayerStatus;
			base.Room.tr_players_changed -= this.OnTrPlayersChanged;
			base.Room.PlayerAdded -= this.OnPlayerAdded;
			base.Room.PlayerRemoved -= this.OnPlayerRemoved;
			base.Room.PlayerChanged -= this.OnPlayerChanged;
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			extension.ServerBound -= this.RoomServerBound;
			extension.ServerUnbound -= this.RoomServerUnbound;
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x000677DC File Offset: 0x00065BDC
		public bool IsEventEnabled(string name)
		{
			bool result = false;
			this.m_eventsEnabled.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x06001974 RID: 6516 RVA: 0x000677FB File Offset: 0x00065BFB
		public void SetEventEnabled(string name, bool enabled)
		{
			this.m_eventsEnabled[name] = enabled;
		}

		// Token: 0x06001975 RID: 6517 RVA: 0x0006780A File Offset: 0x00065C0A
		private void OnPlayerChanged(IGameRoom room, RoomPlayer newPlayer, RoomPlayer oldPlayer)
		{
			this.ProcessEvent("PlayerChanged");
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x00067817 File Offset: 0x00065C17
		private void OnPlayerRemoved(IGameRoom room, RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			this.ProcessEvent("PlayerRemoved");
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x00067824 File Offset: 0x00065C24
		private void OnPlayerAdded(IGameRoom room, ulong profileId)
		{
			this.ProcessEvent("PlayerAdded");
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x00067831 File Offset: 0x00065C31
		private void OnTrPlayersChanged()
		{
			this.ProcessEvent("tr_players_changed");
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x0006783E File Offset: 0x00065C3E
		private void OnTrPlayerStatus(ulong profileId, UserStatus oldStatus, UserStatus newStatus)
		{
			this.ProcessEvent("tr_player_status");
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x0006784B File Offset: 0x00065C4B
		private void OnTrPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			this.ProcessEvent("tr_player_removed");
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x00067858 File Offset: 0x00065C58
		private void OnTrPlayerJoinedSession(ulong profileId)
		{
			this.ProcessEvent("tr_player_joined_session");
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x00067865 File Offset: 0x00065C65
		private GameRoomRetCode OnTrPlayerAddCheck(RoomPlayer user)
		{
			this.ProcessEvent("tr_player_add_check");
			return GameRoomRetCode.OK;
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x00067873 File Offset: 0x00065C73
		private void OnTrPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			this.ProcessEvent("tr_player_added");
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x00067880 File Offset: 0x00065C80
		private void RoomServerUnbound(IGameRoom room, ServerEntity server)
		{
			this.ProcessEvent("ServerUnbound");
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x0006788D File Offset: 0x00065C8D
		private void RoomServerBound(IGameRoom room, ServerEntity server)
		{
			this.ProcessEvent("ServerBound");
		}

		// Token: 0x06001980 RID: 6528 RVA: 0x0006789C File Offset: 0x00065C9C
		private void ProcessEvent(string name)
		{
			bool flag = false;
			this.m_eventsEnabled.TryGetValue(name, out flag);
			if (flag)
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x04000C2D RID: 3117
		private Dictionary<string, bool> m_eventsEnabled;
	}
}
