using System;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using Util.Common;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000C9 RID: 201
	[Service]
	[Singleton]
	internal class RatingSeasonConfigProvider : ServiceModule, IConfigProvider<RatingSeasonConfig>
	{
		// Token: 0x14000012 RID: 18
		// (add) Token: 0x0600033A RID: 826 RVA: 0x0000EF6C File Offset: 0x0000D36C
		// (remove) Token: 0x0600033B RID: 827 RVA: 0x0000EFA4 File Offset: 0x0000D3A4
		public event Action<RatingSeasonConfig> Changed;

		// Token: 0x0600033C RID: 828 RVA: 0x0000EFDC File Offset: 0x0000D3DC
		public override void Init()
		{
			this.m_ratingSeasonConfig = this.GetRatingSeasonConfig();
			ConfigSection ratingSeasonModuleConfig = this.GetRatingSeasonModuleConfig();
			ratingSeasonModuleConfig.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0000F010 File Offset: 0x0000D410
		public override void Stop()
		{
			ConfigSection ratingSeasonModuleConfig = this.GetRatingSeasonModuleConfig();
			ratingSeasonModuleConfig.OnConfigChanged -= this.OnConfigChanged;
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0000F036 File Offset: 0x0000D436
		public RatingSeasonConfig Get()
		{
			return this.m_ratingSeasonConfig;
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0000F040 File Offset: 0x0000D440
		private RatingSeasonConfig GetRatingSeasonConfig()
		{
			ConfigSection ratingSeasonModuleConfig = this.GetRatingSeasonModuleConfig();
			RatingSeasonConfigParser ratingSeasonConfigParser = new RatingSeasonConfigParser();
			return ratingSeasonConfigParser.Parse(ratingSeasonModuleConfig);
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0000F064 File Offset: 0x0000D464
		private void OnConfigChanged(ConfigEventArgs args)
		{
			RatingSeasonConfig ratingSeasonConfig = this.GetRatingSeasonConfig();
			Interlocked.Exchange<RatingSeasonConfig>(ref this.m_ratingSeasonConfig, ratingSeasonConfig);
			this.Changed.SafeInvoke(this.m_ratingSeasonConfig);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0000F098 File Offset: 0x0000D498
		private ConfigSection GetRatingSeasonModuleConfig()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("RatingGame");
			return section.GetSection("RatingSeason");
		}

		// Token: 0x0400015F RID: 351
		private RatingSeasonConfig m_ratingSeasonConfig;
	}
}
