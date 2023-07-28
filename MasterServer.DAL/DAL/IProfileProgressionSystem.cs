using System;

namespace MasterServer.DAL
{
	// Token: 0x0200007C RID: 124
	public interface IProfileProgressionSystem
	{
		// Token: 0x0600015D RID: 349
		DALResult<SProfileProgression> GetProfileProgression(ulong profileId);

		// Token: 0x0600015E RID: 350
		DALResult<SProfileProgression> SetProfileProgression(SProfileProgression progress);

		// Token: 0x0600015F RID: 351
		DALResult<SProfileProgression> IncrementMissionPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000160 RID: 352
		DALResult<SProfileProgression> IncrementZombieMissionPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000161 RID: 353
		DALResult<SProfileProgression> IncrementCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000162 RID: 354
		DALResult<SProfileProgression> IncrementVolcanoCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000163 RID: 355
		DALResult<SProfileProgression> IncrementAnubisCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000164 RID: 356
		DALResult<SProfileProgression> IncrementZombieTowerCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000165 RID: 357
		DALResult<SProfileProgression> IncrementIceBreakerCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000166 RID: 358
		DALResult<SProfileProgression> UnlockTutorial(ulong profileId, int tutorialUnlocked);

		// Token: 0x06000167 RID: 359
		DALResult<SProfileProgression> PassTutorial(ulong profileId, int tutorialPassed);

		// Token: 0x06000168 RID: 360
		DALResult<SProfileProgression> UnlockClass(ulong profileId, int classUnlocked);

		// Token: 0x06000169 RID: 361
		DALResult<SProfileProgression> UnlockMission(ulong profileId, int unlockedMissionType);

		// Token: 0x0600016A RID: 362
		DALResultVoid DeleteProgression(ulong profileId);
	}
}
