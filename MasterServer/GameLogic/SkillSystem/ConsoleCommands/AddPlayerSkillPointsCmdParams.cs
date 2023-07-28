using System;
using CommandLine;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x02000428 RID: 1064
	internal class AddPlayerSkillPointsCmdParams
	{
		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060016D4 RID: 5844 RVA: 0x0005FB57 File Offset: 0x0005DF57
		// (set) Token: 0x060016D5 RID: 5845 RVA: 0x0005FB5F File Offset: 0x0005DF5F
		[Option('p', "profile_id", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060016D6 RID: 5846 RVA: 0x0005FB68 File Offset: 0x0005DF68
		// (set) Token: 0x060016D7 RID: 5847 RVA: 0x0005FB70 File Offset: 0x0005DF70
		[Option('t', "type", Required = true, HelpText = "Skill type")]
		public SkillType Type { get; set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060016D8 RID: 5848 RVA: 0x0005FB79 File Offset: 0x0005DF79
		// (set) Token: 0x060016D9 RID: 5849 RVA: 0x0005FB81 File Offset: 0x0005DF81
		[Option('v', "value", Required = true, HelpText = "Skill value")]
		public double Value { get; set; }
	}
}
