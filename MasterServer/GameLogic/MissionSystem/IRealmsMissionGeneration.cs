using System;
using HK2Net;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003A9 RID: 937
	[Contract]
	internal interface IRealmsMissionGeneration
	{
		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060014B8 RID: 5304
		bool Enabled { get; }

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060014B9 RID: 5305
		bool GenerationRole { get; }

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x060014BA RID: 5306
		bool RealmSyncRole { get; }

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x060014BB RID: 5307
		bool SlaveRole { get; }

		// Token: 0x060014BC RID: 5308
		RealmMissionLoadResult LoadRealmMissions(bool forceSync);

		// Token: 0x060014BD RID: 5309
		bool SaveRealmMissions(MissionSet realm_missions);

		// Token: 0x060014BE RID: 5310
		void SetRealmGeneration(Config cfg);
	}
}
