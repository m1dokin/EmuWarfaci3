using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Enyim.Caching.Memcached;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;

namespace MasterServer.Database
{
	// Token: 0x020001C4 RID: 452
	internal class DALCacheProxy<TDAL> : DALProxyBase<TDAL> where TDAL : class, IBaseDALService
	{
		// Token: 0x170000ED RID: 237
		// (get) Token: 0x0600086E RID: 2158 RVA: 0x00008AB8 File Offset: 0x00006EB8
		protected IObjectCache L1Cache
		{
			get
			{
				QueryContext queryContext = QueryContext.Current;
				return (queryContext == null) ? NullCache.Instance : queryContext.L1Cache;
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600086F RID: 2159 RVA: 0x00008AE4 File Offset: 0x00006EE4
		protected IObjectCache L2Cache
		{
			get
			{
				bool flag = ServicesManager.ExecutionPhase >= ExecutionPhase.Starting && base.DAL.MemcachedService != null && base.DAL.MemcachedService.Connected;
				return (!flag) ? NullCache.Instance : base.DAL.MemcachedService;
			}
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x00008B3C File Offset: 0x00006F3C
		private bool CacheLoad(DALTracker tr, DALCacheProxy<TDAL>.OptionsBase options, out object data)
		{
			data = null;
			if (options.cache_domain == null)
			{
				return false;
			}
			bool result;
			using (DALTracker.CTimeCache ctimeCache = tr.TimeCache(CacheOp.Load))
			{
				bool flag = this.L1Cache.Get(options.cache_domain, out data);
				ctimeCache.CacheHit(CacheLevel.L1, flag);
				if (flag)
				{
					result = true;
				}
				else
				{
					flag = this.L2Cache.Get(options.cache_domain, out data);
					ctimeCache.CacheHit(CacheLevel.L2, flag);
					if (flag)
					{
						this.L1Cache.Store(StoreMode.Set, options.cache_domain, data, options.cache_expiration);
					}
					result = flag;
				}
			}
			return result;
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x00008BEC File Offset: 0x00006FEC
		private void CacheStore(DALTracker tr, DALCacheProxy<TDAL>.OptionsBase options, object data)
		{
			if (options.cache_domain == null)
			{
				return;
			}
			using (tr.TimeCache(CacheOp.Store))
			{
				this.L1Cache.Store(StoreMode.Set, options.cache_domain, data, options.cache_expiration);
				this.L2Cache.Store(StoreMode.Set, options.cache_domain, data, options.cache_expiration);
			}
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x00008C64 File Offset: 0x00007064
		protected T PureGetData<T>(DALCacheProxy<TDAL>.Options<T> options, [CallerMemberName] string methodName = "")
		{
			T result;
			using (DALTracker daltracker = new DALTracker(base.DAL, methodName))
			{
				object obj;
				if (this.CacheLoad(daltracker, options, out obj))
				{
					result = ((obj == null) ? default(T) : ((T)((object)obj)));
				}
				else
				{
					result = default(T);
				}
			}
			return result;
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x00008CDC File Offset: 0x000070DC
		protected T GetData<T>(MethodBase method, DALCacheProxy<TDAL>.Options<T> options)
		{
			T result;
			using (DALTracker daltracker = new DALTracker(base.DAL, method.Name))
			{
				object obj;
				if (this.CacheLoad(daltracker, options, out obj))
				{
					result = ((obj == null) ? default(T) : ((T)((object)obj)));
				}
				else
				{
					T t = base.Query<T>(daltracker, options.get_data, method.Name, options.query_retry);
					object obj2 = EqualityComparer<T>.Default.Equals(t, default(T)) ? null : t;
					this.CacheStore(daltracker, options, t);
					result = t;
				}
			}
			return result;
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x00008DA4 File Offset: 0x000071A4
		protected IEnumerable<T> GetDataStream<T>(MethodBase method, DALCacheProxy<TDAL>.Options<T> options)
		{
			IEnumerable<T> result;
			using (DALTracker daltracker = new DALTracker(base.DAL, method.Name))
			{
				object obj;
				if (this.CacheLoad(daltracker, options, out obj))
				{
					result = ((obj == null) ? new List<T>() : ((List<T>)obj));
				}
				else
				{
					List<T> list = (List<T>)base.Query<T>(daltracker, options.get_data_stream, method.Name, options.query_retry);
					object obj2 = (list == null || list.Count == 0) ? null : list;
					this.CacheStore(daltracker, options, list);
					result = list;
				}
			}
			return result;
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x00008E58 File Offset: 0x00007258
		protected void SetAndClear(MethodBase method, DALCacheProxy<TDAL>.SetOptions options)
		{
			using (DALTracker daltracker = new DALTracker(base.DAL, method.Name))
			{
				base.Query(daltracker, options.set_func, method.Name, options.query_retry);
				this.ClearCache(daltracker, options);
			}
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x00008EBC File Offset: 0x000072BC
		protected T SetAndClearScalar<T>(MethodBase method, DALCacheProxy<TDAL>.SetOptionsScalar<T> options)
		{
			T result;
			using (DALTracker daltracker = new DALTracker(base.DAL, method.Name))
			{
				T t = base.Query<T>(daltracker, options.set_func, method.Name, options.query_retry);
				this.ClearCache(daltracker, options);
				result = t;
			}
			return result;
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x00008F24 File Offset: 0x00007324
		protected IEnumerable<T> SetAndClearStream<T>(MethodBase method, DALCacheProxy<TDAL>.SetOptionsStream<T> options)
		{
			IEnumerable<T> result;
			using (DALTracker daltracker = new DALTracker(base.DAL, method.Name))
			{
				IEnumerable<T> enumerable = base.Query<T>(daltracker, options.set_func, method.Name, options.query_retry);
				this.ClearCache(daltracker, options);
				result = enumerable;
			}
			return result;
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x00008F8C File Offset: 0x0000738C
		protected void PureStore<T>(DALCacheProxy<TDAL>.SetOptionsScalar<T> options, [CallerMemberName] string methodName = "")
		{
			using (DALTracker daltracker = new DALTracker(base.DAL, methodName))
			{
				T value = options.set_func().Value;
				this.CacheStore(daltracker, options, value);
			}
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x00008FE8 File Offset: 0x000073E8
		protected T SetAndStore<T>(MethodBase method, DALCacheProxy<TDAL>.SetOptionsScalar<T> options)
		{
			T result;
			using (DALTracker daltracker = new DALTracker(base.DAL, method.Name))
			{
				T t = base.Query<T>(daltracker, options.set_func, method.Name, options.query_retry);
				using (daltracker.TimeCache(CacheOp.Store))
				{
					this.L1Cache.Store(StoreMode.Replace, options.cache_domain, t);
					this.L2Cache.Store(StoreMode.Replace, options.cache_domain, t);
				}
				result = t;
			}
			return result;
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x000090A0 File Offset: 0x000074A0
		protected void CheckAndStore<T>(MethodBase method, DALCacheProxy<TDAL>.CASOptions<T> options)
		{
			using (DALTracker daltracker = new DALTracker(base.DAL, method.Name))
			{
				this.L1Cache.Remove(options.cache_domain);
				bool flag = false;
				int num = 0;
				while (!flag && num++ < 5)
				{
					ulong cas;
					T cachedValue;
					using (DALTracker.CTimeCache ctimeCache = daltracker.TimeCache(CacheOp.Load))
					{
						object obj;
						bool flag2 = this.L2Cache.Get(options.cache_domain, out obj, out cas);
						ctimeCache.CacheHit(CacheLevel.L2, flag2);
						if (!flag2)
						{
							break;
						}
						cachedValue = ((obj == null) ? default(T) : ((T)((object)obj)));
					}
					T t;
					bool flag3 = options.cas_func(cachedValue, out t);
					if (flag3)
					{
						using (daltracker.TimeCache(CacheOp.Store))
						{
							object value = EqualityComparer<T>.Default.Equals(t, default(T)) ? null : t;
							flag = this.L2Cache.CheckAndSet(options.cache_domain, value, cas);
						}
					}
					else
					{
						flag = true;
					}
				}
				if (!flag)
				{
					this.ClearCache(daltracker, options.cache_domain);
				}
			}
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x00009240 File Offset: 0x00007640
		protected void ClearCache(MethodBase method, DALCacheProxy<TDAL>.SetOptionsBase options)
		{
			using (DALTracker daltracker = new DALTracker(base.DAL, method.Name))
			{
				this.ClearCache(daltracker, options);
			}
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x0000928C File Offset: 0x0000768C
		private void ClearCache(DALTracker tr, DALCacheProxy<TDAL>.SetOptionsBase options)
		{
			if (options.cache_domain != null)
			{
				this.ClearCache(tr, options.cache_domain);
			}
			if (options.cache_domains != null)
			{
				this.ClearCache(tr, options.cache_domains);
			}
		}

		// Token: 0x0600087D RID: 2173 RVA: 0x000092C0 File Offset: 0x000076C0
		private void ClearCache(DALTracker tr, cache_domain cacheDomain)
		{
			cache_domain[] cacheDomains = new cache_domain[]
			{
				cacheDomain
			};
			this.ClearCache(tr, cacheDomains);
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x000092E0 File Offset: 0x000076E0
		private void ClearCache(DALTracker tr, IEnumerable<cache_domain> cacheDomains)
		{
			using (DALTracker.CTimeCache ctimeCache = tr.TimeCache(CacheOp.Clear))
			{
				foreach (cache_domain domain in cacheDomains.SelectMany((cache_domain d) => d.Terminals()))
				{
					this.L1Cache.Remove(domain);
					ctimeCache.CacheClear(CacheLevel.L1);
					this.L2Cache.Remove(domain);
					ctimeCache.CacheClear(CacheLevel.L2);
				}
			}
		}

		// Token: 0x020001C5 RID: 453
		public class OptionsBase
		{
			// Token: 0x040004FA RID: 1274
			public int query_retry;

			// Token: 0x040004FB RID: 1275
			public cache_domain cache_domain;

			// Token: 0x040004FC RID: 1276
			public TimeSpan cache_expiration;
		}

		// Token: 0x020001C6 RID: 454
		public class Options<T> : DALCacheProxy<TDAL>.OptionsBase
		{
			// Token: 0x040004FD RID: 1277
			public Func<DALResult<T>> get_data;

			// Token: 0x040004FE RID: 1278
			public Func<DALResultMulti<T>> get_data_stream;
		}

		// Token: 0x020001C7 RID: 455
		public class SetOptionsBase : DALCacheProxy<TDAL>.OptionsBase
		{
			// Token: 0x040004FF RID: 1279
			public IEnumerable<cache_domain> cache_domains;
		}

		// Token: 0x020001C8 RID: 456
		public class SetOptions : DALCacheProxy<TDAL>.SetOptionsBase
		{
			// Token: 0x04000500 RID: 1280
			public Func<DALResultVoid> set_func;
		}

		// Token: 0x020001C9 RID: 457
		public class SetOptionsScalar<T> : DALCacheProxy<TDAL>.SetOptionsBase
		{
			// Token: 0x04000501 RID: 1281
			public Func<DALResult<T>> set_func;
		}

		// Token: 0x020001CA RID: 458
		public class SetOptionsStream<T> : DALCacheProxy<TDAL>.SetOptionsBase
		{
			// Token: 0x04000502 RID: 1282
			public Func<DALResultMulti<T>> set_func;
		}

		// Token: 0x020001CB RID: 459
		public class CASOptions<T> : DALCacheProxy<TDAL>.OptionsBase
		{
			// Token: 0x04000503 RID: 1283
			public DALCacheProxy<TDAL>.CASOptions<T>.CheckAndStoreDeleg cas_func;

			// Token: 0x020001CC RID: 460
			// (Invoke) Token: 0x06000888 RID: 2184
			public delegate bool CheckAndStoreDeleg(T cachedValue, out T newValue);
		}
	}
}
