using System;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.ClanSystem;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x02000389 RID: 905
	[Job("clan_fixup")]
	internal class ClanFixupJob : Job
	{
		// Token: 0x0600145C RID: 5212 RVA: 0x00052C8A File Offset: 0x0005108A
		public ClanFixupJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IJobMetricsTracker jobMetricsTracker, IClanService clanService) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_clanService = clanService;
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x00052CA0 File Offset: 0x000510A0
		protected override JobResult ExecuteImpl()
		{
			uint p = this.m_clanService.FixupClans();
			Log.Info<uint>("Clans fixed: {0}", p);
			return JobResult.Finished;
		}

		// Token: 0x04000974 RID: 2420
		public const string CLAN_FIXUP_JOB_NAME = "clan_fixup";

		// Token: 0x04000975 RID: 2421
		private readonly IClanService m_clanService;
	}
}
