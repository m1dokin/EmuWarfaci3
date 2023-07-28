using System;
using System.Collections.Generic;
using Enyim.Caching.Memcached;
using HK2Net;

namespace MasterServer.Database
{
	// Token: 0x020001DD RID: 477
	[Contract]
	internal interface IMemcachedService : IObjectCache
	{
		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600092B RID: 2347
		bool Connected { get; }

		// Token: 0x0600092C RID: 2348
		ServerStats Stats();

		// Token: 0x0600092D RID: 2349
		List<MemcachedServer> GetServerList();
	}
}
