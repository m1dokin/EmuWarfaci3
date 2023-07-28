using System;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.DAL;
using Util.Common;

namespace MasterServer.Database
{
	// Token: 0x02000039 RID: 57
	internal abstract class BaseDALService : ServiceModule, IBaseDALService
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x0000805F File Offset: 0x0000645F
		protected BaseDALService(IDAL dal, IDALCookieProvider cookieProvider)
		{
			this.DAL = dal;
			this.m_cookieProvider = cookieProvider;
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x060000D7 RID: 215 RVA: 0x00008078 File Offset: 0x00006478
		// (remove) Token: 0x060000D8 RID: 216 RVA: 0x000080B0 File Offset: 0x000064B0
		public event Action<DALProxyStats> OnDALStats;

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x000080E6 File Offset: 0x000064E6
		// (set) Token: 0x060000DA RID: 218 RVA: 0x000080EE File Offset: 0x000064EE
		public DALConfig Config { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000DB RID: 219 RVA: 0x000080F7 File Offset: 0x000064F7
		public IMemcachedService MemcachedService
		{
			get
			{
				return ServicesManager.GetService<IMemcachedService>();
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000DC RID: 220 RVA: 0x000080FE File Offset: 0x000064FE
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00008106 File Offset: 0x00006506
		private protected IDAL DAL { protected get; private set; }

		// Token: 0x060000DE RID: 222 RVA: 0x00008110 File Offset: 0x00006510
		public override void Init()
		{
			ConfigSection section = Resources.DBMasterSettings.GetSection("DAL");
			this.Config = new DALConfig();
			if (section.HasValue("retries"))
			{
				this.Config.QueryRetry = int.Parse(section.Get("retries"));
			}
			this.Config.Cookie = this.m_cookieProvider.GetCookie();
			Log.Info<int>("DAL: deadlock retries {0}", this.Config.QueryRetry);
			this.ResetClients();
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00008194 File Offset: 0x00006594
		public override void Start()
		{
			base.Start();
			this.DAL.Start();
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000081A7 File Offset: 0x000065A7
		public override void Stop()
		{
			this.DAL.Dispose();
			this.OnDALStats = null;
			base.Stop();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000081C1 File Offset: 0x000065C1
		public void SendDALStats(DALProxyStats stats)
		{
			this.OnDALStats.SafeInvoke(stats);
		}

		// Token: 0x060000E2 RID: 226
		protected abstract void ResetClients();

		// Token: 0x04000074 RID: 116
		private readonly IDALCookieProvider m_cookieProvider;
	}
}
