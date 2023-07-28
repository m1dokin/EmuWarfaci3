using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x02000259 RID: 601
	[Contract]
	public interface IAchievementSystem
	{
		// Token: 0x06000D3B RID: 3387
		AchievementDescription GetAchievementDesc(uint id);

		// Token: 0x06000D3C RID: 3388
		Dictionary<uint, AchievementDescription> GetAllAchievementDescs();

		// Token: 0x06000D3D RID: 3389
		Dictionary<uint, AchievementUpdateChunk> GetCurrentProfileAchievements(ulong profileId);

		// Token: 0x06000D3E RID: 3390
		bool SetAchievementProgress(ulong profileId, AchievementDescription ach, ref AchievementUpdateChunk newState);

		// Token: 0x06000D3F RID: 3391
		bool UpdateAchievementProgress(ulong profileId, AchievementDescription ach, ref AchievementUpdateChunk newState);

		// Token: 0x06000D40 RID: 3392
		AchievementLockStatus DeleteProfileHiddenAchievement(ulong profileId, uint achievementId);
	}
}
