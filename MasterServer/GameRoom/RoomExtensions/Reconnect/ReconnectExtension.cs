using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core.Configs;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoom.RoomExtensions.Reconnect
{
	// Token: 0x020004CB RID: 1227
	[RoomExtension]
	internal class ReconnectExtension : RoomExtensionBase
	{
		// Token: 0x06001A8F RID: 6799 RVA: 0x0006D09F File Offset: 0x0006B49F
		public ReconnectExtension(IConfigProvider<ReconnectConfig> reconnectConfigProvider, IGameRoomOfferService gameRoomOfferService)
		{
			this.m_reconnectConfig = reconnectConfigProvider.Get();
			this.m_gameRoomOfferService = gameRoomOfferService;
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x0006D0BC File Offset: 0x0006B4BC
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			room.tr_player_removed += this.OnPlayerRemoved;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended += this.OnSessionEnded;
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x0006D100 File Offset: 0x0006B500
		public ReconnectResult Reconnect(ulong profileId)
		{
			Dictionary<ulong, ReconnectInfo> reconnectInfos = this.GetReconnectInfos(AccessMode.ReadWrite);
			ReconnectInfo reconnectInfo;
			if (!reconnectInfos.TryGetValue(profileId, out reconnectInfo))
			{
				return ReconnectResult.InvalidReconnectInfo;
			}
			TimeSpan t = DateTime.Now - reconnectInfo.LeaveTime;
			if (t > this.m_reconnectConfig.ReconnectTimeout)
			{
				return ReconnectResult.ReconnectInfoExpired;
			}
			if (base.Room.PlayerCount >= base.Room.MaxPlayers)
			{
				return ReconnectResult.NoFreeSlots;
			}
			ReconnectResult result = ReconnectResult.Success;
			int teamId = reconnectInfo.TeamId;
			int num = base.Room.Players.Count((RoomPlayer p) => p.TeamID == teamId);
			if (num >= base.Room.MaxTeamSize)
			{
				reconnectInfo.TeamId = ((teamId != 1) ? 1 : 2);
				result = ReconnectResult.OtherTeam;
			}
			this.m_gameRoomOfferService.OfferRoomForReconnect(base.Room, profileId);
			return result;
		}

		// Token: 0x06001A92 RID: 6802 RVA: 0x0006D1E0 File Offset: 0x0006B5E0
		public ReconnectInfo GetReconnectInfo(ulong profileId)
		{
			Dictionary<ulong, ReconnectInfo> reconnectInfos = this.GetReconnectInfos(AccessMode.ReadOnly);
			return (!reconnectInfos.ContainsKey(profileId)) ? null : reconnectInfos[profileId];
		}

		// Token: 0x06001A93 RID: 6803 RVA: 0x0006D210 File Offset: 0x0006B610
		protected override void OnDisposing()
		{
			base.Room.tr_player_removed -= this.OnPlayerRemoved;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended -= this.OnSessionEnded;
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x0006D254 File Offset: 0x0006B654
		private void OnPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			ServerExtension extension = base.Room.GetExtension<ServerExtension>();
			if (!extension.GameRunning)
			{
				return;
			}
			ReconnectInfo value = new ReconnectInfo(player);
			Dictionary<ulong, ReconnectInfo> reconnectInfos = this.GetReconnectInfos(AccessMode.ReadWrite);
			reconnectInfos[player.ProfileID] = value;
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x0006D298 File Offset: 0x0006B698
		private void OnSessionEnded(string sessionId, bool abnormal)
		{
			Dictionary<ulong, ReconnectInfo> reconnectInfos = this.GetReconnectInfos(AccessMode.ReadWrite);
			reconnectInfos.Clear();
		}

		// Token: 0x06001A96 RID: 6806 RVA: 0x0006D2B4 File Offset: 0x0006B6B4
		private Dictionary<ulong, ReconnectInfo> GetReconnectInfos(AccessMode accessMode)
		{
			ReconnectState state = base.Room.GetState<ReconnectState>(accessMode);
			return state.ReconnectInfos;
		}

		// Token: 0x04000CB7 RID: 3255
		private readonly ReconnectConfig m_reconnectConfig;

		// Token: 0x04000CB8 RID: 3256
		private readonly IGameRoomOfferService m_gameRoomOfferService;
	}
}
