using System;
using System.Collections.Concurrent;
using Enyim.Caching.Memcached;

namespace MasterServer.Database
{
	// Token: 0x020001DA RID: 474
	internal class L1Cache : IObjectCache
	{
		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000913 RID: 2323 RVA: 0x00022452 File Offset: 0x00020852
		// (set) Token: 0x06000914 RID: 2324 RVA: 0x0002245A File Offset: 0x0002085A
		public bool Enabled { get; private set; }

		// Token: 0x06000915 RID: 2325 RVA: 0x00022463 File Offset: 0x00020863
		public bool Get(cache_domain domain, out object result)
		{
			return this.m_cache.TryGetValue(this.get_key(domain), out result);
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x00022478 File Offset: 0x00020878
		public bool Get(cache_domain domain, out object result, out ulong cas)
		{
			cas = 0UL;
			return this.Get(domain, out result);
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x00022488 File Offset: 0x00020888
		public bool Store(StoreMode mode, cache_domain domain, object value)
		{
			string key = this.get_key(domain);
			switch (mode)
			{
			case StoreMode.Add:
				return this.m_cache.TryAdd(key, value);
			case StoreMode.Replace:
			{
				object comparisonValue;
				while (this.m_cache.TryGetValue(key, out comparisonValue))
				{
					if (this.m_cache.TryUpdate(key, value, comparisonValue))
					{
						return true;
					}
				}
				return false;
			}
			case StoreMode.Set:
				this.m_cache[key] = value;
				return true;
			default:
				throw new NotSupportedException();
			}
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x00022507 File Offset: 0x00020907
		public bool Store(StoreMode mode, cache_domain domain, object value, TimeSpan validFor)
		{
			return this.Store(mode, domain, value);
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x00022512 File Offset: 0x00020912
		public bool Store(StoreMode mode, cache_domain domain, object value, DateTime expiresAt)
		{
			return this.Store(mode, domain, value);
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x0002251D File Offset: 0x0002091D
		public bool CheckAndSet(cache_domain domain, object value, ulong cas)
		{
			return this.Store(StoreMode.Set, domain, value);
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x00022528 File Offset: 0x00020928
		public bool CheckAndSet(cache_domain domain, object value, ulong cas, TimeSpan validFor)
		{
			return this.Store(StoreMode.Set, domain, value);
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x00022533 File Offset: 0x00020933
		public bool CheckAndSet(cache_domain domain, object value, ulong cas, DateTime expiresAt)
		{
			return this.Store(StoreMode.Set, domain, value);
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x00022540 File Offset: 0x00020940
		public void Remove(cache_domain domain)
		{
			object obj;
			this.m_cache.TryRemove(this.get_key(domain), out obj);
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x00022562 File Offset: 0x00020962
		public void Clear()
		{
			this.m_cache.Clear();
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x0002256F File Offset: 0x0002096F
		private string get_key(cache_domain domain)
		{
			return domain.ToString();
		}

		// Token: 0x0400053F RID: 1343
		private ConcurrentDictionary<string, object> m_cache = new ConcurrentDictionary<string, object>();
	}
}
