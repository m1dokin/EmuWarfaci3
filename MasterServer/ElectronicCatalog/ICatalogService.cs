using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog.ShopSupplier;
using MasterServer.Users;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000243 RID: 579
	[Contract]
	internal interface ICatalogService
	{
		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06000C67 RID: 3175
		// (remove) Token: 0x06000C68 RID: 3176
		event CatalogItemsUpdatedDelegate CatalogItemsUpdated;

		// Token: 0x06000C69 RID: 3177
		Dictionary<string, CatalogItem> GetCatalogItems();

		// Token: 0x06000C6A RID: 3178
		bool TryGetCatalogItem(string itemName, out CatalogItem item);

		// Token: 0x06000C6B RID: 3179
		IEnumerable<StoreOffer> GetStoreOffers();

		// Token: 0x06000C6C RID: 3180
		StoreOffer GetStoreOfferById(int supplierId, ulong offerId);

		// Token: 0x06000C6D RID: 3181
		IEnumerable<Currency> GetCurrenciesForOffer(int supplierId, ulong offerid);

		// Token: 0x06000C6E RID: 3182
		Dictionary<ulong, CustomerItem> GetCustomerItems(ulong customerId);

		// Token: 0x06000C6F RID: 3183
		CustomerItem GetCustomerItem(ulong customerId, string itemName, OfferType offerType);

		// Token: 0x06000C70 RID: 3184
		Dictionary<ulong, CustomerItem> GetInactiveCustomerItems(ulong customerId);

		// Token: 0x06000C71 RID: 3185
		void SetMoney(ulong customerId, Currency type, ulong money);

		// Token: 0x06000C72 RID: 3186
		List<CustomerAccount> GetCustomerAccounts(ulong customerId);

		// Token: 0x06000C73 RID: 3187
		List<CustomerAccount> GetCustomerAccounts(ulong customerId, IEnumerable<Currency> currencies);

		// Token: 0x06000C74 RID: 3188
		CustomerAccount GetCustomerAccount(ulong customerId, Currency currency);

		// Token: 0x06000C75 RID: 3189
		ConsumeItemResponse ConsumeItem(ulong customerId, ulong itemId, ushort quantity);

		// Token: 0x06000C76 RID: 3190
		void UpdateItemDurability(ulong customerId, ulong catalogInstanceId, int durabilityPoints);

		// Token: 0x06000C77 RID: 3191
		void RepairPermanentItem(ulong customerId, ulong itemId, int durability, int totalDurability);

		// Token: 0x06000C78 RID: 3192
		AddCustomerItemResponse AddCustomerItem(ulong customerId, OfferItem item, bool ignoreLimit = false);

		// Token: 0x06000C79 RID: 3193
		PurchaseOfferResponse AddCustomerBoxItems(ulong customerId, ulong boxItemId, List<OfferItem> box);

		// Token: 0x06000C7A RID: 3194
		PurchaseResult PurchaseOffer(UserInfo.User user, long offerHash, StoreOffer offer);

		// Token: 0x06000C7B RID: 3195
		PurchaseResult PurchaseOffers(UserInfo.User user, long offerHash, IEnumerable<StoreOffer> offers);

		// Token: 0x06000C7C RID: 3196
		MoneyUpdateResult SpendMoney(ulong customerId, Currency currencyId, ulong ammount, SpendMoneyReason spendMoneyReason);

		// Token: 0x06000C7D RID: 3197
		MoneyUpdateResult SpendMoney(ulong customerId, Currency currencyId, ulong ammount, ulong catalogId, SpendMoneyReason spendMoneyReason);

		// Token: 0x06000C7E RID: 3198
		MoneyUpdateResultMulti SpendMoney(ulong customerId, IEnumerable<KeyValuePair<Currency, ulong>> money, ulong catalogId, SpendMoneyReason spendMoneyReason);

		// Token: 0x06000C7F RID: 3199
		void AddMoney(ulong customerId, Currency currency, ulong gainedMoney, string transactionId = "");

		// Token: 0x06000C80 RID: 3200
		void DeleteCustomerItem(ulong customerId, ulong catalogInstanceId);

		// Token: 0x06000C81 RID: 3201
		bool BackupLogs(TimeSpan logRecordLifetime, TimeSpan dbTimeout, int batchSize);

		// Token: 0x06000C82 RID: 3202
		bool ReloadOffers();
	}
}
