using System;

namespace MasterServer.DAL
{
	// Token: 0x0200005C RID: 92
	[Serializable]
	public struct ConsumeItemResponse
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x00003E31 File Offset: 0x00002231
		public override string ToString()
		{
			return string.Format("<ConsumeItemResponse> {0} {1}", this.ItemsLeft, this.Status);
		}

		// Token: 0x040000F4 RID: 244
		public ushort ItemsLeft;

		// Token: 0x040000F5 RID: 245
		public TransactionStatus Status;
	}
}
