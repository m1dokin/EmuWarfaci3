using System;
using System.Collections.Generic;
using MasterServer.Core;
using OLAPHypervisor;

namespace MasterServer.Telemetry
{
	// Token: 0x020007D4 RID: 2004
	[ConsoleCmdAttributes(CmdName = "debug_telemetry_add", ArgsSize = 2, Help = "Generate debug telemetry stats [num of stats] [test_stat_01|test_stat_02]")]
	internal class DebugTelemetryAddStatsCmd : IConsoleCmd
	{
		// Token: 0x06002907 RID: 10503 RVA: 0x000B1D08 File Offset: 0x000B0108
		public DebugTelemetryAddStatsCmd(ITelemetryService telemetryService)
		{
			this.m_telemetryService = telemetryService;
			this.m_randomer = new Random((int)DateTime.Now.Ticks);
		}

		// Token: 0x06002908 RID: 10504 RVA: 0x000B1D3C File Offset: 0x000B013C
		public void ExecuteCmd(string[] args)
		{
			List<Measure> list = new List<Measure>();
			int num = int.Parse(args[1]);
			string text = args[2];
			DateTime d = DateTime.Now - TimeSpan.FromDays((double)num);
			for (int i = 0; i < num; i++)
			{
				d += TimeSpan.FromDays(1.0);
				string value = d.ToString("yyyy-MM-dd");
				list.Add(new Measure
				{
					AggregateOp = EAggOperation.Sum,
					RowCount = 1L,
					Value = (long)this.m_randomer.Next(100),
					Dimensions = new SortedList<string, string>
					{
						{
							"stat",
							text
						},
						{
							"date",
							value
						}
					}
				});
			}
			this.m_telemetryService.AddMeasure(list);
			MasterServer.Core.Log.Info<string>("Debug stat '{0}' added to telemetry", text);
		}

		// Token: 0x040015DD RID: 5597
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x040015DE RID: 5598
		private const string TIME_FMT = "yyyy-MM-dd";

		// Token: 0x040015DF RID: 5599
		private readonly Random m_randomer;
	}
}
