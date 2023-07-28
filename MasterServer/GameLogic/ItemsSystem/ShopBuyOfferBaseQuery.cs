using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200033C RID: 828
	internal abstract class ShopBuyOfferBaseQuery : BaseQuery
	{
		// Token: 0x0600129E RID: 4766 RVA: 0x0004AA71 File Offset: 0x00048E71
		protected ShopBuyOfferBaseQuery(IItemsPurchase itemsPurchase, IShopBuyMultipleOfferValidation buyMultipleOfferValidation)
		{
			this.m_itemsPurchase = itemsPurchase;
			this.m_buyMultipleOfferValidation = buyMultipleOfferValidation;
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x0600129F RID: 4767
		protected abstract string QueryName { get; }

		// Token: 0x060012A0 RID: 4768 RVA: 0x0004AA88 File Offset: 0x00048E88
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, this.QueryName))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					int supplierId = int.Parse(request.GetAttribute("supplier_id"));
					ulong num = ulong.Parse(request.GetAttribute("offer_id"));
					long offerHash = long.Parse(request.GetAttribute("hash"));
					PurchasedResult purchasedResult = new PurchasedResult(TransactionStatus.INTERNAL_ERROR);
					try
					{
						this.m_buyMultipleOfferValidation.Validate(user, supplierId, new List<ulong>
						{
							num
						});
						purchasedResult = this.m_itemsPurchase.PurchaseOffer(user, supplierId, offerHash, num, new PurchaseHandler(user.ProfileID, response));
						if (purchasedResult.Status == TransactionStatus.OK || purchasedResult.Status == TransactionStatus.NOT_ENOUGH_MONEY)
						{
							ProfileProxy profile = new ProfileProxy(user);
							ProfileReader profileReader = new ProfileReader(profile);
							XmlElement xmlElement = response.OwnerDocument.CreateElement("money");
							response.AppendChild(xmlElement);
							profileReader.ReadProfileMoney(xmlElement);
						}
					}
					catch (OfferNotFoundException ex)
					{
						purchasedResult = new PurchasedResult(TransactionStatus.HASH_MISMATCH);
						Log.Warning(ex.Message);
					}
					finally
					{
						this.m_buyMultipleOfferValidation.Confirm(user, supplierId, from o in purchasedResult.Offers
						select o.StoreID);
					}
					response.SetAttribute("offer_id", num.ToString(CultureInfo.InvariantCulture));
					response.SetAttribute("error_status", ((int)purchasedResult.Status).ToString(CultureInfo.InvariantCulture));
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x04000894 RID: 2196
		private readonly IItemsPurchase m_itemsPurchase;

		// Token: 0x04000895 RID: 2197
		private readonly IShopBuyMultipleOfferValidation m_buyMultipleOfferValidation;
	}
}
