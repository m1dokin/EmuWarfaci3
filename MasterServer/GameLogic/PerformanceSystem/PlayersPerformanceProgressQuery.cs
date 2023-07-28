using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.PerformanceSystem
{
	// Token: 0x020003F7 RID: 1015
	[QueryAttributes(TagName = "players_performance_progress")]
	internal class PlayersPerformanceProgressQuery : BaseQuery
	{
		// Token: 0x060015F5 RID: 5621 RVA: 0x0005B96E File Offset: 0x00059D6E
		public PlayersPerformanceProgressQuery(ISessionStorage sessionStorage, IMissionSystem missionSystem, ILogService logService)
		{
			this.m_sessionStorage = sessionStorage;
			this.m_missionSystem = missionSystem;
			this.m_logService = logService;
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x0005B98C File Offset: 0x00059D8C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "PlayersPerformanceProgressQuery"))
			{
				string attribute = request.GetAttribute("session_id");
				string attribute2 = request.GetAttribute("mission_id");
				int num = int.Parse(request.GetAttribute("passed_sublevels_count"));
				if (!this.m_sessionStorage.ValidateSession(fromJid, attribute))
				{
					result = -3;
				}
				else
				{
					MissionContext mission = this.m_missionSystem.GetMission(attribute2);
					if (mission == null)
					{
						throw new NullReferenceException(string.Format("There is no mission context for mission with id = {0}. Jid = {1}, session id = {2}", attribute2, fromJid, attribute));
					}
					if (num < 0 || mission.subLevels.Count < num)
					{
						throw new ArgumentOutOfRangeException("sublevelCount", string.Format("Get incorrect passed_sublevels_count {0} for session {1} with mission {2} with {3} sublevels", new object[]
						{
							num,
							attribute,
							attribute2,
							mission.subLevels.Count
						}));
					}
					SubLevel subLevel = mission.subLevels[num - 1];
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					IEnumerator enumerator = request.GetElementsByTagName("stat").GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							XmlElement xmlElement = (XmlElement)obj;
							int key = int.Parse(xmlElement.GetAttribute("id"));
							int value = int.Parse(xmlElement.GetAttribute("value"));
							dictionary[key] = value;
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					this.m_logService.Event.PlayersPerformanceProgressLog(attribute, mission.name, mission.difficulty, num, subLevel.name, subLevel.flow, dictionary[0], dictionary[1], dictionary[2], dictionary[3], dictionary[4], dictionary[5]);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x04000A85 RID: 2693
		public const string Name = "players_performance_progress";

		// Token: 0x04000A86 RID: 2694
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000A87 RID: 2695
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000A88 RID: 2696
		private readonly ILogService m_logService;
	}
}
