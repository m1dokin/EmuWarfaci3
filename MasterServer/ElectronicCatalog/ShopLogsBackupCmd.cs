using System;
using MasterServer.Core;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200022D RID: 557
	[ConsoleCmdAttributes(CmdName = "shop_logs_backup", ArgsSize = 0)]
	internal class ShopLogsBackupCmd : IConsoleCmd
	{
		// Token: 0x06000BE1 RID: 3041 RVA: 0x0002D4EC File Offset: 0x0002B8EC
		public ShopLogsBackupCmd(IDebugCatalogService debugCatalogService)
		{
			this.m_debugCatalogService = debugCatalogService;
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x0002D4FB File Offset: 0x0002B8FB
		public void ExecuteCmd(string[] args)
		{
			this.m_debugCatalogService.DebugLogsBackup();
		}

		// Token: 0x04000589 RID: 1417
		private readonly IDebugCatalogService m_debugCatalogService;
	}
}
