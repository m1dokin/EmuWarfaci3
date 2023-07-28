using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000776 RID: 1910
	[QueryAttributes(TagName = "gameroom_setinfo")]
	internal class SetRoomInfoQuery : BaseQuery
	{
		// Token: 0x060027A7 RID: 10151 RVA: 0x000A9130 File Offset: 0x000A7530
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetRoomInfoQuery"))
			{
				ulong profile_id;
				if (!base.GetClientProfileId(fromJid, out profile_id))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("by_mission_key");
					int num;
					if (!int.TryParse(attribute, out num))
					{
						throw new FormatException(string.Format("[SetRoomInfoQuery] Parsing by_mission_key attribute failed. Attribute value: {0}", attribute));
					}
					bool flag = num != 0;
					string missionKey = request.GetAttribute("mission_key");
					string attribute2 = request.GetAttribute("data");
					IGameRoomManager service = ServicesManager.GetService<IGameRoomManager>();
					IGameRoom roomByPlayer = service.GetRoomByPlayer(profile_id);
					if (roomByPlayer == null)
					{
						result = -1;
					}
					else if (roomByPlayer.IsAutoStartMode())
					{
						Log.Warning<string>("[SetRoomInfoQuery] Restricted query for AS room, jid {0}", fromJid);
						result = -1;
					}
					else
					{
						if (!flag)
						{
							if (!Resources.DebugQueriesEnabled)
							{
								Log.Error<string>("Set room mission data is disabled, jid {0}", fromJid);
								return -1;
							}
							IMissionSystem service2 = ServicesManager.GetService<IMissionSystem>();
							service2.AddMission(missionKey, attribute2);
						}
						GameRoomRetCode ret = GameRoomRetCode.ERROR;
						XmlElement xmlElement = roomByPlayer.transaction(fromJid, response.OwnerDocument, AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							MissionExtension extension = r.GetExtension<MissionExtension>();
							ret = extension.SetMission(missionKey);
						});
						if (xmlElement != null)
						{
							response.AppendChild(xmlElement);
						}
						result = (int)ret;
					}
				}
			}
			return result;
		}
	}
}
