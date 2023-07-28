using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001EF RID: 495
	internal interface IAchievementsSystemClient
	{
		// Token: 0x060009A6 RID: 2470
		IEnumerable<AchievementInfo> GetProfileAchievements(ulong profileId);

		// Token: 0x060009A7 RID: 2471
		SAchievementUpdate SetAchievementProgress(ulong profile_id, uint ach_id, int new_progress, uint max_progress, ulong completion_time);

		// Token: 0x060009A8 RID: 2472
		SAchievementUpdate UpdateAchievementProgress(ulong profile_id, uint ach_id, int incr_progress, uint max_progress);

		// Token: 0x060009A9 RID: 2473
		void DeleteProfileAchievements(ulong profile_id);

		// Token: 0x060009AA RID: 2474
		void DeleteProfileAchievement(ulong profile_id, uint achievment_id);
	}
}
