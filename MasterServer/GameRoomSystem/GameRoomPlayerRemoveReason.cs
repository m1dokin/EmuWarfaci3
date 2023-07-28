using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000492 RID: 1170
	public enum GameRoomPlayerRemoveReason
	{
		// Token: 0x04000BBC RID: 3004
		Left,
		// Token: 0x04000BBD RID: 3005
		KickMaster,
		// Token: 0x04000BBE RID: 3006
		KickTimeout,
		// Token: 0x04000BBF RID: 3007
		KickVote,
		// Token: 0x04000BC0 RID: 3008
		KickAdmin,
		// Token: 0x04000BC1 RID: 3009
		KickOverflow,
		// Token: 0x04000BC2 RID: 3010
		KickRankRestricted,
		// Token: 0x04000BC3 RID: 3011
		KickClan,
		// Token: 0x04000BC4 RID: 3012
		KickAntiCheat,
		// Token: 0x04000BC5 RID: 3013
		KickVersionMismatch,
		// Token: 0x04000BC6 RID: 3014
		KickItemNotAvalaible,
		// Token: 0x04000BC7 RID: 3015
		KickRatingGameCouldnotStart,
		// Token: 0x04000BC8 RID: 3016
		KickRatingSessionEnded,
		// Token: 0x04000BC9 RID: 3017
		KickHighLatency,
		// Token: 0x04000BCA RID: 3018
		KickLostConnection,
		// Token: 0x04000BCB RID: 3019
		KickEACOther,
		// Token: 0x04000BCC RID: 3020
		KickEACAuthenticationFailed,
		// Token: 0x04000BCD RID: 3021
		KickEACBanned,
		// Token: 0x04000BCE RID: 3022
		KickEACViolation
	}
}
