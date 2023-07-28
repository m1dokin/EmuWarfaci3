using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020005C8 RID: 1480
	[Contract]
	internal interface ISpecialRewardActionFactory
	{
		// Token: 0x06001FA8 RID: 8104
		Dictionary<string, Type> GetActionTypes();

		// Token: 0x06001FA9 RID: 8105
		ISpecialRewardAction CreateAction(string name, ConfigSection config, bool enableNotifs);
	}
}
