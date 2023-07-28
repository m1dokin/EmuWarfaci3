using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003A4 RID: 932
	[QueryAttributes(TagName = "missions_get_list")]
	internal class GetMissionsQuery : BaseQuery
	{
		// Token: 0x060014B1 RID: 5297 RVA: 0x00054F0D File Offset: 0x0005330D
		public GetMissionsQuery(IMissionGenerationService generationService)
		{
			this.m_generationService = generationService;
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x00054F1C File Offset: 0x0005331C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetMissionsQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -1;
				}
				else
				{
					Dictionary<Guid, MissionContext> missions = this.m_generationService.GetMissions();
					MissionHash missionsHash = this.m_generationService.GetMissionsHash();
					response.SetAttribute("hash", missionsHash.missionHash);
					response.SetAttribute("content_hash", missionsHash.ContentHash);
					foreach (MissionContext m in missions.Values)
					{
						XmlElement xmlElement = response.OwnerDocument.CreateElement("mission");
						MissionExtension.SerializeMission(xmlElement, m, RoomUpdate.Target.Client, RoomUpdate.InformationType.UiBaseInfo | RoomUpdate.InformationType.CrownRewardsInfo);
						response.AppendChild(xmlElement);
					}
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x040009C0 RID: 2496
		private readonly IMissionGenerationService m_generationService;
	}
}
