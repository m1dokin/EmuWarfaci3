using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x0200005F RID: 95
	public interface IECatalog
	{
		// Token: 0x060000BE RID: 190
		DALResultMulti<StoreOffer> GetStoreOffers();

		// Token: 0x060000BF RID: 191
		DALResultMulti<CatalogItem> GetCatalogItems();

		// Token: 0x060000C0 RID: 192
		DALResult<ulong> AddItem(string name, string description, int max_amount, bool stackable, string type);

		// Token: 0x060000C1 RID: 193
		DALResultVoid ActivateItem(ulong store_id, bool active);

		// Token: 0x060000C2 RID: 194
		DALResultVoid RemoveFromStore(ulong store_id);

		// Token: 0x060000C3 RID: 195
		DALResultVoid UpdateOffer(StoreOffer offer);

		// Token: 0x060000C4 RID: 196
		DALResultVoid UpdatePrice(ulong store_id, PriceTag price);

		// Token: 0x060000C5 RID: 197
		DALResultVoid SetMoney(ulong customer_id, Currency currency, ulong money);

		// Token: 0x060000C6 RID: 198
		DALResultMulti<CustomerItem> GetCustomerItems(ulong customer_id);

		// Token: 0x060000C7 RID: 199
		DALResultMulti<CustomerAccount> GetCustomerAccounts(ulong customer_id);

		// Token: 0x060000C8 RID: 200
		DALResult<AddCustomerItemResponse> AddCustomerItem(ulong customer_id, OfferItem item, bool stackingEnabled, bool ignoreLimit);

		// Token: 0x060000C9 RID: 201
		DALResult<PurchaseOfferResponse> AddCustomerBoxItems(ulong customer_id, ulong box_instance_id, IEnumerable<OfferItem> items, bool stackingEnabled);

		// Token: 0x060000CA RID: 202
		DALResult<List<PurchaseOfferResponse>> PurchaseOffers(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, bool stackingEnabled, IPaymentCallback paymentCallback);

		// Token: 0x060000CB RID: 203
		DALResult<List<PurchaseOfferResponse>> PurchaseIngameCoinsOffers(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, IPaymentCallback paymentCallback);

		// Token: 0x060000CC RID: 204
		DALResult<List<PurchaseOfferResponse>> PurchaseOffersWithKeys(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, bool stackingEnabled, IPaymentCallback paymentCallback);

		// Token: 0x060000CD RID: 205
		DALResult<ConsumeItemResponse> ConsumeItem(ulong customerId, ulong itemId, ushort quantity);

		// Token: 0x060000CE RID: 206
		DALResultVoid RepairPermanentItem(ulong customerId, ulong itemId, int durability, int totalDurability);

		// Token: 0x060000CF RID: 207
		DALResultVoid UpdateCatalogItemDurability(ulong customerId, ulong catalogId, int addDurability);

		// Token: 0x060000D0 RID: 208
		DALResult<bool> DebugExpireItem(ulong customerId, ulong itemId, uint seconds);

		// Token: 0x060000D1 RID: 209
		DALResult<MoneyUpdateResult> SpendMoney(ulong customerId, Currency currency, ulong ammount, ulong catalogId, SpendMoneyReason spendMoneyReason, IPaymentCallback paymentCallback);

		// Token: 0x060000D2 RID: 210
		DALResult<MoneyUpdateResultMulti> SpendMoney(ulong customerId, IEnumerable<KeyValuePair<Currency, ulong>> money, ulong catalogId, SpendMoneyReason spendMoneyReason, IPaymentCallback paymentCallback);

		// Token: 0x060000D3 RID: 211
		DALResultVoid DeleteCustomerItem(ulong customer_id, ulong instance_id);

		// Token: 0x060000D4 RID: 212
		DALResultVoid AddMoney(ulong customerId, Currency currencyId, ulong money, string transactionId, TimeSpan lifeTime);

		// Token: 0x060000D5 RID: 213
		DALResult<bool> TryLockUpdaterPermission(string onlineId);

		// Token: 0x060000D6 RID: 214
		DALResultVoid UnlockUpdaterPermission(string onlineId);

		// Token: 0x060000D7 RID: 215
		DALResultVoid ResetUpdaterPermission();

		// Token: 0x060000D8 RID: 216
		DALResult<bool> BackupLogs(TimeSpan logRecordLifetime, TimeSpan dbTimeout, int batchSize);

		// Token: 0x060000D9 RID: 217
		DALResultVoid DebugResetCustomerItems(ulong customer_id);

		// Token: 0x060000DA RID: 218
		DALResult<string> GetTotalDataVersionStamp();

		// Token: 0x060000DB RID: 219
		DALResultMulti<SVersionStamp> GetDataVersionStamps();

		// Token: 0x060000DC RID: 220
		DALResultVoid SetDataVersionStamp(string group, string hash);

		// Token: 0x060000DD RID: 221
		DALResultMulti<EcatLogHistory> DebugGetLogHistory(ulong customerId);

		// Token: 0x060000DE RID: 222
		DALResultVoid DebugGenEcatRecords(uint count, uint dayInterval);

		// Token: 0x060000DF RID: 223
		DALResultVoid DebugClearGiveMoneyTransactionHistory(ulong customerId);
	}
}
