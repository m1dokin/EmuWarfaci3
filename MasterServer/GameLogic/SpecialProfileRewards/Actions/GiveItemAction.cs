using System;
using System.Globalization;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.SpecialProfileRewards.Exceptions;
using Util.Common;

namespace MasterServer.GameLogic.SpecialProfileRewards.Actions
{
	// Token: 0x020005C2 RID: 1474
	[SpecialRewardAction("item")]
	internal class GiveItemAction : SpecialRewardAction
	{
		// Token: 0x06001F95 RID: 8085 RVA: 0x00080C14 File Offset: 0x0007F014
		public GiveItemAction(ConfigSection config, IItemCache itemCacheService, IDALService dalService, IItemService itemService) : base(config)
		{
			this.m_itemCacheService = itemCacheService;
			this.m_dalService = dalService;
			this.m_itemService = itemService;
			this.m_itemName = config.Get("name");
			this.m_expiration = NotificationService.DefaultNotificationTTL;
			if (config.HasValue("expiration"))
			{
				this.m_offerType = OfferType.Expiration;
				this.m_expiration = TimeUtils.GetExpireTime(config.Get("expiration"));
			}
			else if (config.HasValue("amount"))
			{
				this.m_offerType = OfferType.Consumable;
				this.m_amount = ushort.Parse(config.Get("amount"), NumberStyles.None, CultureInfo.InvariantCulture);
				this.m_maxAmount = ((!config.HasValue("max_amount")) ? 0 : ushort.Parse(config.Get("max_amount")));
			}
			else
			{
				this.m_offerType = ((!config.HasValue("regular")) ? OfferType.Permanent : OfferType.Regular);
			}
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06001F96 RID: 8086 RVA: 0x00080D0D File Offset: 0x0007F10D
		public override string PrizeName
		{
			get
			{
				return this.m_itemName;
			}
		}

		// Token: 0x06001F97 RID: 8087 RVA: 0x00080D18 File Offset: 0x0007F118
		public override SNotification Activate(ulong profileId, ILogGroup logGroup, XmlElement userData)
		{
			ulong userID = this.m_dalService.ProfileSystem.GetProfileInfo(profileId).UserID;
			RandomBoxPurchaseHandler randomBoxPurchaseHandler = null;
			GiveItemResponse giveItemResponse;
			switch (this.m_offerType)
			{
			case OfferType.Expiration:
				giveItemResponse = this.m_itemService.GiveExpirableItem(userID, this.m_itemName, this.m_expiration, LogGroup.ProduceType.SpecialReward, logGroup, "-");
				break;
			case OfferType.Permanent:
				giveItemResponse = this.m_itemService.GivePermanentItem(userID, this.m_itemName, LogGroup.ProduceType.SpecialReward, logGroup, "-");
				break;
			case OfferType.Consumable:
				giveItemResponse = this.m_itemService.GiveConsumableItem(userID, this.m_itemName, this.m_amount, LogGroup.ProduceType.SpecialReward, logGroup, this.m_maxAmount, "-");
				break;
			case OfferType.Regular:
				randomBoxPurchaseHandler = RandomBoxPurchaseHandler.Create(profileId, userData);
				giveItemResponse = this.m_itemService.GiveRegularItem(userID, this.m_itemName, LogGroup.ProduceType.SpecialReward, randomBoxPurchaseHandler, logGroup, "-", false);
				break;
			default:
				throw new ApplicationException(string.Format("Can't give item {0}! Unsupported offer type {1}", this.m_itemName, this.m_offerType));
			}
			SNotification result = null;
			if (giveItemResponse.OperationStatus == TransactionStatus.OK && this.m_useNotification)
			{
				bool flag = giveItemResponse.ItemGiven.Item.Type == "random_box";
				SNotification snotification;
				if (flag)
				{
					snotification = randomBoxPurchaseHandler.CreateNotification(giveItemResponse.ItemGiven, string.Empty, true);
				}
				else
				{
					GiveItemResponse givenItem = giveItemResponse;
					TimeSpan expiration = this.m_expiration;
					string empty = string.Empty;
					snotification = ItemGivenNotificationFactory.CreateItemGivenNotification(givenItem, expiration, empty, true, userData);
				}
				result = snotification;
			}
			return result;
		}

		// Token: 0x06001F98 RID: 8088 RVA: 0x00080E9C File Offset: 0x0007F29C
		public override void Validate()
		{
			SItem sitem;
			if (!this.m_itemCacheService.TryGetItem(this.m_itemName, false, out sitem))
			{
				Log.Warning(string.Format("\"{0}\" item was not found in IItemCache", this.m_itemName));
				return;
			}
			if (sitem.Type == "meta_game")
			{
				throw new InvalidGiveItemActionException(string.Format("GiveItemAction can't be set up for MetaGame item. Current item: \"{0}\"", this.m_itemName));
			}
			if (this.m_offerType != OfferType.Regular && (sitem.Type == "bundle" || sitem.Type == "random_box"))
			{
				throw new InvalidGiveItemActionException(string.Format("GiveItemAction for {0} can be only regular. Current item: \"{1}\"", sitem.Type, this.m_itemName));
			}
		}

		// Token: 0x06001F99 RID: 8089 RVA: 0x00080F58 File Offset: 0x0007F358
		public override string ToString()
		{
			return string.Format("item name:{0} type:{1} expiration:{2} amount:{3} max_amount:{4} use_notification:{5}", new object[]
			{
				this.m_itemName,
				this.m_offerType,
				this.m_expiration,
				this.m_amount,
				this.m_maxAmount,
				this.m_useNotification
			});
		}

		// Token: 0x04000F66 RID: 3942
		private readonly string m_itemName;

		// Token: 0x04000F67 RID: 3943
		private readonly OfferType m_offerType;

		// Token: 0x04000F68 RID: 3944
		private readonly TimeSpan m_expiration;

		// Token: 0x04000F69 RID: 3945
		private readonly ushort m_amount;

		// Token: 0x04000F6A RID: 3946
		private readonly ushort m_maxAmount;

		// Token: 0x04000F6B RID: 3947
		private readonly IItemCache m_itemCacheService;

		// Token: 0x04000F6C RID: 3948
		private readonly IDALService m_dalService;

		// Token: 0x04000F6D RID: 3949
		private readonly IItemService m_itemService;
	}
}
