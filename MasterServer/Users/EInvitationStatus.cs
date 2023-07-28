using System;

namespace MasterServer.Users
{
	// Token: 0x020007FA RID: 2042
	public enum EInvitationStatus
	{
		// Token: 0x0400162C RID: 5676
		Accepted,
		// Token: 0x0400162D RID: 5677
		Rejected,
		// Token: 0x0400162E RID: 5678
		AutoReject,
		// Token: 0x0400162F RID: 5679
		Pending,
		// Token: 0x04001630 RID: 5680
		Duplicate,
		// Token: 0x04001631 RID: 5681
		UserOffline,
		// Token: 0x04001632 RID: 5682
		InvalidState,
		// Token: 0x04001633 RID: 5683
		LimitReached,
		// Token: 0x04001634 RID: 5684
		ServiceError,
		// Token: 0x04001635 RID: 5685
		Expired,
		// Token: 0x04001636 RID: 5686
		DuplicateFollow,
		// Token: 0x04001637 RID: 5687
		TargetInvalid,
		// Token: 0x04001638 RID: 5688
		MissionRestricted,
		// Token: 0x04001639 RID: 5689
		RankRestricted,
		// Token: 0x0400163A RID: 5690
		FullRoom,
		// Token: 0x0400163B RID: 5691
		Banned,
		// Token: 0x0400163C RID: 5692
		BuilTypeMismatch,
		// Token: 0x0400163D RID: 5693
		PrivateRoom,
		// Token: 0x0400163E RID: 5694
		NotInClan,
		// Token: 0x0400163F RID: 5695
		NotParticipateToClanWar,
		// Token: 0x04001640 RID: 5696
		ClassRestricted,
		// Token: 0x04001641 RID: 5697
		BuildVersionMismatch,
		// Token: 0x04001642 RID: 5698
		ItemNotAvailable,
		// Token: 0x04001643 RID: 5699
		NotParticipateInRatingGame
	}
}
