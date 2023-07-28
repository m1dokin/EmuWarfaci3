using System;
using System.Collections.Generic;
using OLAPHypervisor;

namespace MasterServer.DAL
{
	// Token: 0x02000098 RID: 152
	public interface ITelemetrySystem
	{
		// Token: 0x060001C5 RID: 453
		void Start();

		// Token: 0x060001C6 RID: 454
		DALResultVoid RunAggregation();

		// Token: 0x060001C7 RID: 455
		DALResultMulti<Measure> GetPlayerStats(ulong profileId);

		// Token: 0x060001C8 RID: 456
		DALResultMulti<Measure> GetPlayerStatsRaw(ulong profileId);

		// Token: 0x060001C9 RID: 457
		DALResultVoid ResetPlayerStats(ulong profileId);

		// Token: 0x060001CA RID: 458
		DALResultVoid ResetTelemetryTestStats();

		// Token: 0x060001CB RID: 459
		DALResult<EAggOperation> GetDefaultAggregationOp();

		// Token: 0x060001CC RID: 460
		DALResult<Dictionary<string, EAggOperation>> GetAggregationOps();

		// Token: 0x060001CD RID: 461
		DALResult<bool> SetAggregationOps(string stat, EAggOperation aggOp);

		// Token: 0x060001CE RID: 462
		DALResultVoid AddMeasure(IEnumerable<Measure> msrs);
	}
}
