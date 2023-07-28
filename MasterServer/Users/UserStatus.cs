using System;

namespace MasterServer.Users
{
	// Token: 0x02000806 RID: 2054
	[Flags]
	public enum UserStatus
	{
		// Token: 0x0400166B RID: 5739
		Offline = 0,
		// Token: 0x0400166C RID: 5740
		Online = 1,
		// Token: 0x0400166D RID: 5741
		Logout = 2,
		// Token: 0x0400166E RID: 5742
		Away = 4,
		// Token: 0x0400166F RID: 5743
		InLobby = 8,
		// Token: 0x04001670 RID: 5744
		InGameRoom = 16,
		// Token: 0x04001671 RID: 5745
		InGame = 32,
		// Token: 0x04001672 RID: 5746
		InShop = 64,
		// Token: 0x04001673 RID: 5747
		InCustomize = 128,
		// Token: 0x04001674 RID: 5748
		InRatingGame = 256,
		// Token: 0x04001675 RID: 5749
		InTutorialGame = 512,
		// Token: 0x04001676 RID: 5750
		BannedInRatingGame = 1024
	}
}
