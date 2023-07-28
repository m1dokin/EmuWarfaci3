using System;
using HK2Net;
using MasterServer.Core.Services.Metrics;
using MasterServer.Platform.ProfanityCheck;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000707 RID: 1799
	[Contract]
	internal interface IProfanityMetricsTracker : IMetricsProvider, IDisposable
	{
		// Token: 0x06002596 RID: 9622
		void ReportProfanityRequest(ProfanityCheckService.CheckType checkType);

		// Token: 0x06002597 RID: 9623
		void ReportProfanityRequestTime(ProfanityCheckService.CheckType checkType, TimeSpan time);

		// Token: 0x06002598 RID: 9624
		void ReportProfanityRequestFailed(ProfanityCheckService.CheckType checkType);

		// Token: 0x06002599 RID: 9625
		void ReportProfanityResult(ProfanityCheckService.CheckType checkType, ProfanityCheckResult checkResult);
	}
}
