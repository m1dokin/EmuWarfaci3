using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Database;
using OLAPHypervisor;

namespace MasterServer.Telemetry
{
	// Token: 0x020007D6 RID: 2006
	[ConsoleCmdAttributes(CmdName = "debug_telemetry_set_agg", ArgsSize = 2, Help = "Set aggregation option for stat [stat name] [sum|discard]")]
	internal class DebugTelemetrySetAggCmd : IConsoleCmd
	{
		// Token: 0x0600290B RID: 10507 RVA: 0x000B1E4A File Offset: 0x000B024A
		public DebugTelemetrySetAggCmd(ITelemetryDALService telemetryDal)
		{
			this.m_telemetryDal = telemetryDal;
		}

		// Token: 0x0600290C RID: 10508 RVA: 0x000B1E5C File Offset: 0x000B025C
		public void ExecuteCmd(string[] args)
		{
			string text = args[1];
			EAggOperation eaggOperation = Utils.ParseEnum<EAggOperation>(args[2]);
			bool flag = this.m_telemetryDal.TelemetrySystem.SetAggregationOps(text, eaggOperation);
			MasterServer.Core.Log.Info<EAggOperation, string, string>("Debug set '{0}' aggregation option for stat '{1}' {2}", eaggOperation, text, (!flag) ? "failed" : "succeeded");
		}

		// Token: 0x040015E1 RID: 5601
		private readonly ITelemetryDALService m_telemetryDal;
	}
}
