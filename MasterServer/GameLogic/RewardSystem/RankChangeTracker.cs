using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.StatsTracking;
using MasterServer.Telemetry;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005AB RID: 1451
	[OrphanService]
	[Singleton]
	internal class RankChangeTracker : ServiceModule
	{
		// Token: 0x06001F1C RID: 7964 RVA: 0x0007E745 File Offset: 0x0007CB45
		public RankChangeTracker(IRankSystem rankSystem, IStatsTracker statsTracker, IDALService dalService, ITelemetryService telemetryService)
		{
			this.m_rankSystem = rankSystem;
			this.m_statsTracker = statsTracker;
			this.m_dalService = dalService;
			this.m_telemetryService = telemetryService;
		}

		// Token: 0x06001F1D RID: 7965 RVA: 0x0007E76A File Offset: 0x0007CB6A
		public override void Start()
		{
			this.m_rankSystem.OnProfileRankChanged += this.OnProfileRankChanged;
		}

		// Token: 0x06001F1E RID: 7966 RVA: 0x0007E783 File Offset: 0x0007CB83
		public override void Stop()
		{
			this.m_rankSystem.OnProfileRankChanged -= this.OnProfileRankChanged;
		}

		// Token: 0x06001F1F RID: 7967 RVA: 0x0007E79C File Offset: 0x0007CB9C
		private void OnProfileRankChanged(SProfileInfo profile, SRankInfo newRank, SRankInfo oldRank, ILogGroup logGroup)
		{
			this.ReportRankChangeToStatistics(profile.Id, newRank.RankId, oldRank.RankId);
		}

		// Token: 0x06001F20 RID: 7968 RVA: 0x0007E7BC File Offset: 0x0007CBBC
		private void ReportRankChangeToStatistics(ulong profileId, int newRankId, int oldRankId)
		{
			this.m_statsTracker.ResetStatistics(profileId, EStatsEvent.RANK_CHANGED, (uint)newRankId);
			TimeSpan timeSpan = this.m_dalService.ProfileSystem.UpdateTimeToRank(profileId);
			this.m_telemetryService.AddMeasure((long)timeSpan.TotalMilliseconds / 100L, new object[]
			{
				"stat",
				"srv_time_to_rank",
				"level",
				oldRankId,
				"date",
				DateTime.Now.ToString("yyyy-MM-01")
			});
		}

		// Token: 0x04000F2A RID: 3882
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000F2B RID: 3883
		private readonly IStatsTracker m_statsTracker;

		// Token: 0x04000F2C RID: 3884
		private readonly IDALService m_dalService;

		// Token: 0x04000F2D RID: 3885
		private readonly ITelemetryService m_telemetryService;
	}
}
