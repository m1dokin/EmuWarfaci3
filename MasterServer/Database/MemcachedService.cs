using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;

namespace MasterServer.Database
{
	// Token: 0x020001DE RID: 478
	[Service]
	[Singleton]
	internal class MemcachedService : ServiceModule, IMemcachedService, IObjectCache
	{
		// Token: 0x0600092E RID: 2350 RVA: 0x0002258E File Offset: 0x0002098E
		public MemcachedService(IDALService dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x0002259D File Offset: 0x0002099D
		public override void Start()
		{
			this.Connect();
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x000225A5 File Offset: 0x000209A5
		public override void Stop()
		{
			if (this.m_gen_update_timer != null)
			{
				this.m_gen_update_timer.Dispose();
				this.m_gen_update_timer = null;
			}
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x000225C4 File Offset: 0x000209C4
		private void Connect()
		{
			ConfigSection section = Resources.DBMasterSettings.GetSection("Memcached");
			if (section.Get("enabled") != "true")
			{
				return;
			}
			Log.Info("Initializing memcached connection...");
			LogManager.AssignFactory(new MasterServerLoggerFactory());
			MemcachedClientConfiguration memcachedClientConfiguration = new MemcachedClientConfiguration();
			ConfigSection section2 = section.GetSection("Servers");
			if (section2 == null)
			{
				Log.Warning("No memcached servers provided in configuration");
				return;
			}
			List<ConfigSection> sections = section2.GetSections("Host");
			ConfigSection section3 = section.GetSection("SocketPool");
			foreach (ConfigSection configSection in sections)
			{
				string text = configSection.Get("address");
				int port = int.Parse(configSection.Get("port"));
				IPAddress[] hostAddresses = Dns.GetHostAddresses(text);
				if (hostAddresses.Length == 0)
				{
					Log.Warning<string>("Unable to retrieve address from specified host name %s", text);
				}
				else
				{
					memcachedClientConfiguration.Servers.Add(new IPEndPoint(hostAddresses[0], port));
				}
			}
			memcachedClientConfiguration.Protocol = MemcachedProtocol.Text;
			memcachedClientConfiguration.SocketPool.MaxPoolSize = int.Parse(section3.Get("maxPoolSize"));
			memcachedClientConfiguration.SocketPool.MinPoolSize = int.Parse(section3.Get("minPoolSize"));
			memcachedClientConfiguration.SocketPool.ConnectionTimeout = TimeSpan.Parse(section3.Get("sendTimeout"));
			memcachedClientConfiguration.SocketPool.ReceiveTimeout = TimeSpan.Parse(section3.Get("recvTimeout"));
			memcachedClientConfiguration.SocketPool.DeadTimeout = TimeSpan.Parse(section3.Get("deadTimeout"));
			memcachedClientConfiguration.NodeLocatorFactory = new KetamaNodeLocatorFactory();
			memcachedClientConfiguration.NodeLocatorFactory.Initialize(new Dictionary<string, string>
			{
				{
					"hashName",
					"fnv1a_32"
				}
			});
			this.m_memcachedPool = ((IMemcachedClientConfiguration)memcachedClientConfiguration).CreatePool();
			this.m_memcachedPool.UseGeneration = true;
			if (section3.HasValue("useGeneration"))
			{
				this.m_memcachedPool.UseGeneration = (section3.Get("useGeneration") == "1");
			}
			if (this.m_memcachedPool.UseGeneration)
			{
				Log.Info("Memcached client started with generation mode support");
			}
			this.m_memcached = new MemcachedClient(this.m_memcachedPool, memcachedClientConfiguration.KeyTransformer, memcachedClientConfiguration.Transcoder);
			if (Resources.DBUpdaterPermission)
			{
				Log.Warning("Flushing memcached data, resetting nodes generations");
				this.m_memcached.FlushAll();
				this.m_memcached.ResetGenerations(this.get_key(cache_domains.generation));
			}
			string text2 = section.Get("generation_tick");
			if (!string.IsNullOrEmpty(text2) && text2 != "00:00:00")
			{
				TimeSpan period = TimeSpan.Parse(text2);
				this.m_gen_update_timer = new SafeTimer(new TimerCallback(this.TickGenerations), null, period);
				this.UpdateGenerations();
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000932 RID: 2354 RVA: 0x000228B8 File Offset: 0x00020CB8
		public bool Connected
		{
			get
			{
				return this.m_memcached != null;
			}
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x000228C8 File Offset: 0x00020CC8
		public List<MemcachedServer> GetServerList()
		{
			List<MemcachedServer> list = new List<MemcachedServer>();
			foreach (IMemcachedNode memcachedNode in this.m_memcachedPool.GetWorkingNodes())
			{
				list.Add(new MemcachedServer(memcachedNode.EndPoint.Address.ToString(), memcachedNode.EndPoint.Port, memcachedNode.IsAlive));
			}
			return list;
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x00022954 File Offset: 0x00020D54
		public bool Get(cache_domain domain, out object result)
		{
			result = null;
			object obj = this.m_memcached.Get(this.get_key(domain));
			if (obj == null)
			{
				return false;
			}
			if (obj is CacheNull)
			{
				return true;
			}
			result = obj;
			return true;
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x00022990 File Offset: 0x00020D90
		public bool Get(cache_domain domain, out object result, out ulong cas)
		{
			result = null;
			CasResult<object> withCas = this.m_memcached.GetWithCas(this.get_key(domain));
			cas = withCas.Cas;
			if (withCas.Result == null)
			{
				return false;
			}
			if (withCas.Result is CacheNull)
			{
				return true;
			}
			result = withCas.Result;
			return true;
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x000229E8 File Offset: 0x00020DE8
		public bool Store(StoreMode mode, cache_domain domain, object value)
		{
			return this.m_memcached.Store(mode, this.get_key(domain), value ?? default(CacheNull));
		}

		// Token: 0x06000937 RID: 2359 RVA: 0x00022A20 File Offset: 0x00020E20
		public bool Store(StoreMode mode, cache_domain domain, object value, TimeSpan validFor)
		{
			if (validFor == TimeSpan.Zero)
			{
				return this.Store(mode, domain, value ?? default(CacheNull));
			}
			return this.m_memcached.Store(mode, this.get_key(domain), value ?? default(CacheNull), validFor);
		}

		// Token: 0x06000938 RID: 2360 RVA: 0x00022A88 File Offset: 0x00020E88
		public bool Store(StoreMode mode, cache_domain domain, object value, DateTime expiresAt)
		{
			return this.m_memcached.Store(mode, this.get_key(domain), value ?? default(CacheNull), expiresAt);
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x00022AC0 File Offset: 0x00020EC0
		public bool CheckAndSet(cache_domain domain, object value, ulong cas)
		{
			return this.m_memcached.Cas(StoreMode.Replace, this.get_key(domain), value ?? default(CacheNull), cas).Result;
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x00022B00 File Offset: 0x00020F00
		public bool CheckAndSet(cache_domain domain, object value, ulong cas, TimeSpan validFor)
		{
			if (validFor == TimeSpan.Zero)
			{
				return this.m_memcached.Cas(StoreMode.Replace, this.get_key(domain), value ?? default(CacheNull), cas).Result;
			}
			return this.m_memcached.Cas(StoreMode.Replace, this.get_key(domain), value ?? default(CacheNull), validFor, cas).Result;
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x00022B88 File Offset: 0x00020F88
		public bool CheckAndSet(cache_domain domain, object value, ulong cas, DateTime expiresAt)
		{
			return this.m_memcached.Cas(StoreMode.Replace, this.get_key(domain), value ?? default(CacheNull), expiresAt, cas).Result;
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x00022BC9 File Offset: 0x00020FC9
		public void Remove(cache_domain domain)
		{
			if (domain.ToString() != cache_domains.all.ToString())
			{
				this.m_memcached.Remove(this.get_key(domain));
			}
			else
			{
				this.m_memcached.FlushAll();
			}
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x00022C08 File Offset: 0x00021008
		public void UpdateGenerations()
		{
			this.m_memcached.UpdateGenerations(this.get_key(cache_domains.generation));
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x00022C20 File Offset: 0x00021020
		public void Clear()
		{
			this.m_memcached.FlushAll();
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x00022C2D File Offset: 0x0002102D
		public ServerStats Stats()
		{
			return this.m_memcached.Stats();
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x00022C3A File Offset: 0x0002103A
		private string get_key(cache_domain domain)
		{
			return string.Format("{0}.{1}", this.m_dal.Config.Cookie, domain);
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x00022C57 File Offset: 0x00021057
		private void TickGenerations(object dummy)
		{
			this.UpdateGenerations();
		}

		// Token: 0x04000543 RID: 1347
		private readonly IDALService m_dal;

		// Token: 0x04000544 RID: 1348
		private IServerPool m_memcachedPool;

		// Token: 0x04000545 RID: 1349
		private MemcachedClient m_memcached;

		// Token: 0x04000546 RID: 1350
		private SafeTimer m_gen_update_timer;
	}
}
