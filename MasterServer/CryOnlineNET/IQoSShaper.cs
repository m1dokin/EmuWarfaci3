using System;
using System.Text;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001A7 RID: 423
	public interface IQoSShaper
	{
		// Token: 0x060007DC RID: 2012
		ShaperDecision IncomingWorkItem(WorkItem item);

		// Token: 0x060007DD RID: 2013
		void WorkItemFinished(WorkItem finished);

		// Token: 0x060007DE RID: 2014
		WorkItem DequeueWorkItem(WorkItem finished);

		// Token: 0x060007DF RID: 2015
		void FillMemoryUsageInfo(StringBuilder stringBuidler);
	}
}
