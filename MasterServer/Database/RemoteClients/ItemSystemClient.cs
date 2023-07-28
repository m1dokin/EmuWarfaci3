using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000202 RID: 514
	internal class ItemSystemClient : DALCacheProxy<IDALService>, IItemSystemClient
	{
		// Token: 0x06000AB6 RID: 2742 RVA: 0x00027F99 File Offset: 0x00026399
		internal void Reset(IItemSystem itemSytem)
		{
			this.m_itemSystem = itemSytem;
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x00027FA4 File Offset: 0x000263A4
		public IEnumerable<SItem> GetAllItems()
		{
			DALCacheProxy<IDALService>.Options<SItem> options = new DALCacheProxy<IDALService>.Options<SItem>
			{
				cache_domain = cache_domains.game_items,
				get_data_stream = (() => this.m_itemSystem.GetAllItems())
			};
			return base.GetDataStream<SItem>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x00027FE4 File Offset: 0x000263E4
		public void AddDefaultProfileItem(ulong id, ulong itemId, ulong slotIds, string config)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.default_profile,
				set_func = (() => this.m_itemSystem.AddDefaultProfileItem(id, itemId, slotIds, config))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0002804C File Offset: 0x0002644C
		public void AddItem(SItem item)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.game_items,
				set_func = (() => this.m_itemSystem.AddItem(item))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x000280A0 File Offset: 0x000264A0
		public void UpdateItem(SItem item)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.game_items,
				set_func = (() => this.m_itemSystem.UpdateItem(item))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x000280F4 File Offset: 0x000264F4
		public void ActivateItem(ulong itemId, bool active)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.game_items,
				set_func = (() => this.m_itemSystem.ActivateItem(itemId, active))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x00028150 File Offset: 0x00026550
		public IEnumerable<SEquipItem> GetProfileItems(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<SEquipItem> options = new DALCacheProxy<IDALService>.Options<SEquipItem>
			{
				cache_domain = cache_domains.profile[profileId].items,
				get_data_stream = (() => this.m_itemSystem.GetProfileItems(profileId))
			};
			return base.GetDataStream<SEquipItem>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x000281B8 File Offset: 0x000265B8
		public IEnumerable<SEquipItem> GetDefaultProfileItems()
		{
			DALCacheProxy<IDALService>.Options<SEquipItem> options = new DALCacheProxy<IDALService>.Options<SEquipItem>
			{
				cache_domain = cache_domains.default_profile,
				get_data_stream = (() => this.m_itemSystem.GetDefaultProfileItems())
			};
			return base.GetDataStream<SEquipItem>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x000281F8 File Offset: 0x000265F8
		public IEnumerable<ulong> GetUnlockedItems(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<ulong> options = new DALCacheProxy<IDALService>.Options<ulong>
			{
				cache_domain = cache_domains.profile[profileId].unlocks,
				get_data_stream = (() => this.m_itemSystem.GetUnlockedItems(profileId))
			};
			return base.GetDataStream<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x00028260 File Offset: 0x00026660
		public ulong AddPurchasedItem(ulong profileId, ulong itemId, ulong catalogId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.AddPurchasedItem(profileId, itemId, catalogId))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x000282D4 File Offset: 0x000266D4
		public void UpdateProfileItem(ulong profileId, ulong profileItemId, ulong slotIds, ulong attachedTo, string config)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.UpdateProfileItem(profileId, profileItemId, slotIds, attachedTo, config))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x00028358 File Offset: 0x00026758
		public void RepairProfileItem(ulong profileId, ulong profileItemId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.RepairProfileItem(profileId, profileItemId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x000283C4 File Offset: 0x000267C4
		public void ExpireProfileItem(ulong profileId, ulong profileItemId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.ExpireProfileItem(profileId, profileItemId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x00028430 File Offset: 0x00026830
		public void ExpireProfileItemConfirmed(ulong profileId, ulong profileItemId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.ExpireProfileItemConfirmed(profileId, profileItemId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0002849C File Offset: 0x0002689C
		public void UnlockItem(ulong profileId, ulong itemId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].unlocks,
				set_func = (() => this.m_itemSystem.UnlockItem(profileId, itemId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x00028508 File Offset: 0x00026908
		public void LockItem(ulong profileId, ulong itemId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].unlocks,
				set_func = (() => this.m_itemSystem.LockItem(profileId, itemId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x00028574 File Offset: 0x00026974
		public ulong GiveItem(ulong profileId, ulong itemId, EProfileItemStatus status)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.GiveItem(profileId, itemId, status))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x000285E8 File Offset: 0x000269E8
		public void DeleteDefaultItems(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.DeleteDefaultItems(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x00028650 File Offset: 0x00026A50
		public void AddDefaultItem(ulong profileId, ulong itemId, ulong slotIds, ulong attached_to, string config)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.AddDefaultItem(profileId, itemId, slotIds, attached_to, config))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x000286D4 File Offset: 0x00026AD4
		public void ClearDefaultProfileItems()
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.default_profile,
				set_func = (() => this.m_itemSystem.ClearDefaultProfileItems())
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x00028714 File Offset: 0x00026B14
		public void DebugUnlockAllItems(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].unlocks,
				set_func = (() => this.m_itemSystem.DebugUnlockAllItems(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0002877C File Offset: 0x00026B7C
		public void DebugResetProfileItems(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId],
				set_func = (() => this.m_itemSystem.DebugResetProfileItems(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x000287DC File Offset: 0x00026BDC
		public void DeleteProfileItem(ulong profileId, ulong profileItemId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.DeleteProfileItem(profileId, profileItemId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x00028848 File Offset: 0x00026C48
		public ulong ExtendProfileItem(ulong profileId, ulong profileItemId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domain = cache_domains.profile[profileId].items,
				set_func = (() => this.m_itemSystem.ExtendProfileItem(profileId, profileItemId))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000557 RID: 1367
		private IItemSystem m_itemSystem;
	}
}
