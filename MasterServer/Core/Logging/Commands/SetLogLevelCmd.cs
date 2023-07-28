using System;
using NLog;
using NLog.Config;

namespace MasterServer.Core.Logging.Commands
{
	// Token: 0x02000105 RID: 261
	[ConsoleCmdAttributes(CmdName = "set_log_level", Help = "Sets log level for all log rules.")]
	internal class SetLogLevelCmd : ConsoleCommand<SetLogLevelCmdParams>
	{
		// Token: 0x06000443 RID: 1091 RVA: 0x000126D4 File Offset: 0x00010AD4
		protected override void Execute(SetLogLevelCmdParams param)
		{
			LogLevel logLevel = LogLevel.FromString(param.Level);
			foreach (LoggingRule loggingRule in LogManager.Configuration.LoggingRules)
			{
				for (int i = LogLevel.Trace.Ordinal; i <= LogLevel.Fatal.Ordinal; i++)
				{
					LogLevel level = LogLevel.FromOrdinal(i);
					if (i < logLevel.Ordinal)
					{
						loggingRule.DisableLoggingForLevel(level);
					}
					else
					{
						loggingRule.EnableLoggingForLevel(level);
					}
				}
			}
			LogManager.ReconfigExistingLoggers();
			GetLogLevelCmd.Execute();
		}
	}
}
