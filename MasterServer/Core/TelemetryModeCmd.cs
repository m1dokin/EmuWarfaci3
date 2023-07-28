using System;
using System.Text;
using Util.Common;

namespace MasterServer.Core
{
	// Token: 0x02000824 RID: 2084
	[ConsoleCmdAttributes(CmdName = "log_verbose_group", ArgsSize = 1)]
	internal class TelemetryModeCmd : IConsoleCmd
	{
		// Token: 0x06002AFF RID: 11007 RVA: 0x000B9CFC File Offset: 0x000B80FC
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 1)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("verbose_group = ");
				stringBuilder.Append(ReflectionUtils.EnumFlagsToString<Log.Group>(Log.VerboseGroup));
				stringBuilder.Append("\nAvailable groups: ");
				stringBuilder.Append(string.Join(" | ", Enum.GetNames(typeof(Log.Group))));
				Log.Info(stringBuilder.ToString());
			}
			else
			{
				Log.SetVerboseGroup(args[1]);
				Log.Info("log_verbose_group = " + ReflectionUtils.EnumFlagsToString<Log.Group>(Log.VerboseGroup));
			}
		}
	}
}
