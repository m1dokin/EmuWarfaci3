using System;
using MasterServer.Core;

namespace MasterServer.Users
{
	// Token: 0x020007F9 RID: 2041
	[ConsoleCmdAttributes(CmdName = "user_info", ArgsSize = 1, Help = "User info by userId")]
	internal class UserInfoCmd : IConsoleCmd
	{
		// Token: 0x060029E9 RID: 10729 RVA: 0x000B4AEB File Offset: 0x000B2EEB
		public UserInfoCmd(IUserRepository userRepository)
		{
			this.m_userRepository = userRepository;
		}

		// Token: 0x060029EA RID: 10730 RVA: 0x000B4AFC File Offset: 0x000B2EFC
		public void ExecuteCmd(string[] args)
		{
			Log.Info("User info");
			ulong num = ulong.Parse(args[1]);
			UserInfo.User userByUserId = this.m_userRepository.GetUserByUserId(num);
			if (userByUserId != null)
			{
				Log.Info(userByUserId.ToString());
			}
			else
			{
				Log.Info<ulong>("Cannot find user with user id {0}", num);
			}
		}

		// Token: 0x0400162A RID: 5674
		private readonly IUserRepository m_userRepository;
	}
}
