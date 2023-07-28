using System;
using System.Threading;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Configuration;
using Util.Common;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200000C RID: 12
	[Service]
	[Singleton]
	internal class ProfileValidationServiceConfigProvider : ServiceModule, IConfigProvider<ProfileValidationServiceConfig>
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00004D32 File Offset: 0x00003132
		public ProfileValidationServiceConfigProvider(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600002D RID: 45 RVA: 0x00004D44 File Offset: 0x00003144
		// (remove) Token: 0x0600002E RID: 46 RVA: 0x00004D7C File Offset: 0x0000317C
		public event Action<ProfileValidationServiceConfig> Changed;

		// Token: 0x0600002F RID: 47 RVA: 0x00004DB4 File Offset: 0x000031B4
		public override void Init()
		{
			base.Init();
			ConfigSection section = this.GetSection();
			this.InitConfig(section);
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00004DE8 File Offset: 0x000031E8
		public override void Stop()
		{
			ConfigSection section = this.GetSection();
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00004E14 File Offset: 0x00003214
		public ProfileValidationServiceConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00004E1C File Offset: 0x0000321C
		private void InitConfig(ConfigSection section)
		{
			bool isCheckAchievementsEnabled;
			section.Get("achievements", out isCheckAchievementsEnabled);
			bool isCheckProfileEnabled;
			section.Get("profile", out isCheckProfileEnabled);
			bool isCheckClanEnabled;
			section.Get("clan", out isCheckClanEnabled);
			bool isCheckHeadEnabled;
			section.Get("head", out isCheckHeadEnabled);
			ProfileValidationServiceConfig value = new ProfileValidationServiceConfig(isCheckAchievementsEnabled, isCheckProfileEnabled, isCheckClanEnabled, isCheckHeadEnabled);
			Interlocked.Exchange<ProfileValidationServiceConfig>(ref this.m_config, value);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004E76 File Offset: 0x00003276
		private void OnConfigChanged(ConfigEventArgs args)
		{
			this.InitConfig(args.Section);
			this.Changed.SafeInvoke(this.m_config);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004E95 File Offset: 0x00003295
		private ConfigSection GetSection()
		{
			return this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration).GetSection("ProfileValidation.Check");
		}

		// Token: 0x04000018 RID: 24
		private readonly IConfigurationService m_configurationService;

		// Token: 0x04000019 RID: 25
		private ProfileValidationServiceConfig m_config;
	}
}
