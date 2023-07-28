using System;

namespace MasterServer.DAL
{
	// Token: 0x02000058 RID: 88
	[Serializable]
	public struct MoneyUpdateResult
	{
		// Token: 0x040000EB RID: 235
		public TransactionStatus Status;

		// Token: 0x040000EC RID: 236
		public ulong Money;
	}
}
