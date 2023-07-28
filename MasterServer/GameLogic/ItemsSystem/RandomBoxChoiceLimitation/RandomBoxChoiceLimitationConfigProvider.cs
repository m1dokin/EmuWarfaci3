using System;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem.RandomBoxChoiceLimitation
{
	// Token: 0x0200008A RID: 138
	[Service]
	[Singleton]
	internal class RandomBoxChoiceLimitationConfigProvider : ServiceModule, IConfigProvider<RandomBoxChoiceLimitationConfig>
	{
		// Token: 0x1400000F RID: 15
		// (add) Token: 0x0600020F RID: 527 RVA: 0x0000BC20 File Offset: 0x0000A020
		// (remove) Token: 0x06000210 RID: 528 RVA: 0x0000BC58 File Offset: 0x0000A058
		public event Action<RandomBoxChoiceLimitationConfig> Changed;

		// Token: 0x06000211 RID: 529 RVA: 0x0000BC8E File Offset: 0x0000A08E
		public RandomBoxChoiceLimitationConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000BC98 File Offset: 0x0000A098
		public override void Init()
		{
			base.Init();
			ConfigSection configSection = this.GetConfigSection();
			configSection.OnConfigChanged += this.OnConfigChanged;
			this.m_config = this.ParseRandomBoxChoiceLimitationConfig(configSection);
			this.ValidateBoxesChoiceLimitationConfig(this.m_config);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000BCE0 File Offset: 0x0000A0E0
		public override void Stop()
		{
			ConfigSection configSection = this.GetConfigSection();
			configSection.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000BD0C File Offset: 0x0000A10C
		private ConfigSection GetConfigSection()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Shop.RandomBoxChoiceLimitation");
			if (section == null)
			{
				throw new Exception(string.Format("Section {0} is missed in module_configuration.xml", "Shop.RandomBoxChoiceLimitation"));
			}
			return section;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000BD48 File Offset: 0x0000A148
		private RandomBoxChoiceLimitationConfig ParseRandomBoxChoiceLimitationConfig(ConfigSection configSection)
		{
			bool enabled;
			configSection.Get("enabled", out enabled);
			uint maxAmount;
			configSection.Get("max_amount", out maxAmount);
			return new RandomBoxChoiceLimitationConfig(enabled, maxAmount);
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000BD76 File Offset: 0x0000A176
		private void ValidateBoxesChoiceLimitationConfig(RandomBoxChoiceLimitationConfig config)
		{
			if (config.Enabled && config.MaxAmount != 1U)
			{
				throw new RandomBoxChoiceLimitationInvalidConfigException("It's forbidden to set max_amount value different from 1");
			}
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000BD9C File Offset: 0x0000A19C
		private void OnConfigChanged(ConfigEventArgs args)
		{
			ConfigSection configSection = this.GetConfigSection();
			RandomBoxChoiceLimitationConfig randomBoxChoiceLimitationConfig = this.ParseRandomBoxChoiceLimitationConfig(configSection);
			this.ValidateBoxesChoiceLimitationConfig(randomBoxChoiceLimitationConfig);
			Interlocked.Exchange<RandomBoxChoiceLimitationConfig>(ref this.m_config, randomBoxChoiceLimitationConfig);
			this.Changed.SafeInvoke(randomBoxChoiceLimitationConfig);
		}

		// Token: 0x040000E9 RID: 233
		private const string RandomBoxChoiceLimitationConfigSectionName = "Shop.RandomBoxChoiceLimitation";

		// Token: 0x040000EA RID: 234
		private const string ChoiceLimitationEnabledValueName = "enabled";

		// Token: 0x040000EB RID: 235
		private const string ChoiceLimitationMaxAmountValueName = "max_amount";

		// Token: 0x040000EC RID: 236
		private RandomBoxChoiceLimitationConfig m_config;
	}
}
