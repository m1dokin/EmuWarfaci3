using System;
using MasterServer.Core;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000555 RID: 1365
	[ConsoleCmdAttributes(CmdName = "friend_invite", ArgsSize = 3, Help = "Invite friend, params initiator pid, target pid [, target nickname]")]
	internal class FriendInviteCmd : IConsoleCmd
	{
		// Token: 0x06001D72 RID: 7538 RVA: 0x000777AD File Offset: 0x00075BAD
		public FriendInviteCmd(IFriendsService friendsService, IUserRepository userRepository)
		{
			this.m_friendsService = friendsService;
			this.m_userRepository = userRepository;
		}

		// Token: 0x06001D73 RID: 7539 RVA: 0x000777C4 File Offset: 0x00075BC4
		public void ExecuteCmd(string[] args)
		{
			ulong profileId = ulong.Parse(args[1]);
			ulong targetId = ulong.Parse(args[2]);
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			EInviteStatus result = this.m_friendsService.Invite(user, targetId, (args.Length <= 3) ? string.Empty : args[3]).Result;
			Log.Info<EInviteStatus>("Invite result : {0}", result);
		}

		// Token: 0x04000E0F RID: 3599
		private readonly IFriendsService m_friendsService;

		// Token: 0x04000E10 RID: 3600
		private readonly IUserRepository m_userRepository;
	}
}
