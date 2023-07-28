using System;
using Enyim.Caching.Memcached;

namespace MasterServer.Database
{
	// Token: 0x020001DC RID: 476
	public interface IObjectCache
	{
		// Token: 0x06000921 RID: 2337
		bool Get(cache_domain domain, out object result);

		// Token: 0x06000922 RID: 2338
		bool Get(cache_domain domain, out object result, out ulong cas);

		// Token: 0x06000923 RID: 2339
		bool Store(StoreMode mode, cache_domain domain, object value);

		// Token: 0x06000924 RID: 2340
		bool Store(StoreMode mode, cache_domain domain, object value, TimeSpan validFor);

		// Token: 0x06000925 RID: 2341
		bool Store(StoreMode mode, cache_domain domain, object value, DateTime expiresAt);

		// Token: 0x06000926 RID: 2342
		bool CheckAndSet(cache_domain domain, object value, ulong cas);

		// Token: 0x06000927 RID: 2343
		bool CheckAndSet(cache_domain domain, object value, ulong cas, TimeSpan validFor);

		// Token: 0x06000928 RID: 2344
		bool CheckAndSet(cache_domain domain, object value, ulong cas, DateTime expiresAt);

		// Token: 0x06000929 RID: 2345
		void Remove(cache_domain domain);

		// Token: 0x0600092A RID: 2346
		void Clear();
	}
}
