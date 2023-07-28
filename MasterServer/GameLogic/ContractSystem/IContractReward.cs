using System;
using MasterServer.Core;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x02000292 RID: 658
	internal interface IContractReward
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000E3A RID: 3642
		string Name { get; }

		// Token: 0x06000E3B RID: 3643
		uint GiveReward(ulong userId, ulong profileId, SRewardMultiplier multiplier, ILogGroup logGroup);
	}
}
