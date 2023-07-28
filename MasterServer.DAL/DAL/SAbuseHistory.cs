using System;

namespace MasterServer.DAL
{
	// Token: 0x02000005 RID: 5
	[Serializable]
	public struct SAbuseHistory
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002150 File Offset: 0x00000550
		public override string ToString()
		{
			return string.Format("From: {0}, To: {1}, Type: {2}, ReportSource: {3}, ReportDate: {4}, Message: {5}", new object[]
			{
				this.From,
				this.To,
				this.Type,
				this.ReportSource,
				this.ReportDate,
				this.Message
			});
		}

		// Token: 0x04000006 RID: 6
		public ulong From;

		// Token: 0x04000007 RID: 7
		public ulong To;

		// Token: 0x04000008 RID: 8
		public string Type;

		// Token: 0x04000009 RID: 9
		public uint ReportSource;

		// Token: 0x0400000A RID: 10
		public DateTime ReportDate;

		// Token: 0x0400000B RID: 11
		public string Message;
	}
}
