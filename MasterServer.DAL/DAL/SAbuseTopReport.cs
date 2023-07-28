using System;

namespace MasterServer.DAL
{
	// Token: 0x02000002 RID: 2
	[Serializable]
	public struct SAbuseTopReport
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000450
		public override string ToString()
		{
			return string.Format("ProfileId {0}, Nickname {1}, Rank {2}, TotalUserReports {3}, TotalAutoReports {4}", new object[]
			{
				this.ProfileId,
				this.Nickname,
				this.RankId,
				this.TotalUserReports,
				this.TotalAutoReports
			});
		}

		// Token: 0x04000001 RID: 1
		public ulong ProfileId;

		// Token: 0x04000002 RID: 2
		public string Nickname;

		// Token: 0x04000003 RID: 3
		public int RankId;

		// Token: 0x04000004 RID: 4
		public ulong TotalUserReports;

		// Token: 0x04000005 RID: 5
		public ulong TotalAutoReports;
	}
}
