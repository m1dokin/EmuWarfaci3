using System;
using CommandLine;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x0200042E RID: 1070
	internal class GetPlayerSkillPointsCmdParams
	{
		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060016EB RID: 5867 RVA: 0x0005FD06 File Offset: 0x0005E106
		// (set) Token: 0x060016EC RID: 5868 RVA: 0x0005FD0E File Offset: 0x0005E10E
		[Option('p', "profile_id", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060016ED RID: 5869 RVA: 0x0005FD17 File Offset: 0x0005E117
		// (set) Token: 0x060016EE RID: 5870 RVA: 0x0005FD1F File Offset: 0x0005E11F
		[Option('t', "type", Required = true, HelpText = "Skill type")]
		public SkillType Type { get; set; }
	}
}
