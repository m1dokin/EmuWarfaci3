using System;
using MasterServer.Core;
using MasterServer.Users;

namespace MasterServer.Platform.Nickname.Commands
{
	// Token: 0x02000685 RID: 1669
	[ConsoleCmdAttributes(CmdName = "get_user_id_by_reserved_nickname", ArgsSize = 1, Help = "Get userId by reserved nickname, params: nickname.")]
	internal class GetUserIdByReservedNicknameCmd : IConsoleCmd
	{
		// Token: 0x06002319 RID: 8985 RVA: 0x00094F73 File Offset: 0x00093373
		public GetUserIdByReservedNicknameCmd(INicknameReservationService service)
		{
			this.m_nicknameReservationService = service;
		}

		// Token: 0x0600231A RID: 8986 RVA: 0x00094F84 File Offset: 0x00093384
		public void ExecuteCmd(string[] args)
		{
			string nickname = args[1];
			ulong p;
			NameReservationResult userIdByReservedNickname = this.m_nicknameReservationService.GetUserIdByReservedNickname(nickname, out p);
			Log.Info<ulong, NameReservationResult>("get_user_id_by_reserved_nickname: {0} with result '{1}'", p, userIdByReservedNickname);
		}

		// Token: 0x040011CE RID: 4558
		private readonly INicknameReservationService m_nicknameReservationService;
	}
}
