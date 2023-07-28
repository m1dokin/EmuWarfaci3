using System;
using HK2Net;
using MasterServer.DAL.CustomRules;
using MasterServer.DAL.FirstWinOfDayByMode;
using MasterServer.DAL.RatingSystem;
using MasterServer.DAL.VoucherSystem;

namespace MasterServer.DAL
{
	// Token: 0x0200006A RID: 106
	[Contract]
	public interface IDAL : IDisposable
	{
		// Token: 0x060000FF RID: 255
		void Start();

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000100 RID: 256
		IItemSystem ItemSystem { get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000101 RID: 257
		IAchievementsSystem AchievementSystem { get; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000102 RID: 258
		IRewardsSystem RewardsSystem { get; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000103 RID: 259
		IProfileSystem ProfileSystem { get; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000104 RID: 260
		IColdStorageSystem ColdStorageSystem { get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000105 RID: 261
		IClanSystem ClanSystem { get; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000106 RID: 262
		IContractSystem ContractSystem { get; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000107 RID: 263
		INotificationSystem NotificationSystem { get; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000108 RID: 264
		IAnnouncmentSystem AnnouncementSystem { get; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000109 RID: 265
		ICommonSystem CommonSystem { get; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600010A RID: 266
		IAbuseReportSystem AbuseSystem { get; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600010B RID: 267
		IPerformanceSystem PerformanceSystem { get; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600010C RID: 268
		IECatalog ECatalog { get; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600010D RID: 269
		IMissionSystem MissionSystem { get; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600010E RID: 270
		ITelemetrySystem TelemetrySystem { get; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600010F RID: 271
		IProfileProgressionSystem ProfileProgressionSystem { get; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000110 RID: 272
		IPlayerStatsSystem PlayerStatsSystem { get; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000111 RID: 273
		ICustomRulesSystem CustomRulesSystem { get; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000112 RID: 274
		ISkillSystem SkillSystem { get; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000113 RID: 275
		IRatingSystem RatingSystem { get; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000114 RID: 276
		IVoucherSystem VoucherSystem { get; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000115 RID: 277
		IRatingGameBanSystem RatingGameBanSystem { get; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000116 RID: 278
		IFirstWinOfDayByModeSystem FirstWinOfDayByModeSystem { get; }

		// Token: 0x06000117 RID: 279
		void ValidateFixedSizeColumnData(string tableName, string columnName, int dataSize);
	}
}
