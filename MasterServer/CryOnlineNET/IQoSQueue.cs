using System;
using System.Threading;
using System.Threading.Tasks;
using HK2Net;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001A6 RID: 422
	[Contract]
	public interface IQoSQueue
	{
		// Token: 0x060007D7 RID: 2007
		IQoSShaper[] GetShapers();

		// Token: 0x060007D8 RID: 2008
		bool QueueWorkItem(TShapingInfo shaping_info, WaitCallback callback);

		// Token: 0x060007D9 RID: 2009
		bool QueueWorkItem(TShapingInfo shaping_info, WaitCallback callback, object state);

		// Token: 0x060007DA RID: 2010
		bool QueueAsyncWorkItem(TShapingInfo shaping_info, Func<object, Task> async_callback);

		// Token: 0x060007DB RID: 2011
		bool QueueAsyncWorkItem(TShapingInfo shaping_info, Func<object, Task> async_callback, object state);
	}
}
