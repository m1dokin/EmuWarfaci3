using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.GameLogic.PerformanceSystem;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x0200026D RID: 621
	[QueryAttributes(TagName = "clan_info")]
	internal class ClanInfoQuery : BaseQuery
	{
		// Token: 0x06000D75 RID: 3445 RVA: 0x00035A05 File Offset: 0x00033E05
		public ClanInfoQuery(IClanPerformanceService clanPerformanceService)
		{
			this.m_clanPerformanceService = clanPerformanceService;
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x00035A14 File Offset: 0x00033E14
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			ClanInfo clanInfo = queryParams[0] as ClanInfo;
			IEnumerable<ClanMemberInfo> enumerable = queryParams[1] as IEnumerable<ClanMemberInfo>;
			if (clanInfo != null)
			{
				ClanPerformanceInfo clanPerformance = this.m_clanPerformanceService.GetClanPerformance(clanInfo.ClanID);
				XmlElement xmlElement = clanInfo.ToXml(request.OwnerDocument, true);
				xmlElement.SetAttribute("leaderboard_position", clanPerformance.Position.ToString());
				request.AppendChild(xmlElement);
				foreach (ClanMemberInfo clanMemberInfo in enumerable)
				{
					XmlElement newChild = clanMemberInfo.ToXml(request.OwnerDocument);
					xmlElement.AppendChild(newChild);
				}
			}
		}

		// Token: 0x04000636 RID: 1590
		private readonly IClanPerformanceService m_clanPerformanceService;
	}
}
