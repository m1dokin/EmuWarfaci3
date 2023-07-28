using System;
using MasterServer.Common;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.Core
{
	// Token: 0x020007A9 RID: 1961
	[ConsoleCmdAttributes(CmdName = "debug_remove_users", ArgsSize = 2, Help = "profile_id, count (optional)")]
	internal class DebugRemoveFakeUsersCmd : IConsoleCmd
	{
		// Token: 0x0600287C RID: 10364 RVA: 0x000AE2C4 File Offset: 0x000AC6C4
		public DebugRemoveFakeUsersCmd(IUserRepository userRepository)
		{
			this.m_userRepository = userRepository;
		}

		// Token: 0x0600287D RID: 10365 RVA: 0x000AE2D4 File Offset: 0x000AC6D4
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			ulong num2 = (args.Length != 3) ? 1UL : ulong.Parse(args[2]);
			for (ulong num3 = num; num3 < num + num2; num3 += 1UL)
			{
				Jid jid = (!Resources.BootstrapMode) ? new Jid("fake_" + num3, "warface", "GameClient") : Utils.MakeJid("fake_" + num3, Resources.BootstrapName, "warface", "GameClient");
				UserInfo.User userByOnlineId = this.m_userRepository.GetUserByOnlineId(jid.ToString());
				if (userByOnlineId != null)
				{
					this.m_userRepository.UserLogout(userByOnlineId, ELogoutType.Logout);
				}
			}
		}

		// Token: 0x04001533 RID: 5427
		private readonly IUserRepository m_userRepository;
	}
}
