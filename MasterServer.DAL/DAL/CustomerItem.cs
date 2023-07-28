using System;

namespace MasterServer.DAL
{
	// Token: 0x02000055 RID: 85
	[Serializable]
	public class CustomerItem
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x00003D08 File Offset: 0x00002108
		public override string ToString()
		{
			return string.Format("<CustomerItem> {0}: item={1}[{2}], exp={3}, totdur={4}, dur={5}, quantity={6} buy time={7} offer_type={8}", new object[]
			{
				this.InstanceID,
				this.CatalogItem.Name,
				this.CatalogItem.ID,
				this.ExpirationTimeUTC,
				this.TotalDurabilityPoints,
				this.DurabilityPoints,
				this.Quantity,
				this.BuyTimeUTC,
				this.OfferType
			});
		}

		// Token: 0x040000D1 RID: 209
		public ulong InstanceID;

		// Token: 0x040000D2 RID: 210
		public CatalogItem CatalogItem;

		// Token: 0x040000D3 RID: 211
		public OfferType OfferType;

		// Token: 0x040000D4 RID: 212
		public int TotalDurabilityPoints;

		// Token: 0x040000D5 RID: 213
		public int DurabilityPoints;

		// Token: 0x040000D6 RID: 214
		public ulong Quantity;

		// Token: 0x040000D7 RID: 215
		public ulong BuyTimeUTC;

		// Token: 0x040000D8 RID: 216
		public ulong ExpirationTimeUTC;

		// Token: 0x040000D9 RID: 217
		public ulong OfferId;

		// Token: 0x040000DA RID: 218
		public ulong AddedQuantity;

		// Token: 0x040000DB RID: 219
		public TransactionStatus Status;
	}
}
