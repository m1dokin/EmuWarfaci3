using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Matchmaking;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005EB RID: 1515
	[QueryAttributes(TagName = "quickplay_maplist")]
	internal class GetAutostartMissionListQuery : PagedQueryStatic
	{
		// Token: 0x0600203D RID: 8253 RVA: 0x0008221E File Offset: 0x0008061E
		public GetAutostartMissionListQuery(IMatchmakingMissionsProvider matchmakingMissionsProvider)
		{
			this.m_matchmakingMissionsProvider = matchmakingMissionsProvider;
		}

		// Token: 0x0600203E RID: 8254 RVA: 0x0008222D File Offset: 0x0008062D
		protected override int GetMaxBatch()
		{
			return 250;
		}

		// Token: 0x0600203F RID: 8255 RVA: 0x00082234 File Offset: 0x00080634
		protected override string GetDataHash()
		{
			return this.m_matchmakingMissionsProvider.AutostartMissions.Aggregate(0, (int acc, string mid) => acc ^ mid.GetHashCode()).ToString();
		}

		// Token: 0x06002040 RID: 8256 RVA: 0x00082280 File Offset: 0x00080680
		protected override List<XmlElement> GetData(XmlDocument doc)
		{
			List<XmlElement> list = new List<XmlElement>();
			foreach (string value in this.m_matchmakingMissionsProvider.AutostartMissions)
			{
				XmlElement xmlElement = doc.CreateElement("map");
				xmlElement.SetAttribute("mission", value);
				list.Add(xmlElement);
			}
			return list;
		}

		// Token: 0x04000FC3 RID: 4035
		private const int MAPS_MAX_BATCH = 250;

		// Token: 0x04000FC4 RID: 4036
		private readonly IMatchmakingMissionsProvider m_matchmakingMissionsProvider;
	}
}
