using System;
using HK2Net;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.MissionSystem.MissionGeneration
{
	// Token: 0x020003A2 RID: 930
	[Contract]
	internal interface IDailyMissionGeneratorFactory
	{
		// Token: 0x060014AE RID: 5294
		IDailyMissionGenerator Create(Config config);
	}
}
