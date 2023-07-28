using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001FF RID: 511
	internal interface IECatalogClient
	{
		// Token: 0x06000A52 RID: 2642
		IEnumerable<StoreOffer> GetStoreOffers(bool useMemcache);

		// Token: 0x06000A53 RID: 2643
		IEnumerable<CatalogItem> GetCatalogItems();

		// Token: 0x06000A54 RID: 2644
		ulong AddItem(string name, string description, int maxAmount, bool stackable, string type);

		// Token: 0x06000A55 RID: 2645
		void RemoveFromStore(ulong storeId);

		// Token: 0x06000A56 RID: 2646
		void ActivateItem(ulong storeId, bool active);

		// Token: 0x06000A57 RID: 2647
		void UpdateOffer(StoreOffer offer);

		// Token: 0x06000A58 RID: 2648
		void UpdatePrice(ulong storeId, PriceTag price);

		// Token: 0x06000A59 RID: 2649
		IEnumerable<CustomerItem> GetCustomerItems(ulong customerId);

		// Token: 0x06000A5A RID: 2650
		void SetMoney(ulong customerId, Currency currency, ulong money);

		// Token: 0x06000A5B RID: 2651
		IEnumerable<CustomerAccount> GetCustomerAccounts(ulong customerId);

		// Token: 0x06000A5C RID: 2652
		AddCustomerItemResponse AddCustomerItem(ulong customerId, OfferItem item, bool stackingEnabled, bool ignoreLimit);

		// Token: 0x06000A5D RID: 2653
		PurchaseOfferResponse AddCustomerBoxItems(ulong customerId, ulong boxInstanceId, IEnumerable<OfferItem> items, bool stackingEnabled);

		// Token: 0x06000A5E RID: 2654
		IEnumerable<PurchaseOfferResponse> PurchaseOffers(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, bool stackingEnabled, IPaymentCallback paymentCallback);

		// Token: 0x06000A5F RID: 2655
		IEnumerable<PurchaseOfferResponse> PurchaseIngameCoinsOffers(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, IPaymentCallback paymentCallback);

		// Token: 0x06000A60 RID: 2656
		IEnumerable<PurchaseOfferResponse> PurchaseOffersWithKeys(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, bool stackingEnabled, IPaymentCallback paymentCallback);

		// Token: 0x06000A61 RID: 2657
		ConsumeItemResponse ConsumeItem(ulong customerId, ulong itemId, ushort quantity);

		// Token: 0x06000A62 RID: 2658
		void RepairPermanentItem(ulong customerId, ulong itemId, int durability, int totalDurability);

		// Token: 0x06000A63 RID: 2659
		bool DebugExpireItem(ulong customerId, ulong itemId, uint seconds);

		// Token: 0x06000A64 RID: 2660
		MoneyUpdateResult SpendMoney(ulong customerId, Currency currencyId, ulong ammount, ulong catalogId, SpendMoneyReason spendMoneyReason, IPaymentCallback paymentCallback);

		// Token: 0x06000A65 RID: 2661
		MoneyUpdateResultMulti SpendMoney(ulong customerId, IEnumerable<KeyValuePair<Currency, ulong>> money, ulong catalogId, SpendMoneyReason spendMoneyReason, IPaymentCallback paymentCallback);

		// Token: 0x06000A66 RID: 2662
		void DeleteCustomerItem(ulong customerId, ulong instanceId);

		// Token: 0x06000A67 RID: 2663
		void AddMoney(ulong customerId, Currency currencyId, ulong money, string transactionId, TimeSpan lifeTime);

		// Token: 0x06000A68 RID: 2664
		void DebugResetCustomerItems(ulong customerId);

		// Token: 0x06000A69 RID: 2665
		bool TryLockUpdaterPermission(string onlineId);

		// Token: 0x06000A6A RID: 2666
		void UnlockUpdaterPermission(string onlineId);

		// Token: 0x06000A6B RID: 2667
		void ResetUpdaterPermission();

		// Token: 0x06000A6C RID: 2668
		bool BackupLogs(TimeSpan logRecordLifetime, TimeSpan dbTimeout, int batchSize);

		// Token: 0x06000A6D RID: 2669
		void UpdateCatalogItemDurability(ulong customerId, ulong catalogId, int addDurability);

		// Token: 0x06000A6E RID: 2670
		string GetTotalDataVersionStamp();

		// Token: 0x06000A6F RID: 2671
		IEnumerable<SVersionStamp> GetDataVersionStamps();

		// Token: 0x06000A70 RID: 2672
		void SetDataVersionStamp(string group, string hash);

		// Token: 0x06000A71 RID: 2673
		IEnumerable<EcatLogHistory> DebugGetLogHistory(ulong customerId);

		// Token: 0x06000A72 RID: 2674
		void DebugGenEcatRecords(uint count, uint dayInterval);

		// Token: 0x06000A73 RID: 2675
		void DebugClearGiveMoneyTransactionHistory(ulong customerId);
	}
}
