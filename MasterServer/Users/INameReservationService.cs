using System;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x0200073E RID: 1854
	[Contract]
	public interface INameReservationService
	{
		// Token: 0x0600263B RID: 9787
		NameReservationResult ReserveName(string name, NameReservationGroup group, ulong userId);

		// Token: 0x0600263C RID: 9788
		NameReservationResult CancelNameReservation(string name, NameReservationGroup group, ulong userId);

		// Token: 0x0600263D RID: 9789
		NameReservationResult GetUserIdByReservedNickname(string nickname, NameReservationGroup nicknames, out ulong userId);
	}
}
