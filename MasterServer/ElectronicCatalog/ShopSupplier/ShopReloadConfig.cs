using System;

namespace MasterServer.ElectronicCatalog.ShopSupplier
{
	// Token: 0x02000055 RID: 85
	public class ShopReloadConfig
	{
		// Token: 0x06000147 RID: 327 RVA: 0x00009B7E File Offset: 0x00007F7E
		internal ShopReloadConfig(bool useMemcache)
		{
			this.UseMemcache = useMemcache;
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000148 RID: 328 RVA: 0x00009B8D File Offset: 0x00007F8D
		// (set) Token: 0x06000149 RID: 329 RVA: 0x00009B95 File Offset: 0x00007F95
		public bool UseMemcache { get; private set; }
	}
}
