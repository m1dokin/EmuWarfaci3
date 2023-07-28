using System;
using System.Threading;
using MasterServer.Core;
using MasterServer.Core.Services.Jobs;
using MasterServer.Core.Services.Jobs.JobSchedulers;
using MasterServer.Core.Services.Jobs.Metrics;

namespace MasterServer.GameLogic.JobSystem.Jobs.Debug
{
	// Token: 0x0200038F RID: 911
	internal class DebugSimpleJob : Job
	{
		// Token: 0x0600146A RID: 5226 RVA: 0x00052E49 File Offset: 0x00051249
		public DebugSimpleJob(string name, byte priority, IJobMetricsTracker jobMetricsTracker) : base(name, priority, new SimpleJobScheduler(), jobMetricsTracker)
		{
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x00052E59 File Offset: 0x00051259
		protected override JobResult ExecuteImpl()
		{
			Log.Info<DebugSimpleJob>("{0} is working now!", this);
			Thread.Sleep(TimeSpan.FromSeconds(30.0));
			return JobResult.Finished;
		}
	}
}
