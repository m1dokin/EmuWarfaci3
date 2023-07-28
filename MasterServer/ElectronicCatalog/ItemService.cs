using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog.Exceptions;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;
using MySql.Data.MySqlClient;
using Util.Common;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000238 RID: 568
	[Service]
	[Singleton]
	internal class ItemService : IItemService, IDebugItemService
	{
		// Token: 0x06000C2D RID: 3117 RVA: 0x0002E64C File Offset: 0x0002CA4C
		public ItemService(ISessionStorage sessionStorage, IItemStats itemStats, IProfileItems profileItems, ICatalogService catalogService, IItemsExpiration itemsExpiration, IGameRoomManager gameRoomManager, ILogService logService, IDALService dalService, IItemCache itemCacheService, IItemsPurchase itemPurchaseService, IItemRepairDescriptionRepository repairRepository)
		{
			this.m_sessionStorage = sessionStorage;
			this.m_itemStats = itemStats;
			this.m_profileItems = profileItems;
			this.m_catalogService = catalogService;
			this.m_itemsExpiration = itemsExpiration;
			this.m_logService = logService;
			this.m_dalService = dalService;
			this.m_itemCacheService = itemCacheService;
			this.m_itemPurchaseService = itemPurchaseService;
			this.m_repairRepository = repairRepository;
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06000C2E RID: 3118 RVA: 0x0002E6B4 File Offset: 0x0002CAB4
		// (remove) Token: 0x06000C2F RID: 3119 RVA: 0x0002E6EC File Offset: 0x0002CAEC
		public event Action<GiveItemResponse> ItemGiven;

		// Token: 0x06000C30 RID: 3120 RVA: 0x0002E724 File Offset: 0x0002CB24
		public RepairItemResponse RepairItem(UserInfo.User user, ulong profileItemId)
		{
			RepairItemResponse result;
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				result = this.RepairItem(user, profileItemId, logGroup);
			}
			return result;
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x0002E76C File Offset: 0x0002CB6C
		public RepairItemResponse RepairItem(UserInfo.User user, ulong profileItemId, ILogGroup logGroup)
		{
			SProfileItem profileItem = this.m_profileItems.GetProfileItem(user.ProfileID, profileItemId);
			if (profileItem == null)
			{
				throw new ItemServiceException(string.Format("No such item {0} in profile {1}", profileItemId, user.ProfileID));
			}
			SRepairItemDesc rid;
			if (!this.m_repairRepository.GetRepairItemDesc(profileItem.ItemID, profileItem.CustomerItem.CatalogItem.ID, out rid))
			{
				throw new ItemServiceException(string.Format("Can't find repair info for item id:{0} name:{1}", profileItem.ItemID, profileItem.GameItem.Name));
			}
			return this.RepairItem(user, profileItemId, profileItem.CalculateRepairCost(rid));
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x0002E810 File Offset: 0x0002CC10
		public RepairItemResponse RepairItem(UserInfo.User user, ulong profileItemId, ulong repairCost)
		{
			RepairItemResponse result;
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				result = this.RepairItem(user, profileItemId, repairCost, logGroup);
			}
			return result;
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x0002E858 File Offset: 0x0002CC58
		public RepairEquipmentOperationResult RepairMultipleItems(UserInfo.User user, IEnumerable<ulong> profileItems)
		{
			Dictionary<ulong, SProfileItem> profileItems2 = this.m_profileItems.GetProfileItems(user.ProfileID, EquipOptions.ActiveOnly, (SProfileItem item) => profileItems.Contains(item.ProfileItemID));
			return this.RepairMultipleItems(user, profileItems2.Values);
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x0002E8A0 File Offset: 0x0002CCA0
		public RepairEquipmentOperationResult RepairMultipleItems(UserInfo.User user, IEnumerable<SProfileItem> expiredProfileItems)
		{
			RepairEquipmentOperationResult repairEquipmentOperationResult = new RepairEquipmentOperationResult
			{
				ProfileId = user.ProfileID
			};
			foreach (SProfileItem sprofileItem in expiredProfileItems)
			{
				SRepairItemDesc rid = default(SRepairItemDesc);
				if (!this.m_repairRepository.GetRepairItemDesc(sprofileItem.ItemID, sprofileItem.CustomerItem.CatalogItem.ID, out rid))
				{
					Log.Warning<ulong, ulong>("Unable to get repair cost for item: ItemID: {0}, ProfileItemID: {1}", sprofileItem.ItemID, sprofileItem.ProfileItemID);
				}
				else
				{
					ulong num = sprofileItem.CalculateRepairCost(rid);
					if (num > 0UL)
					{
						repairEquipmentOperationResult.ItemsToRepair.Add(new RepairItemStatus
						{
							MoneySpent = num,
							RepairStatus = RepairStatus.Undefined,
							ProfileItemId = sprofileItem.ProfileItemID
						});
						repairEquipmentOperationResult.TotalRepairCost += num;
					}
				}
			}
			if (!repairEquipmentOperationResult.ItemsToRepair.Any<RepairItemStatus>())
			{
				return new RepairEquipmentOperationResult();
			}
			repairEquipmentOperationResult.MoneyBeforeRepair = this.m_catalogService.GetCustomerAccount(user.UserID, Currency.GameMoney).Money;
			if (repairEquipmentOperationResult.MoneyBeforeRepair >= repairEquipmentOperationResult.TotalRepairCost)
			{
				foreach (RepairItemStatus repairItemStatus in repairEquipmentOperationResult.ItemsToRepair)
				{
					try
					{
						RepairItemResponse repairItemResponse = this.RepairItem(user, repairItemStatus.ProfileItemId, repairItemStatus.MoneySpent);
						repairItemStatus.RepairStatus = RepairStatus.Ok;
						repairItemStatus.Durability = repairItemResponse.Durability;
						repairItemStatus.TotalDurability = repairItemResponse.TotalDurability;
					}
					catch (ItemServiceException)
					{
						repairItemStatus.RepairStatus = RepairStatus.Failed;
						repairEquipmentOperationResult.OperationGlobalStatus = RepairStatus.PartiallyRepaired;
						repairEquipmentOperationResult.TotalRepairCost -= repairItemStatus.MoneySpent;
						repairItemStatus.MoneySpent = 0UL;
					}
				}
				if (repairEquipmentOperationResult.OperationGlobalStatus != RepairStatus.PartiallyRepaired)
				{
					repairEquipmentOperationResult.OperationGlobalStatus = RepairStatus.Ok;
				}
			}
			else
			{
				repairEquipmentOperationResult.OperationGlobalStatus = RepairStatus.NotEnoughMoney;
			}
			return repairEquipmentOperationResult;
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x0002EAD0 File Offset: 0x0002CED0
		public void ExtendItem(UserInfo.User user, int supplierId, ulong offerId, long offerHash, ulong profileItemId)
		{
			SProfileItem profileItem = this.m_profileItems.GetProfileItem(user.ProfileID, profileItemId);
			if (profileItem == null)
			{
				throw new ItemServiceException(string.Format("Can't extend item, no such item {0} in profile {1}", profileItemId, user.ProfileID));
			}
			if (profileItem.IsDefault)
			{
				throw new ItemServiceException(string.Format("Can't extend item, item {0} in profile {1} is default", profileItemId, user.ProfileID));
			}
			if (!this.m_itemStats.IsItemAvailableForUser(profileItem.ItemID, user))
			{
				throw new ItemServiceException(string.Format("Can't extend item, item {0} is not available for profile {1}", profileItemId, user.ProfileID));
			}
			if (supplierId != 1)
			{
				throw new ItemServiceException(string.Format("Can't extend item, offer from supplier {0} can't be extend for profile {1}", supplierId, user.ProfileID));
			}
			StoreOffer storeOfferById = this.m_catalogService.GetStoreOfferById(supplierId, offerId);
			if (storeOfferById == null)
			{
				throw new ItemServiceHashMismatchException(string.Format("Can't extend item, cannot find offer with id {0} from supplier {1}", offerId, supplierId));
			}
			CatalogItem item = storeOfferById.Content.Item;
			if (storeOfferById.Type == OfferType.Permanent)
			{
				throw new ItemServiceException(string.Format("Can't extend item, item {0} in profile {1} is permanent", profileItemId, user.ProfileID));
			}
			if (item.Name != profileItem.GameItem.Name)
			{
				throw new ItemServiceException(string.Format("Can't extend item, item {0} cannot be extended by offer {1}", profileItemId, offerId));
			}
			if (item.Type == "contract")
			{
				throw new ItemServiceException(string.Format("Can't extend item, attempt to extend contract {0} from {1}", profileItemId, user.ProfileID));
			}
			if (profileItem.ExpirationTimeUTC == 0UL && storeOfferById.Content.ExpirationTime != TimeSpan.Zero)
			{
				throw new ItemServiceException(string.Format("Can't  extend item, durable item {0} cannot be extended by expiration offer {1}", profileItemId, offerId));
			}
			TransactionStatus status = this.m_catalogService.PurchaseOffer(user, offerHash, storeOfferById).Status;
			if (status == TransactionStatus.OK)
			{
				this.m_profileItems.ExtendProfileItem(user.ProfileID, profileItemId);
			}
			Dictionary<Currency, long> dictionary = new Dictionary<Currency, long>
			{
				{
					Currency.GameMoney,
					0L
				},
				{
					Currency.CryMoney,
					0L
				},
				{
					Currency.CrownMoney,
					0L
				}
			};
			foreach (PriceTag priceTag in storeOfferById.Prices)
			{
				dictionary[priceTag.Currency] = (long)priceTag.Price;
			}
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				logGroup.ShopMoneyChangedLog(user.UserID, user.ProfileID, -dictionary[Currency.GameMoney], -dictionary[Currency.CryMoney], -dictionary[Currency.CrownMoney], LogGroup.ProduceType.Extend, status, string.Empty, string.Empty);
				if (status == TransactionStatus.OK)
				{
					logGroup.ShopOfferBoughtLog(user.UserID, user.ProfileID, user.Nickname, user.Rank, user.IP, status, (long)(-(long)storeOfferById.GetPriceByCurrency(Currency.GameMoney)), (long)(-(long)storeOfferById.GetPriceByCurrency(Currency.CryMoney)), (long)(-(long)storeOfferById.GetPriceByCurrency(Currency.CrownMoney)), storeOfferById.GetPriceTagByCurrency(Currency.KeyMoney).KeyCatalogName, (ulong)dictionary[Currency.GameMoney], (ulong)dictionary[Currency.CryMoney], (ulong)dictionary[Currency.CrownMoney], storeOfferById.Status, storeOfferById.Discount, storeOfferById.StoreID, storeOfferById.Type, storeOfferById.Content.Item.ID, storeOfferById.Content.Item.Name, storeOfferById.Content.DurabilityPoints, TimeUtils.GetExpireTime(storeOfferById.Content.ExpirationTime), storeOfferById.Content.Item.Type, (storeOfferById.Content.Quantity <= 0UL) ? 1UL : storeOfferById.Content.Quantity, storeOfferById.Content.Quantity, LogGroup.ProduceType.Extend, profileItem.ProfileItemID, "-");
				}
				else
				{
					logGroup.ShopOfferBoughtFailedLog(user.UserID, user.ProfileID, user.Nickname, user.Rank, user.IP, status, (long)(-(long)storeOfferById.GetPriceByCurrency(Currency.GameMoney)), (long)(-(long)storeOfferById.GetPriceByCurrency(Currency.CryMoney)), (long)(-(long)storeOfferById.GetPriceByCurrency(Currency.CrownMoney)), (ulong)dictionary[Currency.GameMoney], (ulong)dictionary[Currency.CryMoney], (ulong)dictionary[Currency.CrownMoney], storeOfferById.StoreID, storeOfferById.Type, storeOfferById.Content.Item.ID, storeOfferById.Content.Item.Name, storeOfferById.Content.DurabilityPoints, TimeUtils.GetExpireTime(storeOfferById.Content.ExpirationTime), storeOfferById.Content.Quantity, LogGroup.ProduceType.Extend);
				}
			}
			if (status == TransactionStatus.OK && this.m_profileItems.GetProfileItem(user.ProfileID, profileItemId) == null)
			{
				throw new ItemServiceException(string.Format("No such item {0} in profile {1}", profileItemId, user.ProfileID));
			}
			if (status == TransactionStatus.NOT_ENOUGH_MONEY)
			{
				throw new ItemServiceNotEnoughtMoneyException(string.Format("Not enought money for extend, profile {0}, item {1}", user.ProfileID, profileItemId));
			}
			if (status == TransactionStatus.HASH_MISMATCH)
			{
				throw new ItemServiceHashMismatchException(string.Format("Profile {0} try to extend item {1}, with invalid offer hash", user.ProfileID, profileItemId));
			}
			if (status != TransactionStatus.OK)
			{
				throw new ItemServiceException(string.Format("Extend failed with status {0}, for profile {1} item {2}", status, user.ProfileID, profileItemId));
			}
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x0002F07C File Offset: 0x0002D47C
		public GiveItemResponse GivePermanentItem(ulong userId, string itemName, LogGroup.ProduceType produceType, ILogGroup logGroup, string reason = "-")
		{
			OfferItem offerItemByName = this.GetOfferItemByName(itemName);
			SItem sitem;
			if (!this.TryGetUnStackableItemByName(itemName, out sitem))
			{
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			SRepairItemDesc srepairItemDesc;
			if (!this.m_repairRepository.GetRepairItemDesc(sitem.ID, offerItemByName.Item.ID, out srepairItemDesc))
			{
				Log.Warning(string.Format("Can't give item {0}! Can not find repair cost and durability for it.", itemName));
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			if (srepairItemDesc.Durability < 1 && srepairItemDesc.RepairCost < 1)
			{
				Log.Warning(string.Format("Can't give item {0} as permanent!", itemName));
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			offerItemByName.DurabilityPoints = srepairItemDesc.Durability;
			offerItemByName.RepairCost = srepairItemDesc.RepairCost.ToString();
			AddCustomerItemResponse addCustomerItemResponse = this.m_catalogService.AddCustomerItem(userId, offerItemByName, false);
			if (addCustomerItemResponse.Status == TransactionStatus.OK)
			{
				ulong profileIdByUserId = this.GetProfileIdByUserId(userId);
				GiveItemResponse giveItemResponse = new GiveItemResponse(offerItemByName, profileIdByUserId, userId, addCustomerItemResponse.Status, produceType, OfferType.Permanent, logGroup, reason);
				this.ItemGiven.SafeInvoke(giveItemResponse);
				return giveItemResponse;
			}
			return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x0002F18C File Offset: 0x0002D58C
		public GiveItemResponse GiveExpirableItem(ulong userId, string itemName, TimeSpan expirationTime, LogGroup.ProduceType produceType, ILogGroup logGroup, string reason = "-")
		{
			if (expirationTime == TimeSpan.Zero)
			{
				Log.Error<double, string, LogGroup.ProduceType>("Invalid expiration time {0}h for item {1} (give item source {2})", expirationTime.TotalHours, itemName, produceType);
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			OfferItem offerItemByName = this.GetOfferItemByName(itemName);
			offerItemByName.ExpirationTime = expirationTime;
			SItem sitem;
			if (!this.TryGetUnStackableItemByName(itemName, out sitem))
			{
				Log.Warning(string.Format("Can't give item {0}! Can not find it.", itemName));
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			AddCustomerItemResponse addCustomerItemResponse = this.m_catalogService.AddCustomerItem(userId, offerItemByName, false);
			if (addCustomerItemResponse.Status == TransactionStatus.OK)
			{
				ulong profileIdByUserId = this.GetProfileIdByUserId(userId);
				GiveItemResponse giveItemResponse = new GiveItemResponse(offerItemByName, profileIdByUserId, userId, addCustomerItemResponse.Status, produceType, OfferType.Expiration, logGroup, reason);
				this.ItemGiven.SafeInvoke(giveItemResponse);
				return giveItemResponse;
			}
			return new GiveItemResponse(addCustomerItemResponse.Status);
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x0002F250 File Offset: 0x0002D650
		public GiveItemResponse GiveRegularItem(ulong userId, string itemName, LogGroup.ProduceType produceType, IPurchaseListener purchaseListener, ILogGroup logGroup, string reason = "-", bool ignoreLimit = false)
		{
			OfferItem offerItemByName = this.GetOfferItemByName(itemName);
			SItem sitem;
			if (!this.TryGetUnStackableItemByName(itemName, out sitem))
			{
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			ulong profileIdByUserId = this.GetProfileIdByUserId(userId);
			AddCustomerItemResponse addCustomerItemResponse = this.m_catalogService.AddCustomerItem(userId, offerItemByName, ignoreLimit);
			if (addCustomerItemResponse.Status == TransactionStatus.OK)
			{
				bool flag = offerItemByName.Item.Type == "random_box";
				if (flag)
				{
					CustomerItem customerItem = this.m_catalogService.GetCustomerItem(userId, offerItemByName.Item.Name, OfferType.Regular);
					if (customerItem == null)
					{
						throw new ItemServiceCatalogItemNotFoundException(string.Format("Can't find item with name {0} for user {1}", offerItemByName.Item.Name, userId));
					}
					this.m_itemPurchaseService.AddMissingOffer(userId, profileIdByUserId, null, customerItem, purchaseListener, logGroup);
				}
				GiveItemResponse giveItemResponse = new GiveItemResponse(offerItemByName, profileIdByUserId, userId, addCustomerItemResponse.Status, produceType, OfferType.Regular, logGroup, reason);
				this.ItemGiven.SafeInvoke(giveItemResponse);
				return giveItemResponse;
			}
			return new GiveItemResponse(addCustomerItemResponse.Status);
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x0002F348 File Offset: 0x0002D748
		public GiveItemResponse GiveConsumableItem(ulong userId, string itemName, ushort amount, LogGroup.ProduceType produceType, ILogGroup logGroup, ushort maxAmount = 0, string reason = "-")
		{
			OfferItem offerItemByName = this.GetOfferItemByName(itemName);
			if (!offerItemByName.Item.Stackable)
			{
				Log.Warning(string.Format("Can't give item {0} as stackable for UserId: {1}! Item is not stackable.", itemName, userId));
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			SItem sitem;
			if (!this.m_itemCacheService.TryGetItem(itemName, out sitem))
			{
				Log.Warning(string.Format("Can't give item {0}! Can not find it.", itemName));
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			ushort val = ushort.MaxValue;
			if (maxAmount != 0)
			{
				CustomerItem customerItem = this.m_catalogService.GetCustomerItem(userId, itemName, OfferType.Consumable);
				if (customerItem == null)
				{
					val = maxAmount;
				}
				else if ((ulong)maxAmount > customerItem.Quantity)
				{
					val = (ushort)((ulong)maxAmount - customerItem.Quantity);
				}
				else
				{
					val = 0;
				}
			}
			offerItemByName.Quantity = (ulong)Math.Min(amount, val);
			if (offerItemByName.Quantity == 0UL)
			{
				Log.Warning<string, ulong>("Nothing to add {0} for user {1}", itemName, userId);
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			AddCustomerItemResponse addCustomerItemResponse = this.m_catalogService.AddCustomerItem(userId, offerItemByName, false);
			if (addCustomerItemResponse.Status == TransactionStatus.OK)
			{
				ulong profileIdByUserId = this.GetProfileIdByUserId(userId);
				GiveItemResponse giveItemResponse = new GiveItemResponse(offerItemByName, profileIdByUserId, userId, addCustomerItemResponse.Status, produceType, OfferType.Consumable, logGroup, reason);
				this.ItemGiven.SafeInvoke(giveItemResponse);
				return giveItemResponse;
			}
			return new GiveItemResponse(addCustomerItemResponse.Status);
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x0002F48C File Offset: 0x0002D88C
		public GiveItemResponse GiveCoin(ulong userId, ushort amount, LogGroup.ProduceType produceType, ILogGroup logGroup, ushort maxAmount = 0, string reason = "-")
		{
			string name = this.m_catalogService.GetCatalogItems().Values.First((CatalogItem x) => x.Type == "coin").Name;
			if (!name.Contains("coin"))
			{
				Log.Warning("Couldn't find coins in catalog items");
				return new GiveItemResponse(TransactionStatus.INVALID_REQUEST);
			}
			return this.GiveConsumableItem(userId, name, amount, produceType, logGroup, maxAmount, reason);
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x0002F508 File Offset: 0x0002D908
		public ConsumeItemResponse ConsumeItem(UserInfo.User user, string serverJid, string sessionId, uint checkpoint, ulong profileItemId, ushort quantity)
		{
			if (!this.m_sessionStorage.ValidateSession(serverJid, sessionId))
			{
				throw new ItemServiceException(string.Format("Ignoring consume item action from server {0} which has incorrect session id {1}", user.OnlineID, sessionId));
			}
			SProfileItem profileItem = this.m_profileItems.GetProfileItem(user.ProfileID, profileItemId);
			if (profileItem == null)
			{
				throw new ItemServiceException(string.Format("No such item {0} in profile {1} for consuming", profileItemId, user.ProfileID));
			}
			return this.ConsumeItem(user.UserID, user.ProfileID, sessionId, checkpoint, profileItem, quantity);
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x0002F594 File Offset: 0x0002D994
		public ConsumeItemResponse ConsumeItem(ulong userId, ulong profileId, string itemName, ushort quantity)
		{
			SProfileItem value = this.m_profileItems.GetProfileItems(profileId, EquipOptions.All, (SProfileItem x) => x.GameItem.Name.Equals(itemName)).FirstOrDefault<KeyValuePair<ulong, SProfileItem>>().Value;
			if (value == null)
			{
				throw new ItemServiceException(string.Format("No such item {0} in profile {1} for consuming", itemName, profileId));
			}
			return this.ConsumeItem(userId, profileId, string.Empty, 0U, value, quantity);
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x0002F608 File Offset: 0x0002DA08
		public void DeleteItem(UserInfo.User user, ulong profileItemId)
		{
			this.DeleteItem(user, profileItemId, null);
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x0002F614 File Offset: 0x0002DA14
		public void DeleteItem(UserInfo.User user, ulong profileItemId, Action<SProfileItem> action)
		{
			SProfileItem profileItem = this.m_profileItems.GetProfileItem(user.ProfileID, profileItemId, EquipOptions.All);
			if (profileItem == null)
			{
				throw new ItemServiceException(string.Format("No such item {0} in profile {1}", profileItemId, user.ProfileID));
			}
			if (profileItem.IsDefault)
			{
				throw new ItemServiceException(string.Format("Item {0} in profile {1} is default and cannot be deleted", profileItem.ItemID, user.ProfileID));
			}
			if (action != null)
			{
				action(profileItem);
			}
			Dictionary<ulong, CustomerItem> customerItems = this.m_catalogService.GetCustomerItems(user.UserID);
			CustomerItem catalogItem;
			if (!customerItems.TryGetValue(profileItem.CatalogID, out catalogItem))
			{
				Log.Warning<ulong, ulong>("User {0} asked to delete item {1} without catalog id, probably it is old reward item", user.UserID, profileItem.ItemID);
			}
			else
			{
				List<StoreOffer> list = (from ent in this.m_catalogService.GetStoreOffers()
				where ent.Content.Item.ID == catalogItem.CatalogItem.ID
				select ent).ToList<StoreOffer>();
				bool flag;
				if (list.Count > 0)
				{
					flag = list.Exists((StoreOffer of) => of.Prices.Exists((PriceTag pt) => pt.Currency == Currency.CryMoney && pt.Price > 0UL));
				}
				else
				{
					flag = true;
				}
				bool flag2 = flag;
				if (flag2 && !profileItem.IsExpired)
				{
					throw new ItemServiceException(string.Format("Cash item {0} isn't expired yet and cannot be deleted for {1}", profileItem.ItemID, user.UserID));
				}
			}
			if (!profileItem.IsExpired)
			{
				throw new ItemServiceException(string.Format("Can't delete expirable item that doesn't expired yet, profile {0} item {1}", user.ProfileID, profileItem.ItemID));
			}
			if (profileItem.DurabilityPoints > 0)
			{
				throw new ItemServiceException(string.Format("Can't delete durable/permanent item with durability bigger than 0, profile {0} item {1}", user.ProfileID, profileItem.ItemID));
			}
			this.m_catalogService.DeleteCustomerItem(user.UserID, profileItem.CatalogID);
			this.m_profileItems.DeleteProfileItem(user.ProfileID, profileItem.ProfileItemID);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(user.ProfileID);
			this.m_logService.Event.ItemDestroyLog(user.UserID, user.ProfileID, profileInfo.Nickname, profileInfo.RankInfo.RankId, profileItemId, profileItem.CatalogID, profileItem.GameItem.Type, profileItem.GameItem.Name, 0, string.Empty);
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x0002F870 File Offset: 0x0002DC70
		public OfferItem GetOfferItemByName(string itemName)
		{
			CatalogItem item;
			if (!this.m_catalogService.TryGetCatalogItem(itemName, out item))
			{
				throw new ItemServiceCatalogItemNotFoundException(string.Format("[ItemService] Can't find catalog item {0}", itemName));
			}
			return new OfferItem
			{
				ExpirationTime = TimeSpan.Zero,
				Item = item
			};
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x0002F8C0 File Offset: 0x0002DCC0
		public bool CanGiveItem(string itemName, OfferType itemType)
		{
			CatalogItem catalogItem;
			if (!this.m_catalogService.TryGetCatalogItem(itemName, out catalogItem))
			{
				Log.Warning(string.Format("[ItemService] Can't find catalog item {0}", itemName));
				return false;
			}
			SItem item;
			bool flag = (itemType != OfferType.Consumable) ? this.TryGetUnStackableItemByName(itemName, out item) : this.TryGetItemByName(itemName, out item);
			if (itemType != OfferType.Permanent)
			{
				if (itemType == OfferType.Regular)
				{
					flag = (flag && this.CanGiveRegularItem(item));
				}
			}
			else
			{
				flag = (flag && this.CanGivePermanentItem(item));
			}
			return flag;
		}

		// Token: 0x06000C41 RID: 3137 RVA: 0x0002F954 File Offset: 0x0002DD54
		private bool CanGivePermanentItem(SItem item)
		{
			OfferItem offerItemByName = this.GetOfferItemByName(item.Name);
			SRepairItemDesc srepairItemDesc;
			return this.m_repairRepository.GetRepairItemDesc(item.ID, offerItemByName.Item.ID, out srepairItemDesc);
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x0002F990 File Offset: 0x0002DD90
		private bool CanGiveRegularItem(SItem item)
		{
			OfferItem offerItemByName = this.GetOfferItemByName(item.Name);
			SRepairItemDesc srepairItemDesc;
			bool repairItemDesc = this.m_repairRepository.GetRepairItemDesc(item.ID, offerItemByName.Item.ID, out srepairItemDesc);
			if (repairItemDesc)
			{
				Log.Warning<string>("Item {0} couldn't be regular and permanent at the same time", item.Name);
			}
			return !repairItemDesc;
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x0002F9E4 File Offset: 0x0002DDE4
		private bool TryGetItemByName(string itemName, out SItem item)
		{
			if (this.m_itemCacheService.TryGetItem(itemName, out item))
			{
				return true;
			}
			Log.Warning(string.Format("[ItemService] Unknown item {0}", itemName));
			return false;
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x0002FA0C File Offset: 0x0002DE0C
		private bool TryGetUnStackableItemByName(string itemName, out SItem item)
		{
			if (!this.TryGetItemByName(itemName, out item))
			{
				return false;
			}
			StackableItemStats stackableItemStats = this.m_itemStats.GetStackableItemStats(item.ID);
			if (stackableItemStats != null && stackableItemStats.IsStackable)
			{
				Log.Warning(string.Format("[ItemService] Can't give item {0}! Item is stackable.", itemName));
				return false;
			}
			return true;
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x0002FA60 File Offset: 0x0002DE60
		private RepairItemResponse RepairItem(UserInfo.User user, ulong profileItemId, ulong repairCost, ILogGroup logGroup)
		{
			SProfileItem profileItem = this.m_profileItems.GetProfileItem(user.ProfileID, profileItemId);
			if (profileItem == null)
			{
				throw new ItemServiceException(string.Format("No such item {0} in profile {1}", profileItemId, user.ProfileID));
			}
			SRepairItemDesc rid;
			if (!this.m_repairRepository.GetRepairItemDesc(profileItem.ItemID, profileItem.CustomerItem.CatalogItem.ID, out rid))
			{
				throw new ItemServiceException(string.Format("Can't find repair info for item id:{0} name:{1}", profileItem.ItemID, profileItem.GameItem.Name));
			}
			if (!profileItem.IsBroken)
			{
				Log.Warning<ulong, ulong>("Item {0} in profile {1} is not broken. Client data must be out of sync. Correct data will be sent", profileItemId, user.ProfileID);
				CustomerAccount customerAccount = this.m_catalogService.GetCustomerAccounts(user.UserID).Find((CustomerAccount acc) => acc.Currency == Currency.GameMoney);
				return new RepairItemResponse(profileItem.DurabilityPoints, profileItem.TotalDurabilityPoints, 0UL, customerAccount.Money);
			}
			ulong num = profileItem.CalculateRepairCost(rid);
			if (repairCost == num + 1UL)
			{
				repairCost = num;
			}
			if (repairCost == 0UL || repairCost > num)
			{
				throw new ItemServiceException(string.Format("Repair cost for item {0} in profile {1} is invalid. Repair cost = {2}, full repair cost = {3}", new object[]
				{
					profileItem.ProfileItemID,
					user.ProfileID,
					repairCost,
					num
				}));
			}
			MoneyUpdateResult moneyUpdateResult = this.m_catalogService.SpendMoney(user.UserID, Currency.GameMoney, repairCost, profileItem.CatalogID, SpendMoneyReason.RepairItem);
			logGroup.ShopMoneyChangedLog(user.UserID, user.ProfileID, (long)(-(long)repairCost), 0L, 0L, LogGroup.ProduceType.Repair, moneyUpdateResult.Status, string.Empty, string.Empty);
			if (moneyUpdateResult.Status == TransactionStatus.NOT_ENOUGH_MONEY)
			{
				throw new ItemServiceNotEnoughtMoneyException(string.Format("Not enought money for repair, profile {0}, item {1}", user.ProfileID, profileItemId));
			}
			if (moneyUpdateResult.Status != TransactionStatus.OK)
			{
				throw new ItemServiceException(string.Format("Repair failed with status {0}, for profile id {1} item id {2}", moneyUpdateResult.Status, user.ProfileID, profileItemId));
			}
			int num2 = profileItem.DurabilityPoints + profileItem.CalculateRepairDurability(rid, repairCost);
			this.m_catalogService.RepairPermanentItem(user.UserID, profileItem.CatalogID, num2, profileItem.TotalDurabilityPoints);
			this.m_profileItems.RepairProfileItem(user.ProfileID, profileItem.ProfileItemID);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(user.ProfileID);
			logGroup.ItemRepairLog(user.UserID, user.ProfileID, profileInfo.Nickname, profileInfo.RankInfo.RankId, profileItem.ProfileItemID, moneyUpdateResult.Status, num2, profileItem.GameItem.Name, (long)(-(long)repairCost), 0L, 0L);
			return new RepairItemResponse(num2, profileItem.TotalDurabilityPoints, num - repairCost, moneyUpdateResult.Money);
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x0002FD34 File Offset: 0x0002E134
		private ConsumeItemResponse ConsumeItem(ulong userId, ulong profileId, string sessionId, uint checkpoint, SProfileItem profileItem, ushort quantity)
		{
			StackableItemStats stackableItemStats = this.m_itemStats.GetStackableItemStats(profileItem.ItemID);
			if (stackableItemStats == null || !stackableItemStats.IsStackable)
			{
				throw new ItemServiceException(string.Format("Item {0} in profile {1} is not stackable", profileItem.ProfileItemID, profileId));
			}
			ConsumeItemResponse result;
			try
			{
				result = this.m_catalogService.ConsumeItem(userId, profileItem.CatalogID, quantity);
				if (result.Status != TransactionStatus.OK)
				{
					throw new ItemServiceException(string.Format("Not enough item {0} in profile {1} for consuming", profileItem.ProfileItemID, profileId));
				}
			}
			catch (MySqlException innerException)
			{
				throw new ItemServiceDalException(string.Format("Consume item {0} failed for user {1}", profileItem.CatalogID, userId), innerException);
			}
			if (result.ItemsLeft == 0)
			{
				this.m_itemsExpiration.ExpireItem(userId, profileId, profileItem.ProfileItemID);
			}
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profileId);
			string missionType = string.Empty;
			if (roomByPlayer != null)
			{
				roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					missionType = r.MissionType.Name;
				});
			}
			this.m_logService.Event.ItemConsumedLog(userId, profileId, profileItem.GameItem.Type, (ulong)quantity, (ulong)result.ItemsLeft, profileItem.GameItem.Name, sessionId, missionType, checkpoint);
			return result;
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x0002FEA0 File Offset: 0x0002E2A0
		private ulong GetProfileIdByUserId(ulong userId)
		{
			return (from sp in this.m_dalService.ProfileSystem.GetUserProfiles(userId)
			select sp.ProfileID).FirstOrDefault<ulong>();
		}

		// Token: 0x040005A1 RID: 1441
		public const string DefaultReason = "-";

		// Token: 0x040005A2 RID: 1442
		private readonly IItemStats m_itemStats;

		// Token: 0x040005A3 RID: 1443
		private readonly IItemRepairDescriptionRepository m_repairRepository;

		// Token: 0x040005A4 RID: 1444
		private readonly IProfileItems m_profileItems;

		// Token: 0x040005A5 RID: 1445
		private readonly ICatalogService m_catalogService;

		// Token: 0x040005A6 RID: 1446
		private readonly IItemsExpiration m_itemsExpiration;

		// Token: 0x040005A7 RID: 1447
		private readonly ILogService m_logService;

		// Token: 0x040005A8 RID: 1448
		private readonly IDALService m_dalService;

		// Token: 0x040005A9 RID: 1449
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x040005AA RID: 1450
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x040005AB RID: 1451
		private readonly IItemCache m_itemCacheService;

		// Token: 0x040005AC RID: 1452
		private readonly IItemsPurchase m_itemPurchaseService;
	}
}
