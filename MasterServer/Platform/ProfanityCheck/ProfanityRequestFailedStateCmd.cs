using System;
using MasterServer.Core;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006A9 RID: 1705
	[ConsoleCmdAttributes(CmdName = "profanity_request_failed", ArgsSize = 1, Help = "Get\\Sets profanity request failed state")]
	internal class ProfanityRequestFailedStateCmd : IConsoleCmd
	{
		// Token: 0x060023DA RID: 9178 RVA: 0x00096ADC File Offset: 0x00094EDC
		public void ExecuteCmd(string[] args)
		{
			IDebugProfanityCheckService service = ServicesManager.GetService<IDebugProfanityCheckService>();
			if (service != null)
			{
				if (args.Length > 1)
				{
					service.SetRequestFailedState(args[1] == "1");
				}
				Log.Info<bool>("Debug Profanity: request failed state is {0}", service.GetRequestFailedState());
			}
			else
			{
				Log.Warning("Can't find IDebugProfanityCheckService to execute command.");
			}
		}
	}
}
