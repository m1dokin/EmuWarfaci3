using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000494 RID: 1172
	public enum GameRoomType
	{
		// Token: 0x04000BE9 RID: 3049
		PvE_Private = 1,
		// Token: 0x04000BEA RID: 3050
		PvP_Public,
		// Token: 0x04000BEB RID: 3051
		PvP_ClanWar = 4,
		// Token: 0x04000BEC RID: 3052
		PvP_AutoStart = 8,
		// Token: 0x04000BED RID: 3053
		PvE_AutoStart = 16,
		// Token: 0x04000BEE RID: 3054
		PvP_Rating = 32,
		// Token: 0x04000BEF RID: 3055
		PvE = 17,
		// Token: 0x04000BF0 RID: 3056
		PvP = 46,
		// Token: 0x04000BF1 RID: 3057
		All = 63
	}
}
