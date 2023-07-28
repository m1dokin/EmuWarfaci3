using System;
using HK2Net;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A9 RID: 681
	[Contract]
	public interface IDebugCustomRulesService
	{
		// Token: 0x06000E90 RID: 3728
		ICustomRule AddStaticRule(string config, bool enabled);

		// Token: 0x06000E91 RID: 3729
		void RemoveStaticRule(ulong ruleId);
	}
}
