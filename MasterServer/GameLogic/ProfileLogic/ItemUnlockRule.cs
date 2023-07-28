using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x020003FF RID: 1023
	[ProfileProgressionRule(Name = "item_unlock")]
	internal class ItemUnlockRule : ProfileProgressionRuleBase
	{
		// Token: 0x0600161E RID: 5662 RVA: 0x0005D6A0 File Offset: 0x0005BAA0
		public ItemUnlockRule(IProfileProgressionService progressionService, IUserRepository userRepository, ISpecialProfileRewardService specialRewards, IPlayerStatsService playerStatsService, IDALService dalService, ICatalogService catalogService, IProfileItems profileItemsService, ICustomRulesService customRulesService, ICustomRulesStateStorage customRulesStateStorage, IRankSystem rankSystem, IProfileItems profileItems, IItemCache itemCache, ILogService logService, INotificationService notificationService) : base(progressionService, userRepository, specialRewards, playerStatsService, dalService, catalogService, profileItemsService, customRulesService, customRulesStateStorage)
		{
			this.m_rankSystem = rankSystem;
			this.m_profileItems = profileItems;
			this.m_itemCache = itemCache;
			this.m_logService = logService;
			this.m_notificationService = notificationService;
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x0005D6EC File Offset: 0x0005BAEC
		public override void Init(ConfigSection section)
		{
			base.Init(section);
			string text = section.Get("name");
			if (!this.m_itemCache.GetAllItemsByName().TryGetValue(text, out this.m_itemToUnlock))
			{
				throw new ProfileProgressionException(string.Format("Can't find item with name {0}", text));
			}
			this.m_rankSystem.OnProfileRankChanged += this.OnProfileRankChanged;
			this.UserRepository.UserLoggedIn += this.OnUserLoggedIn;
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x0005D767 File Offset: 0x0005BB67
		public override void Dispose()
		{
			base.Dispose();
			this.m_rankSystem.OnProfileRankChanged -= this.OnProfileRankChanged;
			this.UserRepository.UserLoggedIn -= this.OnUserLoggedIn;
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x0005D7A0 File Offset: 0x0005BBA0
		private void OnUserLoggedIn(UserInfo.User user, ELoginType loginType)
		{
			if (this.Check(user.ProfileID))
			{
				Dictionary<ulong, SItem> unlockedItems = this.m_profileItems.GetUnlockedItems(user.ProfileID);
				if (!unlockedItems.ContainsKey(this.m_itemToUnlock.ID))
				{
					this.UnlockItem(user.UserID, user.ProfileID);
				}
			}
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x0005D7F8 File Offset: 0x0005BBF8
		private void OnProfileRankChanged(SProfileInfo profile, SRankInfo newRank, SRankInfo oldRank, ILogGroup logGroup)
		{
			if (base.Enabled && oldRank.RankId < this.RankReached && newRank.RankId >= this.RankReached)
			{
				this.UnlockItem(profile.UserID, profile.Id);
			}
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x0005D848 File Offset: 0x0005BC48
		private void UnlockItem(ulong userId, ulong profileId)
		{
			this.m_profileItems.UnlockItem(profileId, this.m_itemToUnlock.ID);
			this.m_logService.Event.ItemUnlockedLog(userId, profileId, this.m_itemToUnlock.ID, this.m_itemToUnlock.Name, LogGroup.ProduceType.ProfileProgression);
			SNotification item = NotificationFactory.CreateNotification<ItemUnlockedNotification>(ENotificationType.ItemUnlocked, new ItemUnlockedNotification(this.m_itemToUnlock.Name), TimeSpan.FromMinutes(5.0), EConfirmationType.None);
			this.m_notificationService.AddNotifications(profileId, new List<SNotification>
			{
				item
			}, EDeliveryType.SendNow);
		}

		// Token: 0x04000AB3 RID: 2739
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000AB4 RID: 2740
		private readonly IProfileItems m_profileItems;

		// Token: 0x04000AB5 RID: 2741
		private readonly IItemCache m_itemCache;

		// Token: 0x04000AB6 RID: 2742
		private readonly ILogService m_logService;

		// Token: 0x04000AB7 RID: 2743
		private readonly INotificationService m_notificationService;

		// Token: 0x04000AB8 RID: 2744
		private SItem m_itemToUnlock;
	}
}
