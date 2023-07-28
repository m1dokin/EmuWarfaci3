using System;
using System.Collections.Generic;
using OLAPHypervisor;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000215 RID: 533
	internal interface ITelemetrySystemClient
	{
		// Token: 0x06000B90 RID: 2960
		void RunAggregation();

		// Token: 0x06000B91 RID: 2961
		[Obsolete("For receive player statistics use PlayerStatsService")]
		IEnumerable<Measure> GetPlayerStats(ulong profileId);

		// Token: 0x06000B92 RID: 2962
		IEnumerable<Measure> GetPlayerStatsRaw(ulong profileId);

		// Token: 0x06000B93 RID: 2963
		void ResetPlayerStats(ulong profileId);

		// Token: 0x06000B94 RID: 2964
		void ResetTelemetryTestStats();

		// Token: 0x06000B95 RID: 2965
		EAggOperation GetDefaultAggregationOp();

		// Token: 0x06000B96 RID: 2966
		Dictionary<string, EAggOperation> GetAggregationOps();

		// Token: 0x06000B97 RID: 2967
		bool SetAggregationOps(string stat, EAggOperation newAggOp);

		// Token: 0x06000B98 RID: 2968
		void AddMeasure(IEnumerable<Measure> measures);
	}
}
