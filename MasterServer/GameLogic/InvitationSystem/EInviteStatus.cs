using System;

namespace MasterServer.GameLogic.InvitationSystem
{
	// Token: 0x0200030F RID: 783
	public enum EInviteStatus
	{
		// Token: 0x04000810 RID: 2064
		Accepted,
		// Token: 0x04000811 RID: 2065
		Rejected,
		// Token: 0x04000812 RID: 2066
		InviteInProgress,
		// Token: 0x04000813 RID: 2067
		Pending,
		// Token: 0x04000814 RID: 2068
		Duplicate,
		// Token: 0x04000815 RID: 2069
		AlreadyClanMember,
		// Token: 0x04000816 RID: 2070
		NoPermission,
		// Token: 0x04000817 RID: 2071
		KickTimeout,
		// Token: 0x04000818 RID: 2072
		UserOffline,
		// Token: 0x04000819 RID: 2073
		TargetInvalid,
		// Token: 0x0400081A RID: 2074
		InvalidState,
		// Token: 0x0400081B RID: 2075
		LimitReached,
		// Token: 0x0400081C RID: 2076
		TargetLimitReached,
		// Token: 0x0400081D RID: 2077
		ServiceError,
		// Token: 0x0400081E RID: 2078
		Expired,
		// Token: 0x0400081F RID: 2079
		DoNotDisturb
	}
}
