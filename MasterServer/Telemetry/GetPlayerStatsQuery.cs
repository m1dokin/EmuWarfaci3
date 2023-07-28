using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.PlayerStats;
using OLAPHypervisor;

namespace MasterServer.Telemetry
{
	// Token: 0x020007C4 RID: 1988
	[QueryAttributes(TagName = "get_player_stats", QoSClass = "personal_statistics")]
	internal class GetPlayerStatsQuery : BaseQuery
	{
		// Token: 0x060028C1 RID: 10433 RVA: 0x000B0954 File Offset: 0x000AED54
		public GetPlayerStatsQuery(IPlayerStatsService playerStatsService)
		{
			this.m_playerStatsService = playerStatsService;
		}

		// Token: 0x060028C2 RID: 10434 RVA: 0x000B0964 File Offset: 0x000AED64
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			ulong profileId;
			if (!base.GetClientProfileId(fromJid, out profileId))
			{
				return -1;
			}
			foreach (Measure measure in this.m_playerStatsService.GetPlayerStatsAggregated(profileId))
			{
				XmlElement xmlElement = response.OwnerDocument.CreateElement("stat");
				foreach (KeyValuePair<string, string> keyValuePair in measure.Dimensions)
				{
					xmlElement.SetAttribute(keyValuePair.Key, keyValuePair.Value);
				}
				xmlElement.SetAttribute("Value", measure.Value.ToString());
				response.AppendChild(xmlElement);
			}
			return 0;
		}

		// Token: 0x040015A4 RID: 5540
		private readonly IPlayerStatsService m_playerStatsService;
	}
}
