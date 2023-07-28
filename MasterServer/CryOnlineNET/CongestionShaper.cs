using System;
using System.Text;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001A9 RID: 425
	internal class CongestionShaper : IQoSShaper
	{
		// Token: 0x060007ED RID: 2029 RVA: 0x0001E425 File Offset: 0x0001C825
		public CongestionShaper()
		{
			Resources.QoSSettings.GetSection("congestion_shaper").Get("max_pending_requests", out this.max_in_progress);
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0001E44C File Offset: 0x0001C84C
		public ShaperDecision IncomingWorkItem(WorkItem item)
		{
			if (this.max_in_progress == 0 || this.tasks_in_progress < this.max_in_progress)
			{
				this.tasks_in_progress++;
				return ShaperDecision.Execute;
			}
			Log.Warning<int>("[CongestionShaper] QoS queue limit of {0} reached", this.tasks_in_progress);
			return ShaperDecision.Block;
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x0001E48B File Offset: 0x0001C88B
		public void WorkItemFinished(WorkItem finished)
		{
			if (this.tasks_in_progress >= this.max_in_progress)
			{
				Log.Warning("[CongestionShaper] Resumed");
			}
			this.tasks_in_progress--;
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0001E4B6 File Offset: 0x0001C8B6
		public WorkItem DequeueWorkItem(WorkItem finished)
		{
			return null;
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0001E4B9 File Offset: 0x0001C8B9
		public void FillMemoryUsageInfo(StringBuilder stringBuidler)
		{
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0001E4BB File Offset: 0x0001C8BB
		public override string ToString()
		{
			return string.Format("[CongestionShaper]: Tasks in progress {0}", this.tasks_in_progress);
		}

		// Token: 0x040004A8 RID: 1192
		private int max_in_progress;

		// Token: 0x040004A9 RID: 1193
		private int tasks_in_progress;
	}
}
