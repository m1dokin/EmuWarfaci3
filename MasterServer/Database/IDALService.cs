using System;
using HK2Net;
using MasterServer.Database.RemoteClients;

namespace MasterServer.Database
{
	// Token: 0x020001CD RID: 461
	[Contract]
	internal interface IDALService : IBaseDALService
	{
		// Token: 0x170000EF RID: 239
		// (get) Token: 0x0600088B RID: 2187
		IItemSystemClient ItemSystem { get; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x0600088C RID: 2188
		IAchievementsSystemClient AchievementSystem { get; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x0600088D RID: 2189
		IRewardsSystemClient RewardsSystem { get; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x0600088E RID: 2190
		IProfileSystemClient ProfileSystem { get; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x0600088F RID: 2191
		IColdStorageSystemClient ColdStorageSystem { get; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000890 RID: 2192
		IClanSystemClient ClanSystem { get; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000891 RID: 2193
		IContractSystemClient ContractSystem { get; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000892 RID: 2194
		INotificationSystemClient NotificationSystem { get; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000893 RID: 2195
		IAnnouncementSystemClient AnnouncementSystem { get; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000894 RID: 2196
		IAbuseReportSystemClient AbuseSystem { get; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000895 RID: 2197
		ICommonSystemClient CommonSystem { get; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000896 RID: 2198
		IPerformanceSystemClient PerformanceSystem { get; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000897 RID: 2199
		IECatalogClient ECatalog { get; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000898 RID: 2200
		IMissionSystemClient MissionSystem { get; }

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000899 RID: 2201
		IPlayerStatSystemClient PlayerStatSystem { get; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600089A RID: 2202
		IProfileProgressionSystemClient ProfileProgressionSystem { get; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600089B RID: 2203
		ICustomRulesSystemClient CustomRulesSystem { get; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600089C RID: 2204
		ISkillSystemClient SkillSystem { get; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x0600089D RID: 2205
		IRatingSystemClient RatingSystem { get; }

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600089E RID: 2206
		IVoucherSystemClient VoucherSystem { get; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x0600089F RID: 2207
		IRatingGameBanSystemClient RatingRoomBanSystem { get; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060008A0 RID: 2208
		IFirstWinOfDayByModeSystemClient FirstWinOfDayByModeSystem { get; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060008A1 RID: 2209
		IAuthorizationTokenSystemClient AuthorizationTokenSystem { get; }
	}
}
