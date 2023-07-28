using System;
using MasterServer.Core;

namespace MasterServer.Database
{
	// Token: 0x020001CF RID: 463
	[ConsoleCmdAttributes(CmdName = "db_drop_procedure", ArgsSize = 1, Help = "Drop procedure from game database")]
	internal class DbDropProcedureCmd : IConsoleCmd
	{
		// Token: 0x060008BB RID: 2235 RVA: 0x000210D9 File Offset: 0x0001F4D9
		public DbDropProcedureCmd() : this(ServicesManager.GetService<IDebugDbUpdateService>())
		{
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x000210E6 File Offset: 0x0001F4E6
		public DbDropProcedureCmd(IDebugDbUpdateService debugDbUpdateService)
		{
			this.m_debugDbUpdateService = debugDbUpdateService;
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x000210F8 File Offset: 0x0001F4F8
		public void ExecuteCmd(string[] args)
		{
			if (this.m_debugDbUpdateService != null)
			{
				string text = args[1];
				this.m_debugDbUpdateService.DropProcedure(text);
				Log.Info<string>("Debug DB: procedure '{0}' has been dropped", text);
			}
			else
			{
				Log.Warning("Can't find IDebugDbUpdateService to execute command.");
			}
		}

		// Token: 0x0400051B RID: 1307
		private readonly IDebugDbUpdateService m_debugDbUpdateService;
	}
}
