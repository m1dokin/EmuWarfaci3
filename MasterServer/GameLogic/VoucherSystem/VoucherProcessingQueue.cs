using System;
using System.Threading.Tasks;
using MasterServer.Core;
using MasterServer.Telemetry.Metrics;

namespace MasterServer.GameLogic.VoucherSystem
{
	// Token: 0x020000F5 RID: 245
	internal class VoucherProcessingQueue : ProcessingQueue<Task>
	{
		// Token: 0x06000407 RID: 1031 RVA: 0x00011AE0 File Offset: 0x0000FEE0
		public VoucherProcessingQueue(IProcessingQueueMetricsTracker processingQueueMetricsTracker) : base("VoucherProcessingQueue", processingQueueMetricsTracker, true)
		{
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00011AEF File Offset: 0x0000FEEF
		public override void Process(Task task)
		{
			task.Wait();
		}
	}
}
