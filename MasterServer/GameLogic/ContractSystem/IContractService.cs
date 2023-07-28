using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x02000294 RID: 660
	[Contract]
	public interface IContractService
	{
		// Token: 0x06000E42 RID: 3650
		ProfileContract GetProfileContract(ulong profileId);

		// Token: 0x06000E43 RID: 3651
		ProfileContract RotateContract(ulong profileId);

		// Token: 0x06000E44 RID: 3652
		ProfileContract SetContractProgress(ProfileContract contract, SProfileItem contractItem, uint progress, SRewardMultiplier dynamicMultiplier);

		// Token: 0x06000E45 RID: 3653
		ProfileContract ActivateContract(ulong profileId, ulong itemId, string itemName);
	}
}
