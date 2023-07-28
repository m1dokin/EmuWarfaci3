using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;

namespace MasterServer.ElectronicCatalog.ShopSupplier
{
	// Token: 0x02000056 RID: 86
	[Service]
	[Singleton]
	public class ShopReloadConfigProvider : IConfigProvider<ShopReloadConfig>
	{
		// Token: 0x0600014A RID: 330 RVA: 0x00009BA0 File Offset: 0x00007FA0
		public ShopReloadConfigProvider()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("ShopReload");
			if (section == null)
			{
				throw new ConfigSectionNotFoundException("ShopReload");
			}
			section.OnConfigChanged += this.ConfigSectionOnOnConfigChanged;
			this.InitConfig(section);
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x0600014B RID: 331 RVA: 0x00009BF0 File Offset: 0x00007FF0
		// (remove) Token: 0x0600014C RID: 332 RVA: 0x00009C28 File Offset: 0x00008028
		public event Action<ShopReloadConfig> Changed;

		// Token: 0x0600014D RID: 333 RVA: 0x00009C5E File Offset: 0x0000805E
		public ShopReloadConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00009C68 File Offset: 0x00008068
		private void InitConfig(ConfigSection config)
		{
			bool useMemcache;
			config.Get("use_memcache", out useMemcache);
			this.m_config = new ShopReloadConfig(useMemcache);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00009C8E File Offset: 0x0000808E
		private void ConfigSectionOnOnConfigChanged(ConfigEventArgs configEventArgs)
		{
			this.InitConfig(configEventArgs.Section);
		}

		// Token: 0x0400009D RID: 157
		private const string ShopReload = "ShopReload";

		// Token: 0x0400009E RID: 158
		private ShopReloadConfig m_config;
	}
}
