using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.Matchmaking;
using Util.Common;

namespace MasterServer.DebugQueries
{
	// Token: 0x02000050 RID: 80
	[QueryAttributes(TagName = "debug_get_matchmaking_missions")]
	internal class DebugGetMatchmakingMissionsQuery : BaseQuery
	{
		// Token: 0x06000139 RID: 313 RVA: 0x0000993E File Offset: 0x00007D3E
		public DebugGetMatchmakingMissionsQuery(IMissionSystem missionSystem, IMatchmakingMissionsProvider matchmakingMissionsProvider)
		{
			this.m_missionSystem = missionSystem;
			this.m_matchmakingMissionsProvider = matchmakingMissionsProvider;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00009954 File Offset: 0x00007D54
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			IEnumerable<string> missionIds = this.m_matchmakingMissionsProvider.AutostartMissions;
			IEnumerable<MissionContextBase> source = from m in this.m_missionSystem.GetMatchmakingMissions()
			where missionIds.Contains(m.uid)
			select m;
			XmlElement el = response.OwnerDocument.CreateElement("missions");
			response.AppendChild(el);
			IEnumerable<XmlElement> src = from m in source
			select this.CreateMissionElement(el.OwnerDocument, m);
			src.ForEach(delegate(XmlElement e)
			{
				el.AppendChild(e);
			});
			return 0;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x000099E8 File Offset: 0x00007DE8
		private XmlElement CreateMissionElement(XmlDocument doc, MissionContextBase mission)
		{
			XmlElement xmlElement = doc.CreateElement("mission");
			xmlElement.SetAttribute("uid", mission.uid);
			xmlElement.SetAttribute("name", mission.name);
			xmlElement.SetAttribute("game_mode", mission.gameMode);
			return xmlElement;
		}

		// Token: 0x04000094 RID: 148
		private IMissionSystem m_missionSystem;

		// Token: 0x04000095 RID: 149
		private IMatchmakingMissionsProvider m_matchmakingMissionsProvider;
	}
}
