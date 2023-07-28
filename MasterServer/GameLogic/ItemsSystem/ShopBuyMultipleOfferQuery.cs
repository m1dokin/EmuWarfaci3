using System;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200033B RID: 827
	[QueryAttributes(TagName = "shop_buy_multiple_offer", QoSClass = "batch_shop")]
	internal class ShopBuyMultipleOfferQuery : ShopBuyMultipleOfferBaseQuery
	{
		// Token: 0x0600129C RID: 4764 RVA: 0x0004AA60 File Offset: 0x00048E60
		public ShopBuyMultipleOfferQuery(IItemsPurchase itemsPurchase, IShopBuyMultipleOfferValidation buyMultipleOfferValidation) : base(itemsPurchase, buyMultipleOfferValidation)
		{
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x0600129D RID: 4765 RVA: 0x0004AA6A File Offset: 0x00048E6A
		protected override string QueryName
		{
			get
			{
				return "ShopBuyMultipleOfferQuery";
			}
		}
	}
}
