using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000207 RID: 519
	internal interface IPerformanceSystemClient
	{
		// Token: 0x06000AFB RID: 2811
		MasterRecord GetPerformanceMasterRecord(string type, cache_domain domain);

		// Token: 0x06000AFC RID: 2812
		ProfilePerformance GetProfilePerformance(ulong profile_id, List<Guid> currentMissions);

		// Token: 0x06000AFD RID: 2813
		void UpdateMissionPerformance(PerformanceUpdate update, List<Guid> currentMissions);

		// Token: 0x06000AFE RID: 2814
		void CleanupMissionPerformance();

		// Token: 0x06000AFF RID: 2815
		bool TryBeginUpdate(string onlineId, string lockString, TimeSpan updateFreq);

		// Token: 0x06000B00 RID: 2816
		void EndUpdate(List<MasterRecord.Record> missionRecords, string lockString, IEnumerable<cache_domain> domains);

		// Token: 0x06000B01 RID: 2817
		bool SetMissionProfileWin(Guid missionId, ulong profileId);
	}
}
