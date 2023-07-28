using System;
using MasterServer.Core;
using MasterServer.Users;

namespace MasterServer.Platform.Nickname.Commands
{
	// Token: 0x02000684 RID: 1668
	[ConsoleCmdAttributes(CmdName = "cancel_nickname_reservation", ArgsSize = 2, Help = "Cancel the reservation nickname, params: userId, nickname.")]
	internal class CancelNicknameReservationCmd : IConsoleCmd
	{
		// Token: 0x06002317 RID: 8983 RVA: 0x00094F2E File Offset: 0x0009332E
		public CancelNicknameReservationCmd(INicknameReservationService service)
		{
			this.m_nicknameReservationService = service;
		}

		// Token: 0x06002318 RID: 8984 RVA: 0x00094F40 File Offset: 0x00093340
		public void ExecuteCmd(string[] args)
		{
			ulong userId = ulong.Parse(args[1]);
			string nickname = args[2];
			NameReservationResult p = this.m_nicknameReservationService.CancelNicknameReservation(userId, nickname);
			Log.Info<NameReservationResult>("cancel_nickname_reservation: '{0}'", p);
		}

		// Token: 0x040011CD RID: 4557
		private readonly INicknameReservationService m_nicknameReservationService;
	}
}
