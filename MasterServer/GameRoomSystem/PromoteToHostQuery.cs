using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000771 RID: 1905
	[QueryAttributes(TagName = "gameroom_promote_to_host")]
	internal class PromoteToHostQuery : BaseQuery
	{
		// Token: 0x06002782 RID: 10114 RVA: 0x000A8525 File Offset: 0x000A6925
		public PromoteToHostQuery(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06002783 RID: 10115 RVA: 0x000A8534 File Offset: 0x000A6934
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "PromoteToHostQuery"))
			{
				ulong profile_id;
				if (!base.GetClientProfileId(fromJid, out profile_id))
				{
					result = -3;
				}
				else
				{
					ulong newMaster = ulong.Parse(request.GetAttribute("new_host_profile_id"));
					IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profile_id);
					if (roomByPlayer == null)
					{
						result = -1;
					}
					else if (roomByPlayer.IsAutoStartMode())
					{
						Log.Warning<string>("[PromoteToHostQuery] Restricted query for AS room, jid {0}", fromJid);
						result = -1;
					}
					else
					{
						try
						{
							XmlElement xmlElement = roomByPlayer.transaction(fromJid, response.OwnerDocument, AccessMode.ReadWrite, delegate(IGameRoom r)
							{
								r.GetExtension<RoomMasterExtension>().PromoteToMaster(profile_id, newMaster);
							});
							if (xmlElement != null)
							{
								response.AppendChild(xmlElement);
							}
						}
						catch (ApplicationException ex)
						{
							Log.Warning<string>("[PromoteToHostQuery] Failed: {0}", ex.Message);
							return -1;
						}
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x0400147F RID: 5247
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
