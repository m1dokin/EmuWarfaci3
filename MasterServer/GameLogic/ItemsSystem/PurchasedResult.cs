using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200034C RID: 844
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct PurchasedResult
	{
		// Token: 0x060012E7 RID: 4839 RVA: 0x0004C490 File Offset: 0x0004A890
		public PurchasedResult(TransactionStatus status)
		{
			this = new PurchasedResult(status, Enumerable.Empty<StoreOffer>());
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x0004C49E File Offset: 0x0004A89E
		public PurchasedResult(TransactionStatus status, IEnumerable<StoreOffer> offers)
		{
			this = default(PurchasedResult);
			this.Status = status;
			this.Offers = offers;
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060012E9 RID: 4841 RVA: 0x0004C4B5 File Offset: 0x0004A8B5
		// (set) Token: 0x060012EA RID: 4842 RVA: 0x0004C4BD File Offset: 0x0004A8BD
		public TransactionStatus Status { get; private set; }

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060012EB RID: 4843 RVA: 0x0004C4C6 File Offset: 0x0004A8C6
		// (set) Token: 0x060012EC RID: 4844 RVA: 0x0004C4CE File Offset: 0x0004A8CE
		public IEnumerable<StoreOffer> Offers { get; private set; }
	}
}
