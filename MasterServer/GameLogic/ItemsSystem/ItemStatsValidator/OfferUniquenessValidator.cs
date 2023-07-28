using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.ElectronicCatalog.Exceptions;

namespace MasterServer.GameLogic.ItemsSystem.ItemStatsValidator
{
	// Token: 0x0200005D RID: 93
	[Service]
	[Singleton]
	internal class OfferUniquenessValidator : IItemStatsValidator
	{
		// Token: 0x0600016C RID: 364 RVA: 0x00009FFC File Offset: 0x000083FC
		public void Validate(IEnumerable<StoreOffer> offers)
		{
			HashSet<StoreOffer> hashSet = new HashSet<StoreOffer>(new StoreOfferComparer());
			foreach (StoreOffer storeOffer in offers)
			{
				if (!hashSet.Add(storeOffer))
				{
					throw new ShopServiceOfferDublicateException(string.Format("Dublicate offer {0}", storeOffer));
				}
			}
		}
	}
}
