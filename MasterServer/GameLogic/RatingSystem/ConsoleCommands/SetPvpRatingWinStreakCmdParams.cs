using System;
using CommandLine;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x0200008F RID: 143
	internal class SetPvpRatingWinStreakCmdParams
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000229 RID: 553 RVA: 0x0000C140 File Offset: 0x0000A540
		// (set) Token: 0x0600022A RID: 554 RVA: 0x0000C148 File Offset: 0x0000A548
		[Option('p', "profile", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600022B RID: 555 RVA: 0x0000C151 File Offset: 0x0000A551
		// (set) Token: 0x0600022C RID: 556 RVA: 0x0000C159 File Offset: 0x0000A559
		[Option('v', "win streak value", Required = true, HelpText = "Win streak to set")]
		public uint WinStreak { get; set; }
	}
}
