using System;
using System.Linq;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;

namespace MasterServer.Users
{
	// Token: 0x02000751 RID: 1873
	[Job("supported_client_versions_sync")]
	internal class ClientVersionsSyncJob : Job
	{
		// Token: 0x060026AB RID: 9899 RVA: 0x000A3E44 File Offset: 0x000A2244
		public ClientVersionsSyncJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IClientVersionsManagementService clientVersionsManagementService, IJobMetricsTracker jobMetricsTracker) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_clientVersionsManagementService = clientVersionsManagementService;
		}

		// Token: 0x060026AC RID: 9900 RVA: 0x000A3E58 File Offset: 0x000A2258
		protected override JobResult ExecuteImpl()
		{
			this.m_clientVersionsManagementService.SyncClientVersions();
			string p = string.Join("; ", this.m_clientVersionsManagementService.GetClientVersions().ToArray<string>());
			Log.Info<string>("[ClientVersionsSyncJob] Supported client versions: {0}", p);
			return JobResult.Finished;
		}

		// Token: 0x040013EE RID: 5102
		public const string CLIENT_VERSIONS_SYNC_JOB_NAME = "supported_client_versions_sync";

		// Token: 0x040013EF RID: 5103
		private readonly IClientVersionsManagementService m_clientVersionsManagementService;
	}
}
