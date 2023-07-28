using System;
using System.Collections.Generic;
using System.Threading;
using MasterServer.Core.Timers;

namespace MasterServer.Core
{
	// Token: 0x0200002C RID: 44
	public class CacheDictionary<TKey, TValue> : IDisposable
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00006E36 File Offset: 0x00005236
		public CacheDictionary(int cacheTimeoutSec) : this(TimeSpan.FromSeconds((double)cacheTimeoutSec))
		{
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006E45 File Offset: 0x00005245
		public CacheDictionary(int cacheTimeoutSec, CacheDictionaryMode mode) : this(TimeSpan.FromSeconds((double)cacheTimeoutSec), mode)
		{
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006E55 File Offset: 0x00005255
		public CacheDictionary(TimeSpan cacheTimeout) : this(cacheTimeout, CacheDictionaryMode.Touch)
		{
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006E60 File Offset: 0x00005260
		public CacheDictionary(TimeSpan cacheTimeout, CacheDictionaryMode mode)
		{
			this.m_mode = mode;
			this.m_cacheTimeout = cacheTimeout;
			this.m_cleanCache = new SafeTimer(new TimerCallback(this.CleanCache), this, this.m_cacheTimeout, this.m_cacheTimeout);
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600009D RID: 157 RVA: 0x00006EB0 File Offset: 0x000052B0
		// (remove) Token: 0x0600009E RID: 158 RVA: 0x00006EE8 File Offset: 0x000052E8
		public event CacheDictionary<TKey, TValue>.ExpirationDeleg ItemExpired;

		// Token: 0x0600009F RID: 159 RVA: 0x00006F1E File Offset: 0x0000531E
		public void Dispose()
		{
			this.Clear();
			this.m_cleanCache.Dispose();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00006F31 File Offset: 0x00005331
		public void ChangeTimeout(int cacheTimeoutSec)
		{
			this.ChangeTimeout(TimeSpan.FromSeconds((double)cacheTimeoutSec));
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006F40 File Offset: 0x00005340
		public void ChangeTimeout(TimeSpan cacheTimeout)
		{
			object cache = this.m_cache;
			lock (cache)
			{
				this.m_cacheTimeout = cacheTimeout;
				this.m_cleanCache.Change(cacheTimeout, cacheTimeout);
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00006F94 File Offset: 0x00005394
		public bool Add(TKey key, TValue data)
		{
			object cache = this.m_cache;
			bool result;
			lock (cache)
			{
				if (!this.m_cache.ContainsKey(key))
				{
					this.m_cache.Add(key, new CacheDictionary<TKey, TValue>.CacheEntity
					{
						data = data,
						last_accessed = DateTime.Now
					});
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00007014 File Offset: 0x00005414
		public void AddNew(TKey key, TValue data)
		{
			object cache = this.m_cache;
			lock (cache)
			{
				this.m_cache.Add(key, new CacheDictionary<TKey, TValue>.CacheEntity
				{
					data = data,
					last_accessed = DateTime.Now
				});
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00007078 File Offset: 0x00005478
		public TValue Replace(TKey key, TValue data)
		{
			object cache = this.m_cache;
			TValue result;
			lock (cache)
			{
				TValue tvalue = default(TValue);
				CacheDictionary<TKey, TValue>.CacheEntity cacheEntity;
				if (this.m_cache.TryGetValue(key, out cacheEntity))
				{
					cacheEntity.last_accessed = DateTime.Now;
					tvalue = cacheEntity.data;
					cacheEntity.data = data;
				}
				else
				{
					this.m_cache.Add(key, new CacheDictionary<TKey, TValue>.CacheEntity
					{
						data = data,
						last_accessed = DateTime.Now
					});
				}
				result = tvalue;
			}
			return result;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00007120 File Offset: 0x00005520
		public bool AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
		{
			object cache = this.m_cache;
			bool result;
			lock (cache)
			{
				CacheDictionary<TKey, TValue>.CacheEntity cacheEntity;
				if (!this.m_cache.TryGetValue(key, out cacheEntity))
				{
					cacheEntity = new CacheDictionary<TKey, TValue>.CacheEntity
					{
						data = addValueFactory(key),
						last_accessed = DateTime.Now
					};
					this.m_cache.Add(key, cacheEntity);
					result = false;
				}
				else
				{
					cacheEntity.data = updateValueFactory(key, cacheEntity.data);
					if (this.m_mode != CacheDictionaryMode.Expiration)
					{
						cacheEntity.last_accessed = DateTime.Now;
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000071D4 File Offset: 0x000055D4
		public bool Update(TKey key, Func<TKey, TValue, TValue> updateValueFactory)
		{
			object cache = this.m_cache;
			bool result;
			lock (cache)
			{
				CacheDictionary<TKey, TValue>.CacheEntity cacheEntity;
				if (!this.m_cache.TryGetValue(key, out cacheEntity))
				{
					result = false;
				}
				else
				{
					cacheEntity.data = updateValueFactory(key, cacheEntity.data);
					if (this.m_mode != CacheDictionaryMode.Expiration)
					{
						cacheEntity.last_accessed = DateTime.Now;
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00007258 File Offset: 0x00005658
		public bool Pop(TKey key, out TValue val)
		{
			object cache = this.m_cache;
			bool result;
			lock (cache)
			{
				CacheDictionary<TKey, TValue>.CacheEntity cacheEntity;
				if (!this.m_cache.TryGetValue(key, out cacheEntity))
				{
					val = default(TValue);
					result = false;
				}
				else
				{
					val = cacheEntity.data;
					this.m_cache.Remove(key);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000072E0 File Offset: 0x000056E0
		public bool Remove(TKey key)
		{
			object cache = this.m_cache;
			bool result;
			lock (cache)
			{
				result = this.m_cache.Remove(key);
			}
			return result;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x0000732C File Offset: 0x0000572C
		public void Clear()
		{
			object cache = this.m_cache;
			lock (cache)
			{
				this.m_cache.Clear();
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00007374 File Offset: 0x00005774
		public bool Enumerate(CacheDictionary<TKey, TValue>.EnumerateDelegate func)
		{
			object cache = this.m_cache;
			Dictionary<TKey, CacheDictionary<TKey, TValue>.CacheEntity> dictionary;
			lock (cache)
			{
				dictionary = new Dictionary<TKey, CacheDictionary<TKey, TValue>.CacheEntity>(this.m_cache);
			}
			bool flag2 = true;
			foreach (TKey key in dictionary.Keys)
			{
				flag2 = func(key, dictionary[key].data);
				if (!flag2)
				{
					break;
				}
			}
			return flag2;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00007428 File Offset: 0x00005828
		public bool ContainsKey(TKey key)
		{
			object cache = this.m_cache;
			bool result;
			lock (cache)
			{
				result = this.m_cache.ContainsKey(key);
			}
			return result;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00007474 File Offset: 0x00005874
		public TValue Get(TKey key)
		{
			object cache = this.m_cache;
			TValue result;
			lock (cache)
			{
				CacheDictionary<TKey, TValue>.CacheEntity cacheEntity;
				if (this.m_cache.TryGetValue(key, out cacheEntity))
				{
					if (this.m_mode != CacheDictionaryMode.Expiration)
					{
						cacheEntity.last_accessed = DateTime.Now;
					}
					result = cacheEntity.data;
				}
				else
				{
					result = default(TValue);
				}
			}
			return result;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000074F4 File Offset: 0x000058F4
		public TData Get<TData>(TKey key, Func<TKey, TValue, TData> getDataFactory)
		{
			object cache = this.m_cache;
			TData result;
			lock (cache)
			{
				CacheDictionary<TKey, TValue>.CacheEntity cacheEntity;
				if (!this.m_cache.TryGetValue(key, out cacheEntity))
				{
					result = default(TData);
				}
				else
				{
					if (this.m_mode != CacheDictionaryMode.Expiration)
					{
						cacheEntity.last_accessed = DateTime.Now;
					}
					result = getDataFactory(key, cacheEntity.data);
				}
			}
			return result;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000757C File Offset: 0x0000597C
		public bool TryGetValue(TKey key, out TValue val)
		{
			object cache = this.m_cache;
			bool result;
			lock (cache)
			{
				CacheDictionary<TKey, TValue>.CacheEntity cacheEntity;
				if (!this.m_cache.TryGetValue(key, out cacheEntity))
				{
					val = default(TValue);
					result = false;
				}
				else
				{
					val = cacheEntity.data;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000075F4 File Offset: 0x000059F4
		public void Touch(TKey key)
		{
			object cache = this.m_cache;
			lock (cache)
			{
				CacheDictionary<TKey, TValue>.CacheEntity cacheEntity;
				if (this.m_cache.TryGetValue(key, out cacheEntity))
				{
					cacheEntity.last_accessed = DateTime.Now;
				}
			}
		}

		// Token: 0x1700000F RID: 15
		public TValue this[TKey key]
		{
			get
			{
				return this.Get(key);
			}
			set
			{
				this.Add(key, value);
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00007664 File Offset: 0x00005A64
		public int Count()
		{
			object cache = this.m_cache;
			int count;
			lock (cache)
			{
				count = this.m_cache.Keys.Count;
			}
			return count;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000076B4 File Offset: 0x00005AB4
		private void CleanCache(object nul)
		{
			try
			{
				Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
				object cache = this.m_cache;
				lock (cache)
				{
					DateTime now = DateTime.Now;
					foreach (KeyValuePair<TKey, CacheDictionary<TKey, TValue>.CacheEntity> keyValuePair in this.m_cache)
					{
						TimeSpan t = now - keyValuePair.Value.last_accessed;
						if (t > this.m_cacheTimeout)
						{
							dictionary.Add(keyValuePair.Key, keyValuePair.Value.data);
						}
					}
					foreach (TKey key in dictionary.Keys)
					{
						this.m_cache.Remove(key);
					}
				}
				if (this.ItemExpired != null)
				{
					foreach (KeyValuePair<TKey, TValue> keyValuePair2 in dictionary)
					{
						this.ItemExpired(keyValuePair2.Key, keyValuePair2.Value);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x04000056 RID: 86
		private readonly SafeTimer m_cleanCache;

		// Token: 0x04000057 RID: 87
		private TimeSpan m_cacheTimeout;

		// Token: 0x04000058 RID: 88
		private readonly CacheDictionaryMode m_mode;

		// Token: 0x04000059 RID: 89
		private readonly Dictionary<TKey, CacheDictionary<TKey, TValue>.CacheEntity> m_cache = new Dictionary<TKey, CacheDictionary<TKey, TValue>.CacheEntity>();

		// Token: 0x0200002D RID: 45
		private class CacheEntity
		{
			// Token: 0x0400005B RID: 91
			public TValue data;

			// Token: 0x0400005C RID: 92
			public DateTime last_accessed;
		}

		// Token: 0x0200002E RID: 46
		// (Invoke) Token: 0x060000B6 RID: 182
		public delegate bool EnumerateDelegate(TKey key, TValue data);

		// Token: 0x0200002F RID: 47
		// (Invoke) Token: 0x060000BA RID: 186
		public delegate void ExpirationDeleg(TKey key, TValue data);
	}
}
