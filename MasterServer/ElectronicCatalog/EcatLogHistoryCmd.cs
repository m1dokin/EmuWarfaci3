using System;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200022F RID: 559
	[ConsoleCmdAttributes(CmdName = "ecat_log_history", ArgsSize = 1, Help = "userID")]
	internal class EcatLogHistoryCmd : IConsoleCmd
	{
		// Token: 0x06000BE5 RID: 3045 RVA: 0x0002D544 File Offset: 0x0002B944
		public EcatLogHistoryCmd(IDebugCatalogService debugCatalogService)
		{
			this.m_debugCatalogService = debugCatalogService;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0002D554 File Offset: 0x0002B954
		public void ExecuteCmd(string[] args)
		{
			ulong customerId = ulong.Parse(args[1]);
			foreach (EcatLogHistory ecatLogHistory in this.m_debugCatalogService.DebugGetLogHistory(customerId))
			{
				Log.Info(ecatLogHistory.ToString());
			}
		}

		// Token: 0x0400058B RID: 1419
		private readonly IDebugCatalogService m_debugCatalogService;
	}
}
