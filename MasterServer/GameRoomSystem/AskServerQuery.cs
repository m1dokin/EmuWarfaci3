using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000764 RID: 1892
	[QueryAttributes(TagName = "gameroom_askserver")]
	internal class AskServerQuery : BaseQuery
	{
		// Token: 0x06002746 RID: 10054 RVA: 0x000A5FC0 File Offset: 0x000A43C0
		public AskServerQuery(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06002747 RID: 10055 RVA: 0x000A5FD0 File Offset: 0x000A43D0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "AskServerQuery"))
			{
				ulong profile_id;
				if (!base.GetClientProfileId(fromJid, out profile_id))
				{
					result = -3;
				}
				else
				{
					string sServerId = null;
					if (request.HasAttribute("server"))
					{
						sServerId = request.GetAttribute("server");
					}
					IGameRoom room = this.m_gameRoomManager.GetRoomByPlayer(profile_id);
					if (room == null)
					{
						result = -1;
					}
					else if (room.IsAutoStartMode())
					{
						Log.Warning<string>("[AskServerQuery] Restricted query for AS room, jid {0}", fromJid);
						result = -1;
					}
					else
					{
						GameRoomRetCode ret = GameRoomRetCode.OK;
						try
						{
							room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
							{
								RoomMasterExtension extension = r.GetExtension<RoomMasterExtension>();
								if (!extension.IsRoomMaster(profile_id))
								{
									throw new RoomGenericException(room.ID, "Only room master can start session");
								}
								SessionExtension extension2 = r.GetExtension<SessionExtension>();
								ret = extension2.StartSession(sServerId);
							});
						}
						catch (RoomGenericException ex)
						{
							Log.Warning<string>("[AskServerQuery] Failed: {0}", ex.Message);
							return -1;
						}
						result = (int)ret;
					}
				}
			}
			return result;
		}

		// Token: 0x04001441 RID: 5185
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
