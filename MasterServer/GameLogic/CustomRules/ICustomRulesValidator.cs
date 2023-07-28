using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A8 RID: 680
	[Contract]
	public interface ICustomRulesValidator
	{
		// Token: 0x06000E8F RID: 3727
		void Validate(IEnumerable<ICustomRule> rules);
	}
}
