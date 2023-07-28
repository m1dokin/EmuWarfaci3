using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000203 RID: 515
	internal interface IMissionSystemClient
	{
		// Token: 0x06000AD1 RID: 2769
		void RemoveMission(Guid uid);

		// Token: 0x06000AD2 RID: 2770
		SMission GetMission(Guid uid);

		// Token: 0x06000AD3 RID: 2771
		IEnumerable<SMission> GetMissions(int period);

		// Token: 0x06000AD4 RID: 2772
		IEnumerable<Guid> GetCurrentMissions();

		// Token: 0x06000AD5 RID: 2773
		int GetGeneration();

		// Token: 0x06000AD6 RID: 2774
		void SaveGeneration(int generation);

		// Token: 0x06000AD7 RID: 2775
		void SaveMission(Guid uid, string name, string gameMode, string data, int generation);

		// Token: 0x06000AD8 RID: 2776
		void AddCurrentMission(Guid uid);

		// Token: 0x06000AD9 RID: 2777
		void RemoveCurrentMission(Guid uid);

		// Token: 0x06000ADA RID: 2778
		IEnumerable<SoftShufflePoolData> GetSoftShufflePools();

		// Token: 0x06000ADB RID: 2779
		void SaveSoftShufflePool(SoftShufflePoolData pool);
	}
}
