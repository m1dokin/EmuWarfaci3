using System;
using MasterServer.DAL.CustomRules;
using MasterServer.GameLogic.CustomRules.Rules;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002B1 RID: 689
	public interface ICustomRuleStateSerializer
	{
		// Token: 0x06000ECE RID: 3790
		CustomRuleRawState Serialize(object state);

		// Token: 0x06000ECF RID: 3791
		CustomRuleState Deserialize(CustomRuleRawState rawState);
	}
}
