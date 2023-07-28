using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.ElectronicCatalog.ShopSupplier;
using MasterServer.GameLogic.ContractSystem;
using MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler;
using MasterServer.GameLogic.ItemsSystem.RandomBoxChoiceLimitation;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000352 RID: 850
	[Service]
	[Singleton]
	internal class ItemsPurchase : ServiceModule, IItemsPurchase
	{
		// Token: 0x06001303 RID: 4867 RVA: 0x0004C4F8 File Offset: 0x0004A8F8
		public ItemsPurchase(ICatalogService catalogService, IProfileItems profileItems, IItemCache itemCache, IItemStats itemStats, IItemsExpiration itemsExpiration, IItemBoxService itemBoxService, IContractService contractService, ILogService logService, ITagService tagService, IItemRepairDescriptionRepository repairRepository, IEnumerable<IItemsPurchaseHandler> itemPurchaseHandlers, IRandomBoxChoiceLimitationService boxesChoiceLimitationService)
		{
			this.m_catalogService = catalogService;
			this.m_profileItems = profileItems;
			this.m_itemCache = itemCache;
			this.m_itemStats = itemStats;
			this.m_itemsExpiration = itemsExpiration;
			this.m_itemBoxService = itemBoxService;
			this.m_contractService = contractService;
			this.m_logService = logService;
			this.m_tagService = tagService;
			this.m_repairRepository = repairRepository;
			this.m_boxesChoiceLimitationService = boxesChoiceLimitationService;
			this.m_itemPurchaseHandlers = itemPurchaseHandlers.ToDictionary((IItemsPurchaseHandler e) => e.ItemType);
		}

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x06001304 RID: 4868 RVA: 0x0004C58C File Offset: 0x0004A98C
		// (remove) Token: 0x06001305 RID: 4869 RVA: 0x0004C5C4 File Offset: 0x0004A9C4
		public event ItemPurchasedDeleg OnItemPurchased;

		// Token: 0x06001306 RID: 4870 RVA: 0x0004C5FC File Offset: 0x0004A9FC
		public void SyncProfileItemsWithCatalog(ProfileProxy profile)
		{
			this.StackPermanentItems(profile);
			Dictionary<ulong, CustomerItem> customerItems = profile.CustomerItems;
			Dictionary<ulong, SProfileItem> dictionary = new Dictionary<ulong, SProfileItem>();
			IEnumerable<SProfileItem> enumerable = from item in profile.ProfileItems.Values
			where item.CatalogID != 0UL
			select item;
			foreach (SProfileItem sprofileItem in enumerable)
			{
				if (dictionary.ContainsKey(sprofileItem.CatalogID))
				{
					if (dictionary[sprofileItem.CatalogID].IsExpired)
					{
						if (!sprofileItem.IsExpired)
						{
							dictionary[sprofileItem.CatalogID] = sprofileItem;
						}
					}
					else if (!sprofileItem.IsExpired)
					{
						SProfileItem sprofileItem2 = dictionary[sprofileItem.CatalogID];
						if (sprofileItem2.TotalDurabilityPoints != 0 && sprofileItem.TotalDurabilityPoints != 0)
						{
							int num = Math.Max(0, sprofileItem2.TotalDurabilityPoints - sprofileItem2.DurabilityPoints);
							int num2 = Math.Max(0, sprofileItem.TotalDurabilityPoints - sprofileItem.DurabilityPoints);
							if (num2 < num)
							{
								dictionary[sprofileItem.CatalogID] = sprofileItem;
							}
						}
					}
				}
				else
				{
					dictionary.Add(sprofileItem.CatalogID, sprofileItem);
				}
			}
			foreach (SProfileItem sprofileItem3 in enumerable)
			{
				if (dictionary[sprofileItem3.CatalogID].ProfileItemID != sprofileItem3.ProfileItemID)
				{
					Log.Error<ulong, ulong, ulong>("Profile {0} has duplicate catalog id {1} for inventory id {2}", profile.ProfileID, sprofileItem3.CatalogID, sprofileItem3.ProfileItemID);
					this.m_itemsExpiration.UnequipItem(profile.ProfileID, sprofileItem3.ProfileItemID);
					profile.DeleteProfileItem(sprofileItem3.ProfileItemID);
				}
			}
			foreach (KeyValuePair<ulong, SProfileItem> keyValuePair in dictionary)
			{
				ulong key = keyValuePair.Key;
				ulong profileItemID = keyValuePair.Value.ProfileItemID;
				if (!customerItems.ContainsKey(key))
				{
					Log.Info<ulong, ulong, ulong>("Profile {0} has item {1} (ecat id {2}) which is missing from catalog", profile.ProfileID, profileItemID, key);
					this.m_itemsExpiration.UnequipItem(profile.ProfileID, profileItemID);
					profile.DeleteProfileItem(profileItemID);
				}
				else if (keyValuePair.Value.IsExpired)
				{
					if (keyValuePair.Value.OfferType == OfferType.Expiration)
					{
						DateTime dateTime = TimeUtils.UTCTimestampToUTCTime(keyValuePair.Value.ExpirationTimeUTC);
						if (dateTime > DateTime.UtcNow)
						{
							Log.Info<ulong, ulong, DateTime>("Profile {0} extend expired status for item {1} with expiration time {2}", profile.ProfileID, keyValuePair.Value.ProfileItemID, dateTime);
							this.m_profileItems.ExtendProfileItem(profile.ProfileID, keyValuePair.Value.ProfileItemID);
						}
					}
					else if (keyValuePair.Value.OfferType == OfferType.Permanent)
					{
						if (keyValuePair.Value.DurabilityPoints > 0)
						{
							Log.Info<ulong, ulong, int>("Profile {0} repair expired status for item {1} with durability points {2}", profile.ProfileID, keyValuePair.Value.ProfileItemID, keyValuePair.Value.DurabilityPoints);
							this.m_profileItems.RepairProfileItem(profile.ProfileID, keyValuePair.Value.ProfileItemID);
						}
					}
					else if (keyValuePair.Value.OfferType == OfferType.Consumable && keyValuePair.Value.Quantity > 0UL)
					{
						Log.Info<ulong, ulong, ulong>("Profile {0} repair expired status for item {1} with quantity {2}", profile.ProfileID, keyValuePair.Value.ProfileItemID, keyValuePair.Value.Quantity);
						this.m_profileItems.RepairProfileItem(profile.ProfileID, keyValuePair.Value.ProfileItemID);
					}
				}
			}
			foreach (CustomerItem customerItem in customerItems.Values)
			{
				if (!dictionary.ContainsKey(customerItem.InstanceID))
				{
					this.AddMissingOffer(profile.UserID, profile.ProfileID, customerItem);
					Log.Info<ulong, ulong>("Missing catalog item {0} added into profile items for profile {1}", customerItem.CatalogItem.ID, profile.ProfileID);
				}
			}
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x0004CAAC File Offset: 0x0004AEAC
		public TransactionStatus OpenCustomerItemBox(ulong userId, ulong profileId, ulong boxCustomerItemId, string boxName, out List<CustomerItem> purchasedItems)
		{
			return this.OpenCustomerItemsContainer(userId, boxCustomerItemId, boxName, delegate(ulong uid, ulong cachedItemId)
			{
				RandomBoxDesc randomBoxDesc = this.m_itemStats.GetRandomBoxDesc(cachedItemId);
				return this.m_itemBoxService.OpenRandomBox(uid, profileId, randomBoxDesc);
			}, out purchasedItems);
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x0004CAE8 File Offset: 0x0004AEE8
		public TransactionStatus OpenCustomerItemBundle(ulong userId, ulong profileId, ulong bundleCustomerItemId, string bundleName, out List<CustomerItem> purchasedItems)
		{
			return this.OpenCustomerItemsContainer(userId, bundleCustomerItemId, bundleName, (ulong id, ulong cachedItemId) => this.m_itemBoxService.OpenBundle(profileId, cachedItemId), out purchasedItems);
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x0004CB24 File Offset: 0x0004AF24
		public TransactionStatus OpenCustomerItemsContainer(ulong userId, ulong containerId, string containerName, Func<ulong, ulong, IEnumerable<IGenericItem>> extractedItems, out List<CustomerItem> purchasedItems)
		{
			purchasedItems = new List<CustomerItem>();
			SItem sitem;
			this.m_itemCache.TryGetItem(containerName, out sitem);
			List<IGenericItem> list = extractedItems(userId, sitem.ID).ToList<IGenericItem>();
			if (!list.Any<IGenericItem>())
			{
				return TransactionStatus.INVALID_REQUEST;
			}
			List<OfferItem> list2 = new List<OfferItem>();
			Dictionary<string, CatalogItem> catalogItems = this.m_catalogService.GetCatalogItems();
			foreach (IGenericItem genericItem in list)
			{
				OfferItem item = default(OfferItem);
				CatalogItem item2;
				if (!catalogItems.TryGetValue(genericItem.Name, out item2))
				{
					Log.Error<string>("Item {0} isn't active in shop catalog", genericItem.Name);
				}
				else
				{
					item.Item = item2;
					item.ExpirationTime = TimeSpan.Zero;
					if (genericItem.Amount != null)
					{
						item.Quantity = (ulong)((long)genericItem.Amount.Value);
					}
					else if (genericItem.IsExpirable)
					{
						item.ExpirationTime = genericItem.Expiration;
					}
					else
					{
						SItem sitem2;
						this.m_itemCache.TryGetItem(item2.Name, out sitem2);
						SRepairItemDesc srepairItemDesc;
						if (this.m_repairRepository.GetBoxedItemRepairDesc(sitem2.ID, out srepairItemDesc) || this.m_repairRepository.GetRepairItemDesc(sitem2.ID, item2.ID, out srepairItemDesc))
						{
							item.DurabilityPoints = srepairItemDesc.Durability;
							item.RepairCost = srepairItemDesc.RepairCost.ToString(CultureInfo.InvariantCulture);
						}
					}
					list2.Add(item);
				}
			}
			if (!list2.Any<OfferItem>())
			{
				throw new ApplicationException(string.Format("Failed to open random box offer for item {0}", containerName));
			}
			purchasedItems = new List<CustomerItem>();
			PurchaseOfferResponse purchaseOfferResponse = this.m_catalogService.AddCustomerBoxItems(userId, containerId, list2);
			Dictionary<ulong, CustomerItem> customerItems = this.m_catalogService.GetCustomerItems(userId);
			if (purchaseOfferResponse.Status == TransactionStatus.OK)
			{
				int num = 0;
				foreach (KeyValuePair<TransactionStatus, ulong> keyValuePair in purchaseOfferResponse.Items)
				{
					OfferItem offerItem = list2[num];
					CustomerItem customerItem = new CustomerItem
					{
						CatalogItem = offerItem.Item,
						Status = keyValuePair.Key
					};
					if (customerItem.Status == TransactionStatus.OK)
					{
						CustomerItem customerItem2;
						if (!customerItems.TryGetValue(keyValuePair.Value, out customerItem2))
						{
							string message = string.Format("Random box {0}: offer item {1}(id = {2}) was bought successfully, but it isn't present in user {3} items", new object[]
							{
								containerName,
								offerItem.Item.Name,
								offerItem.Item.ID,
								userId
							});
							throw new ItemServiceException(message);
						}
						customerItem.InstanceID = keyValuePair.Value;
						customerItem.BuyTimeUTC = customerItem2.BuyTimeUTC;
						customerItem.ExpirationTimeUTC = TimeUtils.TimeSpanToUTCTimestamp(offerItem.ExpirationTime);
						customerItem.DurabilityPoints = offerItem.DurabilityPoints;
						customerItem.AddedQuantity = offerItem.Quantity;
						customerItem.Quantity = customerItem2.Quantity;
					}
					purchasedItems.Add(customerItem);
					num++;
				}
			}
			return purchaseOfferResponse.Status;
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x0004CE90 File Offset: 0x0004B290
		private TransactionStatus ValidatePurchaseOffer(StoreOffer offer, UserInfo.User user)
		{
			CatalogItem item = offer.Content.Item;
			if (!this.IsItemUnlocked(user.ProfileID, item.Name))
			{
				Log.Warning<ulong, string>("Attempt to buy offer with locked for profile {0} item {1}", user.ProfileID, item.Name);
				return TransactionStatus.INVALID_REQUEST;
			}
			UserTags userTags = this.m_tagService.GetUserTags(user.UserID);
			if (!this.m_itemStats.IsItemAvailableForUser(item.Name, user))
			{
				Log.Warning<ulong, UserTags, string>("Attempt to buy offer with unsupported tag (profile id = {0}, tags = {1}, item name = {2}).", user.ProfileID, userTags, offer.Content.Item.Name);
				return TransactionStatus.INVALID_REQUEST;
			}
			if (offer.Rank > user.Rank)
			{
				Log.Warning<ulong, int, int>("Attempt to buy offer with unsupported rank (profile id = {0}, offer rank = {1}, user rank = {2}).", user.ProfileID, offer.Rank, user.Rank);
				return TransactionStatus.INVALID_REQUEST;
			}
			if (offer.GetPriceByCurrency(Currency.CryMoney) > 0UL && userTags.HasAnyTag(TagService.BlockPurchaseTag))
			{
				Log.Warning<ulong, string>("Attempt to buy blocked offer(profile id = {0}, user tags = {1}).", user.ProfileID, userTags.ToString());
				return TransactionStatus.RESTRICTED_BY_TAG;
			}
			if (offer.Content.Item.Type == "contract")
			{
				ContractDesc contractByName = this.m_itemStats.GetContractByName(offer.Content.Item.Name);
				ProfileContract profileContract = this.m_contractService.RotateContract(user.ProfileID);
				if (profileContract.RotationId != contractByName.Id)
				{
					Log.Warning<ulong, string>("Attempt to buy expired contract for profile {0} item {1}", user.ProfileID, item.Name);
					return TransactionStatus.INVALID_REQUEST;
				}
			}
			return TransactionStatus.OK;
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x0004D004 File Offset: 0x0004B404
		public PurchasedResult PurchaseOffer(UserInfo.User user, int supplierId, long offerHash, ulong offerId, IPurchaseListener listener)
		{
			return this.PurchaseOffers(user, supplierId, offerHash, new List<ulong>
			{
				offerId
			}, listener);
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x0004D02C File Offset: 0x0004B42C
		public PurchasedResult PurchaseOffers(UserInfo.User user, int supplierId, long offerHash, IEnumerable<ulong> offerIds, IPurchaseListener listener)
		{
			if (listener == null)
			{
				listener = new NullPurchaseListener();
			}
			PurchasedResult result = this.ValidateAndGetOffers(user, supplierId, offerIds);
			if (result.Status != TransactionStatus.OK)
			{
				return result;
			}
			return (!result.Offers.All((StoreOffer o) => o.Content.Item.Type == "random_box")) ? this.PurchaseNotBoxedOffers(user, offerHash, result.Offers, listener) : this.PurchaseRandomBoxes(user, offerHash, result.Offers, listener);
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x0004D0B8 File Offset: 0x0004B4B8
		private PurchasedResult ValidateAndGetOffers(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds)
		{
			List<StoreOffer> list = new List<StoreOffer>();
			TransactionStatus transactionStatus = TransactionStatus.INTERNAL_ERROR;
			foreach (ulong num in offerIds)
			{
				StoreOffer storeOfferById = this.m_catalogService.GetStoreOfferById(supplierId, num);
				if (storeOfferById == null)
				{
					throw new OfferNotFoundException(string.Format("Cannot find offer with id {0} from supplier {1}", num, supplierId));
				}
				transactionStatus = this.ValidatePurchaseOffer(storeOfferById, user);
				if (transactionStatus != TransactionStatus.OK)
				{
					return new PurchasedResult(transactionStatus);
				}
				list.Add(storeOfferById);
			}
			return new PurchasedResult(transactionStatus, list);
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0004D170 File Offset: 0x0004B570
		private PurchasedResult PurchaseRandomBoxes(UserInfo.User user, long offerHash, IEnumerable<StoreOffer> offers, IPurchaseListener listener)
		{
			PurchaseResult purchaseResult = new PurchaseResult(TransactionStatus.OK);
			StoreOffer storeOffer = offers.FirstOrDefault<StoreOffer>();
			IEnumerable<StoreOffer> offers2 = Enumerable.Empty<StoreOffer>();
			if (this.m_boxesChoiceLimitationService.IsBoxAvailable(user.ProfileID, storeOffer.Content.Item.Name))
			{
				purchaseResult = this.m_catalogService.PurchaseOffers(user, offerHash, offers);
				offers2 = (from o in offers
				where purchaseResult.CatalogItem.Any((CustomerItem x) => x.Status == TransactionStatus.OK && x.OfferId == o.StoreID)
				select o).ToList<StoreOffer>();
				this.ExtractOfferItems(user, offers, listener, purchaseResult);
			}
			return new PurchasedResult(purchaseResult.Status, offers2);
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0004D210 File Offset: 0x0004B610
		private PurchasedResult PurchaseNotBoxedOffers(UserInfo.User user, long offerHash, IEnumerable<StoreOffer> offers, IPurchaseListener listener)
		{
			PurchaseResult purchaseResult = this.m_catalogService.PurchaseOffers(user, offerHash, offers);
			List<StoreOffer> offers2 = (from offer in offers
			where purchaseResult.CatalogItem.Any((CustomerItem x) => x.Status == TransactionStatus.OK && x.OfferId != 0UL && x.OfferId == offer.StoreID)
			select offer).ToList<StoreOffer>();
			this.ExtractOfferItems(user, offers, listener, purchaseResult);
			return new PurchasedResult(purchaseResult.Status, offers2);
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x0004D270 File Offset: 0x0004B670
		private void ExtractOfferItems(UserInfo.User user, IEnumerable<StoreOffer> offers, IPurchaseListener listener, PurchaseResult purchaseResult)
		{
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				this.LogOffers(user, offers, purchaseResult, logGroup);
				using (IEnumerator<StoreOffer> enumerator = offers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StoreOffer offer = enumerator.Current;
						CustomerItem customerItem2 = purchaseResult.CatalogItem.FirstOrDefault((CustomerItem c) => c.OfferId == offer.StoreID);
						if (customerItem2 != null && customerItem2.Status == TransactionStatus.OK)
						{
							this.AddMissingOffer(user.UserID, user.ProfileID, offer, customerItem2, listener, logGroup);
							purchaseResult.CatalogItem.Remove(customerItem2);
						}
					}
				}
				using (List<CustomerItem>.Enumerator enumerator2 = purchaseResult.CatalogItem.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CustomerItem customerItem = enumerator2.Current;
						if (customerItem.Status.IsOneOf(new TransactionStatus[]
						{
							TransactionStatus.OK,
							TransactionStatus.DELETED
						}))
						{
							SProfileItem sprofileItem = this.m_profileItems.GetProfileItems(user.ProfileID, EquipOptions.ActiveOnly).Values.FirstOrDefault((SProfileItem i) => i.CatalogID == customerItem.InstanceID);
							listener.HandleProfileItem(new SPurchasedItem
							{
								Item = sprofileItem.GameItem,
								ProfileItemID = sprofileItem.ProfileItemID,
								AddedExpiration = TimeUtils.GetExpireTime(TimeUtils.UTCTimestampToTimeSpan(customerItem.ExpirationTimeUTC)),
								AddedQuantity = customerItem.AddedQuantity,
								Status = customerItem.Status
							}, null);
						}
					}
				}
			}
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0004D484 File Offset: 0x0004B884
		private void LogOffers(UserInfo.User user, IEnumerable<StoreOffer> offers, PurchaseResult result, ILogGroup logGroup)
		{
			Dictionary<Currency, ulong> dictionary = new Dictionary<Currency, ulong>
			{
				{
					Currency.GameMoney,
					0UL
				},
				{
					Currency.CryMoney,
					0UL
				},
				{
					Currency.CrownMoney,
					0UL
				}
			};
			try
			{
				List<CustomerAccount> customerAccounts = this.m_catalogService.GetCustomerAccounts(user.UserID);
				foreach (CustomerAccount customerAccount in customerAccounts)
				{
					dictionary[customerAccount.Currency] = customerAccount.Money;
				}
			}
			catch (PaymentServiceException e)
			{
				Log.Error(e);
			}
			using (IEnumerator<StoreOffer> enumerator2 = offers.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					StoreOffer offer = enumerator2.Current;
					CustomerItem customerItem = result.CatalogItem.FirstOrDefault((CustomerItem c) => c.OfferId == offer.StoreID);
					if (customerItem != null)
					{
						logGroup.ShopMoneyChangedLog(user.UserID, user.ProfileID, (long)(-(long)offer.GetPriceByCurrency(Currency.GameMoney)), (long)(-(long)offer.GetPriceByCurrency(Currency.CryMoney)), (long)(-(long)offer.GetPriceByCurrency(Currency.CrownMoney)), LogGroup.ProduceType.Buy, customerItem.Status, string.Empty, string.Empty);
						if (customerItem.Status != TransactionStatus.OK)
						{
							Log.Warning<ulong, ulong, TransactionStatus>("Failed to buy offer {0} for profile {1}, purchase status {2}", offer.StoreID, user.ProfileID, result.Status);
							logGroup.ShopOfferBoughtFailedLog(user.UserID, user.ProfileID, user.Nickname, user.Rank, user.IP, result.Status, (long)(-(long)offer.GetPriceByCurrency(Currency.GameMoney)), (long)(-(long)offer.GetPriceByCurrency(Currency.CryMoney)), (long)(-(long)offer.GetPriceByCurrency(Currency.CrownMoney)), dictionary[Currency.GameMoney], dictionary[Currency.CryMoney], dictionary[Currency.CrownMoney], offer.StoreID, offer.Type, offer.Content.Item.ID, offer.Content.Item.Name, offer.Content.DurabilityPoints, TimeUtils.GetExpireTime(offer.Content.ExpirationTime), offer.Content.Quantity, LogGroup.ProduceType.Buy);
						}
						else
						{
							logGroup.ShopOfferBoughtLog(user.UserID, user.ProfileID, user.Nickname, user.Rank, user.IP, customerItem.Status, (long)(-(long)offer.GetPriceByCurrency(Currency.GameMoney)), (long)(-(long)offer.GetPriceByCurrency(Currency.CryMoney)), (long)(-(long)offer.GetPriceByCurrency(Currency.CrownMoney)), offer.GetPriceTagByCurrency(Currency.KeyMoney).KeyCatalogName, dictionary[Currency.GameMoney], dictionary[Currency.CryMoney], dictionary[Currency.CrownMoney], offer.Status, offer.Discount, offer.StoreID, offer.Type, offer.Content.Item.ID, offer.Content.Item.Name, offer.Content.DurabilityPoints, TimeUtils.GetExpireTime(offer.Content.ExpirationTime), offer.Content.Item.Type, (offer.Content.Quantity <= 0UL) ? 1UL : offer.Content.Quantity, offer.Content.Quantity, LogGroup.ProduceType.Buy, 0UL, "-");
						}
					}
				}
			}
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x0004D89C File Offset: 0x0004BC9C
		private bool IsItemUnlocked(ulong profileId, string itemName)
		{
			SItem sitem = this.m_itemCache.GetAllItemsByName()[itemName];
			if (!sitem.Locked)
			{
				return true;
			}
			List<SItem> list = this.m_profileItems.GetUnlockedItems(profileId).Values.ToList<SItem>();
			return list.Exists((SItem ent) => ent.Name == itemName);
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x0004D904 File Offset: 0x0004BD04
		public void AddMissingOffer(ulong userId, ulong profileId, CustomerItem purchasedItem)
		{
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				this.AddMissingOffer(userId, profileId, null, purchasedItem, new NullPurchaseListener(), logGroup);
			}
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x0004D950 File Offset: 0x0004BD50
		public void AddMissingOffer(ulong userId, ulong profileId, StoreOffer offer, CustomerItem purchasedItem, IPurchaseListener listener, ILogGroup logGroup)
		{
			this.AddMissingOffer(userId, profileId, offer, purchasedItem, listener, logGroup, LogGroup.ProduceType.Buy);
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x0004D964 File Offset: 0x0004BD64
		private void AddMissingOffer(ulong userId, ulong profileId, StoreOffer offer, CustomerItem purchasedItem, IPurchaseListener listener, ILogGroup logGroup, LogGroup.ProduceType produceType)
		{
			SItem item;
			if (this.m_itemCache.GetAllItemsByName().TryGetValue(purchasedItem.CatalogItem.Name, out item))
			{
				if (purchasedItem.CatalogItem.Type == "random_box")
				{
					List<CustomerItem> list;
					if (this.OpenCustomerItemBox(userId, profileId, purchasedItem.InstanceID, purchasedItem.CatalogItem.Name, out list) == TransactionStatus.OK)
					{
						foreach (CustomerItem customerItem in list)
						{
							if (customerItem.Status == TransactionStatus.OK)
							{
								this.AddMissingOffer(userId, profileId, offer, customerItem, listener, logGroup, LogGroup.ProduceType.RandomBox);
							}
							else
							{
								Log.Warning<TransactionStatus, ulong, string>("Random box content error {0} for user id {1} on item {2}", customerItem.Status, userId, customerItem.CatalogItem.Name);
							}
						}
					}
				}
				else if (purchasedItem.CatalogItem.Type == "bundle")
				{
					List<CustomerItem> list2;
					if (this.OpenCustomerItemBundle(userId, profileId, purchasedItem.InstanceID, purchasedItem.CatalogItem.Name, out list2) == TransactionStatus.OK)
					{
						foreach (CustomerItem customerItem2 in list2)
						{
							if (customerItem2.Status == TransactionStatus.OK)
							{
								this.AddMissingOffer(userId, profileId, offer, customerItem2, listener, logGroup, LogGroup.ProduceType.Bundle);
							}
							else
							{
								Log.Warning<TransactionStatus, ulong, string>("Bundle content error {0} for user id {1} on item {2}", customerItem2.Status, userId, customerItem2.CatalogItem.Name);
							}
						}
					}
				}
				else
				{
					IItemsPurchaseHandler itemsPurchaseHandler;
					if (!this.m_itemPurchaseHandlers.TryGetValue(purchasedItem.CatalogItem.Type, out itemsPurchaseHandler))
					{
						itemsPurchaseHandler = this.m_itemPurchaseHandlers[string.Empty];
					}
					ItemPurchaseContext ctx = new ItemPurchaseContext
					{
						UserID = userId,
						ProfileID = profileId,
						PurchasedItem = purchasedItem,
						Item = item,
						Offer = offer,
						Listener = listener,
						LogGroup = logGroup,
						ProduceType = produceType
					};
					itemsPurchaseHandler.HandleItemPurchase(ctx);
				}
				this.OnItemPurchased(item, profileId);
			}
			else
			{
				Log.Warning<string, ulong>("Item {0} missed and cannot be added as purchased for profile {1}", purchasedItem.CatalogItem.Name, profileId);
			}
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x0004DBD8 File Offset: 0x0004BFD8
		private void StackPermanentItems(ProfileProxy profile)
		{
			Dictionary<ulong, CustomerItem> dictionary = new Dictionary<ulong, CustomerItem>();
			List<ulong> list = new List<ulong>();
			Dictionary<ulong, int> dictionary2 = new Dictionary<ulong, int>();
			foreach (CustomerItem customerItem in profile.CustomerItems.Values)
			{
				if (customerItem.OfferType == OfferType.Permanent)
				{
					if (dictionary.ContainsKey(customerItem.CatalogItem.ID))
					{
						if (!dictionary2.ContainsKey(dictionary[customerItem.CatalogItem.ID].InstanceID))
						{
							dictionary2.Add(dictionary[customerItem.CatalogItem.ID].InstanceID, 0);
						}
						Dictionary<ulong, int> dictionary3;
						ulong instanceID;
						(dictionary3 = dictionary2)[instanceID = dictionary[customerItem.CatalogItem.ID].InstanceID] = dictionary3[instanceID] + customerItem.DurabilityPoints;
						list.Add(customerItem.InstanceID);
					}
					else
					{
						dictionary.Add(customerItem.CatalogItem.ID, customerItem);
					}
				}
			}
			if (list.Any<ulong>())
			{
				foreach (ulong num in dictionary2.Keys)
				{
					this.m_catalogService.UpdateItemDurability(profile.UserID, num, dictionary2[num]);
				}
				foreach (ulong catalogInstanceId in list)
				{
					this.m_catalogService.DeleteCustomerItem(profile.UserID, catalogInstanceId);
				}
			}
		}

		// Token: 0x040008C2 RID: 2242
		private readonly ICatalogService m_catalogService;

		// Token: 0x040008C3 RID: 2243
		private readonly IProfileItems m_profileItems;

		// Token: 0x040008C4 RID: 2244
		private readonly IItemCache m_itemCache;

		// Token: 0x040008C5 RID: 2245
		private readonly IItemStats m_itemStats;

		// Token: 0x040008C6 RID: 2246
		private readonly IItemRepairDescriptionRepository m_repairRepository;

		// Token: 0x040008C7 RID: 2247
		private readonly IItemsExpiration m_itemsExpiration;

		// Token: 0x040008C8 RID: 2248
		private readonly IItemBoxService m_itemBoxService;

		// Token: 0x040008C9 RID: 2249
		private readonly IContractService m_contractService;

		// Token: 0x040008CA RID: 2250
		private readonly ILogService m_logService;

		// Token: 0x040008CB RID: 2251
		private readonly ITagService m_tagService;

		// Token: 0x040008CC RID: 2252
		private readonly IRandomBoxChoiceLimitationService m_boxesChoiceLimitationService;

		// Token: 0x040008CD RID: 2253
		private readonly Dictionary<string, IItemsPurchaseHandler> m_itemPurchaseHandlers;
	}
}
