using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004A4 RID: 1188
	public struct CreateRoomParam
	{
		// Token: 0x04000C1B RID: 3099
		public const int RoundLimitDefault = 11;

		// Token: 0x04000C1C RID: 3100
		public const int PreRoundTimeDefault = 30;

		// Token: 0x04000C1D RID: 3101
		public string Mission;

		// Token: 0x04000C1E RID: 3102
		public bool ManualStart;

		// Token: 0x04000C1F RID: 3103
		public bool AllowJoin;

		// Token: 0x04000C20 RID: 3104
		public bool LockedSpectatorCamera;

		// Token: 0x04000C21 RID: 3105
		public int PreRoundTime;

		// Token: 0x04000C22 RID: 3106
		public int RoundLimit;

		// Token: 0x04000C23 RID: 3107
		public bool OvertimeMode;
	}
}
