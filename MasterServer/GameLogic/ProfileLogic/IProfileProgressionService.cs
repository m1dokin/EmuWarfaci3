using System;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200055F RID: 1375
	[Contract]
	internal interface IProfileProgressionService
	{
		// Token: 0x06001DB7 RID: 7607
		ProfileProgressionInfo InitProgression(ulong profileId);

		// Token: 0x06001DB8 RID: 7608
		ProfileProgressionInfo GetProgression(ulong profileId);

		// Token: 0x06001DB9 RID: 7609
		ProfileProgressionInfo IncrementMissionPassCounter(ulong profileId, int value, int maxValue, MissionUnlockBranch branch);

		// Token: 0x06001DBA RID: 7610
		ProfileProgressionInfo UnlockMission(ProfileProgressionInfo progression, ProfileProgressionInfo.MissionType unlockedMissionType, bool silent, ILogGroup logGroup);

		// Token: 0x06001DBB RID: 7611
		ProfileProgressionInfo UnlockTutorial(ProfileProgressionInfo progression, int tutorialId, bool silent, ILogGroup logGroup);

		// Token: 0x06001DBC RID: 7612
		ProfileProgressionInfo UnlockTutorial(ProfileProgressionInfo progression, ProfileProgressionInfo.Tutorial tutorialId, bool silent, ILogGroup logGroup);

		// Token: 0x06001DBD RID: 7613
		ProfileProgressionInfo PassTutorial(ulong profileId, int tutorialId, bool silent, ILogGroup logGroup);

		// Token: 0x06001DBE RID: 7614
		ProfileProgressionInfo PassTutorial(ulong profileId, ProfileProgressionInfo.Tutorial tutorialId, bool silent, ILogGroup logGroup);

		// Token: 0x06001DBF RID: 7615
		ProfileProgressionInfo UnlockClass(ProfileProgressionInfo progression, int classId, bool silent, ILogGroup logGroup);

		// Token: 0x06001DC0 RID: 7616
		ProfileProgressionInfo UnlockClass(ProfileProgressionInfo progression, ProfileProgressionInfo.PlayerClass classId, bool silent, ILogGroup logGroup);

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06001DC1 RID: 7617
		bool IsEnabled { get; }
	}
}
