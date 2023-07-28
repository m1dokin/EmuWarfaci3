using System;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.GameLogic.RatingSystem.Exceptions;
using Util.Common;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000C1 RID: 193
	[Service]
	[Singleton]
	internal class RatingGameBanConfigProvider : ServiceModule, IConfigProvider<RatingGameBanConfig>
	{
		// Token: 0x14000011 RID: 17
		// (add) Token: 0x06000314 RID: 788 RVA: 0x0000E910 File Offset: 0x0000CD10
		// (remove) Token: 0x06000315 RID: 789 RVA: 0x0000E948 File Offset: 0x0000CD48
		public event Action<RatingGameBanConfig> Changed;

		// Token: 0x06000316 RID: 790 RVA: 0x0000E97E File Offset: 0x0000CD7E
		public RatingGameBanConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000E988 File Offset: 0x0000CD88
		public override void Init()
		{
			base.Init();
			ConfigSection configSection = this.GetConfigSection();
			configSection.OnConfigChanged += this.OnConfigChanged;
			this.m_config = this.ParseRatingGameBanConfig(configSection);
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000E9C4 File Offset: 0x0000CDC4
		public override void Stop()
		{
			ConfigSection configSection = this.GetConfigSection();
			configSection.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0000E9F0 File Offset: 0x0000CDF0
		private ConfigSection GetConfigSection()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("RatingGame.Ban");
			if (section == null)
			{
				throw new RatingGameBanConfigException(string.Format("Unable to find section \"{0}\" in module_configuration.xml", "RatingGame.Ban"));
			}
			return section;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0000EA2C File Offset: 0x0000CE2C
		private RatingGameBanConfig ParseRatingGameBanConfig(ConfigSection configSection)
		{
			TimeSpan banTimeout;
			if (!configSection.TryGet("timeout_sec", out banTimeout, default(TimeSpan)))
			{
				throw new RatingGameBanConfigException(string.Format("Unable to retrieve {0} value from \"{1}\" section in module_configuration.xml", "timeout_sec", "RatingGame.Ban"));
			}
			bool enabled;
			configSection.Get("enabled", out enabled);
			return new RatingGameBanConfig(enabled, banTimeout);
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000EA84 File Offset: 0x0000CE84
		private void OnConfigChanged(ConfigEventArgs args)
		{
			ConfigSection configSection = this.GetConfigSection();
			RatingGameBanConfig value = this.ParseRatingGameBanConfig(configSection);
			Interlocked.Exchange<RatingGameBanConfig>(ref this.m_config, value);
			this.Changed.SafeInvokeEach(value);
		}

		// Token: 0x04000150 RID: 336
		private const string RatingGameBanSectionName = "RatingGame.Ban";

		// Token: 0x04000151 RID: 337
		private const string BanTimeoutConfigValueName = "timeout_sec";

		// Token: 0x04000152 RID: 338
		private const string BanEnabledConfigValueName = "enabled";

		// Token: 0x04000153 RID: 339
		private RatingGameBanConfig m_config;
	}
}
