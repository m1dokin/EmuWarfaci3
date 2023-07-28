using System;

namespace MasterServer.DAL
{
	// Token: 0x0200003E RID: 62
	[Serializable]
	public struct SServerEntity
	{
		// Token: 0x04000098 RID: 152
		public string ServerId;

		// Token: 0x04000099 RID: 153
		public string Hostname;

		// Token: 0x0400009A RID: 154
		public int Port;

		// Token: 0x0400009B RID: 155
		public string Node;

		// Token: 0x0400009C RID: 156
		public string OnlineId;

		// Token: 0x0400009D RID: 157
		public int Status;

		// Token: 0x0400009E RID: 158
		public string MissionKey;

		// Token: 0x0400009F RID: 159
		public string Mode;

		// Token: 0x040000A0 RID: 160
		public float PerformanceIndex;

		// Token: 0x040000A1 RID: 161
		public string BuildType;

		// Token: 0x040000A2 RID: 162
		public string MasterServerId;
	}
}
