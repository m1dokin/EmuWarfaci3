using System;
using CommandLine;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x0200042A RID: 1066
	internal class DeletePlayerSkillPointsCmdParams
	{
		// Token: 0x17000214 RID: 532
		// (get) Token: 0x060016DD RID: 5853 RVA: 0x0005FBDF File Offset: 0x0005DFDF
		// (set) Token: 0x060016DE RID: 5854 RVA: 0x0005FBE7 File Offset: 0x0005DFE7
		[Option('p', "profile_id", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x060016DF RID: 5855 RVA: 0x0005FBF0 File Offset: 0x0005DFF0
		// (set) Token: 0x060016E0 RID: 5856 RVA: 0x0005FBF8 File Offset: 0x0005DFF8
		[Option('t', "type", Required = true, HelpText = "Skill type")]
		public SkillType Type { get; set; }
	}
}
