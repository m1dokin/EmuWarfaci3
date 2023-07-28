using System;
using HK2Net;
using MasterServer.Core.Services.Metrics;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x0200071D RID: 1821
	[Contract]
	internal interface IMapVoting : IMetricsProvider, IDisposable
	{
		// Token: 0x060025E6 RID: 9702
		void ReportVotesAfterVotingFinished();
	}
}
