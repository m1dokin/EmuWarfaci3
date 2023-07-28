using System;
using CommandLine;

namespace MasterServer.GameLogic.SkillSystem.ConsoleCommands
{
	// Token: 0x020000F8 RID: 248
	internal class SetPlayerSkillPointsCmdParams
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x00011C63 File Offset: 0x00010063
		// (set) Token: 0x0600040F RID: 1039 RVA: 0x00011C6B File Offset: 0x0001006B
		[Option('p', "profile_id", Required = true, HelpText = "Player profile id")]
		public ulong ProfileId { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x00011C74 File Offset: 0x00010074
		// (set) Token: 0x06000411 RID: 1041 RVA: 0x00011C7C File Offset: 0x0001007C
		[Option('t', "type", Required = true, HelpText = "Skill type")]
		public SkillType Type { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x00011C85 File Offset: 0x00010085
		// (set) Token: 0x06000413 RID: 1043 RVA: 0x00011C8D File Offset: 0x0001008D
		[Option('v', "value", Required = true, HelpText = "Skill value")]
		public double Value { get; set; }
	}
}
