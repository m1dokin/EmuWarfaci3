using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x02000078 RID: 120
	public interface IPerformanceSystem
	{
		// Token: 0x0600014E RID: 334
		DALResultVoid UpdateMissionPerformance(PerformanceUpdate update, List<Guid> current_missions);

		// Token: 0x0600014F RID: 335
		DALResult<ProfilePerformance> GetProfilePerformance(ulong profile_id, List<Guid> current_missions);

		// Token: 0x06000150 RID: 336
		DALResultVoid CleanupMissionPerformance();

		// Token: 0x06000151 RID: 337
		DALResult<MasterRecord> GetPerformanceMasterRecord(string type);

		// Token: 0x06000152 RID: 338
		DALResult<bool> TryBeginUpdate(string onlineId, string lockRecord, TimeSpan updateFreq);

		// Token: 0x06000153 RID: 339
		DALResultVoid EndUpdate(List<MasterRecord.Record> mission_records, string lockRecord);

		// Token: 0x06000154 RID: 340
		DALResult<bool> SetMissionProfileWin(Guid missionId, ulong profileId);
	}
}
