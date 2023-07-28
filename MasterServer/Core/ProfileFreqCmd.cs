using System;

namespace MasterServer.Core
{
	// Token: 0x020007A3 RID: 1955
	[ConsoleCmdAttributes(CmdName = "profile_freq", ArgsSize = 1)]
	internal class ProfileFreqCmd : IConsoleCmd
	{
		// Token: 0x06002871 RID: 10353 RVA: 0x000AE0EC File Offset: 0x000AC4EC
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 1)
			{
				Log.Info<int>("profile_freq = {0}", Profiler.Freq);
				return;
			}
			Profiler.Freq = int.Parse(args[1]);
		}
	}
}
