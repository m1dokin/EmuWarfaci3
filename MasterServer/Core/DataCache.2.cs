using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using MasterServer.Core.Configuration;

namespace MasterServer.Core
{
	// Token: 0x0200010E RID: 270
	public class DataCache<T>
	{
		// Token: 0x06000456 RID: 1110 RVA: 0x00012C58 File Offset: 0x00011058
		public void Init()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("DataCache");
			section.Get("EntityTTL", out this.m_entityTTL);
			section.Get("TokenTTL", out this.m_tokenTTL);
			section.Get("TrashTimeout", out this.m_trashTimeout);
			if (DataCache<T>.<>f__mg$cache0 == null)
			{
				DataCache<T>.<>f__mg$cache0 = new TimerCallback(DataCache<T>.FindZombie);
			}
			this.m_killZombie = new Timer(DataCache<T>.<>f__mg$cache0, this, this.m_trashTimeout * 1000, this.m_trashTimeout * 1000);
			this.m_tokenId = 1;
			this.m_lastTokenTime = DateTime.Now;
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00012CFC File Offset: 0x000110FC
		public int SaveToken(T data, string sToken, CacheType type)
		{
			object thisLock = this.m_thisLock;
			int result;
			lock (thisLock)
			{
				int tokenId = this.GetTokenId();
				DataCache<T>.CacheEntity cacheEntity;
				if (!this.m_dataCache.TryGetValue(new DataCache<T>.TokenKey(sToken, tokenId), out cacheEntity))
				{
					cacheEntity = new DataCache<T>.CacheEntity();
					cacheEntity.data = data;
					cacheEntity.refCount = 1;
					cacheEntity.bornTime = DateTime.Now;
					cacheEntity.type = type;
					this.m_dataCache.Add(new DataCache<T>.TokenKey(sToken, tokenId), cacheEntity);
					if (type == CacheType.Permanent)
					{
						this.m_persistenceCache[sToken] = data;
					}
				}
				else
				{
					cacheEntity.refCount++;
				}
				result = tokenId;
			}
			return result;
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00012DC0 File Offset: 0x000111C0
		public int GetToken(int tokenId, string sToken, out T data)
		{
			data = default(T);
			object thisLock = this.m_thisLock;
			int result;
			lock (thisLock)
			{
				bool flag2 = 0 == tokenId;
				if (flag2)
				{
					tokenId = this.GetTokenId();
				}
				this.LazyFreeToken(sToken);
				DataCache<T>.CacheEntity cacheEntity;
				if (this.m_dataCache.TryGetValue(new DataCache<T>.TokenKey(sToken, tokenId), out cacheEntity))
				{
					if (flag2)
					{
						cacheEntity.refCount++;
					}
					data = cacheEntity.data;
					result = tokenId;
				}
				else if (this.m_persistenceCache.TryGetValue(sToken, out data))
				{
					result = this.SaveToken(data, sToken, CacheType.Permanent);
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x00012E98 File Offset: 0x00011298
		public void FreeToken(int tokenId, string sToken)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				DataCache<T>.CacheEntity cacheEntity;
				if (this.m_dataCache.TryGetValue(new DataCache<T>.TokenKey(sToken, tokenId), out cacheEntity))
				{
					if (cacheEntity.refCount == 0)
					{
						throw new ApplicationException("Invalid FreeToken call without GetToken call before");
					}
					cacheEntity.refCount--;
					if (cacheEntity.refCount == 0 && cacheEntity.type == CacheType.Expirable)
					{
						this.m_dataCache.Remove(new DataCache<T>.TokenKey(sToken, tokenId));
					}
				}
				else
				{
					Log.Error<int>("Invalid token id [{0}]", tokenId);
				}
			}
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00012F4C File Offset: 0x0001134C
		private void LazyFreeToken(string token)
		{
			List<DataCache<T>.TokenKey> list = new List<DataCache<T>.TokenKey>();
			DateTime now = DateTime.Now;
			foreach (DataCache<T>.TokenKey tokenKey in this.m_dataCache.Keys)
			{
				if (tokenKey.sToken == token)
				{
					DataCache<T>.CacheEntity cacheEntity = this.m_dataCache[tokenKey];
					if ((now - cacheEntity.bornTime).TotalSeconds > (double)this.m_entityTTL && cacheEntity.refCount == 0)
					{
						list.Add(tokenKey);
					}
				}
			}
			foreach (DataCache<T>.TokenKey key in list)
			{
				this.m_dataCache.Remove(key);
			}
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x00013058 File Offset: 0x00011458
		private int GetTokenId()
		{
			DateTime now = DateTime.Now;
			if ((now - this.m_lastTokenTime).TotalSeconds > (double)this.m_tokenTTL)
			{
				this.m_tokenId++;
				this.m_lastTokenTime = now;
			}
			return this.m_tokenId;
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x000130A8 File Offset: 0x000114A8
		private static void FindZombie(object obj)
		{
			DataCache<T> dataCache = (DataCache<T>)obj;
			try
			{
				object thisLock = dataCache.m_thisLock;
				lock (thisLock)
				{
					List<DataCache<T>.TokenKey> list = new List<DataCache<T>.TokenKey>();
					DateTime now = DateTime.Now;
					foreach (DataCache<T>.TokenKey tokenKey in dataCache.m_dataCache.Keys)
					{
						DataCache<T>.CacheEntity cacheEntity = dataCache.m_dataCache[tokenKey];
						if ((now - cacheEntity.bornTime).TotalSeconds > (double)dataCache.m_entityTTL)
						{
							list.Add(tokenKey);
						}
					}
					foreach (DataCache<T>.TokenKey key in list)
					{
						dataCache.m_dataCache.Remove(key);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x000131EC File Offset: 0x000115EC
		public void InvalidateCache(CacheType type, string token)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				if (type == CacheType.Permanent)
				{
					if (string.IsNullOrEmpty(token))
					{
						this.m_persistenceCache.Clear();
					}
					else
					{
						this.m_persistenceCache.Remove(token);
					}
				}
				else if (type == CacheType.Regular)
				{
					foreach (DataCache<T>.TokenKey tokenKey in this.m_dataCache.Keys)
					{
						if (string.IsNullOrEmpty(token) || tokenKey.sToken == token)
						{
							DataCache<T>.CacheEntity cacheEntity = this.m_dataCache[tokenKey];
							if (cacheEntity.type == CacheType.Regular)
							{
								cacheEntity.type = CacheType.Expirable;
							}
						}
					}
				}
			}
		}

		// Token: 0x040001D1 RID: 465
		private readonly Dictionary<DataCache<T>.TokenKey, DataCache<T>.CacheEntity> m_dataCache = new Dictionary<DataCache<T>.TokenKey, DataCache<T>.CacheEntity>(new DataCache<T>.TokenKeyComparer());

		// Token: 0x040001D2 RID: 466
		private readonly Dictionary<string, T> m_persistenceCache = new Dictionary<string, T>();

		// Token: 0x040001D3 RID: 467
		private readonly object m_thisLock = new object();

		// Token: 0x040001D4 RID: 468
		private int m_trashTimeout;

		// Token: 0x040001D5 RID: 469
		private int m_tokenId;

		// Token: 0x040001D6 RID: 470
		private DateTime m_lastTokenTime;

		// Token: 0x040001D7 RID: 471
		private int m_tokenTTL;

		// Token: 0x040001D8 RID: 472
		private int m_entityTTL;

		// Token: 0x040001D9 RID: 473
		private Timer m_killZombie;

		// Token: 0x040001DA RID: 474
		[CompilerGenerated]
		private static TimerCallback <>f__mg$cache0;

		// Token: 0x0200010F RID: 271
		private class CacheEntity
		{
			// Token: 0x040001DB RID: 475
			public T data;

			// Token: 0x040001DC RID: 476
			public int refCount;

			// Token: 0x040001DD RID: 477
			public DateTime bornTime;

			// Token: 0x040001DE RID: 478
			public CacheType type;
		}

		// Token: 0x02000110 RID: 272
		private class TokenKey
		{
			// Token: 0x0600045F RID: 1119 RVA: 0x000132F4 File Offset: 0x000116F4
			public TokenKey(string s, int id)
			{
				this.sToken = s;
				this.tokenId = id;
			}

			// Token: 0x040001DF RID: 479
			public string sToken;

			// Token: 0x040001E0 RID: 480
			public int tokenId;
		}

		// Token: 0x02000111 RID: 273
		private class TokenKeyComparer : IEqualityComparer<DataCache<T>.TokenKey>
		{
			// Token: 0x06000461 RID: 1121 RVA: 0x00013312 File Offset: 0x00011712
			public bool Equals(DataCache<T>.TokenKey x, DataCache<T>.TokenKey y)
			{
				return x.sToken == y.sToken && x.tokenId == y.tokenId;
			}

			// Token: 0x06000462 RID: 1122 RVA: 0x0001333B File Offset: 0x0001173B
			public int GetHashCode(DataCache<T>.TokenKey obj)
			{
				return obj.sToken.GetHashCode() ^ obj.tokenId;
			}
		}
	}
}
