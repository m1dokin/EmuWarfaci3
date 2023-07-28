using System;
using Enyim.Caching.Memcached;

namespace MasterServer.Database
{
	// Token: 0x020001D9 RID: 473
	internal class NullCache : IObjectCache
	{
		// Token: 0x06000907 RID: 2311 RVA: 0x0002240D File Offset: 0x0002080D
		public bool Get(cache_domain domain, out object result)
		{
			result = null;
			return false;
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x00022413 File Offset: 0x00020813
		public bool Get(cache_domain domain, out object result, out ulong cas)
		{
			result = null;
			cas = 0UL;
			return false;
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x0002241D File Offset: 0x0002081D
		public bool Store(StoreMode mode, cache_domain domain, object value)
		{
			return false;
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x00022420 File Offset: 0x00020820
		public bool Store(StoreMode mode, cache_domain domain, object value, TimeSpan validFor)
		{
			return false;
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x00022423 File Offset: 0x00020823
		public bool Store(StoreMode mode, cache_domain domain, object value, DateTime expiresAt)
		{
			return false;
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x00022426 File Offset: 0x00020826
		public bool CheckAndSet(cache_domain domain, object value, ulong cas)
		{
			return false;
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x00022429 File Offset: 0x00020829
		public bool CheckAndSet(cache_domain domain, object value, ulong cas, TimeSpan validFor)
		{
			return false;
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0002242C File Offset: 0x0002082C
		public bool CheckAndSet(cache_domain domain, object value, ulong cas, DateTime expiresAt)
		{
			return false;
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x0002242F File Offset: 0x0002082F
		public void Remove(cache_domain domain)
		{
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x00022431 File Offset: 0x00020831
		public void Clear()
		{
		}

		// Token: 0x0400053D RID: 1341
		public static NullCache Instance = new NullCache();
	}
}
