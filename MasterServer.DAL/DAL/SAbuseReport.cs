using System;

namespace MasterServer.DAL
{
	// Token: 0x0200001C RID: 28
	[Serializable]
	public struct SAbuseReport
	{
		// Token: 0x04000045 RID: 69
		public ulong From;

		// Token: 0x04000046 RID: 70
		public ulong To;

		// Token: 0x04000047 RID: 71
		public string Type;

		// Token: 0x04000048 RID: 72
		public DateTime Timestamp;
	}
}
