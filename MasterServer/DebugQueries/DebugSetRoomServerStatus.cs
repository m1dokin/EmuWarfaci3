using System;
using System.Xml;
using DedicatedPoolServer.Model;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.DebugQueries
{
	// Token: 0x020000F6 RID: 246
	[DebugQuery]
	[QueryAttributes(TagName = "debug_set_room_server_status")]
	internal class DebugSetRoomServerStatus : BaseQuery
	{
		// Token: 0x06000409 RID: 1033 RVA: 0x00011AF7 File Offset: 0x0000FEF7
		public DebugSetRoomServerStatus(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00011B08 File Offset: 0x0000FF08
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "DebugSetRoomServerStatus"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("roomId");
					EGameServerStatus serverStatus = Utils.ParseEnum<EGameServerStatus>(request.GetAttribute("serverStatus"));
					IGameRoom room = this.m_gameRoomManager.GetRoom(attribute);
					room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						ServerState state = r.GetState<ServerState>(AccessMode.ReadWrite);
						state.Server.Status = serverStatus;
					});
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x040001B3 RID: 435
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
