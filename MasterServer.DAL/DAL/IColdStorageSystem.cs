using System;
using MasterServer.DAL.Utils;

namespace MasterServer.DAL
{
	// Token: 0x0200007B RID: 123
	public interface IColdStorageSystem
	{
		// Token: 0x06000157 RID: 343
		DALResult<bool?> IsProfileCold(ulong profile_id);

		// Token: 0x06000158 RID: 344
		DALResult<TouchProfileResult> TouchProfile(ulong profile_id, DBVersion current_schema);

		// Token: 0x06000159 RID: 345
		DALResult<bool> MoveProfileToCold(ulong profile_id, TimeSpan threshold, DBVersion current_schema);

		// Token: 0x0600015A RID: 346
		DALResultMulti<ulong> GetUnusedProfiles(TimeSpan threshold, int limit);

		// Token: 0x0600015B RID: 347
		DALResultMulti<ulong> GetColdProfiles(int limit);

		// Token: 0x0600015C RID: 348
		DALResult<ColdProfileData> GetColdProfileData(ulong profile_id, DBVersion current_schema);
	}
}
