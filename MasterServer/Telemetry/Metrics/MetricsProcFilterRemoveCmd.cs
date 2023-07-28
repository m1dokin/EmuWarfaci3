using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MasterServer.Core;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006F3 RID: 1779
	[ConsoleCmdAttributes(CmdName = "metrics_proc_filter_remove", ArgsSize = 1)]
	internal class MetricsProcFilterRemoveCmd : IConsoleCmd
	{
		// Token: 0x06002552 RID: 9554 RVA: 0x0009C7AC File Offset: 0x0009ABAC
		public void ExecuteCmd(string[] args)
		{
			IDalMetricsTracker service = ServicesManager.GetService<IDalMetricsTracker>();
			IEnumerable<string> source = args.Skip(1);
			if (MetricsProcFilterRemoveCmd.<>f__mg$cache0 == null)
			{
				MetricsProcFilterRemoveCmd.<>f__mg$cache0 = new Func<string, int>(int.Parse);
			}
			IEnumerable<int> enumerable = source.Select(MetricsProcFilterRemoveCmd.<>f__mg$cache0);
			foreach (int index in enumerable)
			{
				service.RemoveInterest(index);
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

		// Token: 0x040012E4 RID: 4836
		[CompilerGenerated]
		private static Func<string, int> <>f__mg$cache0;
	}
}
