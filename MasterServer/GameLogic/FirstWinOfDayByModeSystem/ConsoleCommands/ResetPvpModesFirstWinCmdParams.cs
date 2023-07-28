using System;
using CommandLine;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.ConsoleCommands
{
	// Token: 0x0200007B RID: 123
	internal class ResetPvpModesFirstWinCmdParams
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x0000B422 File Offset: 0x00009822
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x0000B42A File Offset: 0x0000982A
		[Option('p', "profile", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }
	}
}
