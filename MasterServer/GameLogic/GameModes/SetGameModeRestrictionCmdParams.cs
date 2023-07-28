using System;
using CommandLine;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020000AD RID: 173
	internal class SetGameModeRestrictionCmdParams
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x0000DCD2 File Offset: 0x0000C0D2
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x0000DCDA File Offset: 0x0000C0DA
		[Option('o', "option", Required = false, HelpText = "game mode")]
		public string Option { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0000DCE3 File Offset: 0x0000C0E3
		// (set) Token: 0x060002CB RID: 715 RVA: 0x0000DCEB File Offset: 0x0000C0EB
		[Option('r', "restriction", Required = true, HelpText = "restriction")]
		public ERoomRestriction Restriction { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0000DCF4 File Offset: 0x0000C0F4
		// (set) Token: 0x060002CD RID: 717 RVA: 0x0000DCFC File Offset: 0x0000C0FC
		[Option('v', "value", Required = true, HelpText = "value to set")]
		public string Value { get; set; }
	}
}
