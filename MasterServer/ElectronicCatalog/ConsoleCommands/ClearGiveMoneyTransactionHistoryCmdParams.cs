using System;
using CommandLine;

namespace MasterServer.ElectronicCatalog.ConsoleCommands
{
	// Token: 0x0200004E RID: 78
	internal class ClearGiveMoneyTransactionHistoryCmdParams
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00009923 File Offset: 0x00007D23
		// (set) Token: 0x06000137 RID: 311 RVA: 0x0000992B File Offset: 0x00007D2B
		[Option('u', "user_id", Required = true, HelpText = "UserId to clear give money transaction history")]
		public ulong UserId { get; set; }
	}
}
