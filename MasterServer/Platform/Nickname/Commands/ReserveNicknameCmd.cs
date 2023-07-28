using System;
using MasterServer.Core;
using MasterServer.Users;

namespace MasterServer.Platform.Nickname.Commands
{
	// Token: 0x02000686 RID: 1670
	[ConsoleCmdAttributes(CmdName = "reserve_nickname", ArgsSize = 2, Help = "Reserve nickname, params: userId, nickname.")]
	internal class ReserveNicknameCmd : IConsoleCmd
	{
		// Token: 0x0600231B RID: 8987 RVA: 0x00094FB0 File Offset: 0x000933B0
		public ReserveNicknameCmd(INicknameReservationService service)
		{
			this.m_nicknameReservationService = service;
		}

		// Token: 0x0600231C RID: 8988 RVA: 0x00094FC0 File Offset: 0x000933C0
		public void ExecuteCmd(string[] args)
		{
			ulong userId = ulong.Parse(args[1]);
			string nickname = args[2];
			NameReservationResult p = this.m_nicknameReservationService.ReserveNickname(userId, nickname);
			Log.Info<NameReservationResult>("reserve_nickname: '{0}'", p);
		}

		// Token: 0x040011CF RID: 4559
		private readonly INicknameReservationService m_nicknameReservationService;
	}
}
