using System;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000381 RID: 897
	[QueryAttributes(TagName = "shop_buy_offer", QoSClass = "shop")]
	internal class ShopBuyOfferQuery : ShopBuyOfferBaseQuery
	{
		// Token: 0x06001445 RID: 5189 RVA: 0x00052884 File Offset: 0x00050C84
		public ShopBuyOfferQuery(IItemsPurchase itemsPurchase, IShopBuyMultipleOfferValidation buyMultipleOfferValidation) : base(itemsPurchase, buyMultipleOfferValidation)
		{
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06001446 RID: 5190 RVA: 0x0005288E File Offset: 0x00050C8E
		protected override string QueryName
		{
			get
			{
				return "ShopBuyOfferQuery";
			}
		}
	}
}
