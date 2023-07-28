using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000777 RID: 1911
	[QueryAttributes(TagName = "gameroom_setteamcolor")]
	internal class SetRoomTeamColorQuery : BaseQuery
	{
		// Token: 0x060027A8 RID: 10152 RVA: 0x000A92CA File Offset: 0x000A76CA
		public SetRoomTeamColorQuery(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x060027A9 RID: 10153 RVA: 0x000A92DC File Offset: 0x000A76DC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetRoomTeamColorQuery"))
			{
				ulong profile_id;
				if (!base.GetClientProfileId(fromJid, out profile_id))
				{
					result = -3;
				}
				else
				{
					int teamId = int.Parse(request.GetAttribute("team_id"));
					uint teamColor = uint.Parse(request.GetAttribute("team_color"));
					IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profile_id);
					if (roomByPlayer == null)
					{
						result = -1;
					}
					else if (roomByPlayer.IsAutoStartMode())
					{
						Log.Warning<string>("[SetRoomTeamColorQuery] Restricted query for AS room, jid {0}", fromJid);
						result = -1;
					}
					else
					{
						XmlElement xmlElement = roomByPlayer.transaction(fromJid, response.OwnerDocument, AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							r.SetTeamColor(teamId, teamColor);
						});
						if (xmlElement != null)
						{
							response.AppendChild(xmlElement);
						}
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x040014B3 RID: 5299
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
