using System;
using HK2Net;

namespace MasterServer.Core
{
	// Token: 0x02000165 RID: 357
	[Service]
	[Singleton]
	internal class SystemTimeSource : ITimeSource
	{
		// Token: 0x06000666 RID: 1638 RVA: 0x0001A264 File Offset: 0x00018664
		public DateTime Now()
		{
			return DateTime.Now;
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0001A26B File Offset: 0x0001866B
		public DateTime UtcNow()
		{
			return DateTime.UtcNow;
		}
	}
}
