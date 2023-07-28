using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000229 RID: 553
	[ConsoleCmdAttributes(CmdName = "shop_get_offers", ArgsSize = 1)]
	internal class ShopGetStoreCmd : IConsoleCmd
	{
		// Token: 0x06000BD9 RID: 3033 RVA: 0x0002D230 File Offset: 0x0002B630
		public ShopGetStoreCmd(IShopService shopService)
		{
			this.m_shopService = shopService;
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x0002D240 File Offset: 0x0002B640
		public void ExecuteCmd(string[] args)
		{
			IEnumerable<StoreOffer> offers = this.m_shopService.GetOffers();
			int num = -1;
			if (args.Length > 1)
			{
				num = int.Parse(args[1]);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Hash: {0}", this.m_shopService.OffersHash));
			foreach (StoreOffer storeOffer in offers)
			{
				if (num == -1 || storeOffer.SupplierID == num)
				{
					stringBuilder.AppendFormat("{0}\n", storeOffer);
				}
			}
			Log.Info<string>("{0}", stringBuilder.ToString());
		}

		// Token: 0x04000585 RID: 1413
		private readonly IShopService m_shopService;
	}
}
