using System;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003A0 RID: 928
	internal interface IDailyMissionGenerator
	{
		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x0600148D RID: 5261
		SoftShufflePools SoftShufflePools { get; }

		// Token: 0x0600148E RID: 5262
		int GetMissionCount(string type);

		// Token: 0x0600148F RID: 5263
		bool IsMissionExpired(MissionContext entry, int newGeneration);

		// Token: 0x06001490 RID: 5264
		bool IsSoftShuffleGenerate(DailyGenSettings cfg);

		// Token: 0x06001491 RID: 5265
		bool IsSoftShuffleGenerate(string type);

		// Token: 0x06001492 RID: 5266
		bool MissionSetValid(MissionSet currentSet);

		// Token: 0x06001493 RID: 5267
		bool ValidateMissionContext(MissionContext missionCtx);

		// Token: 0x06001494 RID: 5268
		MissionSet GenerateNewMissionSet(MissionSet currentSet);

		// Token: 0x06001495 RID: 5269
		int GetTotalGenerationPeriod();

		// Token: 0x06001496 RID: 5270
		void DebugValidateMissionGraphs();

		// Token: 0x06001497 RID: 5271
		void DebugEmulateRotation(int elementsNum, int shufflesNum);
	}
}
