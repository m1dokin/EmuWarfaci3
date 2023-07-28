using System;
using MasterServer.Core;

namespace MasterServer.Database
{
	// Token: 0x020001D0 RID: 464
	[ConsoleCmdAttributes(CmdName = "db_restore_procedures", ArgsSize = 0, Help = "Restore all procedures in game database")]
	internal class DbRestoreProceduresCmd : IConsoleCmd
	{
		// Token: 0x060008BE RID: 2238 RVA: 0x0002113A File Offset: 0x0001F53A
		public DbRestoreProceduresCmd() : this(ServicesManager.GetService<IDebugDbUpdateService>())
		{
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x00021147 File Offset: 0x0001F547
		public DbRestoreProceduresCmd(IDebugDbUpdateService debugDbUpdateService)
		{
			this.m_debugDbUpdateService = debugDbUpdateService;
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x00021156 File Offset: 0x0001F556
		public void ExecuteCmd(string[] args)
		{
			if (this.m_debugDbUpdateService != null)
			{
				this.m_debugDbUpdateService.RestoreProcedures();
				Log.Info("Debug DB: all procedures have been restored");
			}
			else
			{
				Log.Warning("Can't find IDebugDbUpdateService to execute command.");
			}
		}

		// Token: 0x0400051C RID: 1308
		private readonly IDebugDbUpdateService m_debugDbUpdateService;
	}
}
