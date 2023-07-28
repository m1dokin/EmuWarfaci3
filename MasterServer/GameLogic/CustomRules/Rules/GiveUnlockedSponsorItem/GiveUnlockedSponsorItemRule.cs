using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.ElectronicCatalog.Exceptions;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.SponsorSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.CustomRules.Rules.GiveUnlockedSponsorItem
{
	// Token: 0x020002CB RID: 715
	[CustomRule("give_unlocked_sponsor_item")]
	internal class GiveUnlockedSponsorItemRule : CustomRule
	{
		// Token: 0x06000F40 RID: 3904 RVA: 0x0003D28C File Offset: 0x0003B68C
		public GiveUnlockedSponsorItemRule(XmlElement config, IUserRepository userRepository, ILogService logService, INotificationService notificationService, IItemService itemService, ISponsorUnlock sponsorUnlockService, IDALService dalService, ICustomRulesStateStorage customRulesStateStorage, IProfileItems profileItems, ITagService tagService) : base(config, userRepository, logService, notificationService, tagService, GiveUnlockedSponsorItemRule.RULE_CFG_ATTRS)
		{
			this.m_itemService = itemService;
			this.m_sponsorUnlockService = sponsorUnlockService;
			this.m_dalService = dalService;
			this.m_userRepository = userRepository;
			this.m_customRulesStateStorage = customRulesStateStorage;
			this.m_profileItems = profileItems;
			this.m_logService = logService;
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x0003D2E1 File Offset: 0x0003B6E1
		public override bool IsActive()
		{
			return base.Enabled;
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x0003D2E9 File Offset: 0x0003B6E9
		public override void Activate()
		{
			this.m_sponsorUnlockService.ItemUnlocked += this.OnItemUnlocked;
			this.m_userRepository.UserLoggingIn += this.OnUserLoggingIn;
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x0003D319 File Offset: 0x0003B719
		public override void Dispose()
		{
			this.m_userRepository.UserLoggingIn -= this.OnUserLoggingIn;
			this.m_sponsorUnlockService.ItemUnlocked -= this.OnItemUnlocked;
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x0003D349 File Offset: 0x0003B749
		public override string ToString()
		{
			return string.Format("{0}, id={1}", "give_unlocked_sponsor_item", base.RuleID);
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x0003D365 File Offset: 0x0003B765
		protected override ulong GetRuleID(XmlElement config)
		{
			return (ulong)((long)"give_unlocked_sponsor_item".GetHashCode());
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x0003D374 File Offset: 0x0003B774
		private bool OnItemUnlocked(UnlockItemInfo unlockItemInfo)
		{
			if (!base.Enabled)
			{
				return false;
			}
			GiveItemResponse giveItemResponse = this.m_itemService.GivePermanentItem(unlockItemInfo.UserId, unlockItemInfo.ItemName, LogGroup.ProduceType.VendorUnlock, unlockItemInfo.LoggingGroup, "-");
			if (giveItemResponse.OperationStatus == TransactionStatus.OK)
			{
				ulong num = (from sp in this.m_dalService.ProfileSystem.GetUserProfiles(unlockItemInfo.UserId)
				select sp.ProfileID).FirstOrDefault<ulong>();
				if (num != 0UL)
				{
					XmlDocument xmlDocument = new XmlDocument();
					XmlElement userData = (XmlElement)xmlDocument.AppendChild(xmlDocument.CreateElement("sponsor_item"));
					SNotification item = ItemGivenNotificationFactory.CreateItemGivenNotification(giveItemResponse, TimeSpan.FromDays(1.0), string.Empty, true, userData);
					base.ReportCustomRuleTriggered(unlockItemInfo.UserId, num, unlockItemInfo.LoggingGroup);
					base.SendNotifications(num, new List<SNotification>
					{
						item
					});
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x0003D470 File Offset: 0x0003B870
		private void OnUserLoggingIn(UserInfo.User user, ELoginType loginType, DateTime loginTime)
		{
			if (!base.Enabled)
			{
				return;
			}
			ILogGroup logGroup = this.m_logService.CreateGroup();
			if (this.m_customRulesStateStorage.UpdateState(user.ProfileID, this, (CustomRuleState state) => this.UpdateState(user, (GiveUnlockedSponsorItemRuleState)state, logGroup)))
			{
				base.ReportCustomRuleTriggered(user.UserID, user.ProfileID, logGroup);
			}
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x0003D4F8 File Offset: 0x0003B8F8
		private bool UpdateState(UserInfo.User user, GiveUnlockedSponsorItemRuleState state, ILogGroup logGroup)
		{
			if (state.Key.ProfileID == user.ProfileID && state.LastActivationTime != DateTime.MinValue)
			{
				return false;
			}
			IEnumerable<ulong> existingPermanentProfileItemIds = from it in this.m_profileItems.GetProfileItems(user.ProfileID).Values
			where it.CustomerItem.OfferType == OfferType.Permanent
			select it.GameItem.ID;
			IEnumerable<SItem> enumerable = from it in this.m_profileItems.GetUnlockedItems(user.ProfileID).Values
			where !existingPermanentProfileItemIds.Contains(it.ID)
			where GiveUnlockedSponsorItemRule.ITEM_TYPES_TO_GIVE.Contains(it.Type)
			select it;
			foreach (SItem sitem in enumerable)
			{
				try
				{
					this.m_itemService.GivePermanentItem(user.UserID, sitem.Name, LogGroup.ProduceType.VendorUnlock, logGroup, "-");
				}
				catch (ItemServiceCatalogItemNotFoundException e)
				{
					Log.Error(e);
				}
			}
			state.LastActivationTime = DateTime.UtcNow;
			return true;
		}

		// Token: 0x0400070C RID: 1804
		public const string RULE_NAME = "give_unlocked_sponsor_item";

		// Token: 0x0400070D RID: 1805
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"use_notification",
			"enabled"
		};

		// Token: 0x0400070E RID: 1806
		private static readonly string[] ITEM_TYPES_TO_GIVE = new string[]
		{
			"armor",
			"weapon"
		};

		// Token: 0x0400070F RID: 1807
		private readonly IItemService m_itemService;

		// Token: 0x04000710 RID: 1808
		private readonly ISponsorUnlock m_sponsorUnlockService;

		// Token: 0x04000711 RID: 1809
		private readonly IDALService m_dalService;

		// Token: 0x04000712 RID: 1810
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000713 RID: 1811
		private readonly ICustomRulesStateStorage m_customRulesStateStorage;

		// Token: 0x04000714 RID: 1812
		private readonly IProfileItems m_profileItems;

		// Token: 0x04000715 RID: 1813
		private readonly ILogService m_logService;
	}
}
