using System;

namespace MasterServer.DAL
{
	// Token: 0x02000092 RID: 146
	public interface IRewardsSystem
	{
		// Token: 0x060001B5 RID: 437
		DALResultMulti<SSponsorPoints> GetSponsorPoints(ulong profileId);

		// Token: 0x060001B6 RID: 438
		DALResultVoid SetSponsorPoints(ulong profile_id, uint sponsor_id, ulong sponsor_points);

		// Token: 0x060001B7 RID: 439
		DALResult<bool> SetSponsorInfo(ulong profile_id, uint sponsor_id, ulong old_spPts, SRankInfo new_sp);

		// Token: 0x060001B8 RID: 440
		DALResultVoid SetNextUnlockItem(ulong profile_id, uint sponsor_id, ulong next_unlock_item_id);
	}
}
