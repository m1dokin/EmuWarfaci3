using System;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006FF RID: 1791
	[ConsoleCmdAttributes(CmdName = "metrics_enabled", ArgsSize = 1)]
	internal class ToggleMetricsModeCmd : IConsoleCmd
	{
		// Token: 0x06002579 RID: 9593 RVA: 0x0009D4BE File Offset: 0x0009B8BE
		public ToggleMetricsModeCmd(IMetricsService metricsService)
		{
			this.m_metricsService = metricsService;
		}

		// Token: 0x0600257A RID: 9594 RVA: 0x0009D4D0 File Offset: 0x0009B8D0
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 2)
			{
				int num = int.Parse(args[1]);
				this.m_metricsService.Enabled = (num != 0);
			}
			Log.Info<int>("metrics_enabled = {0}", (!this.m_metricsService.Enabled) ? 0 : 1);
		}

		// Token: 0x04001302 RID: 4866
		private readonly IMetricsService m_metricsService;
	}
}
