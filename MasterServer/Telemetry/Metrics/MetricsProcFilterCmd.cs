using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Core;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006F2 RID: 1778
	[ConsoleCmdAttributes(CmdName = "metrics_proc_filter", ArgsSize = 1)]
	internal class MetricsProcFilterCmd : IConsoleCmd
	{
		// Token: 0x06002550 RID: 9552 RVA: 0x0009C6E4 File Offset: 0x0009AAE4
		public void ExecuteCmd(string[] args)
		{
			IDalMetricsTracker service = ServicesManager.GetService<IDalMetricsTracker>();
			foreach (string wildcard in args.Skip(1))
			{
				service.AddInterest(wildcard);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Procedure filters:");
			List<string> interests = service.GetInterests();
			for (int num = 0; num != interests.Count; num++)
			{
				stringBuilder.AppendFormat("  [{0}] {1}\n", num, interests[num]);
			}
			Log.Info(stringBuilder.ToString());
		}
	}
}
