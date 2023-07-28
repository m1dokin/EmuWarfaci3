using System;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000574 RID: 1396
	public enum ENotificationType : uint
	{
		// Token: 0x04000E8C RID: 3724
		Unknown = 1U,
		// Token: 0x04000E8D RID: 3725
		MissionPerformance,
		// Token: 0x04000E8E RID: 3726
		Achievement = 4U,
		// Token: 0x04000E8F RID: 3727
		Message = 8U,
		// Token: 0x04000E90 RID: 3728
		ClanInvite = 16U,
		// Token: 0x04000E91 RID: 3729
		ClanInviteResult = 32U,
		// Token: 0x04000E92 RID: 3730
		FriendInvite = 64U,
		// Token: 0x04000E93 RID: 3731
		FriendInviteResult = 128U,
		// Token: 0x04000E94 RID: 3732
		ItemGiven = 256U,
		// Token: 0x04000E95 RID: 3733
		Announcement = 512U,
		// Token: 0x04000E96 RID: 3734
		Contract = 1024U,
		// Token: 0x04000E97 RID: 3735
		MoneyGiven = 2048U,
		// Token: 0x04000E98 RID: 3736
		ItemUnequipped = 4096U,
		// Token: 0x04000E99 RID: 3737
		RandomBoxGiven = 8192U,
		// Token: 0x04000E9A RID: 3738
		ItemUnlocked = 32768U,
		// Token: 0x04000E9B RID: 3739
		AutoRepairEquipment = 65536U,
		// Token: 0x04000E9C RID: 3740
		NewRankReached = 131072U,
		// Token: 0x04000E9D RID: 3741
		CongratulationMessage = 262144U,
		// Token: 0x04000E9E RID: 3742
		MissionUnlockMessage = 1048576U,
		// Token: 0x04000E9F RID: 3743
		ItemDeleted = 2097152U,
		// Token: 0x04000EA0 RID: 3744
		RatingGameBan = 4194304U,
		// Token: 0x04000EA1 RID: 3745
		All = 4294967295U
	}
}
