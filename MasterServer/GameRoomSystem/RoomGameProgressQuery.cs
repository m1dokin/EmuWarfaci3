using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200061B RID: 1563
	[QueryAttributes(TagName = "gameroom_setgameprogress")]
	internal class RoomGameProgressQuery : BaseQuery
	{
		// Token: 0x0600219E RID: 8606 RVA: 0x0008A051 File Offset: 0x00088451
		public RoomGameProgressQuery(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x0600219F RID: 8607 RVA: 0x0008A060 File Offset: 0x00088460
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			string server_id;
			if (!base.GetServerID(fromJid, out server_id))
			{
				return -3;
			}
			float gameProgress = float.Parse(request.GetAttribute("game_progress"));
			ulong room_id = ulong.Parse(request.GetAttribute("room_id"));
			IGameRoom gameRoom = this.m_gameRoomManager.GetRoomByServer(server_id) ?? this.m_gameRoomManager.GetRoom(room_id);
			if (gameRoom == null)
			{
				return -1;
			}
			gameRoom.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				SessionExtension extension = r.GetExtension<SessionExtension>();
				extension.GameProgress = gameProgress;
			});
			return 0;
		}

		// Token: 0x04001049 RID: 4169
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
