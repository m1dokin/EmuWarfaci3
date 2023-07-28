using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Core;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000711 RID: 1809
	[ConsoleCmdAttributes(CmdName = "metrics_query_filter", ArgsSize = 1)]
	internal class MetricsQueryFilterCmd : IConsoleCmd
	{
		// Token: 0x060025C1 RID: 9665 RVA: 0x0009E7B4 File Offset: 0x0009CBB4
		public void ExecuteCmd(string[] args)
		{
			IQueryMetricsTracker service = ServicesManager.GetService<IQueryMetricsTracker>();
			foreach (string wildcard in args.Skip(1))
			{
				service.AddInterest(wildcard);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Query filters:");
			List<string> interests = service.GetInterests();
			for (int num = 0; num != interests.Count; num++)
			{
				stringBuilder.AppendFormat("  [{0}] {1}\n", num, interests[num]);
			}
			Log.Info(stringBuilder.ToString());
		}
	}
}
