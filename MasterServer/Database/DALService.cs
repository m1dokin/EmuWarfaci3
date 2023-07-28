using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.Database.RemoteClients;

namespace MasterServer.Database
{
	// Token: 0x020001CE RID: 462
	[Service]
	[Singleton]
	internal class DALService : BaseDALService, IDALService, IBaseDALService
	{
		// Token: 0x060008A2 RID: 2210 RVA: 0x00020D1C File Offset: 0x0001F11C
		public DALService(IDAL dal, IDALCookieProvider cookieProvider) : base(dal, cookieProvider)
		{
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x00020E2E File Offset: 0x0001F22E
		public IItemSystemClient ItemSystem
		{
			get
			{
				return this.m_itemSystemClient;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060008A4 RID: 2212 RVA: 0x00020E36 File Offset: 0x0001F236
		public IAchievementsSystemClient AchievementSystem
		{
			get
			{
				return this.m_achievementSystemClient;
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060008A5 RID: 2213 RVA: 0x00020E3E File Offset: 0x0001F23E
		public IRewardsSystemClient RewardsSystem
		{
			get
			{
				return this.m_rewardsSystemClient;
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060008A6 RID: 2214 RVA: 0x00020E46 File Offset: 0x0001F246
		public IProfileSystemClient ProfileSystem
		{
			get
			{
				return this.m_profileSystemClient;
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060008A7 RID: 2215 RVA: 0x00020E4E File Offset: 0x0001F24E
		public IColdStorageSystemClient ColdStorageSystem
		{
			get
			{
				return this.m_coldStorageSystemClient;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060008A8 RID: 2216 RVA: 0x00020E56 File Offset: 0x0001F256
		public IClanSystemClient ClanSystem
		{
			get
			{
				return this.m_clanSystemClient;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x00020E5E File Offset: 0x0001F25E
		public IContractSystemClient ContractSystem
		{
			get
			{
				return this.m_contractSystemClient;
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060008AA RID: 2218 RVA: 0x00020E66 File Offset: 0x0001F266
		public INotificationSystemClient NotificationSystem
		{
			get
			{
				return this.m_notificationClient;
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x00020E6E File Offset: 0x0001F26E
		public IAnnouncementSystemClient AnnouncementSystem
		{
			get
			{
				return this.m_announcementClient;
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060008AC RID: 2220 RVA: 0x00020E76 File Offset: 0x0001F276
		public IAbuseReportSystemClient AbuseSystem
		{
			get
			{
				return this.m_abuseClient;
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060008AD RID: 2221 RVA: 0x00020E7E File Offset: 0x0001F27E
		public ICommonSystemClient CommonSystem
		{
			get
			{
				return this.m_commonSystemClient;
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060008AE RID: 2222 RVA: 0x00020E86 File Offset: 0x0001F286
		public IPerformanceSystemClient PerformanceSystem
		{
			get
			{
				return this.m_performanceSystemClient;
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x00020E8E File Offset: 0x0001F28E
		public IECatalogClient ECatalog
		{
			get
			{
				return this.m_catalogClient;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060008B0 RID: 2224 RVA: 0x00020E96 File Offset: 0x0001F296
		public IMissionSystemClient MissionSystem
		{
			get
			{
				return this.m_missionSystem;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060008B1 RID: 2225 RVA: 0x00020E9E File Offset: 0x0001F29E
		public IPlayerStatSystemClient PlayerStatSystem
		{
			get
			{
				return this.m_playerStatSystemClient;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060008B2 RID: 2226 RVA: 0x00020EA6 File Offset: 0x0001F2A6
		public IProfileProgressionSystemClient ProfileProgressionSystem
		{
			get
			{
				return this.m_profileProgressionSystemClient;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060008B3 RID: 2227 RVA: 0x00020EAE File Offset: 0x0001F2AE
		public ICustomRulesSystemClient CustomRulesSystem
		{
			get
			{
				return this.m_customRulesSystemClient;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060008B4 RID: 2228 RVA: 0x00020EB6 File Offset: 0x0001F2B6
		public ISkillSystemClient SkillSystem
		{
			get
			{
				return this.m_skillSystemClient;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060008B5 RID: 2229 RVA: 0x00020EBE File Offset: 0x0001F2BE
		public IRatingSystemClient RatingSystem
		{
			get
			{
				return this.m_ratingSystemClient;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060008B6 RID: 2230 RVA: 0x00020EC6 File Offset: 0x0001F2C6
		public IVoucherSystemClient VoucherSystem
		{
			get
			{
				return this.m_voucherSystemClient;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060008B7 RID: 2231 RVA: 0x00020ECE File Offset: 0x0001F2CE
		public IRatingGameBanSystemClient RatingRoomBanSystem
		{
			get
			{
				return this.m_ratingGameBanSystemClient;
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060008B8 RID: 2232 RVA: 0x00020ED6 File Offset: 0x0001F2D6
		public IFirstWinOfDayByModeSystemClient FirstWinOfDayByModeSystem
		{
			get
			{
				return this.m_firstWinOfDayByModeSystemClient;
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x00020EDE File Offset: 0x0001F2DE
		public IAuthorizationTokenSystemClient AuthorizationTokenSystem
		{
			get
			{
				return this.m_authorizationTokenSystem;
			}
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x00020EE8 File Offset: 0x0001F2E8
		protected override void ResetClients()
		{
			this.m_itemSystemClient.Reset(base.DAL.ItemSystem);
			this.m_achievementSystemClient.Reset(base.DAL.AchievementSystem);
			this.m_rewardsSystemClient.Reset(base.DAL.RewardsSystem);
			this.m_profileSystemClient.Reset(base.DAL.ProfileSystem);
			this.m_coldStorageSystemClient.Reset(base.DAL.ColdStorageSystem);
			this.m_clanSystemClient.Reset(base.DAL.ClanSystem);
			this.m_contractSystemClient.Reset(base.DAL.ContractSystem);
			this.m_notificationClient.Reset(base.DAL.NotificationSystem);
			this.m_announcementClient.Reset(base.DAL.AnnouncementSystem);
			this.m_abuseClient.Reset(base.DAL.AbuseSystem);
			this.m_commonSystemClient.Reset(base.DAL.CommonSystem);
			this.m_performanceSystemClient.Reset(base.DAL.PerformanceSystem);
			this.m_catalogClient.Reset(base.DAL.ECatalog);
			this.m_missionSystem.Reset(base.DAL.MissionSystem);
			this.m_playerStatSystemClient.Reset(base.DAL.PlayerStatsSystem);
			this.m_profileProgressionSystemClient.Reset(base.DAL.ProfileProgressionSystem);
			this.m_customRulesSystemClient.Reset(base.DAL.CustomRulesSystem);
			this.m_skillSystemClient.Reset(base.DAL.SkillSystem);
			this.m_ratingSystemClient.Reset(base.DAL.RatingSystem);
			this.m_voucherSystemClient.Reset(base.DAL.VoucherSystem);
			this.m_ratingGameBanSystemClient.Reset(base.DAL.RatingGameBanSystem);
			this.m_firstWinOfDayByModeSystemClient.Reset(base.DAL.FirstWinOfDayByModeSystem);
		}

		// Token: 0x04000504 RID: 1284
		private readonly ItemSystemClient m_itemSystemClient = new ItemSystemClient();

		// Token: 0x04000505 RID: 1285
		private readonly AchievementsSystemClient m_achievementSystemClient = new AchievementsSystemClient();

		// Token: 0x04000506 RID: 1286
		private readonly RewardsSystemClient m_rewardsSystemClient = new RewardsSystemClient();

		// Token: 0x04000507 RID: 1287
		private readonly ProfileSystemClient m_profileSystemClient = new ProfileSystemClient();

		// Token: 0x04000508 RID: 1288
		private readonly ColdStorageSystemClient m_coldStorageSystemClient = new ColdStorageSystemClient();

		// Token: 0x04000509 RID: 1289
		private readonly ClanSystemClient m_clanSystemClient = new ClanSystemClient();

		// Token: 0x0400050A RID: 1290
		private readonly ContractSystemClient m_contractSystemClient = new ContractSystemClient();

		// Token: 0x0400050B RID: 1291
		private readonly NotificationSystemClient m_notificationClient = new NotificationSystemClient();

		// Token: 0x0400050C RID: 1292
		private readonly AnnouncementSystemClient m_announcementClient = new AnnouncementSystemClient();

		// Token: 0x0400050D RID: 1293
		private readonly AbuseReportSystemClient m_abuseClient = new AbuseReportSystemClient();

		// Token: 0x0400050E RID: 1294
		private readonly CommonSystemClient m_commonSystemClient = new CommonSystemClient();

		// Token: 0x0400050F RID: 1295
		private readonly PerformanceSystemClient m_performanceSystemClient = new PerformanceSystemClient();

		// Token: 0x04000510 RID: 1296
		private readonly ECatalogClient m_catalogClient = new ECatalogClient();

		// Token: 0x04000511 RID: 1297
		private readonly MissionSystemClient m_missionSystem = new MissionSystemClient();

		// Token: 0x04000512 RID: 1298
		private readonly PlayerStatSystemClient m_playerStatSystemClient = new PlayerStatSystemClient();

		// Token: 0x04000513 RID: 1299
		private readonly ProfileProgressionSystemClient m_profileProgressionSystemClient = new ProfileProgressionSystemClient();

		// Token: 0x04000514 RID: 1300
		private readonly CustomRulesSystemClient m_customRulesSystemClient = new CustomRulesSystemClient();

		// Token: 0x04000515 RID: 1301
		private readonly SkillSystemClient m_skillSystemClient = new SkillSystemClient();

		// Token: 0x04000516 RID: 1302
		private readonly RatingSystemClient m_ratingSystemClient = new RatingSystemClient();

		// Token: 0x04000517 RID: 1303
		private readonly VoucherSystemClient m_voucherSystemClient = new VoucherSystemClient();

		// Token: 0x04000518 RID: 1304
		private readonly RatingGameBanSystemClient m_ratingGameBanSystemClient = new RatingGameBanSystemClient();

		// Token: 0x04000519 RID: 1305
		private readonly FirstWinOfDayByModeSystemClient m_firstWinOfDayByModeSystemClient = new FirstWinOfDayByModeSystemClient();

		// Token: 0x0400051A RID: 1306
		private readonly AuthorizationTokenSystemClient m_authorizationTokenSystem = new AuthorizationTokenSystemClient();
	}
}
