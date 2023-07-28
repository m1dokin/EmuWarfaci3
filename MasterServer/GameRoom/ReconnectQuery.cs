using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Database;
using MasterServer.GameRoom.RoomExtensions.Reconnect;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom
{
	// Token: 0x020004AA RID: 1194
	[QueryAttributes(TagName = "gameroom_reconnect")]
	internal class ReconnectQuery : BaseQuery
	{
		// Token: 0x06001965 RID: 6501 RVA: 0x000671D6 File Offset: 0x000655D6
		public ReconnectQuery(IGameRoomManager gameRoomManager, IDALService dalService, ILogService logService)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_dalService = dalService;
			this.m_logService = logService;
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x000671F4 File Offset: 0x000655F4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			ulong profileId;
			if (!ulong.TryParse(request.GetAttribute("profile_id"), out profileId))
			{
				return -1;
			}
			ulong userID = this.m_dalService.ProfileSystem.GetProfileInfo(profileId).UserID;
			if (userID == 0UL)
			{
				return -1;
			}
			ulong num;
			if (!ulong.TryParse(request.GetAttribute("room_id"), out num))
			{
				return -1;
			}
			IGameRoom room = this.m_gameRoomManager.GetRoom(num);
			if (room == null)
			{
				this.LogReconnect(userID, profileId, num, GameRoomType.All, ReconnectResult.NoRoom);
				return -1;
			}
			ReconnectResult reconnectResult = ReconnectResult.InvalidReconnectInfo;
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				reconnectResult = r.ReconnectPlayer(profileId);
			});
			this.LogReconnect(userID, profileId, num, room.Type, reconnectResult);
			if (!reconnectResult.IsOneOf(new ReconnectResult[]
			{
				ReconnectResult.Success,
				ReconnectResult.OtherTeam
			}))
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x000672E1 File Offset: 0x000656E1
		private void LogReconnect(ulong userId, ulong profileId, ulong roomId, GameRoomType roomType, ReconnectResult result)
		{
			this.m_logService.Event.RoomReconnectLog(userId, profileId, roomId, roomType, result);
		}

		// Token: 0x04000C28 RID: 3112
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000C29 RID: 3113
		private readonly IDALService m_dalService;

		// Token: 0x04000C2A RID: 3114
		private readonly ILogService m_logService;
	}
}
