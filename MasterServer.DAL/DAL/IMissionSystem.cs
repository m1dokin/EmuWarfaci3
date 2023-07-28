using System;

namespace MasterServer.DAL
{
	// Token: 0x02000042 RID: 66
	public interface IMissionSystem
	{
		// Token: 0x0600009D RID: 157
		DALResultVoid RemoveMission(Guid uid);

		// Token: 0x0600009E RID: 158
		DALResult<SMission> GetMission(Guid id);

		// Token: 0x0600009F RID: 159
		DALResultMulti<SMission> GetMissions(int period);

		// Token: 0x060000A0 RID: 160
		DALResultMulti<Guid> GetCurrentMissions();

		// Token: 0x060000A1 RID: 161
		DALResult<int> GetGeneration();

		// Token: 0x060000A2 RID: 162
		DALResultVoid SaveGeneration(int generation);

		// Token: 0x060000A3 RID: 163
		DALResultVoid SaveMission(Guid uid, string name, string gameMode, string data, int generation);

		// Token: 0x060000A4 RID: 164
		DALResultVoid AddCurrentMission(Guid uid);

		// Token: 0x060000A5 RID: 165
		DALResultVoid RemoveCurrentMission(Guid uid);

		// Token: 0x060000A6 RID: 166
		DALResultMulti<SoftShufflePoolData> GetSoftShufflePools();

		// Token: 0x060000A7 RID: 167
		DALResultVoid SaveSoftShufflePool(SoftShufflePoolData pool);
	}
}
