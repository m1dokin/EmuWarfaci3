using System;

namespace MasterServer.DAL
{
	// Token: 0x0200000D RID: 13
	[Serializable]
	public struct LeaderboardEntry
	{
		// Token: 0x04000015 RID: 21
		public int Rank;

		// Token: 0x04000016 RID: 22
		public ulong ProfileID;

		// Token: 0x04000017 RID: 23
		public string Nickname;

		// Token: 0x04000018 RID: 24
		public int Class;

		// Token: 0x04000019 RID: 25
		public ulong Experience;
	}
}
