using System;
using MasterServer.Common;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.Core
{
	// Token: 0x020007A8 RID: 1960
	[ConsoleCmdAttributes(CmdName = "debug_add_users", ArgsSize = 2, Help = "profile_id, count (optional)")]
	internal class DebugAddFakeUsersCmd : IConsoleCmd
	{
		// Token: 0x0600287A RID: 10362 RVA: 0x000AE1B8 File Offset: 0x000AC5B8
		public DebugAddFakeUsersCmd(IUserRepository userRepository)
		{
			this.m_userRepository = userRepository;
		}

		// Token: 0x0600287B RID: 10363 RVA: 0x000AE1C8 File Offset: 0x000AC5C8
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			ulong num2 = (args.Length != 3) ? 1UL : ulong.Parse(args[2]);
			for (ulong num3 = num; num3 < num + num2; num3 += 1UL)
			{
				Jid jid = (!Resources.BootstrapMode) ? new Jid("fake_" + num3, "warface", "GameClient") : Utils.MakeJid("fake_" + num3, Resources.BootstrapName, "warface", "GameClient");
				UserInfo.User user = this.m_userRepository.MakeFake(num3 + 1000UL, num3, jid.ToString(), 0, 0UL, "global");
				using (UserLoginContext userLoginContext = new UserLoginContext(this.m_userRepository, user, ELoginType.Ordinary, DateTime.Now))
				{
					userLoginContext.Commit();
				}
			}
		}

		// Token: 0x04001532 RID: 5426
		private readonly IUserRepository m_userRepository;
	}
}
