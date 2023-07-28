using System;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200034B RID: 843
	public struct SPurchasedItem
	{
		// Token: 0x040008BA RID: 2234
		public SItem Item;

		// Token: 0x040008BB RID: 2235
		public ulong ProfileItemID;

		// Token: 0x040008BC RID: 2236
		public TransactionStatus Status;

		// Token: 0x040008BD RID: 2237
		public string AddedExpiration;

		// Token: 0x040008BE RID: 2238
		public ulong AddedQuantity;
	}
}
