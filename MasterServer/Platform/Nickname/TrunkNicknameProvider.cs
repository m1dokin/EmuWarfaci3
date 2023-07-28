using System;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.Database;
using MasterServer.Users;

namespace MasterServer.Platform.Nickname
{
	// Token: 0x0200068D RID: 1677
	[Service]
	[Singleton]
	internal class TrunkNicknameProvider : ServiceModule, INicknameProvider, INicknameReservationService, IExternalNicknameSyncService
	{
		// Token: 0x06002329 RID: 9001 RVA: 0x00094FFC File Offset: 0x000933FC
		public TrunkNicknameProvider(IDALService dalService, INameReservationService nameReservationService)
		{
			this.m_dalService = dalService;
			this.m_nameReservationService = nameReservationService;
		}

		// Token: 0x1400009A RID: 154
		// (add) Token: 0x0600232A RID: 9002 RVA: 0x00095014 File Offset: 0x00093414
		// (remove) Token: 0x0600232B RID: 9003 RVA: 0x0009504C File Offset: 0x0009344C
		public event ProfileRenamedDelegate ProfileRenamed;

		// Token: 0x0600232C RID: 9004 RVA: 0x00095084 File Offset: 0x00093484
		public string GetNickname(ulong userId, ulong profileId)
		{
			return this.m_dalService.ProfileSystem.GetProfileInfo(profileId).Nickname;
		}

		// Token: 0x0600232D RID: 9005 RVA: 0x000950AA File Offset: 0x000934AA
		public NameReservationResult ReserveNickname(ulong userId, string nickname)
		{
			return this.m_nameReservationService.ReserveName(nickname, NameReservationGroup.NICKNAMES, userId);
		}

		// Token: 0x0600232E RID: 9006 RVA: 0x000950BA File Offset: 0x000934BA
		public NameReservationResult CancelNicknameReservation(ulong userId, string nickname)
		{
			return this.m_nameReservationService.CancelNameReservation(nickname, NameReservationGroup.NICKNAMES, userId);
		}

		// Token: 0x0600232F RID: 9007 RVA: 0x000950CA File Offset: 0x000934CA
		public NameReservationResult GetUserIdByReservedNickname(string nickname, out ulong userId)
		{
			return this.m_nameReservationService.GetUserIdByReservedNickname(nickname, NameReservationGroup.NICKNAMES, out userId);
		}

		// Token: 0x06002330 RID: 9008 RVA: 0x000950DA File Offset: 0x000934DA
		public bool SyncNickname(ulong profileId, string nickname)
		{
			return true;
		}

		// Token: 0x040011D8 RID: 4568
		private readonly IDALService m_dalService;

		// Token: 0x040011D9 RID: 4569
		private readonly INameReservationService m_nameReservationService;
	}
}
