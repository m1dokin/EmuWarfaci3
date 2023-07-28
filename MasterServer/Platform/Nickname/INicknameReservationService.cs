using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;
using MasterServer.Users;

namespace MasterServer.Platform.Nickname
{
	// Token: 0x0200068B RID: 1675
	[Contract]
	[BootstrapExplicit]
	internal interface INicknameReservationService
	{
		// Token: 0x06002323 RID: 8995
		NameReservationResult ReserveNickname(ulong userId, string nickname);

		// Token: 0x06002324 RID: 8996
		NameReservationResult CancelNicknameReservation(ulong userId, string nickname);

		// Token: 0x06002325 RID: 8997
		NameReservationResult GetUserIdByReservedNickname(string nickname, out ulong userId);
	}
}
