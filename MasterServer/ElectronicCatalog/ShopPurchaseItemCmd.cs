using System;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog.ShopSupplier;
using MasterServer.Users;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000052 RID: 82
	[ConsoleCmdAttributes(CmdName = "shop_purchase_offer", Help = "Purchase specified offer for user")]
	internal class ShopPurchaseItemCmd : ConsoleCommand<ShopPurchaseItemCmdParams>
	{
		// Token: 0x0600013D RID: 317 RVA: 0x00009A94 File Offset: 0x00007E94
		public ShopPurchaseItemCmd(ICatalogService catalogService, IUserRepository userRepository, IShopService shopService)
		{
			this.m_catalogService = catalogService;
			this.m_userRepository = userRepository;
			this.m_shopService = shopService;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00009AB4 File Offset: 0x00007EB4
		protected override void Execute(ShopPurchaseItemCmdParams param)
		{
			UserInfo.User user = this.m_userRepository.MakeFake(param.UserId, 0UL, string.Empty, 0, 0UL, "global");
			StoreOffer storeOfferById = this.m_catalogService.GetStoreOfferById(param.SupplierId, param.OfferId);
			PurchaseResult purchaseResult = this.m_catalogService.PurchaseOffers(user, this.m_shopService.OffersHash, new StoreOffer[]
			{
				storeOfferById
			});
			Log.Info<string>("{0}", purchaseResult.Status.ToString());
		}

		// Token: 0x04000096 RID: 150
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000097 RID: 151
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000098 RID: 152
		private readonly IShopService m_shopService;
	}
}
