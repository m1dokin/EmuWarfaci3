using System;
using CommandLine;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x0200040F RID: 1039
	internal class GetPvpRatingCmdParams
	{
		// Token: 0x17000203 RID: 515
		// (get) Token: 0x0600166B RID: 5739 RVA: 0x0005E364 File Offset: 0x0005C764
		// (set) Token: 0x0600166C RID: 5740 RVA: 0x0005E36C File Offset: 0x0005C76C
		[Option('p', "profile", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }
	}
}
