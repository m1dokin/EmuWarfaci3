using System;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000718 RID: 1816
	public class ShopSupplierRequest
	{
		// Token: 0x060025D6 RID: 9686 RVA: 0x0009EE54 File Offset: 0x0009D254
		protected ShopSupplierRequest(string name)
		{
			this.Name = name;
		}

		// Token: 0x04001346 RID: 4934
		public readonly string Name;

		// Token: 0x04001347 RID: 4935
		public static readonly ShopSupplierRequest GetOffers = new ShopSupplierRequest("get_offers");

		// Token: 0x04001348 RID: 4936
		public static readonly ShopSupplierRequest PurchaseOffer = new ShopSupplierRequest("purchase_offer");

		// Token: 0x04001349 RID: 4937
		public static readonly ShopSupplierRequest OfferHash = new ShopSupplierRequest("offer_hash");
	}
}
