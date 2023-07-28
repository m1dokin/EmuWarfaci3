using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000234 RID: 564
	[OrphanService]
	[Singleton]
	internal class ItemGivenLogger : ServiceModule
	{
		// Token: 0x06000C14 RID: 3092 RVA: 0x0002E2A5 File Offset: 0x0002C6A5
		public ItemGivenLogger(IItemService itemService, ICatalogService catalogService, IUserRepository userRepository, IDALService dalService)
		{
			this.m_itemService = itemService;
			this.m_catalogService = catalogService;
			this.m_userRepository = userRepository;
			this.m_dalService = dalService;
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x0002E2CA File Offset: 0x0002C6CA
		public override void Init()
		{
			base.Init();
			this.m_itemService.ItemGiven += this.LogGiveItem;
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x0002E2E9 File Offset: 0x0002C6E9
		public override void Stop()
		{
			this.m_itemService.ItemGiven -= this.LogGiveItem;
			base.Stop();
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0002E308 File Offset: 0x0002C708
		internal void LogGiveItem(GiveItemResponse logInfo)
		{
			OfferItem itemGiven = logInfo.ItemGiven;
			ulong userId = logInfo.UserId;
			ulong profileId = logInfo.ProfileId;
			UserInfo.User userByUserId = this.m_userRepository.GetUserByUserId(userId);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			Dictionary<Currency, ulong> dictionary = new Dictionary<Currency, ulong>();
			try
			{
				List<CustomerAccount> customerAccounts = this.m_catalogService.GetCustomerAccounts(userId);
				foreach (CustomerAccount customerAccount in customerAccounts)
				{
					dictionary[customerAccount.Currency] = customerAccount.Money;
				}
			}
			catch (PaymentServiceException e)
			{
				Log.Error(e);
			}
			ILogGroup loggingGroup = logInfo.LoggingGroup;
			ulong userId2 = userId;
			ulong profileId2 = profileId;
			string nickname = (profileInfo.UserID != userId) ? string.Empty : profileInfo.Nickname;
			int rankId = (profileInfo.UserID != userId) ? 0 : profileInfo.RankInfo.RankId;
			string ip = (userByUserId == null) ? string.Empty : userByUserId.IP;
			TransactionStatus transactionStatus = TransactionStatus.OK;
			long spentGameMoney = 0L;
			long spentCryMoney = 0L;
			long spentCrownMoney = 0L;
			string empty = string.Empty;
			ulong gameMoney = (!dictionary.ContainsKey(Currency.GameMoney)) ? 0UL : dictionary[Currency.GameMoney];
			ulong cryMoney = (!dictionary.ContainsKey(Currency.CryMoney)) ? 0UL : dictionary[Currency.CryMoney];
			ulong crownMoney = (!dictionary.ContainsKey(Currency.CrownMoney)) ? 0UL : dictionary[Currency.CrownMoney];
			string empty2 = string.Empty;
			uint discount = 0U;
			ulong shopOfferId = 0UL;
			OfferType itemOfferType = logInfo.ItemOfferType;
			ulong id = itemGiven.Item.ID;
			string name = itemGiven.Item.Name;
			int durabilityPoints = itemGiven.DurabilityPoints;
			string expireTime = TimeUtils.GetExpireTime(itemGiven.ExpirationTime);
			string type = itemGiven.Item.Type;
			ulong itemsLeft = (itemGiven.Quantity <= 0UL) ? 1UL : itemGiven.Quantity;
			ulong quantity = itemGiven.Quantity;
			LogGroup.ProduceType logProduceType = logInfo.LogProduceType;
			string reason = logInfo.Reason;
			loggingGroup.ShopOfferBoughtLog(userId2, profileId2, nickname, rankId, ip, transactionStatus, spentGameMoney, spentCryMoney, spentCrownMoney, empty, gameMoney, cryMoney, crownMoney, empty2, discount, shopOfferId, itemOfferType, id, name, durabilityPoints, expireTime, type, itemsLeft, quantity, logProduceType, 0UL, reason);
		}

		// Token: 0x0400059D RID: 1437
		private readonly IItemService m_itemService;

		// Token: 0x0400059E RID: 1438
		private readonly ICatalogService m_catalogService;

		// Token: 0x0400059F RID: 1439
		private readonly IUserRepository m_userRepository;

		// Token: 0x040005A0 RID: 1440
		private readonly IDALService m_dalService;
	}
}
