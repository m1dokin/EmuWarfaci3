using System;

namespace MasterServer.Core
{
	// Token: 0x02000029 RID: 41
	[ConsoleCmdAttributes(CmdName = "quit", ArgsSize = 0, Help = "Close MasterServer instance")]
	internal class QuitCmd : IConsoleCmd
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00006DAC File Offset: 0x000051AC
		public void ExecuteCmd(string[] args)
		{
			try
			{
				Log.Info("Closing MasterServer ...");
				if (Environment.OSVersion.Platform != PlatformID.Unix)
				{
					ServicesManager.Stop();
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
			finally
			{
				Environment.Exit(0);
			}
		}
	}
}
