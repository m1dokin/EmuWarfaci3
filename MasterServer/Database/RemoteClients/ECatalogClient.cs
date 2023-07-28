using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000200 RID: 512
	internal class ECatalogClient : DALCacheProxy<IDALService>, IECatalogClient
	{
		// Token: 0x06000A75 RID: 2677 RVA: 0x00026BD7 File Offset: 0x00024FD7
		internal void Reset(IECatalog eCatalog)
		{
			this.m_eCatalog = eCatalog;
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x00026BE0 File Offset: 0x00024FE0
		public IEnumerable<StoreOffer> GetStoreOffers(bool useMemcache)
		{
			cache_domain cache_domain = (!useMemcache) ? null : cache_domains.shop_offers;
			DALCacheProxy<IDALService>.Options<StoreOffer> options = new DALCacheProxy<IDALService>.Options<StoreOffer>
			{
				cache_domain = cache_domain,
				get_data_stream = (() => this.m_eCatalog.GetStoreOffers())
			};
			return base.GetDataStream<StoreOffer>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x00026C2C File Offset: 0x0002502C
		public IEnumerable<CatalogItem> GetCatalogItems()
		{
			DALCacheProxy<IDALService>.Options<CatalogItem> options = new DALCacheProxy<IDALService>.Options<CatalogItem>
			{
				cache_domain = cache_domains.catalog_items,
				get_data_stream = (() => this.m_eCatalog.GetCatalogItems())
			};
			return base.GetDataStream<CatalogItem>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x00026C6C File Offset: 0x0002506C
		public ulong AddItem(string name, string description, int maxAmount, bool stackable, string type)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.shop_offers,
					cache_domains.catalog_items
				},
				set_func = (() => this.m_eCatalog.AddItem(name, description, maxAmount, stackable, type))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x00026CF0 File Offset: 0x000250F0
		public void RemoveFromStore(ulong storeId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.shop_offers,
					cache_domains.catalog_items
				},
				set_func = (() => this.m_eCatalog.RemoveFromStore(storeId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x00026D54 File Offset: 0x00025154
		public void ActivateItem(ulong storeId, bool active)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.shop_offers,
					cache_domains.catalog_items
				},
				set_func = (() => this.m_eCatalog.ActivateItem(storeId, active))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x00026DC0 File Offset: 0x000251C0
		public void UpdateOffer(StoreOffer offer)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.shop_offers,
					cache_domains.catalog_items
				},
				set_func = (() => this.m_eCatalog.UpdateOffer(offer))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x00026E24 File Offset: 0x00025224
		public void UpdatePrice(ulong storeId, PriceTag price)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.shop_offers,
				set_func = (() => this.m_eCatalog.UpdatePrice(storeId, price))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x00026E80 File Offset: 0x00025280
		public IEnumerable<CustomerItem> GetCustomerItems(ulong customerId)
		{
			DALCacheProxy<IDALService>.Options<CustomerItem> options = new DALCacheProxy<IDALService>.Options<CustomerItem>
			{
				cache_domain = cache_domains.customer[customerId].items,
				get_data_stream = (() => this.m_eCatalog.GetCustomerItems(customerId))
			};
			return base.GetDataStream<CustomerItem>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x00026EE8 File Offset: 0x000252E8
		public void SetMoney(ulong customerId, Currency currency, ulong money)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.customer[customerId].accounts,
				set_func = (() => this.m_eCatalog.SetMoney(customerId, currency, money))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x00026F5C File Offset: 0x0002535C
		public IEnumerable<CustomerAccount> GetCustomerAccounts(ulong customerId)
		{
			DALCacheProxy<IDALService>.Options<CustomerAccount> options = new DALCacheProxy<IDALService>.Options<CustomerAccount>
			{
				cache_domain = cache_domains.customer[customerId].accounts,
				get_data_stream = (() => this.m_eCatalog.GetCustomerAccounts(customerId))
			};
			return base.GetDataStream<CustomerAccount>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x00026FC4 File Offset: 0x000253C4
		public AddCustomerItemResponse AddCustomerItem(ulong customerId, OfferItem item, bool stackingEnabled, bool ignoreLimit)
		{
			if (customerId == 0UL)
			{
				throw new Exception(string.Format("AddCustomerItem was called for userId 0, item: {0}", item));
			}
			DALCacheProxy<IDALService>.SetOptionsScalar<AddCustomerItemResponse> options = new DALCacheProxy<IDALService>.SetOptionsScalar<AddCustomerItemResponse>
			{
				cache_domain = cache_domains.customer[customerId].items,
				set_func = (() => this.m_eCatalog.AddCustomerItem(customerId, item, stackingEnabled, ignoreLimit))
			};
			return base.SetAndClearScalar<AddCustomerItemResponse>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x00027068 File Offset: 0x00025468
		public PurchaseOfferResponse AddCustomerBoxItems(ulong customerId, ulong boxInstanceId, IEnumerable<OfferItem> items, bool stackingEnabled)
		{
			if (customerId == 0UL)
			{
				throw new Exception(string.Format("AddCustomerBoxItems was called for userId 0, box id {0}, items: {1}", boxInstanceId, string.Join(Environment.NewLine, from item in items
				select item.ToString()).ToArray<char>()));
			}
			DALCacheProxy<IDALService>.SetOptionsScalar<PurchaseOfferResponse> options = new DALCacheProxy<IDALService>.SetOptionsScalar<PurchaseOfferResponse>
			{
				cache_domain = cache_domains.customer[customerId].items,
				set_func = (() => this.m_eCatalog.AddCustomerBoxItems(customerId, boxInstanceId, items, stackingEnabled))
			};
			return base.SetAndClearScalar<PurchaseOfferResponse>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x00027144 File Offset: 0x00025544
		public IEnumerable<PurchaseOfferResponse> PurchaseOffers(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, bool stackingEnabled, IPaymentCallback paymentCallback)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<List<PurchaseOfferResponse>> options = new DALCacheProxy<IDALService>.SetOptionsScalar<List<PurchaseOfferResponse>>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.customer[customerId].accounts,
					cache_domains.customer[customerId].items
				},
				set_func = (() => this.m_eCatalog.PurchaseOffers(customerId, supplierId, offers, offerHash, stackingEnabled, paymentCallback))
			};
			return base.SetAndClearScalar<List<PurchaseOfferResponse>>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x000271F4 File Offset: 0x000255F4
		public IEnumerable<PurchaseOfferResponse> PurchaseIngameCoinsOffers(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, IPaymentCallback paymentCallback)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<List<PurchaseOfferResponse>> options = new DALCacheProxy<IDALService>.SetOptionsScalar<List<PurchaseOfferResponse>>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.customer[customerId].accounts,
					cache_domains.customer[customerId].items
				},
				set_func = (() => this.m_eCatalog.PurchaseIngameCoinsOffers(customerId, supplierId, offers, offerHash, paymentCallback))
			};
			return base.SetAndClearScalar<List<PurchaseOfferResponse>>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x0002729C File Offset: 0x0002569C
		public IEnumerable<PurchaseOfferResponse> PurchaseOffersWithKeys(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, bool stackingEnabled, IPaymentCallback paymentCallback)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<List<PurchaseOfferResponse>> options = new DALCacheProxy<IDALService>.SetOptionsScalar<List<PurchaseOfferResponse>>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.customer[customerId].accounts,
					cache_domains.customer[customerId].items
				},
				set_func = (() => this.m_eCatalog.PurchaseOffersWithKeys(customerId, supplierId, offers, offerHash, stackingEnabled, paymentCallback))
			};
			return base.SetAndClearScalar<List<PurchaseOfferResponse>>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x0002734C File Offset: 0x0002574C
		public ConsumeItemResponse ConsumeItem(ulong customerId, ulong itemId, ushort quantity)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ConsumeItemResponse> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ConsumeItemResponse>
			{
				cache_domain = cache_domains.customer[customerId].items,
				set_func = (() => this.m_eCatalog.ConsumeItem(customerId, itemId, quantity))
			};
			return base.SetAndClearScalar<ConsumeItemResponse>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x000273C0 File Offset: 0x000257C0
		public void RepairPermanentItem(ulong customerId, ulong itemId, int durability, int totalDurability)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.customer[customerId].items,
				set_func = (() => this.m_eCatalog.RepairPermanentItem(customerId, itemId, durability, totalDurability))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x0002743C File Offset: 0x0002583C
		public bool DebugExpireItem(ulong customerId, ulong itemId, uint seconds)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				cache_domain = cache_domains.customer[customerId].items,
				set_func = (() => this.m_eCatalog.DebugExpireItem(customerId, itemId, seconds))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x000274B0 File Offset: 0x000258B0
		public MoneyUpdateResult SpendMoney(ulong customerId, Currency currencyId, ulong ammount, ulong catalogId, SpendMoneyReason spendMoneyReason, IPaymentCallback paymentCallback)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<MoneyUpdateResult> options = new DALCacheProxy<IDALService>.SetOptionsScalar<MoneyUpdateResult>
			{
				cache_domain = cache_domains.customer[customerId].accounts,
				set_func = (() => this.m_eCatalog.SpendMoney(customerId, currencyId, ammount, catalogId, spendMoneyReason, paymentCallback))
			};
			return base.SetAndClearScalar<MoneyUpdateResult>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x0002753C File Offset: 0x0002593C
		public MoneyUpdateResultMulti SpendMoney(ulong customerId, IEnumerable<KeyValuePair<Currency, ulong>> money, ulong catalogId, SpendMoneyReason spendMoneyReason, IPaymentCallback paymentCallback)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<MoneyUpdateResultMulti> options = new DALCacheProxy<IDALService>.SetOptionsScalar<MoneyUpdateResultMulti>
			{
				cache_domain = cache_domains.customer[customerId].accounts,
				set_func = (() => this.m_eCatalog.SpendMoney(customerId, money, catalogId, spendMoneyReason, paymentCallback))
			};
			return base.SetAndClearScalar<MoneyUpdateResultMulti>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x000275C0 File Offset: 0x000259C0
		public void DeleteCustomerItem(ulong customerId, ulong instanceId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.customer[customerId].items,
				set_func = (() => this.m_eCatalog.DeleteCustomerItem(customerId, instanceId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x0002762C File Offset: 0x00025A2C
		public void AddMoney(ulong customerId, Currency currencyId, ulong money, string transaction, TimeSpan lifeTime)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				query_retry = base.DAL.Config.QueryRetry,
				cache_domain = cache_domains.customer[customerId].accounts,
				set_func = (() => this.m_eCatalog.AddMoney(customerId, currencyId, money, transaction, lifeTime))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x000276C8 File Offset: 0x00025AC8
		public void DebugResetCustomerItems(ulong customerId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.customer[customerId].items,
				set_func = (() => this.m_eCatalog.DebugResetCustomerItems(customerId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x00027730 File Offset: 0x00025B30
		public bool TryLockUpdaterPermission(string onlineId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				set_func = (() => this.m_eCatalog.TryLockUpdaterPermission(onlineId))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x00027778 File Offset: 0x00025B78
		public void UnlockUpdaterPermission(string onlineId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_eCatalog.UnlockUpdaterPermission(onlineId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x000277C0 File Offset: 0x00025BC0
		public void ResetUpdaterPermission()
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_eCatalog.ResetUpdaterPermission())
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x000277F4 File Offset: 0x00025BF4
		public bool BackupLogs(TimeSpan logRecordLifetime, TimeSpan dbTimeout, int batchSize)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				set_func = (() => this.m_eCatalog.BackupLogs(logRecordLifetime, dbTimeout, batchSize))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x0002784C File Offset: 0x00025C4C
		public void UpdateCatalogItemDurability(ulong customerId, ulong catalogId, int addDurability)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.customer[customerId].items,
				set_func = (() => this.m_eCatalog.UpdateCatalogItemDurability(customerId, catalogId, addDurability))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x000278C0 File Offset: 0x00025CC0
		public string GetTotalDataVersionStamp()
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<string> options = new DALCacheProxy<IDALService>.SetOptionsScalar<string>
			{
				set_func = (() => this.m_eCatalog.GetTotalDataVersionStamp())
			};
			return base.SetAndClearScalar<string>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x000278F4 File Offset: 0x00025CF4
		public IEnumerable<SVersionStamp> GetDataVersionStamps()
		{
			DALCacheProxy<IDALService>.Options<SVersionStamp> options = new DALCacheProxy<IDALService>.Options<SVersionStamp>
			{
				get_data_stream = (() => this.m_eCatalog.GetDataVersionStamps())
			};
			return base.GetDataStream<SVersionStamp>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x00027928 File Offset: 0x00025D28
		public void SetDataVersionStamp(string group, string hash)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_eCatalog.SetDataVersionStamp(group, hash))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00027978 File Offset: 0x00025D78
		public IEnumerable<EcatLogHistory> DebugGetLogHistory(ulong customerID)
		{
			DALCacheProxy<IDALService>.Options<EcatLogHistory> options = new DALCacheProxy<IDALService>.Options<EcatLogHistory>
			{
				get_data_stream = (() => this.m_eCatalog.DebugGetLogHistory(customerID))
			};
			return base.GetDataStream<EcatLogHistory>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x000279C0 File Offset: 0x00025DC0
		public void DebugGenEcatRecords(uint count, uint dayInterval)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_eCatalog.DebugGenEcatRecords(count, dayInterval))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x00027A10 File Offset: 0x00025E10
		public void DebugClearGiveMoneyTransactionHistory(ulong customerId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_eCatalog.DebugClearGiveMoneyTransactionHistory(customerId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000555 RID: 1365
		private IECatalog m_eCatalog;
	}
}
