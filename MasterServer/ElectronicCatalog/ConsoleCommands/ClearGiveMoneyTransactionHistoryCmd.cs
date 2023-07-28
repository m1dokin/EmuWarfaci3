using System;
using MasterServer.Core;

namespace MasterServer.ElectronicCatalog.ConsoleCommands
{
	// Token: 0x0200004D RID: 77
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_clear_money_tr_history")]
	internal class ClearGiveMoneyTransactionHistoryCmd : ConsoleCommand<ClearGiveMoneyTransactionHistoryCmdParams>
	{
		// Token: 0x06000133 RID: 307 RVA: 0x000098EF File Offset: 0x00007CEF
		public ClearGiveMoneyTransactionHistoryCmd(IDebugCatalogService debugCatalogService)
		{
			this.m_debugCatalogService = debugCatalogService;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000098FE File Offset: 0x00007CFE
		protected override void Execute(ClearGiveMoneyTransactionHistoryCmdParams param)
		{
			this.m_debugCatalogService.ClearGiveMoneyTransactionHistory(param.UserId);
			Log.Info("[debug_clear_money_tr_history]: Done");
		}

		// Token: 0x04000092 RID: 146
		private readonly IDebugCatalogService m_debugCatalogService;
	}
}
