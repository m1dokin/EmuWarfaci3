using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000355 RID: 853
	[Service]
	[Singleton]
	internal class ItemsExpiration : ServiceModule, IItemsExpiration
	{
		// Token: 0x06001326 RID: 4902 RVA: 0x0004DFB4 File Offset: 0x0004C3B4
		public ItemsExpiration(IDALService dalService, IUserRepository userRepository, IProfileItems profileItemsService, IGameRoomManager gameRoomManager, ISessionStorage sessionStorage, ICatalogService catalogService, IItemCache itemCacheService, ILogService logService, ICustomRulesService customRulesService, ICustomRulesStateStorage customRulesStateStorage, IClassPresenceService classPresenceService)
		{
			this.m_dalService = dalService;
			this.m_userRepository = userRepository;
			this.m_profileItemsService = profileItemsService;
			this.m_gameRoomManager = gameRoomManager;
			this.m_sessionStorage = sessionStorage;
			this.m_catalogService = catalogService;
			this.m_itemCacheService = itemCacheService;
			this.m_logService = logService;
			this.m_customRulesService = customRulesService;
			this.m_customRulesStateStorage = customRulesStateStorage;
			this.m_classPresenceService = classPresenceService;
		}

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06001327 RID: 4903 RVA: 0x0004E01C File Offset: 0x0004C41C
		// (remove) Token: 0x06001328 RID: 4904 RVA: 0x0004E054 File Offset: 0x0004C454
		public event ItemExpiredDelegate OnItemExpired;

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06001329 RID: 4905 RVA: 0x0004E08C File Offset: 0x0004C48C
		// (remove) Token: 0x0600132A RID: 4906 RVA: 0x0004E0C4 File Offset: 0x0004C4C4
		public event Action<UserInfo.User, string, IList<SProfileItem>> OnGotBrokenPermanentItems;

		// Token: 0x0600132B RID: 4907 RVA: 0x0004E0FC File Offset: 0x0004C4FC
		public override void Init()
		{
			base.Init();
			this.m_gameRoomManager.RoomOpened += this.GameRoomOpened;
			this.m_classPresenceService.ClassPresenceReceived += this.OnClassPresenceReceived;
			ConfigSection section = Resources.Rewards.GetSection("Rewards");
			section.OnConfigChanged += this.OnConfigChanged;
			this.FillItemRepairMultiplier();
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x0004E168 File Offset: 0x0004C568
		public override void Stop()
		{
			this.m_gameRoomManager.RoomOpened -= this.GameRoomOpened;
			this.m_classPresenceService.ClassPresenceReceived -= this.OnClassPresenceReceived;
			ConfigSection section = Resources.Rewards.GetSection("Rewards");
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x0004E1CC File Offset: 0x0004C5CC
		public void ExpireItemsByDate(ProfileProxy profile)
		{
			List<ulong> list = new List<ulong>();
			foreach (SProfileItem sprofileItem in profile.GetExpiredByDateProfileItems().Values)
			{
				if (sprofileItem.ExpirationTime < DateTime.UtcNow)
				{
					list.Add(sprofileItem.ProfileItemID);
				}
			}
			if (list.Count > 0)
			{
				this.ProcessExpiredItems(profile, list);
			}
		}

		// Token: 0x0600132E RID: 4910 RVA: 0x0004E264 File Offset: 0x0004C664
		private void ExpireItemsByDurabilityPoints(ProfileProxy profile, List<ItemsExpiration.DurabilityPointsInputData> inputData, CustomParams restrictions)
		{
			List<ulong> list = new List<ulong>();
			List<ItemsExpiration.ExpiredItemPoints> list2 = new List<ItemsExpiration.ExpiredItemPoints>();
			foreach (SProfileItem sprofileItem in profile.GetExpiredByDurabilityProfileItems(EquipOptions.All).Values)
			{
				int num = 0;
				int durabilityPoints = sprofileItem.DurabilityPoints;
				if (durabilityPoints > 0 && sprofileItem.SlotIDs > 0UL)
				{
					foreach (ItemsExpiration.DurabilityPointsInputData durabilityPointsInputData in inputData)
					{
						if (durabilityPointsInputData.classId < 5)
						{
							int num2 = durabilityPointsInputData.classId * 5;
							ulong num3 = 31UL << num2;
							if ((num3 & sprofileItem.SlotIDs) != 0UL && (restrictions == null || restrictions.IsItemAllowed(sprofileItem, durabilityPointsInputData.classId)))
							{
								num = (int)Math.Ceiling((double)(this.m_itemRepairMultiplier * (float)durabilityPointsInputData.spentPoints));
								num = Math.Min(durabilityPoints, num);
								if (num > 0)
								{
									list2.Add(new ItemsExpiration.ExpiredItemPoints(sprofileItem, num, durabilityPoints - num));
								}
							}
						}
						else
						{
							Log.Warning<int, ulong>("Invalid {0} class id for durability points expiration, profile {1}", durabilityPointsInputData.classId, profile.ProfileID);
						}
					}
				}
				if (durabilityPoints >= 0 && durabilityPoints == num)
				{
					list.Add(sprofileItem.ProfileItemID);
				}
			}
			SProfileInfo profileInfo = profile.ProfileInfo;
			foreach (ItemsExpiration.ExpiredItemPoints expiredItemPoints in list2)
			{
				this.m_catalogService.UpdateItemDurability(profile.UserID, expiredItemPoints.item.CatalogID, -expiredItemPoints.spentPoints);
				this.m_logService.Event.ItemUseLog(profile.UserID, profile.ProfileID, profile.Nickname, profileInfo.RankInfo.RankId, expiredItemPoints.item.ProfileItemID, expiredItemPoints.item.GameItem.Name, expiredItemPoints.item.GameItem.Type, expiredItemPoints.currPoints);
			}
			if (list.Any<ulong>())
			{
				this.ProcessExpiredItems(profile, list);
			}
		}

		// Token: 0x0600132F RID: 4911 RVA: 0x0004E4F8 File Offset: 0x0004C8F8
		private void ProcessExpiredItems(ProfileProxy profile, List<ulong> expiredItemsId)
		{
			foreach (ulong num in expiredItemsId)
			{
				SProfileItem profileItem = this.m_profileItemsService.GetProfileItem(profile.ProfileID, num, EquipOptions.ActiveOnly);
				if (profileItem == null)
				{
					profileItem = this.m_profileItemsService.GetProfileItem(profile.ProfileID, num);
					if (profileItem == null)
					{
						throw new NullReferenceException(string.Format("There is no profile item with id = {0} for profile with id = {1}, so it was deleted during game session", num, profile.ProfileID));
					}
					throw new Exception(string.Format("Profile item {0} with id = {1} and profile item id = {2} in profile {3} became inactive during game session", new object[]
					{
						profileItem.GameItem.Name,
						profileItem.GameItem.ID,
						num,
						profile.ProfileID
					}));
				}
				else
				{
					SItem gameItem = profileItem.GameItem;
					bool flag = this.ExpireItem(profile.UserID, profile.ProfileID, num);
					if (flag)
					{
						this.m_logService.Event.ItemExpireLog(profile.UserID, profile.ProfileID, num, gameItem.Name);
					}
				}
			}
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x0004E63C File Offset: 0x0004CA3C
		public bool ExpireItem(ulong user_id, ulong profileId, ulong itemId)
		{
			SProfileItem profileItem = this.m_profileItemsService.GetProfileItem(profileId, itemId, EquipOptions.All);
			if (profileItem == null)
			{
				Log.Error<ulong, ulong>("No such item {0} in profile {1}", itemId, profileId);
				return false;
			}
			if (profileItem.IsDefault)
			{
				Log.Warning<ulong>("Cannot expire default item '{0)'", profileItem.ProfileItemID);
				return false;
			}
			this.UnequipItem(profileId, itemId);
			this.m_profileItemsService.ExpireProfileItem(profileId, itemId);
			if (this.OnItemExpired != null)
			{
				this.OnItemExpired(user_id, profileItem);
			}
			return true;
		}

		// Token: 0x06001331 RID: 4913 RVA: 0x0004E6B8 File Offset: 0x0004CAB8
		public void UnequipItem(ulong profileId, ulong itemId)
		{
			SProfileItem profileItem = this.m_profileItemsService.GetProfileItem(profileId, itemId, EquipOptions.All);
			if (profileItem != null && profileItem.SlotIDs != 0UL)
			{
				Dictionary<ulong, KeyValuePair<ulong, string>> dictionary = new Dictionary<ulong, KeyValuePair<ulong, string>>();
				dictionary.Add(itemId, new KeyValuePair<ulong, string>(profileItem.SlotIDs, profileItem.Config));
				ulong num = 31UL;
				for (int i = 0; i < 5; i++)
				{
					ulong num2 = profileItem.SlotIDs & num;
					if (num2 != 0UL)
					{
						foreach (ItemsExpiration.SSlotInfo sslotInfo in this.GetProfileDefaultItemSlotIds(profileId))
						{
							ulong num3 = sslotInfo.DefaultSlotIDs & num;
							if (num2 == num3)
							{
								KeyValuePair<ulong, string> keyValuePair;
								if (!dictionary.TryGetValue(sslotInfo.ProfileItemID, out keyValuePair))
								{
									dictionary.Add(sslotInfo.ProfileItemID, new KeyValuePair<ulong, string>(num2 | sslotInfo.CurrentSlotIDs, sslotInfo.Config));
								}
								else
								{
									dictionary[sslotInfo.ProfileItemID] = new KeyValuePair<ulong, string>(keyValuePair.Key | num2, keyValuePair.Value);
								}
							}
						}
					}
					num <<= 5;
				}
				foreach (KeyValuePair<ulong, KeyValuePair<ulong, string>> keyValuePair2 in dictionary)
				{
					ulong key = keyValuePair2.Key;
					ulong key2 = keyValuePair2.Value.Key;
					string value = keyValuePair2.Value.Value;
					this.m_profileItemsService.UpdateProfileItem(profileId, key, key2, 0UL, value);
				}
			}
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x0004E874 File Offset: 0x0004CC74
		public void ExpireItems(ClassPresenceData classPresenceData)
		{
			try
			{
				CustomParams cp = this.m_sessionStorage.GetData<CustomParams>(classPresenceData.sessionId, ESessionData.Restrictions);
				classPresenceData.presence.Keys.SafeForEach(delegate(ulong profileId)
				{
					List<ClassPresenceData.PresenceData> source = classPresenceData.presence[profileId];
					List<ItemsExpiration.DurabilityPointsInputData> inputData = (from data in source
					select new ItemsExpiration.DurabilityPointsInputData(data.ClassId, data.PlayedTimeSec)).ToList<ItemsExpiration.DurabilityPointsInputData>();
					ProfileProxy profile = new ProfileProxy(profileId, this.m_userRepository, this.m_dalService, this.m_catalogService, this.m_profileItemsService, this.m_customRulesService, this.m_customRulesStateStorage);
					this.ExpireItemsByDurabilityPoints(profile, inputData, cp);
					this.ExpireItemsByDate(profile);
				});
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x0004E904 File Offset: 0x0004CD04
		private void OnClassPresenceReceived(ClassPresenceData data)
		{
			this.ExpireItems(data);
			(from e in (from profileId in data.presence.Keys
			select this.m_userRepository.GetUser(profileId) into user
			where user != null
			select user).SafeSelect((UserInfo.User user) => new
			{
				User = user,
				ExpiredItems = this.m_profileItemsService.GetProfileItems(user.ProfileID, EquipOptions.ActiveOnly | EquipOptions.FilterByTags, (SProfileItem it) => it.OfferType == OfferType.Permanent && it.IsBroken && it.SlotIDs != 0UL).Values.ToList<SProfileItem>()
			})
			where e.ExpiredItems.Any<SProfileItem>()
			select e).SafeForEach(delegate(e)
			{
				this.OnGotBrokenPermanentItems.SafeInvokeEach(e.User, data.sessionId, e.ExpiredItems);
			});
		}

		// Token: 0x06001334 RID: 4916 RVA: 0x0004E9B8 File Offset: 0x0004CDB8
		private void FillItemRepairMultiplier()
		{
			ConfigSection section = Resources.Rewards.GetSection("Rewards");
			float num = 1f;
			string text = section.Get("ItemRepairMultiplier");
			try
			{
				num = float.Parse(text);
			}
			catch (FormatException innerException)
			{
				throw new FormatException(string.Format("Cannot convert section ItemRepairMultiplier value '{0}' in 'rewards_configuration.xml'", text), innerException);
			}
			if (num > 100f || num < 0f)
			{
				throw new ArgumentOutOfRangeException(string.Format("ItemRepairMultiplier should be in range [0; {0}], value {1} isn't valid", 100f, num));
			}
			this.m_itemRepairMultiplier = num;
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x0004EA54 File Offset: 0x0004CE54
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (args.Name.Equals("ItemRepairMultiplier", StringComparison.InvariantCultureIgnoreCase))
			{
				this.FillItemRepairMultiplier();
			}
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x0004EA74 File Offset: 0x0004CE74
		private IEnumerable<ItemsExpiration.SSlotInfo> GetProfileDefaultItemSlotIds(ulong profile_id)
		{
			List<ItemsExpiration.SSlotInfo> list = new List<ItemsExpiration.SSlotInfo>();
			Dictionary<ulong, ulong> dictionary = new Dictionary<ulong, ulong>();
			foreach (SEquipItem sequipItem in this.m_itemCacheService.GetDefaultProfileItems().Values)
			{
				dictionary.Add(sequipItem.ItemID, sequipItem.SlotIDs);
			}
			foreach (SProfileItem sprofileItem in this.m_profileItemsService.GetProfileItems(profile_id).Values)
			{
				ItemsExpiration.SSlotInfo item;
				item.ProfileItemID = sprofileItem.ProfileItemID;
				item.Config = sprofileItem.Config;
				item.CurrentSlotIDs = sprofileItem.SlotIDs;
				if (dictionary.TryGetValue(sprofileItem.ItemID, out item.DefaultSlotIDs))
				{
					list.Add(item);
				}
			}
			return list;
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x0004EB90 File Offset: 0x0004CF90
		private void GameRoomOpened(IGameRoom room)
		{
			room.PlayerAdded += delegate(IGameRoom _, ulong profileId)
			{
				this.RoomPlayerAdded(profileId);
			};
			room.Players.ForEach(delegate(RoomPlayer p)
			{
				this.RoomPlayerAdded(p.ProfileID);
			});
		}

		// Token: 0x06001338 RID: 4920 RVA: 0x0004EBBC File Offset: 0x0004CFBC
		private void RoomPlayerAdded(ulong profileId)
		{
			try
			{
				this.ExpireItemsByDate(new ProfileProxy(profileId));
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x040008D1 RID: 2257
		public const float MaxItemRepairMultiplier = 100f;

		// Token: 0x040008D2 RID: 2258
		private readonly IDALService m_dalService;

		// Token: 0x040008D3 RID: 2259
		private readonly IUserRepository m_userRepository;

		// Token: 0x040008D4 RID: 2260
		private readonly IProfileItems m_profileItemsService;

		// Token: 0x040008D5 RID: 2261
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x040008D6 RID: 2262
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x040008D7 RID: 2263
		private readonly ICatalogService m_catalogService;

		// Token: 0x040008D8 RID: 2264
		private readonly IItemCache m_itemCacheService;

		// Token: 0x040008D9 RID: 2265
		private readonly ILogService m_logService;

		// Token: 0x040008DA RID: 2266
		private readonly ICustomRulesService m_customRulesService;

		// Token: 0x040008DB RID: 2267
		private readonly ICustomRulesStateStorage m_customRulesStateStorage;

		// Token: 0x040008DC RID: 2268
		private readonly IClassPresenceService m_classPresenceService;

		// Token: 0x040008DF RID: 2271
		private float m_itemRepairMultiplier;

		// Token: 0x02000356 RID: 854
		private struct SSlotInfo
		{
			// Token: 0x040008E2 RID: 2274
			public ulong ProfileItemID;

			// Token: 0x040008E3 RID: 2275
			public ulong CurrentSlotIDs;

			// Token: 0x040008E4 RID: 2276
			public ulong DefaultSlotIDs;

			// Token: 0x040008E5 RID: 2277
			public string Config;
		}

		// Token: 0x02000357 RID: 855
		private struct DurabilityPointsInputData
		{
			// Token: 0x0600133D RID: 4925 RVA: 0x0004EC25 File Offset: 0x0004D025
			public DurabilityPointsInputData(int cid, int sp)
			{
				this.classId = cid;
				this.spentPoints = sp;
			}

			// Token: 0x040008E6 RID: 2278
			public int classId;

			// Token: 0x040008E7 RID: 2279
			public int spentPoints;
		}

		// Token: 0x02000358 RID: 856
		private struct ExpiredItemPoints
		{
			// Token: 0x0600133E RID: 4926 RVA: 0x0004EC35 File Offset: 0x0004D035
			public ExpiredItemPoints(SProfileItem item, int sp, int cp)
			{
				this.item = item;
				this.spentPoints = sp;
				this.currPoints = cp;
			}

			// Token: 0x040008E8 RID: 2280
			public SProfileItem item;

			// Token: 0x040008E9 RID: 2281
			public int spentPoints;

			// Token: 0x040008EA RID: 2282
			public int currPoints;
		}
	}
}
