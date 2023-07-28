using System;
using MasterServer.Core.Configuration;

namespace MasterServer.Core.Web
{
	// Token: 0x02000169 RID: 361
	internal class MSWebRequestConfig
	{
		// Token: 0x0600067D RID: 1661 RVA: 0x0001A4CC File Offset: 0x000188CC
		public MSWebRequestConfig(ConfigSection sec)
		{
			int num;
			sec.Get("RequestTimeoutSec", out num);
			this.DefaultTimeout = ((num <= 0) ? TimeSpan.MinValue : TimeSpan.FromSeconds((double)num));
			sec.Get("KeepAlive", out this.KeepAlive);
			sec.Get("MaxPendingRequests", out this.MaxPendingRequests);
			int num2;
			sec.Get("AllocTimeoutMS", out num2);
			this.AllocationTimeout = ((num2 <= 0) ? TimeSpan.MinValue : TimeSpan.FromMilliseconds((double)num2));
		}

		// Token: 0x040003FE RID: 1022
		public readonly TimeSpan DefaultTimeout;

		// Token: 0x040003FF RID: 1023
		public readonly bool KeepAlive;

		// Token: 0x04000400 RID: 1024
		public readonly int MaxPendingRequests;

		// Token: 0x04000401 RID: 1025
		public readonly TimeSpan AllocationTimeout;
	}
}
