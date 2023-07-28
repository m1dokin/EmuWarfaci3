using System;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x0200009C RID: 156
	[Service]
	[Singleton]
	internal class RatingWinStreakConfigProvider : ServiceModule, IConfigProvider<RatingWinStreakConfig>
	{
		// Token: 0x14000010 RID: 16
		// (add) Token: 0x0600025F RID: 607 RVA: 0x0000C594 File Offset: 0x0000A994
		// (remove) Token: 0x06000260 RID: 608 RVA: 0x0000C5CC File Offset: 0x0000A9CC
		public event Action<RatingWinStreakConfig> Changed;

		// Token: 0x06000261 RID: 609 RVA: 0x0000C604 File Offset: 0x0000AA04
		public override void Init()
		{
			this.InitWinStreakConfig();
			ConfigSection winStreakConfigData = this.GetWinStreakConfigData();
			winStreakConfigData.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000C630 File Offset: 0x0000AA30
		public override void Stop()
		{
			ConfigSection winStreakConfigData = this.GetWinStreakConfigData();
			winStreakConfigData.OnConfigChanged -= this.OnConfigChanged;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000C656 File Offset: 0x0000AA56
		public RatingWinStreakConfig Get()
		{
			return this.m_winStreakConfig;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000C660 File Offset: 0x0000AA60
		private void InitWinStreakConfig()
		{
			ConfigSection winStreakConfigData = this.GetWinStreakConfigData();
			RatingWinStreakConfigParser ratingWinStreakConfigParser = new RatingWinStreakConfigParser();
			RatingWinStreakConfig ratingWinStreakConfig = ratingWinStreakConfigParser.Parse(winStreakConfigData);
			RatingWinStreakConfigValidator ratingWinStreakConfigValidator = new RatingWinStreakConfigValidator();
			ratingWinStreakConfigValidator.Validate(ratingWinStreakConfig);
			Interlocked.Exchange<RatingWinStreakConfig>(ref this.m_winStreakConfig, ratingWinStreakConfig);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000C69C File Offset: 0x0000AA9C
		private ConfigSection GetWinStreakConfigData()
		{
			Config ratingCurveConfig = Resources.RatingCurveConfig;
			return ratingCurveConfig.GetSection("win_streak");
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000C6BC File Offset: 0x0000AABC
		private void OnConfigChanged(ConfigEventArgs args)
		{
			this.InitWinStreakConfig();
		}

		// Token: 0x0400010A RID: 266
		private RatingWinStreakConfig m_winStreakConfig;
	}
}
