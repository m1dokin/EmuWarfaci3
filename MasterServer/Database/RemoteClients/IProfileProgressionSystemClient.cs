using System;
using MasterServer.DAL;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x0200020B RID: 523
	internal interface IProfileProgressionSystemClient
	{
		// Token: 0x06000B15 RID: 2837
		SProfileProgression GetProfileProgression(ulong profileId);

		// Token: 0x06000B16 RID: 2838
		SProfileProgression SetProfileProgression(ulong profileId, ProfileProgressionInfo progress);

		// Token: 0x06000B17 RID: 2839
		SProfileProgression IncrementMissionPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000B18 RID: 2840
		SProfileProgression IncrementZombieMissionPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000B19 RID: 2841
		SProfileProgression IncrementCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000B1A RID: 2842
		SProfileProgression IncrementVolcanoCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000B1B RID: 2843
		SProfileProgression IncrementAnubisCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000B1C RID: 2844
		SProfileProgression IncrementZombieTowerCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000B1D RID: 2845
		SProfileProgression IncrementIceBreakerCampaignPassCounter(ulong profileId, int value, int maxValue);

		// Token: 0x06000B1E RID: 2846
		SProfileProgression UnlockTutorial(ulong profileId, ProfileProgressionInfo.Tutorial tutorialUnlocked);

		// Token: 0x06000B1F RID: 2847
		SProfileProgression PassTutorial(ulong profileId, ProfileProgressionInfo.Tutorial tutorialPassed);

		// Token: 0x06000B20 RID: 2848
		SProfileProgression UnlockClass(ulong profileId, ProfileProgressionInfo.PlayerClass classUnlocked);

		// Token: 0x06000B21 RID: 2849
		SProfileProgression UnlockMission(ulong profileId, ProfileProgressionInfo.MissionType unlockedMissionType);

		// Token: 0x06000B22 RID: 2850
		void DeleteProgression(ulong profileId);
	}
}
