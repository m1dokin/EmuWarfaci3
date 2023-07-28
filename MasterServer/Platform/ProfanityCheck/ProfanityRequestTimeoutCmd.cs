using System;
using MasterServer.Core;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006AA RID: 1706
	[ConsoleCmdAttributes(CmdName = "profanity_request_timeout", ArgsSize = 1, Help = "Get\\Sets profanity request timeout")]
	internal class ProfanityRequestTimeoutCmd : IConsoleCmd
	{
		// Token: 0x060023DC RID: 9180 RVA: 0x00096B38 File Offset: 0x00094F38
		public void ExecuteCmd(string[] args)
		{
			IDebugProfanityCheckService service = ServicesManager.GetService<IDebugProfanityCheckService>();
			if (service != null)
			{
				if (args.Length > 1)
				{
					service.SetRequestTimeout(TimeSpan.FromMilliseconds(uint.Parse(args[1])));
				}
				Log.Info<TimeSpan>("Debug Profanity: request timeout is {0}", service.GetRequestTimeout());
			}
			else
			{
				Log.Warning("Can't find IDebugProfanityCheckService to execute command.");
			}
		}
	}
}
