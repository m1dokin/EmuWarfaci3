using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MasterServer.Core;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000712 RID: 1810
	[ConsoleCmdAttributes(CmdName = "metrics_query_filter_remove", ArgsSize = 1)]
	internal class MetricsQueryFilterRemoveCmd : IConsoleCmd
	{
		// Token: 0x060025C3 RID: 9667 RVA: 0x0009E87C File Offset: 0x0009CC7C
		public void ExecuteCmd(string[] args)
		{
			IQueryMetricsTracker service = ServicesManager.GetService<IQueryMetricsTracker>();
			IEnumerable<string> source = args.Skip(1);
			if (MetricsQueryFilterRemoveCmd.<>f__mg$cache0 == null)
			{
				MetricsQueryFilterRemoveCmd.<>f__mg$cache0 = new Func<string, int>(int.Parse);
			}
			IEnumerable<int> enumerable = source.Select(MetricsQueryFilterRemoveCmd.<>f__mg$cache0);
			foreach (int index in enumerable)
			{
				service.RemoveInterest(index);
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

		// Token: 0x0400133B RID: 4923
		[CompilerGenerated]
		private static Func<string, int> <>f__mg$cache0;
	}
}
