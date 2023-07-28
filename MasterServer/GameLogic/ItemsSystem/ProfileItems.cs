using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000372 RID: 882
	[Service]
	[Singleton]
	internal class ProfileItems : ServiceModule, IProfileItems
	{
		// Token: 0x060013F8 RID: 5112 RVA: 0x000511D8 File Offset: 0x0004F5D8
		public ProfileItems(IProfileItemsComposerFactory composerFactory, IDALService dalService, ICatalogService catalogService, IItemsValidator itemsValidator, IItemCache itemCacheService)
		{
			this.m_composerFactory = composerFactory;
			this.m_dalService = dalService;
			this.m_catalogService = catalogService;
			this.m_itemsValidator = itemsValidator;
			this.m_itemCacheService = itemCacheService;
			this.m_composers = new List<KeyValuePair<int, IProfileItemsComposer>>();
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x00051228 File Offset: 0x0004F628
		public override void Init()
		{
			IEnumerable<KeyValuePair<int, IProfileItemsComposer>> composers = this.m_composerFactory.GetComposers();
			object composersLocker = this.m_composersLocker;
			lock (composersLocker)
			{
				this.m_composers.AddRange(composers);
				this.m_composers.Sort((KeyValuePair<int, IProfileItemsComposer> a, KeyValuePair<int, IProfileItemsComposer> b) => Comparer<int>.Default.Compare(a.Key, b.Key));
			}
		}

		// Token: 0x060013FA RID: 5114 RVA: 0x000512A8 File Offset: 0x0004F6A8
		public override void Stop()
		{
			object composersLocker = this.m_composersLocker;
			lock (composersLocker)
			{
				this.m_composers.Clear();
			}
		}

		// Token: 0x060013FB RID: 5115 RVA: 0x000512F0 File Offset: 0x0004F6F0
		private IEnumerable<SEquipItem> GetComposedEquipItems(ulong profileId, EquipOptions options)
		{
			object composersLocker = this.m_composersLocker;
			IList<KeyValuePair<int, IProfileItemsComposer>> list;
			lock (composersLocker)
			{
				list = this.m_composers.ToList<KeyValuePair<int, IProfileItemsComposer>>();
			}
			List<SEquipItem> list2 = new List<SEquipItem>();
			foreach (KeyValuePair<int, IProfileItemsComposer> keyValuePair in list)
			{
				keyValuePair.Value.Compose(profileId, options, list2);
			}
			return list2;
		}

		// Token: 0x060013FC RID: 5116 RVA: 0x00051394 File Offset: 0x0004F794
		public void UpdateProfileItem(ulong profileID, ulong profileItemID, ulong slotIds, ulong attachedTo, string config)
		{
			this.m_dalService.ItemSystem.UpdateProfileItem(profileID, profileItemID, slotIds, attachedTo, config);
		}

		// Token: 0x060013FD RID: 5117 RVA: 0x000513B0 File Offset: 0x0004F7B0
		public void AddDefaultItems(ulong profileID)
		{
			foreach (SEquipItem sequipItem in this.m_itemCacheService.GetDefaultProfileItems().Values)
			{
				ulong num = 0UL;
				foreach (SlotInfo slotInfo in this.m_itemsValidator.GetSlotsInfo(sequipItem.SlotIDs))
				{
					if (slotInfo.maxCount - slotInfo.minCount > 1)
					{
						num |= slotInfo.id;
					}
				}
				if (num != 0UL)
				{
					this.m_dalService.ItemSystem.AddDefaultItem(profileID, sequipItem.ItemID, num, sequipItem.AttachedTo, sequipItem.Config);
				}
			}
		}

		// Token: 0x060013FE RID: 5118 RVA: 0x000514B0 File Offset: 0x0004F8B0
		public void DeleteDefaultItems(ulong profileID)
		{
			this.m_dalService.ItemSystem.DeleteDefaultItems(profileID);
		}

		// Token: 0x060013FF RID: 5119 RVA: 0x000514C3 File Offset: 0x0004F8C3
		public Dictionary<ulong, SProfileItem> GetProfileItems(ulong profileID)
		{
			return this.GetProfileItems(profileID, EquipOptions.ActiveOnly);
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x000514CD File Offset: 0x0004F8CD
		public Dictionary<ulong, SProfileItem> GetProfileItems(ulong profileID, EquipOptions options)
		{
			return this.GetProfileItems(profileID, options, (SProfileItem i) => true);
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x000514F4 File Offset: 0x0004F8F4
		public Dictionary<ulong, SProfileItem> GetProfileItems(ulong profileId, EquipOptions options, Predicate<SProfileItem> pred)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			Dictionary<ulong, SProfileItem> dictionary = new Dictionary<ulong, SProfileItem>();
			Dictionary<ulong, SItem> allItems = this.m_itemCacheService.GetAllItems(false);
			Dictionary<ulong, CustomerItem> customerItems = this.m_catalogService.GetCustomerItems(profileInfo.UserID);
			if (!options.HasFlag(EquipOptions.ActiveOnly))
			{
				foreach (KeyValuePair<ulong, CustomerItem> keyValuePair in this.m_catalogService.GetInactiveCustomerItems(profileInfo.UserID))
				{
					customerItems[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			foreach (SEquipItem sequipItem in this.GetComposedEquipItems(profileId, options))
			{
				SItem sitem = allItems[sequipItem.ItemID];
				if (!options.HasFlag(EquipOptions.ActiveOnly) || sitem.Active)
				{
					CustomerItem cat = null;
					if (sequipItem.CatalogID != 0UL && !customerItems.TryGetValue(sequipItem.CatalogID, out cat))
					{
						Log.Warning<ulong, ulong, ulong>("Can't find CustomerItem with CatalogId '{0}' for ProfileItem '{1}' from ProfileId '{2}'", sequipItem.CatalogID, sequipItem.ProfileItemID, profileId);
					}
					SProfileItem sprofileItem = new SProfileItem(sequipItem, sitem, cat);
					if (pred(sprofileItem))
					{
						dictionary.Add(sequipItem.ProfileItemID, sprofileItem);
					}
				}
			}
			return dictionary;
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x000516A4 File Offset: 0x0004FAA4
		public SProfileItem GetProfileItem(ulong profileID, ulong inventoryID)
		{
			return this.GetProfileItem(profileID, inventoryID, EquipOptions.ActiveOnly);
		}

		// Token: 0x06001403 RID: 5123 RVA: 0x000516B0 File Offset: 0x0004FAB0
		public SProfileItem GetProfileItem(ulong profileID, ulong inventoryID, EquipOptions options)
		{
			Dictionary<ulong, SProfileItem> profileItems = this.GetProfileItems(profileID, options, (SProfileItem PI) => PI.ProfileItemID == inventoryID);
			SProfileItem result;
			profileItems.TryGetValue(inventoryID, out result);
			return result;
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x000516EF File Offset: 0x0004FAEF
		public Dictionary<ulong, SProfileItem> GetProfileDefaultItems(ulong profile_id)
		{
			return this.GetProfileItems(profile_id, EquipOptions.ActiveOnly, (SProfileItem PI) => PI.IsDefault);
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x00051718 File Offset: 0x0004FB18
		public Dictionary<ulong, SItem> GetUnlockedItems(ulong profile_id)
		{
			Dictionary<ulong, SItem> allItems = this.m_itemCacheService.GetAllItems();
			Dictionary<ulong, SItem> dictionary = new Dictionary<ulong, SItem>();
			foreach (ulong key in this.m_dalService.ItemSystem.GetUnlockedItems(profile_id))
			{
				SItem value;
				if (allItems.TryGetValue(key, out value))
				{
					dictionary.Add(key, value);
				}
			}
			return dictionary;
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x000517A0 File Offset: 0x0004FBA0
		public Dictionary<ulong, SProfileItem> GetExpiredProfileItems(ulong profileId)
		{
			return this.GetProfileItems(profileId, EquipOptions.All, (SProfileItem PI) => PI.Status == EProfileItemStatus.EXPIRED);
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x000517C7 File Offset: 0x0004FBC7
		public Dictionary<ulong, SProfileItem> GetExpiredByDateProfileItems(ulong profile_id)
		{
			return this.GetExpiredByDateProfileItems(profile_id, EquipOptions.ActiveOnly);
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x000517D1 File Offset: 0x0004FBD1
		public Dictionary<ulong, SProfileItem> GetExpiredByDateProfileItems(ulong profileId, EquipOptions options)
		{
			return this.GetProfileItems(profileId, options, (SProfileItem PI) => PI.Status == EProfileItemStatus.BOUGHT && PI.OfferType == OfferType.Expiration);
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x000517F8 File Offset: 0x0004FBF8
		public Dictionary<ulong, SProfileItem> GetExpiredByDurabilityProfileItems(ulong profile_id)
		{
			return this.GetExpiredByDurabilityProfileItems(profile_id, EquipOptions.ActiveOnly);
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x00051802 File Offset: 0x0004FC02
		public Dictionary<ulong, SProfileItem> GetExpiredByDurabilityProfileItems(ulong profileId, EquipOptions options)
		{
			return this.GetProfileItems(profileId, options, (SProfileItem pi) => pi.Status == EProfileItemStatus.BOUGHT && pi.OfferType == OfferType.Permanent);
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x00051829 File Offset: 0x0004FC29
		public ulong AddPurchasedItem(ulong profileID, ulong item_id, ulong catalog_id)
		{
			return this.m_dalService.ItemSystem.AddPurchasedItem(profileID, item_id, catalog_id);
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x0005183E File Offset: 0x0004FC3E
		public void RepairProfileItem(ulong profileID, ulong profile_item_id)
		{
			this.m_dalService.ItemSystem.RepairProfileItem(profileID, profile_item_id);
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x00051852 File Offset: 0x0004FC52
		public void ExpireProfileItem(ulong profileID, ulong profile_item_id)
		{
			this.m_dalService.ItemSystem.ExpireProfileItem(profileID, profile_item_id);
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x00051866 File Offset: 0x0004FC66
		public void ExpireProfileItemConfirmed(ulong profileID, ulong profile_item_id)
		{
			this.m_dalService.ItemSystem.ExpireProfileItemConfirmed(profileID, profile_item_id);
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x0005187A File Offset: 0x0004FC7A
		public ulong GiveItem(ulong profileID, ulong itemId, EProfileItemStatus status)
		{
			return this.m_dalService.ItemSystem.GiveItem(profileID, itemId, status);
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x0005188F File Offset: 0x0004FC8F
		public void DeleteProfileItem(ulong profileID, ulong profile_item_id)
		{
			this.m_dalService.ItemSystem.DeleteProfileItem(profileID, profile_item_id);
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x000518A3 File Offset: 0x0004FCA3
		public ulong ExtendProfileItem(ulong profileID, ulong profile_item_id)
		{
			return this.m_dalService.ItemSystem.ExtendProfileItem(profileID, profile_item_id);
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x000518B7 File Offset: 0x0004FCB7
		public void UnlockItem(ulong profileID, ulong itemId)
		{
			this.m_dalService.ItemSystem.UnlockItem(profileID, itemId);
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x000518CB File Offset: 0x0004FCCB
		public void LockItem(ulong profileID, ulong itemId)
		{
			this.m_dalService.ItemSystem.LockItem(profileID, itemId);
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x000518DF File Offset: 0x0004FCDF
		public SProfileItem BuildProfileItem(SEquipItem equipItem)
		{
			return this.BuildProfileItem(equipItem, null);
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x000518EC File Offset: 0x0004FCEC
		public SProfileItem BuildProfileItem(SEquipItem equipItem, CustomerItem customerItem)
		{
			SItem game = this.m_itemCacheService.GetAllItems(false)[equipItem.ItemID];
			return new SProfileItem(equipItem, game, customerItem);
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x00051919 File Offset: 0x0004FD19
		public void RegisterProfileItemsComposer(IProfileItemsComposer composer)
		{
			this.RegisterProfileItemsComposer(composer, 50);
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x00051924 File Offset: 0x0004FD24
		public void RegisterProfileItemsComposer(IProfileItemsComposer composer, int priority)
		{
			object composersLocker = this.m_composersLocker;
			lock (composersLocker)
			{
				this.m_composers.Add(new KeyValuePair<int, IProfileItemsComposer>(priority, composer));
				this.m_composers.Sort((KeyValuePair<int, IProfileItemsComposer> a, KeyValuePair<int, IProfileItemsComposer> b) => Comparer<int>.Default.Compare(a.Key, b.Key));
			}
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x0005199C File Offset: 0x0004FD9C
		public void UnregisterProfileItemsComposer(IProfileItemsComposer composer)
		{
			object composersLocker = this.m_composersLocker;
			lock (composersLocker)
			{
				this.m_composers.RemoveAll((KeyValuePair<int, IProfileItemsComposer> x) => x.Value == composer);
			}
		}

		// Token: 0x0400093B RID: 2363
		private readonly IProfileItemsComposerFactory m_composerFactory;

		// Token: 0x0400093C RID: 2364
		private readonly List<KeyValuePair<int, IProfileItemsComposer>> m_composers;

		// Token: 0x0400093D RID: 2365
		private readonly object m_composersLocker = new object();

		// Token: 0x0400093E RID: 2366
		private readonly IDALService m_dalService;

		// Token: 0x0400093F RID: 2367
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000940 RID: 2368
		private readonly IItemsValidator m_itemsValidator;

		// Token: 0x04000941 RID: 2369
		private readonly IItemCache m_itemCacheService;
	}
}
