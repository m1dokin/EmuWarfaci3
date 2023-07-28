using System;
using HK2Net;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006FC RID: 1788
	[Contract]
	internal interface IMatchMakingTracker
	{
		// Token: 0x0600256E RID: 9582
		void TrackRegionViolation(string regionId);
	}
}
