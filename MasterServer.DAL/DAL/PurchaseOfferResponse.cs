using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x0200005A RID: 90
	[Serializable]
	public struct PurchaseOfferResponse
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x00003DD6 File Offset: 0x000021D6
		public static bool operator ==(PurchaseOfferResponse var1, PurchaseOfferResponse var2)
		{
			return var1.Equals(var2);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00003DEB File Offset: 0x000021EB
		public static bool operator !=(PurchaseOfferResponse var1, PurchaseOfferResponse var2)
		{
			return !var1.Equals(var2);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00003E03 File Offset: 0x00002203
		public override string ToString()
		{
			return string.Format("<PurchaseResponse> {0}", this.Status);
		}

		// Token: 0x040000EF RID: 239
		public ulong OfferId;

		// Token: 0x040000F0 RID: 240
		public TransactionStatus Status;

		// Token: 0x040000F1 RID: 241
		public List<KeyValuePair<TransactionStatus, ulong>> Items;
	}
}
