using System;

namespace MasterServer.DAL
{
	// Token: 0x02000046 RID: 70
	[Serializable]
	public struct SPendingNotification
	{
		// Token: 0x040000A8 RID: 168
		public ulong ID;

		// Token: 0x040000A9 RID: 169
		public ulong ProfileId;

		// Token: 0x040000AA RID: 170
		public uint Type;

		// Token: 0x040000AB RID: 171
		public uint ConfirmationType;

		// Token: 0x040000AC RID: 172
		public DateTime ExpirationTimeUTC;

		// Token: 0x040000AD RID: 173
		public byte[] Data;

		// Token: 0x040000AE RID: 174
		public string Message;
	}
}
