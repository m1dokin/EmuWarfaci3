using System;
using CommandLine;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x020000C5 RID: 197
	internal class UpdateSeasonForPlayerCmdParams
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600032F RID: 815 RVA: 0x0000EE2A File Offset: 0x0000D22A
		// (set) Token: 0x06000330 RID: 816 RVA: 0x0000EE32 File Offset: 0x0000D232
		[Option('p', "profile", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }
	}
}
