using System;
using System.Threading.Tasks;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001A5 RID: 421
	public class WorkItem
	{
		// Token: 0x060007D5 RID: 2005 RVA: 0x0001DF63 File Offset: 0x0001C363
		public WorkItem(TShapingInfo shaping_info, Func<object, Task> callback, object state)
		{
			this.shaping_info = shaping_info;
			this.callback = callback;
			this.state = state;
			this.queue_time = DateTime.UtcNow;
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0001DF8B File Offset: 0x0001C38B
		public override string ToString()
		{
			return string.Format("WorkItem: From='{0}', Name='{1}', QueueTime='{2}'", this.shaping_info.from_jid, this.shaping_info.query_name, this.queue_time);
		}

		// Token: 0x040004A1 RID: 1185
		public TShapingInfo shaping_info;

		// Token: 0x040004A2 RID: 1186
		public Func<object, Task> callback;

		// Token: 0x040004A3 RID: 1187
		public object state;

		// Token: 0x040004A4 RID: 1188
		public DateTime queue_time;
	}
}
