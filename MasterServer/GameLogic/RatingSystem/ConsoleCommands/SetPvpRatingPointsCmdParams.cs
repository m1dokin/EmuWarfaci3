using System;
using CommandLine;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x020000EB RID: 235
	internal class SetPvpRatingPointsCmdParams
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060003D7 RID: 983 RVA: 0x00010CFF File Offset: 0x0000F0FF
		// (set) Token: 0x060003D8 RID: 984 RVA: 0x00010D07 File Offset: 0x0000F107
		[Option('p', "profile", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060003D9 RID: 985 RVA: 0x00010D10 File Offset: 0x0000F110
		// (set) Token: 0x060003DA RID: 986 RVA: 0x00010D18 File Offset: 0x0000F118
		[Option('v', "points value", Required = true, HelpText = "Points to set")]
		public uint Points { get; set; }
	}
}
