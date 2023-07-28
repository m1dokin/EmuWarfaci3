using System;
using CommandLine;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x02000411 RID: 1041
	internal class AddPvpRatingPointsCmdParams
	{
		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06001670 RID: 5744 RVA: 0x0005E3D9 File Offset: 0x0005C7D9
		// (set) Token: 0x06001671 RID: 5745 RVA: 0x0005E3E1 File Offset: 0x0005C7E1
		[Option('p', "profile", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06001672 RID: 5746 RVA: 0x0005E3EA File Offset: 0x0005C7EA
		// (set) Token: 0x06001673 RID: 5747 RVA: 0x0005E3F2 File Offset: 0x0005C7F2
		[Option('v', "points value", Required = true, HelpText = "Points to add")]
		public int Points { get; set; }
	}
}
