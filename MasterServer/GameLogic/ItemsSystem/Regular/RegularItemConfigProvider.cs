using System;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.ItemsSystem.RegularItem;
using MasterServer.GameLogic.ItemsSystem.RegularItem.Exceptions;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem.Regular
{
	// Token: 0x02000074 RID: 116
	[Service]
	[Singleton]
	internal class RegularItemConfigProvider : ServiceModule, IConfigProvider<RegularItemConfig>
	{
		// Token: 0x060001BE RID: 446 RVA: 0x0000B1CB File Offset: 0x000095CB
		public RegularItemConfigProvider(IConfigurationService configurationService, IRegularItemConfigParser configParser)
		{
			this.m_configurationService = configurationService;
			this.m_configParser = configParser;
		}

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x060001BF RID: 447 RVA: 0x0000B1E4 File Offset: 0x000095E4
		// (remove) Token: 0x060001C0 RID: 448 RVA: 0x0000B21C File Offset: 0x0000961C
		public event Action<RegularItemConfig> Changed;

		// Token: 0x060001C1 RID: 449 RVA: 0x0000B252 File Offset: 0x00009652
		public override void Init()
		{
			this.m_configSection = this.GetConfigSection();
			this.m_configSection.OnConfigChanged += this.OnConfigChanged;
			this.ParseConfig();
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000B27D File Offset: 0x0000967D
		public override void Stop()
		{
			this.m_configSection.OnConfigChanged -= this.OnConfigChanged;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000B296 File Offset: 0x00009696
		public RegularItemConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000B2A0 File Offset: 0x000096A0
		private ConfigSection GetConfigSection()
		{
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			ConfigSection section = config.GetSection("Shop");
			ConfigSection section2 = section.GetSection("RegularItem");
			if (section2 == null)
			{
				throw new RegularItemConfigException("There is no config section for regular item");
			}
			return section2;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000B2E8 File Offset: 0x000096E8
		private void ParseConfig()
		{
			this.m_config = this.m_configParser.Parse(this.m_configSection);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000B301 File Offset: 0x00009701
		private void OnConfigChanged(ConfigEventArgs args)
		{
			this.ParseConfig();
			this.Changed.SafeInvoke(this.m_config);
		}

		// Token: 0x040000CF RID: 207
		public const string ShopSection = "Shop";

		// Token: 0x040000D0 RID: 208
		public const string RegularItemSection = "RegularItem";

		// Token: 0x040000D1 RID: 209
		private readonly IConfigurationService m_configurationService;

		// Token: 0x040000D2 RID: 210
		private readonly IRegularItemConfigParser m_configParser;

		// Token: 0x040000D3 RID: 211
		private RegularItemConfig m_config;

		// Token: 0x040000D4 RID: 212
		private ConfigSection m_configSection;
	}
}
