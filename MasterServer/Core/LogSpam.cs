using System;
using MasterServer.Users;

namespace MasterServer.Core
{
	// Token: 0x02000126 RID: 294
	[ConsoleCmdAttributes(CmdName = "log_spam", ArgsSize = 1, Help = "Spam log server with a lot og log events")]
	internal class LogSpam : IConsoleCmd
	{
		// Token: 0x060004D8 RID: 1240 RVA: 0x00014EDD File Offset: 0x000132DD
		public LogSpam(ILogService logService)
		{
			this.m_logService = logService;
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00014EEC File Offset: 0x000132EC
		public void ExecuteCmd(string[] args)
		{
			int num = int.Parse(args[1]);
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				this.m_logService.Event.AbuseReportLog((ulong)num2, (ulong)num2, "127.0.0.1", (ulong)num2, (ulong)num2, "type" + num2, "comment" + num2);
				this.m_logService.Event.CharacterLogoutLog((ulong)num2, DateTime.Now, "127.0.0.1", (ulong)num2, "nickname" + num2, (int)num2, (ulong)num2, TimeSpan.FromSeconds(num2), TimeSpan.FromSeconds(num2), (ulong)num2, (ulong)num2, (ulong)num2, num2 + "@warface/client", "token" + num2, "tags" + num2, ELogoutType.LostConnection);
				num2 += 1U;
			}
		}

		// Token: 0x04000207 RID: 519
		private readonly ILogService m_logService;
	}
}
