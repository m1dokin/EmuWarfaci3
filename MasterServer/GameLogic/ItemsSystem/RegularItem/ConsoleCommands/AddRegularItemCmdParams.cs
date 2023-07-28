using System;
using CommandLine;

namespace MasterServer.GameLogic.ItemsSystem.RegularItem.ConsoleCommands
{
	// Token: 0x0200006F RID: 111
	internal class AddRegularItemCmdParams
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001AE RID: 430 RVA: 0x0000B0DA File Offset: 0x000094DA
		// (set) Token: 0x060001AF RID: 431 RVA: 0x0000B0E2 File Offset: 0x000094E2
		[Option('u', "user_id", Required = true, HelpText = "Player user id")]
		public ulong UserId { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000B0EB File Offset: 0x000094EB
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x0000B0F3 File Offset: 0x000094F3
		[Option('i', "item_name", Required = true, HelpText = "Name of the item to add")]
		public string ItemName { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x0000B0FC File Offset: 0x000094FC
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x0000B104 File Offset: 0x00009504
		[Option('c', "count", Required = true, HelpText = "Number of item copies to add")]
		public uint Count { get; set; }
	}
}
