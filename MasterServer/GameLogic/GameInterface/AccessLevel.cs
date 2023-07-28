using System;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002DB RID: 731
	[Flags]
	public enum AccessLevel
	{
		// Token: 0x04000751 RID: 1873
		Basic = 1,
		// Token: 0x04000752 RID: 1874
		Moderator = 2,
		// Token: 0x04000753 RID: 1875
		Admin = 8,
		// Token: 0x04000754 RID: 1876
		Debug = 65535
	}
}
