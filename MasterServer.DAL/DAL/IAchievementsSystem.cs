using System;

namespace MasterServer.DAL
{
	// Token: 0x0200000C RID: 12
	public interface IAchievementsSystem
	{
		// Token: 0x06000017 RID: 23
		DALResultMulti<AchievementInfo> GetProfileAchievements(ulong profile_id);

		// Token: 0x06000018 RID: 24
		DALResult<SAchievementUpdate> SetAchievementProgress(ulong profile_id, uint ach_id, int new_progress, uint max_progress, ulong completion_time);

		// Token: 0x06000019 RID: 25
		DALResult<SAchievementUpdate> UpdateAchievementProgress(ulong profile_id, uint ach_id, int incr_progress, uint max_progress);

		// Token: 0x0600001A RID: 26
		DALResultVoid DeleteProfileAchievements(ulong profile_id);

		// Token: 0x0600001B RID: 27
		DALResultVoid DeleteProfileAchievement(ulong profile_id, uint achievment_id);
	}
}
