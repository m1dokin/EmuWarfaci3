using System;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000541 RID: 1345
	[ConsoleCmdAttributes(CmdName = "dump_abuse_reports", ArgsSize = 1, Help = "Dump abuse reports from ProfileID")]
	internal class DumpAbuseReportsCmd : IConsoleCmd
	{
		// Token: 0x06001D23 RID: 7459 RVA: 0x00075D4D File Offset: 0x0007414D
		public DumpAbuseReportsCmd(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x00075D5C File Offset: 0x0007415C
		public void ExecuteCmd(string[] args)
		{
			ulong profile_id;
			if (args.Length >= 2 && ulong.TryParse(args[1], out profile_id))
			{
				Log.Info<string>("Reports from PID {0}:", args[1]);
				foreach (SAbuseReport sabuseReport in this.m_dalService.AbuseSystem.GetAbuseReports(profile_id))
				{
					Log.Info("\t[{0}] From PID {1} To PID {2}; Reason: {3}", new object[]
					{
						sabuseReport.Timestamp,
						sabuseReport.From,
						sabuseReport.To,
						sabuseReport.Type
					});
				}
			}
			else
			{
				Log.Error("Enter correct Profile ID");
			}
		}

		// Token: 0x04000DEB RID: 3563
		private readonly IDALService m_dalService;
	}
}
