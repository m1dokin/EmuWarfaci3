using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002A6 RID: 678
	// (Invoke) Token: 0x06000E81 RID: 3713
	public delegate void RuleSetUpdatedDeleg(IEnumerable<ICustomRule> active, IEnumerable<ICustomRule> disabled);
}
