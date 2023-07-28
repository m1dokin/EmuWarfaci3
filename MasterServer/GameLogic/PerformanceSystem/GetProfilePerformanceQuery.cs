using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.FirstWinOfDayByModeSystem;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.PerformanceSystem
{
	// Token: 0x02000679 RID: 1657
	[QueryAttributes(TagName = "get_profile_performance")]
	internal class GetProfilePerformanceQuery : BaseQuery
	{
		// Token: 0x060022F9 RID: 8953 RVA: 0x00092470 File Offset: 0x00090870
		public GetProfilePerformanceQuery(IMissionGenerationService missionGenerationService, IMissionPerformanceService missionPerformanceService, IFirstWinOfDayByModeService firstWinOfDayByModeService)
		{
			this.m_missionGenerationService = missionGenerationService;
			this.m_missionPerformanceService = missionPerformanceService;
			this.m_firstWinOfDayByModeService = firstWinOfDayByModeService;
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x00092490 File Offset: 0x00090890
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetProfilePerformanceQuery"))
			{
				ulong num;
				if (!base.GetClientProfileId(fromJid, out num))
				{
					result = -3;
				}
				else
				{
					ProfilePerformanceInfo profilePerformance = this.m_missionPerformanceService.GetProfilePerformance(num);
					response.SetAttribute("missions_hash", this.m_missionGenerationService.GetMissionsHash().missionHash);
					XmlDocument ownerDocument = response.OwnerDocument;
					XmlElement xmlElement = ownerDocument.CreateElement("pve_missions_performance");
					response.AppendChild(xmlElement);
					foreach (ProfilePerformanceInfo.MissionPerformance missionPerformance in profilePerformance.MissionPerformances.Values)
					{
						XmlElement newChild = missionPerformance.WriteToXml(ownerDocument);
						xmlElement.AppendChild(newChild);
					}
					XmlElement xmlElement2 = ownerDocument.CreateElement("pvp_modes_to_complete");
					response.AppendChild(xmlElement2);
					IEnumerable<string> pvpModesWithBonusAvailable = this.m_firstWinOfDayByModeService.GetPvpModesWithBonusAvailable(num);
					foreach (string innerText in pvpModesWithBonusAvailable)
					{
						XmlElement xmlElement3 = ownerDocument.CreateElement("mode");
						xmlElement3.InnerText = innerText;
						xmlElement2.AppendChild(xmlElement3);
					}
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0400118F RID: 4495
		private readonly IMissionGenerationService m_missionGenerationService;

		// Token: 0x04001190 RID: 4496
		private readonly IMissionPerformanceService m_missionPerformanceService;

		// Token: 0x04001191 RID: 4497
		private readonly IFirstWinOfDayByModeService m_firstWinOfDayByModeService;
	}
}
