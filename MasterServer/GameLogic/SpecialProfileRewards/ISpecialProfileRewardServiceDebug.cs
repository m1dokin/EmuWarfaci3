using System;
using HK2Net;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020005C6 RID: 1478
	[Contract]
	internal interface ISpecialProfileRewardServiceDebug : ISpecialProfileRewardService
	{
		// Token: 0x06001FA4 RID: 8100
		void DumpRewardSets();
	}
}
