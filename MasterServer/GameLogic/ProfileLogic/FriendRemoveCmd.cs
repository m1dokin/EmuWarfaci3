using System;
using MasterServer.Core;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000556 RID: 1366
	[ConsoleCmdAttributes(CmdName = "friend_remove", ArgsSize = 3, Help = "Invite friend, params initiator pid, target nickname")]
	internal class FriendRemoveCmd : IConsoleCmd
	{
		// Token: 0x06001D74 RID: 7540 RVA: 0x00077824 File Offset: 0x00075C24
		public FriendRemoveCmd(IFriendsService friendsService, IUserRepository userRepository)
		{
			this.m_friendsService = friendsService;
			this.m_userRepository = userRepository;
		}

		// Token: 0x06001D75 RID: 7541 RVA: 0x0007783C File Offset: 0x00075C3C
		public void ExecuteCmd(string[] args)
		{
			ulong profileId = ulong.Parse(args[1]);
			string targetNickname = args[2];
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			this.m_friendsService.RemoveFriend(user, targetNickname);
		}

		// Token: 0x04000E11 RID: 3601
		private readonly IFriendsService m_friendsService;

		// Token: 0x04000E12 RID: 3602
		private readonly IUserRepository m_userRepository;
	}
}
