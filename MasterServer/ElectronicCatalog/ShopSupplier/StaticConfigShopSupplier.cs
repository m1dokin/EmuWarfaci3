using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog.ShopSupplier.Serializers;
using MasterServer.GameLogic.ItemsSystem.Regular;
using MasterServer.GameLogic.RandomBoxValidationSystem;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.ElectronicCatalog.ShopSupplier
{
	// Token: 0x02000253 RID: 595
	[Service]
	[Singleton]
	internal class StaticConfigShopSupplier : ShopSupplier, IECatalogOffersUpdater
	{
		// Token: 0x06000D1B RID: 3355 RVA: 0x00033037 File Offset: 0x00031437
		public StaticConfigShopSupplier(IDALService dalService, IShopSupplierMetricsTracker metricsTracker, IDBUpdateService dbUpdateService, IConfigProvider<ShopReloadConfig> shopReloadConfigProvider, IOfferValidationService offerValidator, IConfigProvider<RegularItemConfig> regularItemConfigProvider) : base(metricsTracker)
		{
			this.m_dalService = dalService;
			this.m_dbUpdateService = dbUpdateService;
			this.m_shopReloadConfigProvider = shopReloadConfigProvider;
			this.m_offerValidator = offerValidator;
			this.m_regularItemConfigProvider = regularItemConfigProvider;
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000D1C RID: 3356 RVA: 0x00033066 File Offset: 0x00031466
		public override int SupplierId
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0003306C File Offset: 0x0003146C
		protected override IEnumerable<StoreOffer> GetOffersImpl()
		{
			List<StoreOffer> list = new List<StoreOffer>();
			ShopReloadConfig shopReloadConfig = this.m_shopReloadConfigProvider.Get();
			foreach (StoreOffer storeOffer in this.m_dalService.ECatalog.GetStoreOffers(shopReloadConfig.UseMemcache))
			{
				if (storeOffer.Prices.Count((PriceTag p) => p.Price > 0UL) > 1)
				{
					throw new ApplicationException(string.Format("Offer {0} for item '{1}' has price in different currencies. It's restricted by design.", storeOffer.StoreID, storeOffer.Content.Item.Name));
				}
				storeOffer.SupplierID = this.SupplierId;
				list.Add(storeOffer);
			}
			return list;
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x00033150 File Offset: 0x00031550
		protected override PurchaseResult PurchaseOffersImpl(UserInfo.User user, IEnumerable<StoreOffer> offers, IPaymentCallback paymentCallback)
		{
			List<ulong> offers2 = (from o in offers
			select o.StoreID).ToList<ulong>();
			RegularItemConfig regularItemConfig = this.m_regularItemConfigProvider.Get();
			bool stackingEnabled = regularItemConfig.StackingEnabled;
			CustomerItem customerItem = null;
			long offerHashImpl = this.GetOfferHashImpl(false);
			StoreOffer storeOffer = offers.FirstOrDefault((StoreOffer o) => o.IsKeyPriceOffer());
			List<PurchaseOfferResponse> list;
			if (storeOffer != null)
			{
				IEnumerable<CustomerItem> customerItems = this.m_dalService.ECatalog.GetCustomerItems(user.UserID);
				PriceTag keyPrice = storeOffer.GetPriceTagByCurrency(Currency.KeyMoney);
				customerItem = customerItems.FirstOrDefault((CustomerItem x) => x.CatalogItem.Name == keyPrice.KeyCatalogName);
				list = new List<PurchaseOfferResponse>(this.m_dalService.ECatalog.PurchaseOffersWithKeys(user.UserID, storeOffer.SupplierID, offers2, offerHashImpl, stackingEnabled, paymentCallback));
			}
			else
			{
				storeOffer = offers.FirstOrDefault((StoreOffer o) => o.IsIngameCoin());
				if (storeOffer != null)
				{
					list = new List<PurchaseOfferResponse>(this.m_dalService.ECatalog.PurchaseIngameCoinsOffers(user.UserID, storeOffer.SupplierID, offers2, offerHashImpl, paymentCallback));
				}
				else
				{
					list = new List<PurchaseOfferResponse>(this.m_dalService.ECatalog.PurchaseOffers(user.UserID, offers.First<StoreOffer>().SupplierID, offers2, offerHashImpl, stackingEnabled, paymentCallback));
				}
			}
			List<CustomerItem> list2 = new List<CustomerItem>();
			TransactionStatus status = TransactionStatus.INTERNAL_ERROR;
			IEnumerable<CustomerItem> customerItems2 = this.m_dalService.ECatalog.GetCustomerItems(user.UserID);
			using (IEnumerator<StoreOffer> enumerator = offers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					StaticConfigShopSupplier.<PurchaseOffersImpl>c__AnonStorey1 <PurchaseOffersImpl>c__AnonStorey2 = new StaticConfigShopSupplier.<PurchaseOffersImpl>c__AnonStorey1();
					<PurchaseOffersImpl>c__AnonStorey2.tmpoffer = enumerator.Current;
					CustomerItem purchasedItem = new CustomerItem
					{
						CatalogItem = <PurchaseOffersImpl>c__AnonStorey2.tmpoffer.Content.Item
					};
					PurchaseOfferResponse purchaseOfferResponse = list.FirstOrDefault((PurchaseOfferResponse c) => c.OfferId == <PurchaseOffersImpl>c__AnonStorey2.tmpoffer.StoreID);
					if (purchaseOfferResponse != default(PurchaseOfferResponse))
					{
						status = purchaseOfferResponse.Status;
						purchasedItem.Status = status;
						purchasedItem.OfferId = <PurchaseOffersImpl>c__AnonStorey2.tmpoffer.StoreID;
						if (purchasedItem.Status == TransactionStatus.OK)
						{
							purchasedItem.InstanceID = purchaseOfferResponse.Items[0].Value;
							CustomerItem customerItem2 = customerItems2.FirstOrDefault((CustomerItem x) => x.InstanceID == purchasedItem.InstanceID);
							if (customerItem2 == null)
							{
								throw new InvalidOperationException(string.Format("Can't find customer item Id:{0} for user Id:{1} when he buys offer Id:{2}, item count {3}", new object[]
								{
									purchasedItem.InstanceID,
									user.UserID,
									purchasedItem.OfferId,
									customerItems2.Count<CustomerItem>()
								}));
							}
							purchasedItem.BuyTimeUTC = customerItem2.BuyTimeUTC;
							purchasedItem.ExpirationTimeUTC = TimeUtils.TimeSpanToUTCTimestamp(<PurchaseOffersImpl>c__AnonStorey2.tmpoffer.Content.ExpirationTime);
							purchasedItem.DurabilityPoints = <PurchaseOffersImpl>c__AnonStorey2.tmpoffer.Content.DurabilityPoints;
							purchasedItem.AddedQuantity = <PurchaseOffersImpl>c__AnonStorey2.tmpoffer.Content.Quantity;
							purchasedItem.Quantity = customerItem2.Quantity;
							purchasedItem.OfferType = customerItem2.OfferType;
							purchasedItem.TotalDurabilityPoints = customerItem2.TotalDurabilityPoints;
							purchasedItem.CatalogItem = customerItem2.CatalogItem;
							if (customerItem != null)
							{
								CustomerItem customerItem3 = new CustomerItem
								{
									CatalogItem = customerItem.CatalogItem
								};
								customerItem3.Status = TransactionStatus.DELETED;
								customerItem3.OfferId = 0UL;
								customerItem3.InstanceID = customerItem.InstanceID;
								customerItem3.BuyTimeUTC = customerItem.BuyTimeUTC;
								customerItem3.ExpirationTimeUTC = customerItem.ExpirationTimeUTC;
								customerItem3.DurabilityPoints = customerItem.DurabilityPoints;
								customerItem3.AddedQuantity = 0UL;
								customerItem3.Quantity = customerItem.Quantity;
								customerItem3.OfferType = customerItem.OfferType;
								customerItem3.TotalDurabilityPoints = customerItem.TotalDurabilityPoints;
								list2.Add(customerItem3);
							}
						}
						list2.Add(purchasedItem);
						list.Remove(purchaseOfferResponse);
					}
				}
			}
			return new PurchaseResult(status, list2);
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x00033604 File Offset: 0x00031A04
		protected override long GetOfferHashImpl(bool force)
		{
			return long.Parse(this.m_dbUpdateService.GetECatDataGroupHash("offers", force));
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0003361C File Offset: 0x00031A1C
		public void Update(IDBUpdateService updater)
		{
			string filename = Path.Combine(Resources.GetResourcesDirectory(), "catalog_offers.xml");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			string text = xmlDocument.DocumentElement.GetAttribute("hash");
			if (Resources.DebugContentEnabled)
			{
				CRC32 crc = new CRC32();
				crc.GetHash(Encoding.ASCII.GetBytes(text + Resources.DebugContentEnabled));
				text = crc.CRCVal.ToString();
			}
			if (updater.GetECatDataGroupHash(xmlDocument.DocumentElement.Name) != text)
			{
				Log.Info<string, string>("Update data group {0}, hash {1}", xmlDocument.DocumentElement.Name, text);
				this.UpdateCatalogOffers(xmlDocument);
				updater.SetECatDataGroupHash(xmlDocument.DocumentElement.Name, text);
			}
			else
			{
				Log.Info<string>("Same hash for group {0}", xmlDocument.DocumentElement.Name);
			}
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x00033704 File Offset: 0x00031B04
		private void UpdateCatalogOffers(XmlDocument document)
		{
			ShopReloadConfig shopReloadConfig = this.m_shopReloadConfigProvider.Get();
			Dictionary<ulong, StoreOffer> dictionary = this.m_dalService.ECatalog.GetStoreOffers(shopReloadConfig.UseMemcache).ToDictionary((StoreOffer o) => o.StoreID);
			List<StoreOffer> list = new List<StoreOffer>();
			StoreOfferXmlSerializer storeOfferXmlSerializer = new StoreOfferXmlSerializer(this.m_dalService);
			IEnumerator enumerator = document.DocumentElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement element = (XmlElement)obj;
					StoreOffer storeOffer = storeOfferXmlSerializer.Deserialize(element);
					if (storeOffer != null)
					{
						storeOffer.SupplierID = this.SupplierId;
						list.Add(storeOffer);
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
			if (ServicesManager.ExecutionPhase == ExecutionPhase.Started)
			{
				this.m_offerValidator.Validate(dictionary.Values, list);
			}
			foreach (StoreOffer storeOffer2 in list)
			{
				this.m_dalService.ECatalog.UpdateOffer(storeOffer2);
				this.m_dalService.ECatalog.UpdatePrice(storeOffer2.StoreID, storeOffer2.GetOriginalPriceTag(Currency.CryMoney));
				this.m_dalService.ECatalog.UpdatePrice(storeOffer2.StoreID, storeOffer2.GetOriginalPriceTag(Currency.GameMoney));
				this.m_dalService.ECatalog.UpdatePrice(storeOffer2.StoreID, storeOffer2.GetOriginalPriceTag(Currency.CrownMoney));
				this.m_dalService.ECatalog.UpdatePrice(storeOffer2.StoreID, storeOffer2.GetOriginalPriceTag(Currency.KeyMoney));
				dictionary.Remove(storeOffer2.StoreID);
			}
			foreach (ulong num in dictionary.Keys)
			{
				this.m_dalService.ECatalog.RemoveFromStore(num);
				Log.Info<ulong>("Deleting missed offer {0}", num);
			}
		}

		// Token: 0x040005FC RID: 1532
		private readonly IDALService m_dalService;

		// Token: 0x040005FD RID: 1533
		private readonly IDBUpdateService m_dbUpdateService;

		// Token: 0x040005FE RID: 1534
		private readonly IOfferValidationService m_offerValidator;

		// Token: 0x040005FF RID: 1535
		private readonly IConfigProvider<ShopReloadConfig> m_shopReloadConfigProvider;

		// Token: 0x04000600 RID: 1536
		private readonly IConfigProvider<RegularItemConfig> m_regularItemConfigProvider;

		// Token: 0x04000601 RID: 1537
		private const string OffersHash = "offers";

		// Token: 0x04000602 RID: 1538
		public const int supplierId = 1;
	}
}
