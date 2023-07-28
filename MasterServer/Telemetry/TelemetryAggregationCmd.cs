using System;
using MasterServer.Core;

namespace MasterServer.Telemetry
{
	// Token: 0x020007D2 RID: 2002
	[ConsoleCmdAttributes(CmdName = "telemetry_run_aggregation", ArgsSize = 0)]
	internal class TelemetryAggregationCmd : IConsoleCmd
	{
		// Token: 0x06002904 RID: 10500 RVA: 0x000B1BFC File Offset: 0x000AFFFC
		public void ExecuteCmd(string[] args)
		{
			ITelemetryService service = ServicesManager.GetService<ITelemetryService>();
			service.RunAggregation();
		}
	}
}
