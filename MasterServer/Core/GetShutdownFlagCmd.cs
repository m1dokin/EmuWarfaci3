using System;

namespace MasterServer.Core
{
	// Token: 0x02000028 RID: 40
	[ConsoleCmdAttributes(CmdName = "app_get_shutdown_flag", ArgsSize = 0)]
	internal class GetShutdownFlagCmd : IConsoleCmd
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00006D8D File Offset: 0x0000518D
		public void ExecuteCmd(string[] args)
		{
			Log.Info<bool>("Server shutdown flag is set to {0}", ServicesManager.GetService<IApplicationService>().IsShutdownScheduled);
		}
	}
}
