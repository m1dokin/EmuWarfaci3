using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameLogic.CustomRules.Rules;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.Users
{
	// Token: 0x020007DE RID: 2014
	internal class ProfileProxy : IProfileProxy
	{
		// Token: 0x0600291A RID: 10522 RVA: 0x000B23F4 File Offset: 0x000B07F4
		public ProfileProxy(ulong profileId, IUserRepository userRepository, IDALService dalService, ICatalogService catalogService, IProfileItems profileItemsService, ICustomRulesService customRulesService, ICustomRulesStateStorage customRulesStateStorage)
		{
			this.ProfileID = profileId;
			this.UserRepository = userRepository;
			this.DAL = dalService;
			this.ECat = catalogService;
			this.ProfileItemsService = profileItemsService;
			this.m_customRulesService = customRulesService;
			this.m_customRulesStateStorage = customRulesStateStorage;
		}

		// Token: 0x0600291B RID: 10523 RVA: 0x000B2431 File Offset: 0x000B0831
		public ProfileProxy(ulong profileId) : this(profileId, ServicesManager.GetService<IUserRepository>(), ServicesManager.GetService<IDALService>(), ServicesManager.GetService<ICatalogService>(), ServicesManager.GetService<IProfileItems>(), ServicesManager.GetService<ICustomRulesService>(), ServicesManager.GetService<ICustomRulesStateStorage>())
		{
		}

		// Token: 0x0600291C RID: 10524 RVA: 0x000B2458 File Offset: 0x000B0858
		public ProfileProxy(UserInfo.User user) : this(user.ProfileID)
		{
			this.m_user_info = user;
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x0600291D RID: 10525 RVA: 0x000B246D File Offset: 0x000B086D
		// (set) Token: 0x0600291E RID: 10526 RVA: 0x000B2475 File Offset: 0x000B0875
		public ulong ProfileID { get; private set; }

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x0600291F RID: 10527 RVA: 0x000B247E File Offset: 0x000B087E
		public ulong UserID
		{
			get
			{
				return this.UserInfo.UserID;
			}
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06002920 RID: 10528 RVA: 0x000B248B File Offset: 0x000B088B
		public string Nickname
		{
			get
			{
				return this.UserInfo.Nickname;
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06002921 RID: 10529 RVA: 0x000B2498 File Offset: 0x000B0898
		public UserInfo.User UserInfo
		{
			get
			{
				if (this.m_user_info == null)
				{
					this.m_user_info = this.UserRepository.GetUser(this.ProfileID);
					if (this.m_user_info == null)
					{
						this.m_user_info = this.UserRepository.Make(this.ProfileInfo, new ProfileProgressionInfo(this.ProfileID, ServicesManager.GetService<IDALService>()));
					}
				}
				return this.m_user_info;
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06002922 RID: 10530 RVA: 0x000B2500 File Offset: 0x000B0900
		public ConsecutiveLoginBonusRuleState LoginBonusState
		{
			get
			{
				foreach (ICustomRule customRule in this.m_customRulesService.GetActiveRules())
				{
					if (customRule.Enabled && customRule is ConsecutiveLoginBonusRule)
					{
						return (ConsecutiveLoginBonusRuleState)this.m_customRulesStateStorage.GetState(this.ProfileID, customRule);
					}
				}
				return null;
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06002923 RID: 10531 RVA: 0x000B2590 File Offset: 0x000B0990
		public SProfileInfo ProfileInfo
		{
			get
			{
				return this.DAL.ProfileSystem.GetProfileInfo(this.ProfileID);
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06002924 RID: 10532 RVA: 0x000B25A8 File Offset: 0x000B09A8
		public Dictionary<ulong, SProfileItem> ProfileItems
		{
			get
			{
				return this.ProfileItemsService.GetProfileItems(this.ProfileID);
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06002925 RID: 10533 RVA: 0x000B25BB File Offset: 0x000B09BB
		public IEnumerable<CustomerAccount> Accounts
		{
			get
			{
				return this.ECat.GetCustomerAccounts(this.UserID);
			}
		}

		// Token: 0x06002926 RID: 10534 RVA: 0x000B25CE File Offset: 0x000B09CE
		public CustomerAccount Account(Currency currency)
		{
			return this.ECat.GetCustomerAccount(this.UserID, currency);
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06002927 RID: 10535 RVA: 0x000B25E2 File Offset: 0x000B09E2
		public Dictionary<ulong, CustomerItem> CustomerItems
		{
			get
			{
				return this.ECat.GetCustomerItems(this.UserID);
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06002928 RID: 10536 RVA: 0x000B25F5 File Offset: 0x000B09F5
		public IEnumerable<SSponsorPoints> SponsorPoints
		{
			get
			{
				return this.DAL.RewardsSystem.GetSponsorPoints(this.ProfileID);
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06002929 RID: 10537 RVA: 0x000B260D File Offset: 0x000B0A0D
		public Dictionary<ulong, SItem> UnlockedItems
		{
			get
			{
				return this.ProfileItemsService.GetUnlockedItems(this.ProfileID);
			}
		}

		// Token: 0x0600292A RID: 10538 RVA: 0x000B2620 File Offset: 0x000B0A20
		public Dictionary<ulong, SProfileItem> GetProfileItems(Predicate<SProfileItem> pred)
		{
			Dictionary<ulong, SProfileItem> dictionary = new Dictionary<ulong, SProfileItem>();
			foreach (KeyValuePair<ulong, SProfileItem> keyValuePair in this.ProfileItems)
			{
				if (pred(keyValuePair.Value))
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return dictionary;
		}

		// Token: 0x0600292B RID: 10539 RVA: 0x000B26A4 File Offset: 0x000B0AA4
		public Dictionary<ulong, SProfileItem> GetProfileDefaultItems()
		{
			return this.ProfileItemsService.GetProfileDefaultItems(this.ProfileID);
		}

		// Token: 0x0600292C RID: 10540 RVA: 0x000B26B7 File Offset: 0x000B0AB7
		public Dictionary<ulong, SProfileItem> GetExpiredProfileItems()
		{
			return this.ProfileItemsService.GetExpiredProfileItems(this.ProfileID);
		}

		// Token: 0x0600292D RID: 10541 RVA: 0x000B26CA File Offset: 0x000B0ACA
		public Dictionary<ulong, SProfileItem> GetExpiredByDateProfileItems()
		{
			return this.ProfileItemsService.GetExpiredByDateProfileItems(this.ProfileID);
		}

		// Token: 0x0600292E RID: 10542 RVA: 0x000B26DD File Offset: 0x000B0ADD
		public Dictionary<ulong, SProfileItem> GetExpiredByDateProfileItems(EquipOptions options)
		{
			return this.ProfileItemsService.GetExpiredByDateProfileItems(this.ProfileID, options);
		}

		// Token: 0x0600292F RID: 10543 RVA: 0x000B26F1 File Offset: 0x000B0AF1
		public Dictionary<ulong, SProfileItem> GetExpiredByDurabilityProfileItems()
		{
			return this.ProfileItemsService.GetExpiredByDurabilityProfileItems(this.ProfileID);
		}

		// Token: 0x06002930 RID: 10544 RVA: 0x000B2704 File Offset: 0x000B0B04
		public Dictionary<ulong, SProfileItem> GetExpiredByDurabilityProfileItems(EquipOptions options)
		{
			return this.ProfileItemsService.GetExpiredByDurabilityProfileItems(this.ProfileID, options);
		}

		// Token: 0x06002931 RID: 10545 RVA: 0x000B2718 File Offset: 0x000B0B18
		public void SetProfileRankInfo(ulong old_exp, SRankInfo rank_info)
		{
			this.DAL.ProfileSystem.SetProfileRankInfo(this.ProfileID, old_exp, rank_info);
		}

		// Token: 0x06002932 RID: 10546 RVA: 0x000B2734 File Offset: 0x000B0B34
		public ulong GiveItem(ulong item_id, EProfileItemStatus status)
		{
			return this.ProfileItemsService.GiveItem(this.ProfileID, item_id, status);
		}

		// Token: 0x06002933 RID: 10547 RVA: 0x000B2756 File Offset: 0x000B0B56
		public void UpdateProfileItem(ulong profile_item_id, ulong slot_ids, ulong attached_to, string config)
		{
			this.ProfileItemsService.UpdateProfileItem(this.ProfileID, profile_item_id, slot_ids, attached_to, config);
		}

		// Token: 0x06002934 RID: 10548 RVA: 0x000B276E File Offset: 0x000B0B6E
		public void DeleteProfileItem(ulong profile_item_id)
		{
			this.ProfileItemsService.DeleteProfileItem(this.ProfileID, profile_item_id);
		}

		// Token: 0x06002935 RID: 10549 RVA: 0x000B2782 File Offset: 0x000B0B82
		public bool SetSponsorInfo(uint sponsor_id, ulong old_sp_pts, SRankInfo new_sp)
		{
			return this.DAL.RewardsSystem.SetSponsorInfo(this.ProfileID, sponsor_id, old_sp_pts, new_sp);
		}

		// Token: 0x06002936 RID: 10550 RVA: 0x000B279D File Offset: 0x000B0B9D
		public void SetNextUnlockItem(uint sponsor_id, ulong next_unlock_item_id)
		{
			this.DAL.RewardsSystem.SetNextUnlockItem(this.ProfileID, sponsor_id, next_unlock_item_id);
		}

		// Token: 0x040015EE RID: 5614
		private UserInfo.User m_user_info;

		// Token: 0x040015EF RID: 5615
		private readonly IUserRepository UserRepository;

		// Token: 0x040015F0 RID: 5616
		private readonly IDALService DAL;

		// Token: 0x040015F1 RID: 5617
		private readonly ICatalogService ECat;

		// Token: 0x040015F2 RID: 5618
		private readonly IProfileItems ProfileItemsService;

		// Token: 0x040015F3 RID: 5619
		private readonly ICustomRulesService m_customRulesService;

		// Token: 0x040015F4 RID: 5620
		private readonly ICustomRulesStateStorage m_customRulesStateStorage;
	}
}
