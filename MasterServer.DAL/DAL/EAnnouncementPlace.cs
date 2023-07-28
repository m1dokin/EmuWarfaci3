using System;

namespace MasterServer.DAL
{
	// Token: 0x0200000F RID: 15
	[Flags]
	public enum EAnnouncementPlace
	{
		// Token: 0x0400001B RID: 27
		None = 1,
		// Token: 0x0400001C RID: 28
		Shop = 2,
		// Token: 0x0400001D RID: 29
		Storage = 4,
		// Token: 0x0400001E RID: 30
		GameRoom = 8,
		// Token: 0x0400001F RID: 31
		InGame = 16
	}
}
