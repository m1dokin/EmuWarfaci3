using System;
using System.Threading.Tasks;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A6 RID: 1446
	internal interface IRewardMultiplierProvider
	{
		// Token: 0x06001F0F RID: 7951
		Task<SRewardMultiplier> GetMultipliers(ulong profileID);
	}
}
