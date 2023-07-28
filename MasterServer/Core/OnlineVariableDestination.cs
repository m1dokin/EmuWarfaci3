using System;

namespace MasterServer.Core
{
	// Token: 0x02000156 RID: 342
	[Flags]
	public enum OnlineVariableDestination
	{
		// Token: 0x040003D3 RID: 979
		Client = 1,
		// Token: 0x040003D4 RID: 980
		Session_PvP = 2,
		// Token: 0x040003D5 RID: 981
		Session_PvE = 4,
		// Token: 0x040003D6 RID: 982
		Session = 6
	}
}
