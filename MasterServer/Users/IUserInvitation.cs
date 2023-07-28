using System;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x020007FB RID: 2043
	[Contract]
	internal interface IUserInvitation
	{
		// Token: 0x060029EB RID: 10731
		EInvitationStatus SendInvitation(UserInfo.User fromUser, UserInfo.User toUser, string groupId, string isFollow);

		// Token: 0x060029EC RID: 10732
		EInvitationStatus InvitationResponse(string ticketId, EInvitationStatus status);
	}
}
