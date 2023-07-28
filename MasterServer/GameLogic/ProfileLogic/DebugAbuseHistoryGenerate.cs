using System;
using MasterServer.Core;
using MasterServer.Database;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000543 RID: 1347
	[ConsoleCmdAttributes(CmdName = "abuse_history_generate", ArgsSize = 2, Help = "Generate N reports, in day interval [Now - interval; Now]")]
	internal class DebugAbuseHistoryGenerate : IConsoleCmd
	{
		// Token: 0x06001D27 RID: 7463 RVA: 0x00075E50 File Offset: 0x00074250
		public DebugAbuseHistoryGenerate(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x06001D28 RID: 7464 RVA: 0x00075E5F File Offset: 0x0007425F
		public void ExecuteCmd(string[] args)
		{
			Log.Info("Abuse history generation started");
			this.m_dalService.AbuseSystem.GenererateAbuseHistory(uint.Parse(args[1]), uint.Parse(args[2]));
			Log.Info("Abuse history generation finished");
		}

		// Token: 0x04000DED RID: 3565
		private readonly IDALService m_dalService;
	}
}
