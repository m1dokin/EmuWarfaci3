using System;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200055C RID: 1372
	internal interface IProfileProgressionRule : IDisposable
	{
		// Token: 0x06001DB3 RID: 7603
		void Init(ConfigSection section);

		// Token: 0x06001DB4 RID: 7604
		ProfileProgressionInfo TrigerRule(ulong profileId, ProfileProgressionInfo info, ILogGroup logGroup);

		// Token: 0x06001DB5 RID: 7605
		ProfileProgressionInfo ProcessRewardData(MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup);
	}
}
