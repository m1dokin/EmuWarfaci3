using System;
using CommandLine;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x020000B0 RID: 176
	internal class ResetProfileAchievementsCmdParams
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x0000DD7F File Offset: 0x0000C17F
		// (set) Token: 0x060002D4 RID: 724 RVA: 0x0000DD87 File Offset: 0x0000C187
		[Option('p', "profileId", Required = true, HelpText = "profileId")]
		public ulong ProfileId { get; set; }
	}
}
