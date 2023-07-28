using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services.Jobs;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.DAL.CustomRules;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.ElectronicCatalog.Exceptions;
using MasterServer.GameLogic.Achievements;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.ManualRoomService;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameLogic.RatingSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.GameLogic.SpecialProfileRewards.Actions;
using MasterServer.GameRoomSystem;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Users;
using MasterServer.Users.AuthorizationTokens;
using MasterServer.XMPP;
using OLAPHypervisor;
using Util.Common;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002E3 RID: 739
	internal class GameInterfaceContext : IGameInterfaceContext, IDisposable
	{
		// Token: 0x0600102F RID: 4143 RVA: 0x0003F484 File Offset: 0x0003D884
		public GameInterfaceContext(AccessLevel access_level, ILogGroup log_group) : this(access_level, log_group, ServicesManager.GetService<IDALService>(), ServicesManager.GetService<IProfileItems>(), ServicesManager.GetService<IItemCache>(), ServicesManager.GetService<ICatalogService>(), ServicesManager.GetService<IDebugCatalogService>(), ServicesManager.GetService<INotificationService>(), ServicesManager.GetService<IUserRepository>(), ServicesManager.GetService<IPunishmentService>(), ServicesManager.GetService<IAnnouncementService>(), ServicesManager.GetService<IAuthService>(), ServicesManager.GetService<ICommunicationStatsService>(), ServicesManager.GetService<IClanService>(), ServicesManager.GetService<IPlayerStatsService>(), ServicesManager.GetService<ITelemetryDALService>(), ServicesManager.GetService<ISpecialProfileRewardService>(), ServicesManager.GetService<IProfileProgressionService>(), ServicesManager.GetService<ISessionInfoService>(), ServicesManager.GetService<ICustomRulesService>(), ServicesManager.GetService<IRankSystem>(), ServicesManager.GetService<IAchievementSystem>(), ServicesManager.GetService<IGameRoomManager>(), ServicesManager.GetService<IFriendsService>(), ServicesManager.GetService<IOnlineClient>(), ServicesManager.GetService<IQueryManager>(), ServicesManager.GetService<IClientVersionsManagementService>(), ServicesManager.GetService<ITagService>(), ServicesManager.GetService<IItemService>(), ServicesManager.GetService<IJobSchedulerService>(), ServicesManager.GetService<IRatingSeasonService>(), ServicesManager.GetService<IRatingGameBanService>(), ServicesManager.GetService<IColdStorageService>(), ServicesManager.GetService<IShopService>(), ServicesManager.GetService<IManualRoomService>(), ServicesManager.GetService<IAuthorizationTokenService>())
		{
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x0003F544 File Offset: 0x0003D944
		public GameInterfaceContext(AccessLevel accessLevel, ILogGroup logGroup, IDALService dalService, IProfileItems profileItemService, IItemCache itemCacheService, ICatalogService catalogService, IDebugCatalogService debugCatalogService, INotificationService notificationService, IUserRepository userRepository, IPunishmentService punishmentService, IAnnouncementService announcementService, IAuthService authService, ICommunicationStatsService communicationStatsService, IClanService clanService, IPlayerStatsService playerStatsService, ITelemetryDALService telemetryDalService, ISpecialProfileRewardService specialRewardsService, IProfileProgressionService profileProgressionService, ISessionInfoService sessionInfoService, ICustomRulesService customRulesService, IRankSystem rankSystem, IAchievementSystem achievementSystem, IGameRoomManager gameRoomManager, IFriendsService friendsService, IOnlineClient onlineClient, IQueryManager queryManager, IClientVersionsManagementService clientVersionsManagementService, ITagService tagsService, IItemService itemService, IJobSchedulerService jobSchedulerService, IRatingSeasonService ratingSeasonService, IRatingGameBanService ratingGameBanService, IColdStorageService coldStorageService, IShopService shopService, IManualRoomService manualRoomService, IAuthorizationTokenService authorizationTokenService)
		{
			this.m_dalService = dalService;
			this.m_profileItemService = profileItemService;
			this.m_itemCacheService = itemCacheService;
			this.m_catalogService = catalogService;
			this.m_debugCatalogService = debugCatalogService;
			this.m_notificationService = notificationService;
			this.m_userRepository = userRepository;
			this.m_punishmentService = punishmentService;
			this.m_announcementService = announcementService;
			this.m_authService = authService;
			this.m_communicationStatsService = communicationStatsService;
			this.m_clanService = clanService;
			this.m_playerStatsService = playerStatsService;
			this.m_telemetryDalService = telemetryDalService;
			this.m_specialRewardsService = specialRewardsService;
			this.m_profileProgressionService = profileProgressionService;
			this.m_sessionInfoService = sessionInfoService;
			this.m_customRulesService = customRulesService;
			this.m_rankSystem = rankSystem;
			this.m_achievementSystem = achievementSystem;
			this.m_gameRoomManager = gameRoomManager;
			this.m_friendsService = friendsService;
			this.m_onlineClient = onlineClient;
			this.m_queryManager = queryManager;
			this.m_clientVersionsManagementService = clientVersionsManagementService;
			this.m_tagService = tagsService;
			this.m_itemService = itemService;
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_ratingSeasonService = ratingSeasonService;
			this.m_ratingGameBanService = ratingGameBanService;
			this.m_coldStorageService = coldStorageService;
			this.m_shopService = shopService;
			this.m_manualRoomService = manualRoomService;
			this.m_authorizationTokenService = authorizationTokenService;
			this.AccessLvl = accessLevel;
			this.LogGroup = logGroup;
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x0003F674 File Offset: 0x0003DA74
		public void Dispose()
		{
			this.LogGroup.Dispose();
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06001032 RID: 4146 RVA: 0x0003F681 File Offset: 0x0003DA81
		// (set) Token: 0x06001033 RID: 4147 RVA: 0x0003F689 File Offset: 0x0003DA89
		public ILogGroup LogGroup { get; private set; }

		// Token: 0x06001034 RID: 4148 RVA: 0x0003F692 File Offset: 0x0003DA92
		public int TotalOnlineUsers()
		{
			return this.m_communicationStatsService.TotalOnlineUsers;
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x0003F6A0 File Offset: 0x0003DAA0
		public string GetServerStatus()
		{
			if (!Resources.IsDevMode)
			{
				throw new NotImplementedException("This API method is avalaible in dev mode");
			}
			return ServicesManager.ExecutionPhase.ToString();
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x0003F6D5 File Offset: 0x0003DAD5
		public void Quit()
		{
			if (!Resources.IsDevMode)
			{
				throw new NotImplementedException("This API method is avalaible in dev mode");
			}
			ConsoleCmdManager.ExecuteCmd("quit");
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x0003F6F8 File Offset: 0x0003DAF8
		public IPAddress GetUserIPByProfileId(ulong profileId)
		{
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			string text = (user == null) ? this.GetOnlineProfileInfo(this.GetUserID(profileId)).IPAddress : user.IP;
			return string.IsNullOrEmpty(text) ? IPAddress.None : IPAddress.Parse(text);
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x0003F754 File Offset: 0x0003DB54
		public Dictionary<string, int> ServerOnlineUsers()
		{
			return this.m_communicationStatsService.ServerOnlineUsers;
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x0003F764 File Offset: 0x0003DB64
		public bool GiveItem(ulong userId, string itemName, string expire, string message, string reason, bool notify = true)
		{
			return this.GiveItem(userId, itemName, OfferType.Expiration, expire, message, reason, notify, false);
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x0003F784 File Offset: 0x0003DB84
		public bool GiveItem(ulong userId, string itemName, OfferType offerType, string parameter, string message, string reason, bool notify = true, bool ignoreLimit = false)
		{
			ulong profileID = this.GetProfileID(userId);
			IPurchaseListener purchaseListener = null;
			TimeSpan timeSpan = NotificationService.DefaultNotificationTTL;
			GiveItemResponse giveItemResponse;
			switch (offerType)
			{
			case OfferType.Expiration:
				timeSpan = TimeUtils.GetExpireTime(parameter);
				giveItemResponse = this.m_itemService.GiveExpirableItem(userId, itemName, timeSpan, MasterServer.Core.LogGroup.ProduceType.GameInterface, this.LogGroup, reason);
				goto IL_AA;
			case OfferType.Permanent:
				giveItemResponse = this.m_itemService.GivePermanentItem(userId, itemName, MasterServer.Core.LogGroup.ProduceType.GameInterface, this.LogGroup, reason);
				goto IL_AA;
			case OfferType.Regular:
				purchaseListener = RandomBoxPurchaseHandler.Create(profileID, null);
				giveItemResponse = this.m_itemService.GiveRegularItem(userId, itemName, MasterServer.Core.LogGroup.ProduceType.GameInterface, purchaseListener, this.LogGroup, reason, ignoreLimit);
				goto IL_AA;
			}
			throw new ApplicationException(string.Format("Can't give item {0}! Unsupported offer type {1}", itemName, offerType));
			IL_AA:
			if (giveItemResponse.OperationStatus == TransactionStatus.OK)
			{
				string correlationId = GameInterfaceContext.GetCorrelationId();
				MasterServer.Core.Log.Info("[GI operation id '{0}': item '{1}' of type '{2}' was given to user '{3}'", new object[]
				{
					correlationId,
					itemName,
					offerType,
					userId
				});
				if (profileID != 0UL)
				{
					bool flag = giveItemResponse.ItemGiven.Item.Type == "random_box";
					SNotification item = (!flag) ? ItemGivenNotificationFactory.CreateItemGivenNotification(giveItemResponse, timeSpan, message, notify, null) : purchaseListener.CreateNotification(giveItemResponse.ItemGiven, message, notify);
					this.m_notificationService.AddNotifications(profileID, new List<SNotification>
					{
						item
					}, EDeliveryType.SendNowOrLater);
				}
			}
			return giveItemResponse.OperationStatus == TransactionStatus.OK;
		}

		// Token: 0x0600103B RID: 4155 RVA: 0x0003F8F3 File Offset: 0x0003DCF3
		private static string GetCorrelationId()
		{
			return string.Format("{0}_{1}", Resources.ServerName, Guid.NewGuid());
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x0003F910 File Offset: 0x0003DD10
		public bool RemoveItem(ulong userId, string itemName, bool all, string reason)
		{
			ulong profileID = this.GetProfileID(userId);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileID);
			Dictionary<ulong, CustomerItem> customerItems = this.m_catalogService.GetCustomerItems(userId);
			int num = 0;
			foreach (CustomerItem customerItem in customerItems.Values)
			{
				if (customerItem.CatalogItem.Name == itemName)
				{
					this.m_catalogService.DeleteCustomerItem(userId, customerItem.InstanceID);
					this.LogGroup.ItemDestroyLog(userId, profileID, profileInfo.Nickname, profileInfo.RankInfo.RankId, 0UL, customerItem.InstanceID, customerItem.CatalogItem.Type, customerItem.CatalogItem.Name, customerItem.DurabilityPoints, reason);
					num++;
					if (!all)
					{
						break;
					}
				}
			}
			return num > 0;
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x0003FA18 File Offset: 0x0003DE18
		public bool RemoveItem(ulong userId, ulong customerItemId, string reason)
		{
			ulong profileID = this.GetProfileID(userId);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileID);
			Dictionary<ulong, CustomerItem> customerItems = this.m_catalogService.GetCustomerItems(userId);
			CustomerItem customerItem;
			if (!customerItems.TryGetValue(customerItemId, out customerItem))
			{
				return false;
			}
			this.m_catalogService.DeleteCustomerItem(userId, customerItemId);
			this.LogGroup.ItemDestroyLog(userId, profileID, profileInfo.Nickname, profileInfo.RankInfo.RankId, 0UL, customerItem.InstanceID, customerItem.CatalogItem.Type, customerItem.CatalogItem.Name, customerItem.DurabilityPoints, reason);
			return true;
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x0003FAB0 File Offset: 0x0003DEB0
		public bool RemovePermanentItem(ulong userId, string itemName, string reason)
		{
			Dictionary<ulong, CustomerItem> customerItems = this.m_catalogService.GetCustomerItems(userId);
			CustomerItem customerItem = customerItems.Values.FirstOrDefault((CustomerItem ci) => ci.CatalogItem.Name == itemName && ci.TotalDurabilityPoints > 0);
			if (customerItem == null || customerItem.DurabilityPoints < customerItem.TotalDurabilityPoints)
			{
				return false;
			}
			if (customerItem.DurabilityPoints > customerItem.TotalDurabilityPoints)
			{
				this.m_catalogService.UpdateItemDurability(userId, customerItem.InstanceID, -customerItem.TotalDurabilityPoints);
			}
			else
			{
				this.m_catalogService.DeleteCustomerItem(userId, customerItem.InstanceID);
			}
			ulong profileID = this.GetProfileID(userId);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileID);
			this.LogGroup.ItemDestroyLog(userId, profileID, profileInfo.Nickname, profileInfo.RankInfo.RankId, 0UL, customerItem.InstanceID, customerItem.CatalogItem.Type, customerItem.CatalogItem.Name, customerItem.TotalDurabilityPoints, reason);
			return true;
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x0003FBAC File Offset: 0x0003DFAC
		public bool RemoveProfileItem(ulong profileId, ulong profileItemId, string reason)
		{
			ulong userID = this.GetUserID(profileId);
			SProfileItem profileItem = this.m_profileItemService.GetProfileItem(profileId, profileItemId);
			return profileItem != null && this.RemoveItem(userID, profileItem.CatalogID, reason);
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x0003FBE8 File Offset: 0x0003DFE8
		private void SendMoneyGivenNotification(ulong profileId, Currency currency, ulong money, EDeliveryType deliveryType, EConfirmationType confirmationType, string message, bool notify)
		{
			SNotification item = NotificationFactory.CreateNotification<string>(ENotificationType.MoneyGiven, new SMoneyRewardNotification
			{
				curr = currency,
				amount = money,
				notify = notify
			}.ToXml().OuterXml, TimeSpan.FromDays(1.0), confirmationType, message);
			this.m_notificationService.AddNotifications(profileId, new List<SNotification>
			{
				item
			}, deliveryType);
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x0003FC60 File Offset: 0x0003E060
		public bool NotifyMoneyGiven(ulong userId, Currency curr, ulong money)
		{
			ulong profileID = this.GetProfileID(userId);
			if (profileID != 0UL)
			{
				this.SendMoneyGivenNotification(profileID, curr, money, EDeliveryType.SendNow, EConfirmationType.None, string.Empty, true);
			}
			return true;
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x0003FC90 File Offset: 0x0003E090
		public TransactionStatus GiveMoney(ulong userId, Currency curr, ulong money, string message, bool notify, string transactionId, string reason)
		{
			ulong profileID = this.GetProfileID(userId);
			try
			{
				this.m_catalogService.AddMoney(userId, curr, money, transactionId);
				this.LogGroup.ShopMoneyChangedLog(userId, profileID, (long)((curr != Currency.GameMoney) ? 0UL : money), (long)((curr != Currency.CryMoney) ? 0UL : money), (long)((curr != Currency.CrownMoney) ? 0UL : money), MasterServer.Core.LogGroup.ProduceType.GameInterface, TransactionStatus.OK, transactionId, reason);
				if (profileID != 0UL)
				{
					this.SendMoneyGivenNotification(profileID, curr, money, EDeliveryType.SendNowOrLater, EConfirmationType.Confirmation, message, notify);
				}
			}
			catch (ECatGiveMoneyTransactionException e)
			{
				MasterServer.Core.Log.Error(e);
				return TransactionStatus.GIVE_MONEY_TRANSACTION_COLLISION;
			}
			return TransactionStatus.OK;
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x0003FD34 File Offset: 0x0003E134
		public bool SpendMoney(ulong customer_id, Currency currency_id, ulong amount, string reason)
		{
			ulong profileID = this.GetProfileID(customer_id);
			MoneyUpdateResult moneyUpdateResult = this.m_catalogService.SpendMoney(customer_id, currency_id, amount, SpendMoneyReason.GameInterface);
			this.LogGroup.ShopMoneyChangedLog(customer_id, profileID, (long)((currency_id != Currency.GameMoney) ? 0UL : (-(long)amount)), (long)((currency_id != Currency.CryMoney) ? 0UL : (-(long)amount)), (long)((currency_id != Currency.CrownMoney) ? 0UL : (-(long)amount)), MasterServer.Core.LogGroup.ProduceType.GameInterface, moneyUpdateResult.Status, string.Empty, reason);
			return moneyUpdateResult.Status == TransactionStatus.OK;
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x0003FDB0 File Offset: 0x0003E1B0
		public int GiveCoins(ulong userId, ushort coins, string message, string reason, bool notify)
		{
			GiveItemResponse giveItemResponse = this.m_itemService.GiveCoin(userId, coins, MasterServer.Core.LogGroup.ProduceType.GameInterface, this.LogGroup, 0, reason);
			if (giveItemResponse.OperationStatus == TransactionStatus.OK)
			{
				this.NotifyConsumableGiven(userId, "coin", giveItemResponse, notify, message);
			}
			List<CustomerItem> source = new List<CustomerItem>(this.m_catalogService.GetCustomerItems(userId).Values);
			CustomerItem customerItem = source.FirstOrDefault((CustomerItem x) => x.CatalogItem.Type == "coin");
			MasterServer.Core.Log.Info<TransactionStatus>("[gi_give_coins]: Operation status: {0}", giveItemResponse.OperationStatus);
			return (customerItem == null) ? 0 : ((int)customerItem.Quantity);
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x0003FE50 File Offset: 0x0003E250
		public int GiveConsumable(ulong userId, string itemName, ushort count, string message, string reason, bool notify)
		{
			GiveItemResponse giveItemResponse = this.m_itemService.GiveConsumableItem(userId, itemName, count, MasterServer.Core.LogGroup.ProduceType.GameInterface, this.LogGroup, 0, reason);
			if (giveItemResponse.OperationStatus == TransactionStatus.OK)
			{
				this.NotifyConsumableGiven(userId, itemName, giveItemResponse, notify, message);
			}
			List<CustomerItem> source = new List<CustomerItem>(this.m_catalogService.GetCustomerItems(userId).Values);
			CustomerItem customerItem = source.FirstOrDefault((CustomerItem x) => x.CatalogItem.Name == itemName);
			MasterServer.Core.Log.Info<TransactionStatus>("[gi_give_consumable]: Operation status: {0}", giveItemResponse.OperationStatus);
			return (customerItem == null) ? 0 : ((int)customerItem.Quantity);
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x0003FEF4 File Offset: 0x0003E2F4
		private void NotifyConsumableGiven(ulong userId, string itemName, GiveItemResponse response, bool notify, string message)
		{
			string correlationId = GameInterfaceContext.GetCorrelationId();
			MasterServer.Core.Log.Info("[GI operation id '{0}': item '{1}' of type '{2}' was given to user '{3}'", new object[]
			{
				correlationId,
				itemName,
				OfferType.Consumable,
				userId
			});
			ulong profileID = this.GetProfileID(userId);
			if (profileID != 0UL)
			{
				SNotification item = ItemGivenNotificationFactory.CreateItemGivenNotification(response, TimeSpan.FromDays(7.0), message, notify, null);
				this.m_notificationService.AddNotifications(profileID, new List<SNotification>
				{
					item
				}, EDeliveryType.SendNowOrLater);
			}
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x0003FF78 File Offset: 0x0003E378
		public bool UnlockItem(ulong profileId, string itemName)
		{
			ulong userID = this.GetUserID(profileId);
			Dictionary<string, SItem> allItemsByName = this.m_itemCacheService.GetAllItemsByName();
			SItem sitem = allItemsByName[itemName];
			this.m_profileItemService.UnlockItem(profileId, sitem.ID);
			this.LogGroup.ItemUnlockedLog(userID, profileId, sitem.ID, sitem.Name, MasterServer.Core.LogGroup.ProduceType.GameInterface);
			return true;
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x0003FFCE File Offset: 0x0003E3CE
		public bool UnlockAllItems(ulong profileId)
		{
			this.m_dalService.ItemSystem.DebugUnlockAllItems(profileId);
			return true;
		}

		// Token: 0x06001049 RID: 4169 RVA: 0x0003FFE2 File Offset: 0x0003E3E2
		public bool BanPlayer(ulong profileId, TimeSpan banTime, string message)
		{
			this.m_punishmentService.BanPlayer(profileId, banTime, message, BanReportSource.GI);
			return true;
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x0003FFF4 File Offset: 0x0003E3F4
		public bool CancelBanPlayer(ulong profileId)
		{
			this.m_punishmentService.UnBanPlayer(profileId);
			return true;
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x00040003 File Offset: 0x0003E403
		public bool MutePlayer(ulong profileId, TimeSpan muteTime)
		{
			this.m_punishmentService.MutePlayer(profileId, muteTime);
			return true;
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x00040013 File Offset: 0x0003E413
		public bool CancelMutePlayer(ulong profileId)
		{
			this.m_punishmentService.UnMute(profileId);
			return true;
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x00040022 File Offset: 0x0003E422
		public bool KickPlayer(ulong profileId)
		{
			this.m_punishmentService.KickPlayer(profileId);
			return true;
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x00040031 File Offset: 0x0003E431
		public bool ForceLogout(ulong profileId)
		{
			this.m_punishmentService.ForceLogout(profileId);
			return true;
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x00040040 File Offset: 0x0003E440
		public bool SendPlainTextNotification(ulong profileId, string notification, TimeSpan expiration)
		{
			this.m_notificationService.AddNotification<string>(profileId, ENotificationType.Message, notification, expiration, EDeliveryType.SendNowOrLater, EConfirmationType.None);
			return true;
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x00040058 File Offset: 0x0003E458
		public string GetAnnouncements()
		{
			IEnumerable<Announcement> announcements = this.m_announcementService.GetAnnouncements();
			return Utils.SerializeCollectionToString<Announcement>(announcements);
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x00040078 File Offset: 0x0003E478
		public string GetActiveAnnouncements()
		{
			IEnumerable<Announcement> announcementsToSend = this.m_announcementService.GetAnnouncementsToSend();
			return Utils.SerializeCollectionToString<Announcement>(announcementsToSend);
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x00040098 File Offset: 0x0003E498
		public Announcement GetAnnouncement(ulong id)
		{
			Announcement announcement;
			return (!this.m_announcementService.GetAnnouncement(id, out announcement)) ? null : announcement;
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x000400BF File Offset: 0x0003E4BF
		public bool AddAnnouncement(Announcement announcement)
		{
			this.m_announcementService.Add(announcement);
			return true;
		}

		// Token: 0x06001054 RID: 4180 RVA: 0x000400CE File Offset: 0x0003E4CE
		public bool ModifyAnnouncement(Announcement announcement)
		{
			this.m_announcementService.ModifyAnnouncement(announcement);
			return true;
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x000400DD File Offset: 0x0003E4DD
		public bool DeleteAnnouncement(ulong id)
		{
			return this.m_announcementService.Remove(id);
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x000400EC File Offset: 0x0003E4EC
		public ulong GetUserID(ulong profileId)
		{
			return this.m_dalService.ProfileSystem.GetProfileInfo(profileId).UserID;
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x00040112 File Offset: 0x0003E512
		public ulong GetProfileID(ulong userId)
		{
			return (from sp in this.m_dalService.ProfileSystem.GetUserProfiles(userId)
			select sp.ProfileID).FirstOrDefault<ulong>();
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x0004014C File Offset: 0x0003E54C
		public ulong GetProfileIDByNickname(string nick)
		{
			return this.m_dalService.ProfileSystem.GetProfileIDByNickname(nick);
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x00040160 File Offset: 0x0003E560
		private ProfileInfo GetOnlineProfileInfo(ulong userId)
		{
			List<SAuthProfile> source = this.m_dalService.ProfileSystem.GetUserProfiles(userId).ToList<SAuthProfile>();
			return (!source.Any<SAuthProfile>()) ? default(ProfileInfo) : this.m_sessionInfoService.GetProfileInfo(source.First<SAuthProfile>().Nickname);
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x000401B8 File Offset: 0x0003E5B8
		public string GetDefaultItems()
		{
			Dictionary<ulong, SItem> allItems = this.m_itemCacheService.GetAllItems();
			Dictionary<ulong, SEquipItem> defaultProfileItems = this.m_itemCacheService.GetDefaultProfileItems();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SEquipItem sequipItem in defaultProfileItems.Values)
			{
				SItem sitem = allItems[sequipItem.ItemID];
				stringBuilder.AppendLine(sitem.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x00040250 File Offset: 0x0003E650
		public string GetProfileItems(ulong profileId)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			if (profileInfo.Id == 0UL)
			{
				return string.Format("Profile with Id {0} doesn't exist.", profileId);
			}
			IEnumerable<SProfileItem> source;
			if (this.m_dalService.ColdStorageSystem.IsProfileCold(profileId).ValueOrDefault<bool>())
			{
				ColdProfileData coldProfileData = this.m_dalService.ColdStorageSystem.GetColdProfileData(profileId, Resources.LatestDbUpdateVersion);
				List<SEquipItem> equipItems = coldProfileData.EquipItems;
				Dictionary<ulong, CustomerItem> customerItems = this.m_catalogService.GetCustomerItems(profileInfo.UserID);
				foreach (KeyValuePair<ulong, CustomerItem> keyValuePair in this.m_catalogService.GetInactiveCustomerItems(profileInfo.UserID))
				{
					customerItems.Add(keyValuePair.Key, keyValuePair.Value);
				}
				source = equipItems.Select(delegate(SEquipItem ei)
				{
					CustomerItem customerItem = null;
					if (ei.CatalogID > 0UL)
					{
						customerItems.TryGetValue(ei.CatalogID, out customerItem);
					}
					return this.m_profileItemService.BuildProfileItem(ei, customerItem);
				});
			}
			else
			{
				source = this.m_profileItemService.GetProfileItems(profileId, EquipOptions.All).Values;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SProfileItem sprofileItem in from pi in source
			orderby pi.GameItem.Active descending
			select pi)
			{
				if (!sprofileItem.IsDefault && (!sprofileItem.IsReward || !sprofileItem.GameItem.IsAttachmentItem))
				{
					stringBuilder.AppendLine(sprofileItem.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x00040438 File Offset: 0x0003E838
		public string GetProfileUnlockedItems(ulong profileId)
		{
			if (this.m_dalService.ProfileSystem.GetProfileInfo(profileId).Id == 0UL)
			{
				return string.Format("Profile with Id {0} doesn't exist.", profileId);
			}
			IEnumerable<SItem> collection;
			if (this.m_dalService.ColdStorageSystem.IsProfileCold(profileId).ValueOrDefault<bool>())
			{
				Dictionary<ulong, SItem> allItems = this.m_itemCacheService.GetAllItems();
				ColdProfileData coldProfileData = this.m_dalService.ColdStorageSystem.GetColdProfileData(profileId, Resources.LatestDbUpdateVersion);
				List<ulong> unlockItems = coldProfileData.UnlockItems;
				List<SItem> list = new List<SItem>();
				foreach (ulong key in unlockItems)
				{
					SItem item;
					if (allItems.TryGetValue(key, out item))
					{
						list.Add(item);
					}
				}
				collection = list;
			}
			else
			{
				collection = this.m_profileItemService.GetUnlockedItems(profileId).Values;
			}
			return Utils.SerializeCollectionToString<SItem>(collection);
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x00040544 File Offset: 0x0003E944
		public string GetProfileAchievements(ulong profileId)
		{
			if (this.m_dalService.ProfileSystem.GetProfileInfo(profileId).Id == 0UL)
			{
				return string.Format("Profile with Id {0} doesn't exist.", profileId);
			}
			IEnumerable<AchievementInfo> collection;
			if (this.m_dalService.ColdStorageSystem.IsProfileCold(profileId).ValueOrDefault<bool>())
			{
				ColdProfileData coldProfileData = this.m_dalService.ColdStorageSystem.GetColdProfileData(profileId, Resources.LatestDbUpdateVersion);
				collection = coldProfileData.Achievements;
			}
			else
			{
				collection = this.m_dalService.AchievementSystem.GetProfileAchievements(profileId);
			}
			return Utils.SerializeCollectionToString<AchievementInfo>(collection);
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x000405D8 File Offset: 0x0003E9D8
		public string GetProfileSponsorPoints(ulong profileId)
		{
			if (this.m_dalService.ProfileSystem.GetProfileInfo(profileId).Id == 0UL)
			{
				return string.Format("Profile with Id {0} doesn't exist.", profileId);
			}
			IEnumerable<SSponsorPoints> sponsorPoints;
			if (this.m_dalService.ColdStorageSystem.IsProfileCold(profileId).ValueOrDefault<bool>())
			{
				ColdProfileData coldProfileData = this.m_dalService.ColdStorageSystem.GetColdProfileData(profileId, Resources.LatestDbUpdateVersion);
				sponsorPoints = coldProfileData.SponsorPoints;
			}
			else
			{
				sponsorPoints = this.m_dalService.RewardsSystem.GetSponsorPoints(profileId);
			}
			return Utils.SerializeCollectionToString<SSponsorPoints>(sponsorPoints);
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x0004066C File Offset: 0x0003EA6C
		public string GetAllAchievements()
		{
			Dictionary<uint, AchievementDescription> allAchievementDescs = this.m_achievementSystem.GetAllAchievementDescs();
			return Utils.SerializeCollectionToString<AchievementDescription>(allAchievementDescs.Values);
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x00040690 File Offset: 0x0003EA90
		public string GetProfilePersistentSettings(ulong profileId)
		{
			if (this.m_dalService.ProfileSystem.GetProfileInfo(profileId).Id == 0UL)
			{
				return string.Format("Profile with Id {0} doesn't exist.", profileId);
			}
			IEnumerable<SPersistentSettings> persistentSettings;
			if (this.m_dalService.ColdStorageSystem.IsProfileCold(profileId).ValueOrDefault<bool>())
			{
				ColdProfileData coldProfileData = this.m_dalService.ColdStorageSystem.GetColdProfileData(profileId, Resources.LatestDbUpdateVersion);
				persistentSettings = coldProfileData.PersistentSettings;
			}
			else
			{
				persistentSettings = this.m_dalService.ProfileSystem.GetPersistentSettings(profileId);
			}
			return Utils.SerializeCollectionToString<SPersistentSettings>(persistentSettings);
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x00040724 File Offset: 0x0003EB24
		public string GetProfileContract(ulong profileId)
		{
			if (this.m_dalService.ProfileSystem.GetProfileInfo(profileId).Id == 0UL)
			{
				return string.Format("Profile with Id {0} doesn't exist.", profileId);
			}
			ProfileContract profileContract;
			if (this.m_dalService.ColdStorageSystem.IsProfileCold(profileId).ValueOrDefault<bool>())
			{
				ColdProfileData coldProfileData = this.m_dalService.ColdStorageSystem.GetColdProfileData(profileId, Resources.LatestDbUpdateVersion);
				profileContract = coldProfileData.ProfileContract;
			}
			else
			{
				profileContract = this.m_dalService.ContractSystem.GetContractInfo(profileId);
			}
			return (profileContract == null) ? "No contract info" : profileContract.ToString();
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x000407C8 File Offset: 0x0003EBC8
		public ClanInfo GetProfileClan(ulong profileId)
		{
			return this.m_clanService.GetClanInfoByPid(profileId);
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x000407D8 File Offset: 0x0003EBD8
		public bool SetAccessLevel(SUserAccessLevel level)
		{
			UserInfo.User userByUserId = this.m_userRepository.GetUserByUserId(level.user_id);
			bool flag = this.m_authService.SetAccessLevel(level);
			if (flag && userByUserId != null && level.CheckPrivileges(userByUserId.IP))
			{
				UserInfo.User userInfo = userByUserId.CloneWithAccessLevel(level.accessLevel);
				this.m_userRepository.SetUserInfo(userInfo);
			}
			return flag;
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x0004083E File Offset: 0x0003EC3E
		public List<SUserAccessLevel> GetAccessLevel(ulong userId)
		{
			return this.m_authService.GetAccessLevel(userId);
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x0004084C File Offset: 0x0003EC4C
		public List<SUserAccessLevel> GetAccessLevel()
		{
			return this.m_authService.GetAccessLevel();
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x0004085C File Offset: 0x0003EC5C
		public bool RemoveAccessLevel(ulong id, ulong user_id)
		{
			bool flag = this.m_authService.RemoveAccessLevel(id, user_id);
			UserInfo.User userByUserId = this.m_userRepository.GetUserByUserId(user_id);
			if (flag && userByUserId != null)
			{
				List<SUserAccessLevel> accessLevel = this.m_authService.GetAccessLevel(user_id);
				UserInfo.User userInfo = userByUserId.CloneWithAccessLevel(SUserAccessLevel.GetUserAccessLevel(user_id, userByUserId.IP, accessLevel).accessLevel);
				this.m_userRepository.SetUserInfo(userInfo);
			}
			return flag;
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x000408C8 File Offset: 0x0003ECC8
		public ClanInfo GetClanInfo(ulong clanId)
		{
			return this.m_clanService.GetClanInfo(clanId);
		}

		// Token: 0x06001068 RID: 4200 RVA: 0x000408D6 File Offset: 0x0003ECD6
		public ClanInfo GetClanInfoByName(string clanName)
		{
			return this.m_clanService.GetClanInfoByName(clanName);
		}

		// Token: 0x06001069 RID: 4201 RVA: 0x000408E4 File Offset: 0x0003ECE4
		public string GetClanDesc(ulong clanId)
		{
			ClanInfo clanInfo = this.m_clanService.GetClanInfo(clanId);
			if (clanInfo == null)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(Convert.FromBase64String(clanInfo.Description));
		}

		// Token: 0x0600106A RID: 4202 RVA: 0x0004091F File Offset: 0x0003ED1F
		public IEnumerable<ClanMember> GetClanMembers(ulong clanId)
		{
			return this.m_clanService.GetClanMembers(clanId);
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x0004092D File Offset: 0x0003ED2D
		public bool RemoveClanMember(ulong profileId)
		{
			this.m_clanService.LeaveClan(profileId);
			return true;
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x0004093D File Offset: 0x0003ED3D
		public int AddClanMember(ulong clanId, ulong profileId)
		{
			return (int)this.m_clanService.AddClanMember(clanId, profileId);
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x0004094C File Offset: 0x0003ED4C
		public bool SetClanRole(ulong initiatorId, ulong targetId, EClanRole role)
		{
			return this.m_clanService.SetClanRole(initiatorId, targetId, role);
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x0004095C File Offset: 0x0003ED5C
		public int CreateClan(ulong profileId, string clanName, string desc)
		{
			ulong num = 0UL;
			return (int)this.m_clanService.CreateClanWithoutItem(profileId, ref num, clanName, Convert.ToBase64String(Encoding.UTF8.GetBytes(desc)));
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x0004098B File Offset: 0x0003ED8B
		public bool RemoveClan(ulong initiatorId)
		{
			return this.m_clanService.RemoveClan(initiatorId);
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x00040999 File Offset: 0x0003ED99
		public IEnumerable<SAbuseHistory> GetAbuseReportsByDate(DateTime startTime, DateTime endTime, sbyte reportSource, uint count)
		{
			return this.m_dalService.AbuseSystem.GetAbuseHistoryByDate(TimeUtils.LocalTimeToUTCTimestamp(startTime), TimeUtils.LocalTimeToUTCTimestamp(endTime), reportSource, count);
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x000409BA File Offset: 0x0003EDBA
		public uint GetAbusesCount(DateTime startTime, DateTime endTime)
		{
			return this.m_dalService.AbuseSystem.GetAbusesCount(TimeUtils.LocalTimeToUTCTimestamp(startTime), TimeUtils.LocalTimeToUTCTimestamp(endTime));
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x000409D8 File Offset: 0x0003EDD8
		public IEnumerable<SAbuseHistory> GetAbuseReportsFromUser(ulong profile_id)
		{
			return this.m_dalService.AbuseSystem.GetAbuseHistoryFromUser(profile_id);
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x000409EB File Offset: 0x0003EDEB
		public IEnumerable<SAbuseHistory> GetAbuseReportsToUser(ulong profile_id)
		{
			return this.m_dalService.AbuseSystem.GetAbuseHistoryToUser(profile_id);
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x000409FE File Offset: 0x0003EDFE
		public IEnumerable<SAbuseTopReport> GetTopAbuseReports(uint count)
		{
			return this.m_dalService.AbuseSystem.GetTopAbusers(count);
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x00040A11 File Offset: 0x0003EE11
		public string MakeRemoteScreenShot(ulong profileId, bool frontBuffer, int count, float scaleW, float scaleH)
		{
			return this.m_punishmentService.MakeScreenShot(profileId, frontBuffer, count, scaleW, scaleH);
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x00040A28 File Offset: 0x0003EE28
		public string GetPlayerStat(ulong profileId)
		{
			if (this.m_dalService.ProfileSystem.GetProfileInfo(profileId).Id == 0UL)
			{
				return string.Format("Profile with Id {0} doesn't exist.", profileId);
			}
			List<Measure> list = new List<Measure>();
			if (this.m_dalService.ColdStorageSystem.IsProfileCold(profileId).ValueOrDefault<bool>())
			{
				ColdProfileData coldProfileData = this.m_dalService.ColdStorageSystem.GetColdProfileData(profileId, Resources.LatestDbUpdateVersion);
				if (coldProfileData.PlayerStatistics != null)
				{
					list = coldProfileData.PlayerStatistics.Measures;
				}
			}
			else
			{
				list = this.m_playerStatsService.GetPlayerStats(profileId);
			}
			list.Sort((Measure a, Measure b) => a.Dimensions["stat"].CompareTo(b.Dimensions["stat"]));
			return Utils.SerializeCollectionToString<Measure>(list);
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x00040AF0 File Offset: 0x0003EEF0
		public string GetPlayerStatFromTelem(ulong profileId)
		{
			List<Measure> list = this.m_telemetryDalService.TelemetrySystem.GetPlayerStats(profileId).ToList<Measure>();
			list.Sort((Measure a, Measure b) => a.Dimensions["stat"].CompareTo(b.Dimensions["stat"]));
			return Utils.SerializeCollectionToString<Measure>(list);
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x00040B40 File Offset: 0x0003EF40
		public void ResetProfile(ulong userId, bool full)
		{
			ulong profileID = this.GetProfileID(userId);
			this.m_dalService.ItemSystem.DebugResetProfileItems(profileID);
			this.m_debugCatalogService.DebugResetCustomerItems(userId);
			if (full)
			{
				MasterServer.Core.Log.Info<ulong, ulong>("Reseting progress and money for profile {0}, user {1}", profileID, userId);
				foreach (Currency type in Enum.GetValues(typeof(Currency)).Cast<Currency>())
				{
					try
					{
						this.m_catalogService.SetMoney(userId, type, 0UL);
					}
					catch (PaymentServiceException e)
					{
						MasterServer.Core.Log.Error(e);
					}
				}
				if (this.m_clanService.GetMemberInfo(profileID) != null)
				{
					this.m_clanService.LeaveClan(profileID);
				}
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileID);
				this.m_dalService.ProfileSystem.DeleteProfile(userId, profileID, profileInfo.Nickname);
				this.LogGroup.CharacterDeletionLog(userId, profileID, profileInfo.Nickname);
			}
			else
			{
				this.m_dalService.ProfileSystem.ClearPersistentSettings(profileID, "wcs");
			}
		}

		// Token: 0x06001079 RID: 4217 RVA: 0x00040C7C File Offset: 0x0003F07C
		public void UnlockTutorial(ulong profileId, string tutorial, bool silent, string ev)
		{
			ProfileProgressionInfo.Tutorial tutorial2 = Utils.ParseEnum<ProfileProgressionInfo.Tutorial>(tutorial);
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			ProfileProgressionInfo progression = (user == null) ? this.m_profileProgressionService.GetProgression(profileId) : user.ProfileProgression;
			ProfileProgressionInfo profileProgressionInfo = this.m_profileProgressionService.UnlockTutorial(progression, tutorial2, silent, this.LogGroup);
			if (!string.IsNullOrEmpty(ev) && profileProgressionInfo.IsTutorialUnlocked(tutorial2))
			{
				this.m_specialRewardsService.ProcessEvent(ev, profileId, this.LogGroup);
			}
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x00040D00 File Offset: 0x0003F100
		public void UnlockClass(ulong profileId, string sClass, bool silent, string ev)
		{
			ProfileProgressionInfo.PlayerClass playerClass = Utils.ParseEnum<ProfileProgressionInfo.PlayerClass>(sClass);
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			ProfileProgressionInfo progression = (user == null) ? this.m_profileProgressionService.GetProgression(profileId) : user.ProfileProgression;
			ProfileProgressionInfo profileProgressionInfo = this.m_profileProgressionService.UnlockClass(progression, playerClass, silent, this.LogGroup);
			if (!string.IsNullOrEmpty(ev) && profileProgressionInfo.IsClassUnlocked(playerClass))
			{
				this.m_specialRewardsService.ProcessEvent(ev, profileId, this.LogGroup);
			}
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x00040D84 File Offset: 0x0003F184
		public IEnumerable<string> UnlockMission(ulong profileId, string mission, bool silent)
		{
			ProfileProgressionInfo.MissionType unlockedMissionType = Utils.ParseEnum<ProfileProgressionInfo.MissionType>(mission);
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			ProfileProgressionInfo progression = (user == null) ? this.m_profileProgressionService.GetProgression(profileId) : user.ProfileProgression;
			ProfileProgressionInfo profileProgressionInfo = this.m_profileProgressionService.UnlockMission(progression, unlockedMissionType, silent, this.LogGroup);
			IEnumerable<ProfileProgressionInfo.MissionType> unlockedMissions = profileProgressionInfo.GetUnlockedMissions();
			return from x in unlockedMissions
			select x.ToString();
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x00040E04 File Offset: 0x0003F204
		public void UnlockAchievement(ulong profileId, uint achievementId)
		{
			AchievementDescription achievementDesc = this.m_achievementSystem.GetAchievementDesc(achievementId);
			AchievementUpdateChunk achievementUpdateChunk = new AchievementUpdateChunk(achievementDesc.Id, (int)achievementDesc.Amount, TimeUtils.LocalTimeToUTCTimestamp(DateTime.UtcNow));
			this.m_achievementSystem.SetAchievementProgress(profileId, achievementDesc, ref achievementUpdateChunk);
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x00040E4B File Offset: 0x0003F24B
		public AchievementLockStatus LockHiddenAchievement(ulong profileId, uint achievementId)
		{
			return this.m_achievementSystem.DeleteProfileHiddenAchievement(profileId, achievementId);
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x00040E5C File Offset: 0x0003F25C
		private string ProgressionEnumToStr<T>(Predicate<T> pred)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Array values = Enum.GetValues(typeof(T));
			IEnumerator enumerator = values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					T t = (T)((object)obj);
					if (pred(t))
					{
						stringBuilder.AppendLine(string.Format("\t{0}", t));
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
			return stringBuilder.ToString();
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x00040EF8 File Offset: 0x0003F2F8
		private string FormatProgressionData<T>(string header, Predicate<T> pred)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("------------------------------------------");
			stringBuilder.AppendLine(header);
			stringBuilder.AppendLine(this.ProgressionEnumToStr<T>(pred));
			return stringBuilder.ToString();
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x00040F34 File Offset: 0x0003F334
		public string GetProfileProgression(ulong profileId)
		{
			ProfileProgressionInfo progression;
			if (this.m_dalService.ColdStorageSystem.IsProfileCold(profileId).ValueOrDefault<bool>())
			{
				ColdProfileData coldProfileData = this.m_dalService.ColdStorageSystem.GetColdProfileData(profileId, Resources.LatestDbUpdateVersion);
				progression = new ProfileProgressionInfo(coldProfileData.ProfileProgression, this.m_dalService, this.m_profileProgressionService);
			}
			else
			{
				progression = this.m_profileProgressionService.GetProgression(profileId);
			}
			StringBuilder stringBuilder = new StringBuilder();
			Predicate<ProfileProgressionInfo.Tutorial> pred = (ProfileProgressionInfo.Tutorial tut) => progression.IsTutorialUnlocked(tut);
			stringBuilder.AppendLine(this.FormatProgressionData<ProfileProgressionInfo.Tutorial>("UnlockedTutorials:", pred));
			Predicate<ProfileProgressionInfo.Tutorial> pred2 = (ProfileProgressionInfo.Tutorial tut) => progression.IsTutorialPassed(tut);
			stringBuilder.AppendLine(this.FormatProgressionData<ProfileProgressionInfo.Tutorial>("PassedTutorials:", pred2));
			Predicate<ProfileProgressionInfo.PlayerClass> pred3 = (ProfileProgressionInfo.PlayerClass playersClass) => progression.IsClassUnlocked(playersClass);
			stringBuilder.AppendLine(this.FormatProgressionData<ProfileProgressionInfo.PlayerClass>("UnlockedClasses:", pred3));
			stringBuilder.AppendLine("------------------------------------------");
			stringBuilder.AppendLine("Unlocked Missions:");
			stringBuilder.AppendLine(string.Join("\n\t", (from x in progression.GetUnlockedMissions()
			select x.ToString()).ToArray<string>()));
			return stringBuilder.ToString();
		}

		// Token: 0x06001081 RID: 4225 RVA: 0x00041078 File Offset: 0x0003F478
		public string GetUserTags(ulong userId)
		{
			UserTags userTags = this.m_tagService.GetUserTags(userId);
			return string.Format("user tags: {0};", userTags.ToString());
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x000410A2 File Offset: 0x0003F4A2
		public void SetUserTags(ulong userId, string tags)
		{
			this.m_tagService.SetUserTags(userId, new UserTags(tags));
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x000410B6 File Offset: 0x0003F4B6
		public void RemoveUserTags(ulong userId)
		{
			this.m_tagService.RemoveUserTags(userId);
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x000410C4 File Offset: 0x0003F4C4
		private string SerializeCustomRules(IEnumerable<CustomRuleInfo> rules)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CustomRuleInfo customRuleInfo in rules)
			{
				stringBuilder.AppendFormat("{0} {1} {2}: {3}\n", new object[]
				{
					customRuleInfo.RuleID,
					customRuleInfo.Source,
					(!customRuleInfo.Enabled) ? "OFF" : "ON",
					customRuleInfo.Data
				});
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x00041174 File Offset: 0x0003F574
		public string CustomRuleList()
		{
			IEnumerable<CustomRuleInfo> rules = this.m_dalService.CustomRulesSystem.GetRules();
			return this.SerializeCustomRules(rules);
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x0004119C File Offset: 0x0003F59C
		public ulong CustomRuleAdd(string config)
		{
			ICustomRule customRule = this.m_customRulesService.AddRule(config, false);
			return customRule.RuleID;
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x000411C0 File Offset: 0x0003F5C0
		public string CustomRuleEnable(IEnumerable<ulong> rules, bool enable, bool cleanUp)
		{
			rules.SafeForEach(delegate(ulong r)
			{
				this.m_customRulesService.EnableRule(r, enable);
				if (cleanUp)
				{
					this.m_customRulesService.CleanRuleState(r);
				}
			});
			return this.CustomRuleList();
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x00041200 File Offset: 0x0003F600
		public string CustomRuleRemove(ulong ruleId)
		{
			this.m_customRulesService.RemoveRule(ruleId);
			return this.CustomRuleList();
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x00041214 File Offset: 0x0003F614
		public string RecoverVouchers()
		{
			if (Resources.RealmDBUpdaterPermission)
			{
				this.m_jobSchedulerService.AddJob("corrupted_voucher_synchronization");
			}
			return (!Resources.RealmDBUpdaterPermission) ? "This command should be executed on realm db-updater" : string.Format("Job {0} added", "corrupted_voucher_synchronization");
		}

		// Token: 0x0600108A RID: 4234 RVA: 0x00041253 File Offset: 0x0003F653
		public string ReloadOffers()
		{
			return (!this.m_catalogService.ReloadOffers()) ? "This command should be executed on realm db-updater" : "Offers was reloaded successfully";
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x00041274 File Offset: 0x0003F674
		public void LoadOffers()
		{
			this.m_shopService.LoadOffers();
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x00041281 File Offset: 0x0003F681
		public void FlushUserProfile(ulong userId)
		{
			this.m_dalService.ProfileSystem.FlushProfileCache(userId, this.GetProfileID(userId));
		}

		// Token: 0x0600108D RID: 4237 RVA: 0x0004129C File Offset: 0x0003F69C
		public ulong AddExp(ulong profileId, long amount, LevelChangeReason reason)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			ulong num = this.m_rankSystem.AddExperience(profileId, (ulong)amount, reason, this.LogGroup);
			return num + profileInfo.RankInfo.Points;
		}

		// Token: 0x0600108E RID: 4238 RVA: 0x000412E0 File Offset: 0x0003F6E0
		public Dictionary<Currency, ulong> GetMoney(ulong profileId)
		{
			List<CustomerAccount> customerAccounts = this.m_catalogService.GetCustomerAccounts(this.GetUserID(profileId));
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
			foreach (CustomerAccount customerAccount in customerAccounts)
			{
				dictionary[customerAccount.Currency] = customerAccount.Money;
			}
			return dictionary;
		}

		// Token: 0x0600108F RID: 4239 RVA: 0x0004137C File Offset: 0x0003F77C
		public SProfileInfo GetProfileInfo(ulong profileId)
		{
			return this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x00041390 File Offset: 0x0003F790
		public SProfileInfoEx GetProfileInfoEx(ulong profileId)
		{
			SProfileInfoEx result = default(SProfileInfoEx);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			ProfileInfo profileInfo2 = this.m_sessionInfoService.GetProfileInfo(profileInfo.Nickname);
			List<CustomerAccount> customerAccounts = this.m_catalogService.GetCustomerAccounts(profileInfo.UserID);
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
			foreach (CustomerAccount customerAccount in customerAccounts)
			{
				dictionary[customerAccount.Currency] = customerAccount.Money;
			}
			IEnumerable<SSponsorPoints> sponsorPoints = this.m_dalService.RewardsSystem.GetSponsorPoints(profileId);
			Dictionary<uint, SSponsorPoints> dictionary2 = new Dictionary<uint, SSponsorPoints>
			{
				{
					0U,
					default(SSponsorPoints)
				},
				{
					1U,
					default(SSponsorPoints)
				},
				{
					2U,
					default(SSponsorPoints)
				}
			};
			foreach (SSponsorPoints value in sponsorPoints)
			{
				dictionary2[value.SponsorID] = value;
			}
			PunishmentStatus punishmentStatus = PunishmentStatus.NONE;
			if (profileInfo.MuteTime > DateTime.Now)
			{
				punishmentStatus |= PunishmentStatus.MUTED;
			}
			if (profileInfo.BanTime > DateTime.Now)
			{
				punishmentStatus |= PunishmentStatus.BANNED;
			}
			result.UserId = profileInfo.UserID;
			result.ProfileId = profileId;
			result.Nickname = profileInfo.Nickname;
			result.Gender = profileInfo.Gender;
			result.GameMoney = dictionary[Currency.GameMoney];
			result.CrownMoney = dictionary[Currency.CrownMoney];
			result.CryMoney = dictionary[Currency.CryMoney];
			result.Experience = profileInfo.RankInfo.Points;
			result.RankId = profileInfo.RankInfo.RankId;
			result.SponsorPoints = (from point in dictionary2.Values
			select point.RankInfo.Points).ToList<ulong>();
			result.SponsorStages = (from stage in dictionary2.Values
			select stage.RankInfo.RankId).ToList<int>();
			result.CreationTime = profileInfo.CreateTime;
			result.LastSeenTime = profileInfo.LastSeenTime;
			result.IsOnline = (profileInfo2.Status != UserStatus.Offline);
			result.PunishmentStatus = punishmentStatus;
			result.BanEndTime = profileInfo.BanTime;
			result.MuteEndTime = profileInfo.MuteTime;
			ClanInfo clanInfoByPid = this.m_clanService.GetClanInfoByPid(profileId);
			if (clanInfoByPid != null)
			{
				result.ClanId = clanInfoByPid.ClanID;
				result.ClanName = clanInfoByPid.Name;
			}
			return result;
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x000416B8 File Offset: 0x0003FAB8
		public Dictionary<ulong, SItem> GetAllItems()
		{
			IItemCache service = ServicesManager.GetService<IItemCache>();
			return service.GetAllItems();
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x000416D1 File Offset: 0x0003FAD1
		public IEnumerable<SFriend> GetFriends(ulong profileId)
		{
			return this.m_dalService.ProfileSystem.GetFriends(profileId);
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x000416E4 File Offset: 0x0003FAE4
		public bool SetObserver(ulong profileId, bool enable)
		{
			return this.m_gameRoomManager.SetObserver(profileId, enable);
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x000416F3 File Offset: 0x0003FAF3
		public string CreateRoom(string masterId, RoomReference roomRef, CreateRoomParam param)
		{
			return this.m_manualRoomService.CreateRoom(masterId, roomRef, param);
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x00041703 File Offset: 0x0003FB03
		public string GetRoomInfo(string masterId, RoomReference roomRef)
		{
			return this.m_manualRoomService.GetRoomInfo(masterId, roomRef);
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x00041712 File Offset: 0x0003FB12
		public string AddPlayerToRoom(string masterId, RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> playersInfos)
		{
			return this.m_manualRoomService.AddPlayer(masterId, roomRef, playersInfos);
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x00041722 File Offset: 0x0003FB22
		public string RemovePlayerFromRoom(string masterId, RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> playersInfos)
		{
			return this.m_manualRoomService.RemovePlayer(masterId, roomRef, playersInfos);
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x00041732 File Offset: 0x0003FB32
		public string StartRoom(string masterId, RoomReference roomRef, int team1Score, int team2Score)
		{
			return this.m_manualRoomService.StartSession(masterId, roomRef, team1Score, team2Score);
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x00041744 File Offset: 0x0003FB44
		public string PauseGameSession(string masterId, RoomReference roomRef)
		{
			return this.m_manualRoomService.PauseSession(masterId, roomRef);
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x00041753 File Offset: 0x0003FB53
		public string ResumeGameSession(string masterId, RoomReference roomRef)
		{
			return this.m_manualRoomService.ResumeSession(masterId, roomRef);
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x00041762 File Offset: 0x0003FB62
		public string StopGameSession(string masterId, RoomReference roomRef)
		{
			return this.m_manualRoomService.StopSession(masterId, roomRef);
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x00041774 File Offset: 0x0003FB74
		public string AddFriend(ulong senderProfileID, ulong targetProfileID)
		{
			Task<EInviteStatus> task = this.m_friendsService.Invite(senderProfileID, targetProfileID);
			return task.ToString();
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x00041795 File Offset: 0x0003FB95
		public void RemoveFriend(ulong senderProfileID, ulong targetProfileID)
		{
			this.m_friendsService.RemoveFriend(senderProfileID, targetProfileID);
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x000417A4 File Offset: 0x0003FBA4
		public string GetPendingFriends(ulong profileID)
		{
			INotificationSerializer notificationSerializer = this.m_notificationService.GetNotificationSerializer(ENotificationType.FriendInvite);
			IEnumerable<SNotification> pendingByType = this.m_notificationService.GetPendingByType(profileID, ENotificationType.FriendInvite);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SNotification snotification in pendingByType)
			{
				SInvitationFriendData sinvitationFriendData = (SInvitationFriendData)notificationSerializer.Deserialize(snotification.Data);
				stringBuilder.AppendFormat("{0} {1} {2}\n", sinvitationFriendData.Initiator.ProfileId, sinvitationFriendData.Initiator.Nickname, snotification.ID);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x00041868 File Offset: 0x0003FC68
		public void RespondToInvitation(ulong profileID, ulong notifID, bool accept)
		{
			XmlElement xmlElement = new XmlDocument().CreateElement("notification");
			xmlElement.SetAttribute("result", ((!accept) ? 1 : 0).ToString());
			xmlElement.SetAttribute("location", string.Empty);
			this.m_notificationService.Confirm(profileID, notifID, xmlElement);
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x000418CC File Offset: 0x0003FCCC
		public string InviteClanMember(ulong senderProfileID, ulong targetProfileID)
		{
			Task<EInviteStatus> task = this.m_clanService.Invite(senderProfileID, targetProfileID);
			return task.ToString();
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x000418ED File Offset: 0x0003FCED
		public bool LeaveClan(ulong profileID)
		{
			return this.m_clanService.LeaveClan(profileID);
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x000418FC File Offset: 0x0003FCFC
		public string GetPendingClans(ulong profileID)
		{
			INotificationSerializer notificationSerializer = this.m_notificationService.GetNotificationSerializer(ENotificationType.ClanInvite);
			IEnumerable<SNotification> pendingByType = this.m_notificationService.GetPendingByType(profileID, ENotificationType.ClanInvite);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SNotification snotification in pendingByType)
			{
				SInvitationClanData sinvitationClanData = (SInvitationClanData)notificationSerializer.Deserialize(snotification.Data);
				stringBuilder.AppendFormat("{0} {1} {2} {3}\n", new object[]
				{
					sinvitationClanData.Initiator.ProfileId,
					sinvitationClanData.Initiator.Nickname,
					snotification.ID,
					sinvitationClanData.clan_id
				});
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x000419DC File Offset: 0x0003FDDC
		public void SetSupportedClientVersions(params string[] versions)
		{
			this.m_clientVersionsManagementService.SetClientVersions(versions);
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x000419EA File Offset: 0x0003FDEA
		public void AddSupportedClientVersions(params string[] versions)
		{
			this.m_clientVersionsManagementService.AddClientVersions(versions);
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x000419F8 File Offset: 0x0003FDF8
		public void RemoveSupportedClientVersions(params string[] versions)
		{
			this.m_clientVersionsManagementService.RemoveClientVersions(versions);
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x00041A06 File Offset: 0x0003FE06
		public IEnumerable<string> GetSupportedClientVersions()
		{
			return this.m_clientVersionsManagementService.GetClientVersions();
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x00041A13 File Offset: 0x0003FE13
		public void BroadcastClientVersionsReload()
		{
			this.m_queryManager.Request("master_server_bcast", this.m_onlineClient.TargetRoute, new object[]
			{
				"reload_client_versions"
			});
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x00041A40 File Offset: 0x0003FE40
		public void BlockPurchase(ulong userId, string notification)
		{
			this.m_tagService.AddUserTags(userId, TagService.BlockPurchaseTag);
			ulong profileID = this.GetProfileID(userId);
			this.LogGroup.BlockPurchase(userId, profileID);
			if (!string.IsNullOrEmpty(notification))
			{
				this.SendPlainTextNotification(profileID, notification, TimeSpan.FromDays(1.0));
			}
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x00041A98 File Offset: 0x0003FE98
		public void UnblockPurchase(ulong userId, string notification)
		{
			this.m_tagService.RemoveUserTags(userId, TagService.BlockPurchaseTag);
			ulong profileID = this.GetProfileID(userId);
			this.LogGroup.UnblockPurchase(userId, profileID);
			if (!string.IsNullOrEmpty(notification))
			{
				this.SendPlainTextNotification(profileID, notification, TimeSpan.FromDays(1.0));
			}
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x00041AED File Offset: 0x0003FEED
		public string TestAccessLevelAdmin()
		{
			return "Admin access level enabled";
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x00041AF4 File Offset: 0x0003FEF4
		public string TestAccessLevelBasic()
		{
			return "Basic access level enabled";
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x00041AFB File Offset: 0x0003FEFB
		public string TestAccessLevelModerator()
		{
			return "Moderator access level enabled";
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x00041B02 File Offset: 0x0003FF02
		public string TestAccessLevelDebug()
		{
			return "Debug access level enabled";
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x00041B0C File Offset: 0x0003FF0C
		public string GetRatingSeason()
		{
			RatingSeason ratingSeason = this.m_ratingSeasonService.GetRatingSeason();
			return ratingSeason.ToString();
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x00041B2C File Offset: 0x0003FF2C
		public uint GetProfileRatingPoints(ulong profileId)
		{
			Rating playerRating = this.m_ratingSeasonService.GetPlayerRating(profileId);
			return playerRating.Points;
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x00041B4C File Offset: 0x0003FF4C
		public bool SetProfileRatingPoints(ulong profileId, uint ratingPointsToSet)
		{
			return this.m_ratingSeasonService.SetPlayerRatingPoints(profileId, ratingPointsToSet);
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x00041B5C File Offset: 0x0003FF5C
		public IEnumerable<ulong> GetTopRatingPlayers(uint playersCount)
		{
			return this.m_ratingSeasonService.GetTopRatingPlayers(playersCount);
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x00041B78 File Offset: 0x0003FF78
		public bool RatingGameBan(ulong profileId, TimeSpan banTimeOut, string msg)
		{
			bool? flag = this.m_coldStorageService.IsProfileCold(profileId);
			if (flag != null && !flag.Value)
			{
				this.m_ratingGameBanService.BanRatingGameForPlayers(new ulong[]
				{
					profileId
				}, banTimeOut, msg);
				return this.m_ratingGameBanService.IsPlayerBanned(profileId);
			}
			MasterServer.Core.Log.Info<ulong>("Profile with profileId: {0} doesn't exist. Trying to ban unexisted player!", profileId);
			return false;
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x00041BDC File Offset: 0x0003FFDC
		public bool RatingGameUnban(ulong profileId)
		{
			bool? flag = this.m_coldStorageService.IsProfileCold(profileId);
			if (flag != null && !flag.Value)
			{
				this.m_ratingGameBanService.UnbanRatingGameForPlayers(new ulong[]
				{
					profileId
				});
				return this.m_ratingGameBanService.IsPlayerBanned(profileId);
			}
			MasterServer.Core.Log.Info<ulong>("Profile with profileId: {0} doesn't exist. Trying to unban unexisted player!", profileId);
			return false;
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x00041C3C File Offset: 0x0004003C
		public bool ValidateAuthorizationToken(ulong userId, string tokenStr)
		{
			return this.m_authorizationTokenService.ValidateToken(userId, tokenStr);
		}

		// Token: 0x0400076E RID: 1902
		public readonly AccessLevel AccessLvl;

		// Token: 0x0400076F RID: 1903
		private readonly IDALService m_dalService;

		// Token: 0x04000770 RID: 1904
		private readonly IProfileItems m_profileItemService;

		// Token: 0x04000771 RID: 1905
		private readonly IItemCache m_itemCacheService;

		// Token: 0x04000772 RID: 1906
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000773 RID: 1907
		private readonly IDebugCatalogService m_debugCatalogService;

		// Token: 0x04000774 RID: 1908
		private readonly INotificationService m_notificationService;

		// Token: 0x04000775 RID: 1909
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000776 RID: 1910
		private readonly IPunishmentService m_punishmentService;

		// Token: 0x04000777 RID: 1911
		private readonly IAnnouncementService m_announcementService;

		// Token: 0x04000778 RID: 1912
		private readonly IAuthService m_authService;

		// Token: 0x04000779 RID: 1913
		private readonly ICommunicationStatsService m_communicationStatsService;

		// Token: 0x0400077A RID: 1914
		private readonly IClanService m_clanService;

		// Token: 0x0400077B RID: 1915
		private readonly IPlayerStatsService m_playerStatsService;

		// Token: 0x0400077C RID: 1916
		private readonly IProfileProgressionService m_profileProgressionService;

		// Token: 0x0400077D RID: 1917
		private readonly ISpecialProfileRewardService m_specialRewardsService;

		// Token: 0x0400077E RID: 1918
		private readonly ITelemetryDALService m_telemetryDalService;

		// Token: 0x0400077F RID: 1919
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04000780 RID: 1920
		private readonly ICustomRulesService m_customRulesService;

		// Token: 0x04000781 RID: 1921
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000782 RID: 1922
		private readonly IAchievementSystem m_achievementSystem;

		// Token: 0x04000783 RID: 1923
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000784 RID: 1924
		private readonly IFriendsService m_friendsService;

		// Token: 0x04000785 RID: 1925
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04000786 RID: 1926
		private readonly IQueryManager m_queryManager;

		// Token: 0x04000787 RID: 1927
		private readonly IClientVersionsManagementService m_clientVersionsManagementService;

		// Token: 0x04000788 RID: 1928
		private readonly ITagService m_tagService;

		// Token: 0x04000789 RID: 1929
		private readonly IItemService m_itemService;

		// Token: 0x0400078A RID: 1930
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x0400078B RID: 1931
		private readonly IRatingSeasonService m_ratingSeasonService;

		// Token: 0x0400078C RID: 1932
		private readonly IRatingGameBanService m_ratingGameBanService;

		// Token: 0x0400078D RID: 1933
		private readonly IColdStorageService m_coldStorageService;

		// Token: 0x0400078E RID: 1934
		private readonly IShopService m_shopService;

		// Token: 0x0400078F RID: 1935
		private readonly IManualRoomService m_manualRoomService;

		// Token: 0x04000790 RID: 1936
		private readonly IAuthorizationTokenService m_authorizationTokenService;
	}
}
