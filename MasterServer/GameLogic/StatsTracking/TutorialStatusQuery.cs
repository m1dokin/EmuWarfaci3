using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005D9 RID: 1497
	[QueryAttributes(TagName = "tutorial_status")]
	internal class TutorialStatusQuery : BaseQuery
	{
		// Token: 0x06001FEA RID: 8170 RVA: 0x00081F8B File Offset: 0x0008038B
		public TutorialStatusQuery(IMissionSystem missionSystem, ITutorialStatsTracker statsTracker)
		{
			this.m_missionSystem = missionSystem;
			this.m_statsTracker = statsTracker;
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x00081FA4 File Offset: 0x000803A4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "TutorialStatusQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("id");
					MissionContext mission = this.m_missionSystem.GetMission(attribute);
					if (mission == null)
					{
						Log.Warning<string>("Query tutorial_status : Unexpected : GetMission returned null for mission_key '{0}'", attribute);
						result = -1;
					}
					else
					{
						int tutorialMission = mission.tutorialMission;
						if (tutorialMission == 0)
						{
							Log.Warning<string>("Query tutorial_status asks mission {0} which is not tutorial", mission.name);
							result = -1;
						}
						else
						{
							string attribute2 = request.GetAttribute("step");
							TutorialEvent tutorialEvent = (TutorialEvent)uint.Parse(request.GetAttribute("event"));
							if (!Enum.IsDefined(typeof(TutorialEvent), tutorialEvent))
							{
								Log.Warning<TutorialEvent>("Query tutorial_status : Unexpected {0} tutorial event type", tutorialEvent);
								result = -1;
							}
							else
							{
								this.m_statsTracker.TrackEvent(tutorialEvent, user, tutorialMission, attribute2, response);
								result = 0;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04000FA4 RID: 4004
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000FA5 RID: 4005
		private readonly ITutorialStatsTracker m_statsTracker;
	}
}
