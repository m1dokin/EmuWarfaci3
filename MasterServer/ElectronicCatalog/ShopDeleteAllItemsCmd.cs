using System;
using MasterServer.Core;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200022E RID: 558
	[ConsoleCmdAttributes(CmdName = "shop_delete_all_items", ArgsSize = 1)]
	internal class ShopDeleteAllItemsCmd : IConsoleCmd
	{
		// Token: 0x06000BE3 RID: 3043 RVA: 0x0002D508 File Offset: 0x0002B908
		public ShopDeleteAllItemsCmd(IDebugCatalogService debugCatalogService)
		{
			this.m_debugCatalogService = debugCatalogService;
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x0002D518 File Offset: 0x0002B918
		public void ExecuteCmd(string[] args)
		{
			ulong customerId = ulong.Parse(args[1]);
			this.m_debugCatalogService.DebugDeleteAllItems(customerId);
			Log.Info("Items have been deleted.");
		}

		// Token: 0x0400058A RID: 1418
		private readonly IDebugCatalogService m_debugCatalogService;
	}
}
