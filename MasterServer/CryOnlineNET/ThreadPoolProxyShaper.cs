using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001B0 RID: 432
	internal class ThreadPoolProxyShaper : IQoSShaper
	{
		// Token: 0x0600080E RID: 2062 RVA: 0x0001EFF3 File Offset: 0x0001D3F3
		public ThreadPoolProxyShaper()
		{
			Resources.QoSSettings.GetSection("thread_shaper").Get("max_executed_items", out this.max_executed_items);
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0001F030 File Offset: 0x0001D430
		public ShaperDecision IncomingWorkItem(WorkItem item)
		{
			if (this.max_executed_items == 0 || this.max_executed_items > this.total_executed_items)
			{
				this.total_executed_items++;
				this.executing_items.Add(item);
				return ShaperDecision.Execute;
			}
			this.queued_items.Enqueue(item);
			return ShaperDecision.Queued;
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0001F082 File Offset: 0x0001D482
		public void WorkItemFinished(WorkItem finished)
		{
			this.total_executed_items--;
			this.executing_items.Remove(finished);
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0001F0A0 File Offset: 0x0001D4A0
		public WorkItem DequeueWorkItem(WorkItem finished)
		{
			if (this.queued_items.Count > 0)
			{
				this.total_executed_items++;
				WorkItem workItem = this.queued_items.Dequeue();
				this.executing_items.Add(workItem);
				return workItem;
			}
			return null;
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x0001F0E7 File Offset: 0x0001D4E7
		public void FillMemoryUsageInfo(StringBuilder stringBuidler)
		{
			stringBuidler.AppendFormat("QoS Queue Size: {0}\n", this.queued_items.Count);
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0001F108 File Offset: 0x0001D508
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[ThreadPoolProxyShaper]:");
			stringBuilder.AppendLine(string.Format("\tExecuting:", new object[0]));
			foreach (WorkItem arg in this.executing_items)
			{
				stringBuilder.AppendLine(string.Format("\t\t{0}", arg));
			}
			stringBuilder.AppendLine(string.Format("\tSummary:", new object[0]));
			foreach (IGrouping<string, WorkItem> grouping in from qi in this.queued_items
			group qi by qi.shaping_info.query_name)
			{
				stringBuilder.AppendLine(string.Format("\t\t{0}:{1}", grouping.Key, grouping.Count<WorkItem>()));
			}
			stringBuilder.AppendLine(string.Format("\tQueue:", new object[0]));
			foreach (WorkItem arg2 in this.queued_items)
			{
				stringBuilder.AppendLine(string.Format("\t\t{0}", arg2));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040004BA RID: 1210
		private int total_executed_items;

		// Token: 0x040004BB RID: 1211
		private int max_executed_items;

		// Token: 0x040004BC RID: 1212
		private List<WorkItem> executing_items = new List<WorkItem>();

		// Token: 0x040004BD RID: 1213
		private Queue<WorkItem> queued_items = new Queue<WorkItem>();
	}
}
