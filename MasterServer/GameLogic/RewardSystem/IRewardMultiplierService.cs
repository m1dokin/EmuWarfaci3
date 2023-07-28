using System;
using System.Threading.Tasks;
using HK2Net;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A7 RID: 1447
	[Contract]
	internal interface IRewardMultiplierService
	{
		// Token: 0x06001F10 RID: 7952
		Task<SRewardMultiplier> GetResultMultiplier(ulong profileID);

		// Token: 0x06001F11 RID: 7953
		void RegisterRewardMultiplierProvider(IRewardMultiplierProvider provider);

		// Token: 0x06001F12 RID: 7954
		void UnregisterRewardMultiplierProvider(IRewardMultiplierProvider provider);
	}
}
