using System;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x020006E2 RID: 1762
	[Contract]
	internal interface IUserInvitationTicketRepository : IDisposable
	{
		// Token: 0x1400009E RID: 158
		// (add) Token: 0x060024F9 RID: 9465
		// (remove) Token: 0x060024FA RID: 9466
		event Action<UserInvitation.Ticket> TicketExpired;

		// Token: 0x060024FB RID: 9467
		bool TryGetValue(string key, out UserInvitation.Ticket ticket);

		// Token: 0x060024FC RID: 9468
		void Add(string key, UserInvitation.Ticket ticket);

		// Token: 0x060024FD RID: 9469
		void Remove(string key);
	}
}
