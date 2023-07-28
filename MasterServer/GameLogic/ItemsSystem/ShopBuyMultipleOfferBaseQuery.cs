using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.GameLogic.RandomBoxValidationSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200033A RID: 826
	internal abstract class ShopBuyMultipleOfferBaseQuery : BaseQuery
	{
		// Token: 0x06001296 RID: 4758 RVA: 0x0004A744 File Offset: 0x00048B44
		protected ShopBuyMultipleOfferBaseQuery(IItemsPurchase itemsPurchase, IShopBuyMultipleOfferValidation buyMultipleOfferValidation)
		{
			this.m_itemsPurchase = itemsPurchase;
			this.m_buyMultipleOfferValidation = buyMultipleOfferValidation;
			ConfigSection section = Resources.ModuleSettings.GetSection("Shop");
			this.m_maxBatchSize = int.Parse(section.Get("MaxBatchSize"));
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06001297 RID: 4759
		protected abstract string QueryName { get; }

		// Token: 0x06001298 RID: 4760 RVA: 0x0004A7A0 File Offset: 0x00048BA0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, this.QueryName))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					return -3;
				}
				int supplierId = int.Parse(request.GetAttribute("supplier_id"));
				long offerHash = long.Parse(request.GetAttribute("hash"));
				List<ulong> list = new List<ulong>();
				IEnumerator enumerator = request.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlElement xmlElement = (XmlElement)obj;
						if (xmlElement.Name == "offer")
						{
							list.Add(ulong.Parse(xmlElement.GetAttribute("id")));
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				TransactionStatus transactionStatus = this.ProcessQueryImpl(list, offerHash, user, supplierId, response);
				string name = "error_status";
				int num = (int)transactionStatus;
				response.SetAttribute(name, num.ToString(CultureInfo.InvariantCulture));
			}
			return 0;
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x0004A8CC File Offset: 0x00048CCC
		private TransactionStatus ProcessQueryImpl(IList<ulong> offersId, long offerHash, UserInfo.User user, int supplierId, XmlElement response)
		{
			PurchasedResult purchasedResult = new PurchasedResult(TransactionStatus.INTERNAL_ERROR);
			if (offersId.Count >= 1 && offersId.Count <= this.m_maxBatchSize)
			{
				try
				{
					offersId = this.m_buyMultipleOfferValidation.Validate(user, supplierId, offersId).ToList<ulong>();
					purchasedResult = this.m_itemsPurchase.PurchaseOffers(user, supplierId, offerHash, offersId, new PurchaseHandler(user.ProfileID, response));
					if (purchasedResult.Status == TransactionStatus.OK || purchasedResult.Status == TransactionStatus.NOT_ENOUGH_MONEY || purchasedResult.Offers.Any<StoreOffer>())
					{
						ProfileProxy profile = new ProfileProxy(user);
						ProfileReader profileReader = new ProfileReader(profile);
						XmlElement xmlElement = response.OwnerDocument.CreateElement("money");
						response.AppendChild(xmlElement);
						profileReader.ReadProfileMoney(xmlElement);
					}
				}
				catch (OfferNotFoundException e)
				{
					purchasedResult = new PurchasedResult(TransactionStatus.INTERNAL_ERROR);
					Log.Warning(e);
				}
				catch (RandomBoxValidationException e2)
				{
					purchasedResult = new PurchasedResult(TransactionStatus.INVALID_REQUEST);
					Log.Warning(e2);
				}
				finally
				{
					this.m_buyMultipleOfferValidation.Confirm(user, supplierId, from o in purchasedResult.Offers
					select o.StoreID);
				}
			}
			else
			{
				purchasedResult = new PurchasedResult(TransactionStatus.INVALID_REQUEST);
			}
			return purchasedResult.Status;
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x0004AA34 File Offset: 0x00048E34
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (args.Name.Equals("MaxBatchSize", StringComparison.InvariantCultureIgnoreCase))
			{
				this.m_maxBatchSize = args.iValue;
			}
		}

		// Token: 0x04000890 RID: 2192
		private readonly IItemsPurchase m_itemsPurchase;

		// Token: 0x04000891 RID: 2193
		private readonly IShopBuyMultipleOfferValidation m_buyMultipleOfferValidation;

		// Token: 0x04000892 RID: 2194
		private int m_maxBatchSize;
	}
}
