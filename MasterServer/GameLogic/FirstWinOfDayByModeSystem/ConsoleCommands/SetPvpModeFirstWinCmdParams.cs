using System;
using CommandLine;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.ConsoleCommands
{
	// Token: 0x02000078 RID: 120
	internal class SetPvpModeFirstWinCmdParams
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0000B392 File Offset: 0x00009792
		// (set) Token: 0x060001CF RID: 463 RVA: 0x0000B39A File Offset: 0x0000979A
		[Option('p', "profile", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000B3A3 File Offset: 0x000097A3
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x0000B3AB File Offset: 0x000097AB
		[Option('m', "mode", Required = true, HelpText = "Game mode")]
		public string GameMode { get; set; }
	}
}
