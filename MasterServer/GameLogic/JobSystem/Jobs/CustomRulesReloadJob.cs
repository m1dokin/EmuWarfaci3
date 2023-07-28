using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.GameLogic.CustomRules;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x0200038D RID: 909
	[Job("custom_rules_reload")]
	internal class CustomRulesReloadJob : Job
	{
		// Token: 0x06001466 RID: 5222 RVA: 0x00052DEC File Offset: 0x000511EC
		public CustomRulesReloadJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IJobMetricsTracker jobMetricsTracker, ICustomRulesService rulesService) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_rulesService = rulesService;
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x00052DFF File Offset: 0x000511FF
		protected override JobResult ExecuteImpl()
		{
			this.m_rulesService.ReloadRules();
			return JobResult.Finished;
		}

		// Token: 0x0400097E RID: 2430
		public const string CUSTOM_RULES_RELOAD_JOB_NAME = "custom_rules_reload";

		// Token: 0x0400097F RID: 2431
		private readonly ICustomRulesService m_rulesService;
	}
}
