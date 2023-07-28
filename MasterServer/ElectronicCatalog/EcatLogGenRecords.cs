using System;
using MasterServer.Core;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000230 RID: 560
	[ConsoleCmdAttributes(CmdName = "debug_ecat_log_generate", ArgsSize = 2, Help = "Generate N records in ecat log, in day interval [Now - interval; Now]")]
	internal class EcatLogGenRecords : IConsoleCmd
	{
		// Token: 0x06000BE7 RID: 3047 RVA: 0x0002D5C8 File Offset: 0x0002B9C8
		public EcatLogGenRecords(IDebugCatalogService debugCatalogService)
		{
			this.m_debugCatalogService = debugCatalogService;
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x0002D5D7 File Offset: 0x0002B9D7
		public void ExecuteCmd(string[] args)
		{
			this.m_debugCatalogService.DebugGenEcatRecords(uint.Parse(args[1]), uint.Parse(args[2]));
		}

		// Token: 0x0400058C RID: 1420
		private readonly IDebugCatalogService m_debugCatalogService;
	}
}
