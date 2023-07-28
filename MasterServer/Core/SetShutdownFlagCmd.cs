using System;

namespace MasterServer.Core
{
	// Token: 0x02000027 RID: 39
	[ConsoleCmdAttributes(CmdName = "app_schedule_shutdown", ArgsSize = 1)]
	internal class SetShutdownFlagCmd : IConsoleCmd
	{
		// Token: 0x06000092 RID: 146 RVA: 0x00006D40 File Offset: 0x00005140
		public void ExecuteCmd(string[] args)
		{
			TimeSpan timeout = (args.Length <= 1) ? TimeSpan.Zero : new TimeSpan(0, 0, int.Parse(args[1]));
			ServicesManager.GetService<IApplicationService>().ScheduleShutdown(timeout);
			Log.Info("Server shutdown was scheduled");
		}
	}
}
