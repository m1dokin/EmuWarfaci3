using System;
using System.Text;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001B1 RID: 433
	internal class TimeoutShaper : IQoSShaper
	{
		// Token: 0x06000815 RID: 2069 RVA: 0x0001F2BD File Offset: 0x0001D6BD
		public TimeoutShaper()
		{
			Resources.QoSSettings.GetSection("timeout_shaper").Get("queue_ttl_sec", out this.queue_ttl_sec);
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x0001F2E4 File Offset: 0x0001D6E4
		public ShaperDecision IncomingWorkItem(WorkItem item)
		{
			if (this.queue_ttl_sec > 0)
			{
				TimeSpan timeSpan = DateTime.UtcNow - item.queue_time;
				if (timeSpan.TotalSeconds > (double)this.queue_ttl_sec)
				{
					Log.Warning<string, double>("Drop query {0} from the timeout shaper queue, ttl {1}", item.shaping_info.query_name, timeSpan.TotalSeconds);
					return ShaperDecision.Discard;
				}
			}
			return ShaperDecision.Execute;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0001F340 File Offset: 0x0001D740
		public void WorkItemFinished(WorkItem finished)
		{
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x0001F342 File Offset: 0x0001D742
		public WorkItem DequeueWorkItem(WorkItem finished)
		{
			return null;
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x0001F345 File Offset: 0x0001D745
		public void FillMemoryUsageInfo(StringBuilder stringBuidler)
		{
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x0001F347 File Offset: 0x0001D747
		public override string ToString()
		{
			return "[TimeoutShaper]:";
		}

		// Token: 0x040004BF RID: 1215
		private int queue_ttl_sec;
	}
}
