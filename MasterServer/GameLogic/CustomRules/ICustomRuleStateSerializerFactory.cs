using System;
using HK2Net;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002B3 RID: 691
	[Contract]
	public interface ICustomRuleStateSerializerFactory
	{
		// Token: 0x06000ED3 RID: 3795
		CustomRuleStateInfo GetInfo(Type stateType);

		// Token: 0x06000ED4 RID: 3796
		ICustomRuleStateSerializer GetSerializer(Type stateType);
	}
}
