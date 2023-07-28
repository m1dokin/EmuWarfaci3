using System;

namespace MasterServer.Core
{
	// Token: 0x02000159 RID: 345
	[ConsoleCmdAttributes(CmdName = "dump_online_variables", ArgsSize = 0)]
	internal class DumpOnlineVariablesCmd : IConsoleCmd
	{
		// Token: 0x06000610 RID: 1552 RVA: 0x00018530 File Offset: 0x00016930
		public void ExecuteCmd(string[] args)
		{
			IOnlineVariables service = ServicesManager.GetService<IOnlineVariables>();
			service.Dump();
		}
	}
}
