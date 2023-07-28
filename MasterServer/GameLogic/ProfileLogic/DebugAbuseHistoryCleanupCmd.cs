using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000542 RID: 1346
	[ConsoleCmdAttributes(CmdName = "abuse_history_cleanup", ArgsSize = 0, Help = "Call non-scheduled abuse history cleanup")]
	internal class DebugAbuseHistoryCleanupCmd : IConsoleCmd
	{
		// Token: 0x06001D25 RID: 7461 RVA: 0x00075E34 File Offset: 0x00074234
		public DebugAbuseHistoryCleanupCmd(IAbuseReportService abuseReportService)
		{
			this.m_abuseReportService = abuseReportService;
		}

		// Token: 0x06001D26 RID: 7462 RVA: 0x00075E43 File Offset: 0x00074243
		public void ExecuteCmd(string[] args)
		{
			this.m_abuseReportService.DebugCleanupHistory();
		}

		// Token: 0x04000DEC RID: 3564
		private readonly IAbuseReportService m_abuseReportService;
	}
}
