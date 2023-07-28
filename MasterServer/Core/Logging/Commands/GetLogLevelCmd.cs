using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace MasterServer.Core.Logging.Commands
{
	// Token: 0x02000103 RID: 259
	[ConsoleCmdAttributes(CmdName = "get_log_level", Help = "Gets log level of all log rules.")]
	internal class GetLogLevelCmd : IConsoleCmd
	{
		// Token: 0x06000439 RID: 1081 RVA: 0x00012428 File Offset: 0x00010828
		public static void Execute()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(string.Format("Available log levels: {0}", string.Join(",", from l in GetLogLevelCmd.GetLogLevels()
			select l.Name)));
			stringBuilder.AppendLine("Log rules:");
			foreach (LoggingRule loggingRule in LogManager.Configuration.LoggingRules)
			{
				if (loggingRule.Targets.Any<Target>())
				{
					string arg = string.Join(",", from l in loggingRule.Levels
					select l.Name);
					string arg2 = string.Join(",", from t in loggingRule.Targets
					select t.Name);
					stringBuilder.AppendLine(string.Format("\tLevels='{0}' Targets='{1}'", arg, arg2));
				}
			}
			GetLogLevelCmd.Logger.Fatal(stringBuilder.ToString());
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x00012574 File Offset: 0x00010974
		public void ExecuteCmd(string[] args)
		{
			GetLogLevelCmd.Execute();
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x0001257C File Offset: 0x0001097C
		private static IEnumerable<LogLevel> GetLogLevels()
		{
			for (int i = LogLevel.Trace.Ordinal; i <= LogLevel.Fatal.Ordinal; i++)
			{
				yield return LogLevel.FromOrdinal(i);
			}
			yield break;
		}

		// Token: 0x040001BC RID: 444
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
	}
}
