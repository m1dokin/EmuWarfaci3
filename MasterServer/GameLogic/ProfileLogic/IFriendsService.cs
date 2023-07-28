using System;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000553 RID: 1363
	[Contract]
	internal interface IFriendsService
	{
		// Token: 0x06001D53 RID: 7507
		void SendFriendListUpdate(ulong profileId, string onlineId);

		// Token: 0x06001D54 RID: 7508
		Task<EInviteStatus> Invite(UserInfo.User initiator, ulong targetId, string targetNickname);

		// Token: 0x06001D55 RID: 7509
		Task<EInviteStatus> Invite(ulong sourceId, ulong targetId);

		// Token: 0x06001D56 RID: 7510
		void RemoveFriend(UserInfo.User initiator, string targetNickname);

		// Token: 0x06001D57 RID: 7511
		void RemoveFriend(ulong sourceId, ulong targetId);
	}
}
