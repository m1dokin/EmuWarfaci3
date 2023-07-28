using System;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.Attributes;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;
using MasterServer.ElectronicCatalog;

namespace MasterServer.GameLogic.JobSystem.Jobs
{
	// Token: 0x020000B2 RID: 178
	[Job("offer_synchronization")]
	internal class OfferSynchronizationJob : Job
	{
		// Token: 0x060002DA RID: 730 RVA: 0x0000DE86 File Offset: 0x0000C286
		public OfferSynchronizationJob(ConfigSection jobConfigSection, IJobScheduler jobScheduler, IJobMetricsTracker jobMetricsTracker, IShopService shopService) : base(jobConfigSection, jobScheduler, jobMetricsTracker)
		{
			this.m_shopService = shopService;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000DE99 File Offset: 0x0000C299
		protected override JobResult ExecuteImpl()
		{
			this.m_shopService.LoadOffers();
			return JobResult.Finished;
		}

		// Token: 0x04000139 RID: 313
		public const string OFFER_SYNCHRONIZATION_JOB_NAME = "offer_synchronization";

		// Token: 0x0400013A RID: 314
		private readonly IShopService m_shopService;
	}
}
