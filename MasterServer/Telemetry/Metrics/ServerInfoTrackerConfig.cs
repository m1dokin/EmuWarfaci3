using System;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006D8 RID: 1752
	public class ServerInfoTrackerConfig
	{
		// Token: 0x060024D3 RID: 9427 RVA: 0x0009A1E2 File Offset: 0x000985E2
		public ServerInfoTrackerConfig(TimeSpan reportTime)
		{
			this.ReportTime = reportTime;
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x060024D4 RID: 9428 RVA: 0x0009A1F1 File Offset: 0x000985F1
		// (set) Token: 0x060024D5 RID: 9429 RVA: 0x0009A1F9 File Offset: 0x000985F9
		public TimeSpan ReportTime { get; private set; }
	}
}
