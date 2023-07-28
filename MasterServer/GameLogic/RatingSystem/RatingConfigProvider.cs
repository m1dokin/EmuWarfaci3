using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000F0 RID: 240
	[Service]
	[Singleton]
	internal class RatingConfigProvider : ServiceModule, IConfigProvider<RatingConfig>
	{
		// Token: 0x14000016 RID: 22
		// (add) Token: 0x060003E9 RID: 1001 RVA: 0x00010FBC File Offset: 0x0000F3BC
		// (remove) Token: 0x060003EA RID: 1002 RVA: 0x00010FF4 File Offset: 0x0000F3F4
		public event Action<RatingConfig> Changed;

		// Token: 0x060003EB RID: 1003 RVA: 0x0001102C File Offset: 0x0000F42C
		public override void Init()
		{
			Config ratingCurveConfig = Resources.RatingCurveConfig;
			RatingConfigParser ratingConfigParser = new RatingConfigParser();
			RatingConfig ratingConfig = ratingConfigParser.Parse(ratingCurveConfig);
			RatingConfigValidator ratingConfigValidator = new RatingConfigValidator();
			ratingConfigValidator.Validate(ratingConfig);
			this.m_ratingConfig = ratingConfig;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00011061 File Offset: 0x0000F461
		public RatingConfig Get()
		{
			return this.m_ratingConfig;
		}

		// Token: 0x040001A1 RID: 417
		private RatingConfig m_ratingConfig;
	}
}
