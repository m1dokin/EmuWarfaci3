using System;
using HK2Net;
using MasterServer.Core.Configuration;
using MasterServer.Database;

namespace MasterServer.GameLogic.MissionSystem.MissionGeneration
{
	// Token: 0x020003A3 RID: 931
	[Service]
	[Singleton]
	internal class DailyMissionGeneratorFactory : IDailyMissionGeneratorFactory
	{
		// Token: 0x060014AF RID: 5295 RVA: 0x00054EE3 File Offset: 0x000532E3
		public DailyMissionGeneratorFactory(IDALService dalService, IMissionSystem missionSystem)
		{
			this.m_dalService = dalService;
			this.m_missionSystem = missionSystem;
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x00054EF9 File Offset: 0x000532F9
		public IDailyMissionGenerator Create(Config config)
		{
			return new DailyMissionGenerator(config, this.m_missionSystem, this.m_dalService);
		}

		// Token: 0x040009BE RID: 2494
		private readonly IDALService m_dalService;

		// Token: 0x040009BF RID: 2495
		private readonly IMissionSystem m_missionSystem;
	}
}
