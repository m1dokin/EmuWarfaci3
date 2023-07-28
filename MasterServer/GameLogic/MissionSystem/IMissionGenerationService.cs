using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003B1 RID: 945
	[Contract]
	internal interface IMissionGenerationService
	{
		// Token: 0x14000043 RID: 67
		// (add) Token: 0x060014EF RID: 5359
		// (remove) Token: 0x060014F0 RID: 5360
		event Action MissionSetUpdated;

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x060014F1 RID: 5361
		// (remove) Token: 0x060014F2 RID: 5362
		event Action<Guid, MissionContext> MissionExpired;

		// Token: 0x060014F3 RID: 5363
		Dictionary<Guid, MissionContext> GetMissions();

		// Token: 0x060014F4 RID: 5364
		MissionHash GetMissionsHash();

		// Token: 0x060014F5 RID: 5365
		void ReloadMissionSetFromDB(bool force);

		// Token: 0x060014F6 RID: 5366
		void ReloadMissionSetFromRealm(bool force);

		// Token: 0x060014F7 RID: 5367
		void RegenerateMissionSet();
	}
}
