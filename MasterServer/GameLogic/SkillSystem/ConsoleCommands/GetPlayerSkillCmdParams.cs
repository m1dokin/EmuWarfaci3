using System;
using CommandLine;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x0200042B RID: 1067
	internal class GetPlayerSkillCmdParams
	{
		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060016E2 RID: 5858 RVA: 0x0005FC09 File Offset: 0x0005E009
		// (set) Token: 0x060016E3 RID: 5859 RVA: 0x0005FC11 File Offset: 0x0005E011
		[Option('p', "profile", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060016E4 RID: 5860 RVA: 0x0005FC1A File Offset: 0x0005E01A
		// (set) Token: 0x060016E5 RID: 5861 RVA: 0x0005FC22 File Offset: 0x0005E022
		[Option('t', "type", Required = true, HelpText = "Skill type")]
		public SkillType SkillType { get; set; }
	}
}
