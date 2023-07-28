using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.PerformanceSystem
{
	// Token: 0x020003F3 RID: 1011
	[Service]
	[Singleton]
	internal class MissionPerformanceService : ServiceModule, IMissionPerformanceService
	{
		// Token: 0x060015E6 RID: 5606 RVA: 0x0005B553 File Offset: 0x00059953
		public MissionPerformanceService(IMissionGenerationService missionGenerationService, IGameRoomManager gameRoomManager, ISessionStorage sessionStorage, IDALService dalService)
		{
			this.m_missionGenerationService = missionGenerationService;
			this.m_gameRoomManager = gameRoomManager;
			this.m_sessionStorage = sessionStorage;
			this.m_dalService = dalService;
		}

		// Token: 0x060015E7 RID: 5607 RVA: 0x0005B578 File Offset: 0x00059978
		public override void Start()
		{
			base.Start();
			this.m_gameRoomManager.SessionStarted += this.OnSessionStarted;
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x0005B597 File Offset: 0x00059997
		public override void Stop()
		{
			this.m_gameRoomManager.SessionStarted -= this.OnSessionStarted;
			base.Stop();
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x0005B5B8 File Offset: 0x000599B8
		public ProfilePerformanceInfo GetProfilePerformance(ulong profile_id)
		{
			ProfilePerformanceInfo result = new ProfilePerformanceInfo(profile_id);
			this.InitProfilePerformance(ref result);
			return result;
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x0005B5D8 File Offset: 0x000599D8
		public void UpdatePlayersPerformance(PerformanceUpdate upd)
		{
			Dictionary<Guid, MissionContext> missions = this.m_missionGenerationService.GetMissions();
			if (!missions.ContainsKey(upd.MissionID))
			{
				return;
			}
			List<Guid> currentMissions = new List<Guid>(missions.Keys);
			this.m_dalService.PerformanceSystem.UpdateMissionPerformance(upd, currentMissions);
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x0005B624 File Offset: 0x00059A24
		private void InitProfilePerformance(ref ProfilePerformanceInfo ret)
		{
			List<Guid> currentMissions = new List<Guid>(this.m_missionGenerationService.GetMissions().Keys);
			foreach (ProfilePerformance.MissionPerfInfo missionPerfInfo in this.m_dalService.PerformanceSystem.GetProfilePerformance(ret.ProfileID, currentMissions).Missions)
			{
				ProfilePerformanceInfo.MissionPerformance missionPerformance;
				if (!ret.MissionPerformances.TryGetValue(missionPerfInfo.MissionID.ToString(), out missionPerformance))
				{
					missionPerformance = new ProfilePerformanceInfo.MissionPerformance
					{
						MissionID = missionPerfInfo.MissionID
					};
					ret.MissionPerformances.Add(missionPerformance.MissionID.ToString(), missionPerformance);
				}
				missionPerformance.Status = missionPerfInfo.Status;
				foreach (PerformanceInfo performanceInfo in missionPerfInfo.Performances)
				{
					ProfilePerformanceInfo.StatPerformance statPerformance;
					if (!missionPerformance.StatPerformances.TryGetValue(performanceInfo.Stat, out statPerformance))
					{
						statPerformance = new ProfilePerformanceInfo.StatPerformance
						{
							Stat = performanceInfo.Stat
						};
						missionPerformance.StatPerformances.Add(statPerformance.Stat, statPerformance);
					}
					statPerformance.ProfilePerformance = performanceInfo.Performance;
				}
			}
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x0005B7CC File Offset: 0x00059BCC
		private void OnSessionStarted(IGameRoom room, string sessionId)
		{
			this.m_sessionStorage.AddData(sessionId, ESessionData.ProfilePerformanceInfo, new ProfilePerformanceSessionStorage());
		}

		// Token: 0x04000A7F RID: 2687
		private readonly IMissionGenerationService m_missionGenerationService;

		// Token: 0x04000A80 RID: 2688
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000A81 RID: 2689
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000A82 RID: 2690
		private readonly IDALService m_dalService;
	}
}
