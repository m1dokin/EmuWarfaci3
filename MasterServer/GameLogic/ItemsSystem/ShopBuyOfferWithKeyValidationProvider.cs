using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000383 RID: 899
	[Service]
	[Singleton]
	internal class ShopBuyOfferWithKeyValidationProvider : IMultipleOfferValidationProvider, IDisposable
	{
		// Token: 0x06001448 RID: 5192 RVA: 0x0005289E File Offset: 0x00050C9E
		public ShopBuyOfferWithKeyValidationProvider(ICatalogService catalogService)
		{
			this.m_catalogService = catalogService;
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x000528AD File Offset: 0x00050CAD
		public void Initialize()
		{
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x000528AF File Offset: 0x00050CAF
		public void Dispose()
		{
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x000528B4 File Offset: 0x00050CB4
		public IEnumerable<ulong> Validate(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds)
		{
			if (supplierId != 1)
			{
				return offerIds;
			}
			HashSet<ulong> hashSet = new HashSet<ulong>();
			foreach (ulong num in offerIds)
			{
				StoreOffer storeOfferById = this.m_catalogService.GetStoreOfferById(supplierId, num);
				if (storeOfferById == null)
				{
					Log.Warning<ulong>("Can't find offer with id '{0}' to validate.", num);
				}
				else if (storeOfferById.IsKeyPriceOffer())
				{
					if (hashSet.Contains(num))
					{
						throw new ShopBuyOfferWithKeyValidationException(string.Format("User with profile id '{0}' tries to buy reserved offer '{1}' more than one time", user.ProfileID, num));
					}
					hashSet.Add(num);
				}
			}
			return offerIds;
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x00052978 File Offset: 0x00050D78
		public void Confirm(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds)
		{
			if (supplierId != 1)
			{
				return;
			}
			foreach (ulong num in offerIds)
			{
				if (this.m_catalogService.GetStoreOfferById(supplierId, num) == null)
				{
					Log.Warning(string.Format("Can't find offer with id '{0}' to confirm.", num));
				}
			}
		}

		// Token: 0x04000963 RID: 2403
		private readonly ICatalogService m_catalogService;
	}
}
