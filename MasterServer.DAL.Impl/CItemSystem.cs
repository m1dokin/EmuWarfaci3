using System;
using MasterServer.Database;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000015 RID: 21
	internal class CItemSystem : IItemSystem
	{
		// Token: 0x060000B7 RID: 183 RVA: 0x000073D7 File Offset: 0x000055D7
		public CItemSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00007408 File Offset: 0x00005608
		public DALResultMulti<SItem> GetAllItems()
		{
			CacheProxy.Options<SItem> options = new CacheProxy.Options<SItem>
			{
				db_query = "CALL GetAllItems()",
				db_serializer = this.m_item_serializer
			};
			return this.m_dal.CacheProxy.GetStream<SItem>(options);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00007448 File Offset: 0x00005648
		public DALResultVoid AddDefaultProfileItem(ulong id, ulong itemId, ulong slotIds, string config)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL AddDefaultProfileItem(?id, ?item, ?slot, ?cfg)", new object[]
			{
				"?id",
				id,
				"?item",
				itemId,
				"?slot",
				slotIds,
				"?cfg",
				config
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000074C0 File Offset: 0x000056C0
		public DALResultVoid AddItem(SItem item)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL AddItem(?active, ?locked, ?name, ?shop, ?slots, ?type)", new object[]
			{
				"?active",
				(!item.Active) ? 0 : 1,
				"?locked",
				(!item.Locked) ? 0 : 1,
				"?name",
				item.Name,
				"?shop",
				(!item.ShopContent) ? 0 : 1,
				"?slots",
				item.Slots,
				"?type",
				item.Type
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00007594 File Offset: 0x00005794
		public DALResultVoid UpdateItem(SItem item)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UpdateItem(?active, ?locked, ?name, ?shop, ?slots, ?type)", new object[]
			{
				"?active",
				(!item.Active) ? 0 : 1,
				"?locked",
				(!item.Locked) ? 0 : 1,
				"?name",
				item.Name,
				"?shop",
				(!item.ShopContent) ? 0 : 1,
				"?slots",
				item.Slots,
				"?type",
				item.Type
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00007668 File Offset: 0x00005868
		public DALResultVoid ActivateItem(ulong item_id, bool active)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL ActivateItem(?id, ?active)", new object[]
			{
				"?id",
				item_id,
				"?active",
				(!active) ? 0 : 1
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000076CC File Offset: 0x000058CC
		public DALResultMulti<SEquipItem> GetProfileItems(ulong profile_id)
		{
			CacheProxy.Options<SEquipItem> options = new CacheProxy.Options<SEquipItem>
			{
				db_serializer = this.m_profile_item_serializer
			};
			options.query("CALL GetProfileItems(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.GetStream<SEquipItem>(options);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00007720 File Offset: 0x00005920
		public DALResultMulti<SEquipItem> GetDefaultProfileItems()
		{
			CacheProxy.Options<SEquipItem> options = new CacheProxy.Options<SEquipItem>
			{
				db_query = "CALL GetDefaultProfileItems()",
				db_serializer = this.m_profile_default_item_serializer,
				db_mode = DBAccessMode.Slave
			};
			cache_domain default_profile = cache_domains.default_profile;
			return this.m_dal.CacheProxy.GetStream<SEquipItem>(options);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000776C File Offset: 0x0000596C
		public DALResultMulti<ulong> GetUnlockedItems(ulong profile_id)
		{
			CacheProxy.Options<ulong> options = new CacheProxy.Options<ulong>
			{
				db_serializer = new UInt64FieldSerializer("id")
			};
			options.query("CALL GetUnlockedItems(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.GetStream<ulong>(options);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000077C4 File Offset: 0x000059C4
		public DALResult<ulong> AddPurchasedItem(ulong profile_id, ulong item_id, ulong catalog_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT AddPurchasedItem(?pid, ?item, ?cid, ?status)", new object[]
			{
				"?pid",
				profile_id,
				"?item",
				item_id,
				"?cid",
				catalog_id,
				"?status",
				EProfileItemStatus.BOUGHT.ToString().ToLower()
			});
			ulong val = ulong.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00007864 File Offset: 0x00005A64
		public DALResultVoid UpdateProfileItem(ulong profile_id, ulong profile_item_id, ulong slot_ids, ulong attached_to, string config)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UpdateInventoryItem(?id, ?profile_id, ?slot, ?att, ?cfg)", new object[]
			{
				"?id",
				profile_item_id,
				"?profile_id",
				profile_id,
				"?slot",
				slot_ids,
				"?att",
				attached_to,
				"?cfg",
				config
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000078F0 File Offset: 0x00005AF0
		public DALResultVoid RepairProfileItem(ulong profile_id, ulong profile_item_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL RepairProfileItem(?id, ?profile_id)", new object[]
			{
				"?id",
				profile_item_id,
				"?profile_id",
				profile_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00007948 File Offset: 0x00005B48
		public DALResultVoid ExpireProfileItem(ulong profile_id, ulong profile_item_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL ExpireProfileItem(?id, ?profile_id)", new object[]
			{
				"?id",
				profile_item_id,
				"?profile_id",
				profile_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000079A0 File Offset: 0x00005BA0
		public DALResultVoid ExpireProfileItemConfirmed(ulong profile_id, ulong profile_item_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL ExpireProfileItemConfirmed(?id, ?profile_id)", new object[]
			{
				"?id",
				profile_item_id,
				"?profile_id",
				profile_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000079F8 File Offset: 0x00005BF8
		public DALResultVoid UnlockItem(ulong profileId, ulong itemId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UnlockItem(?pid, ?item)", new object[]
			{
				"?pid",
				profileId,
				"?item",
				itemId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00007A50 File Offset: 0x00005C50
		public DALResultVoid LockItem(ulong profileId, ulong itemId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL LockItem(?pid, ?item)", new object[]
			{
				"?pid",
				profileId,
				"?item",
				itemId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00007AA8 File Offset: 0x00005CA8
		public DALResult<ulong> GiveItem(ulong profileId, ulong itemId, EProfileItemStatus status)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT GiveItem(?pid, ?item, ?status)", new object[]
			{
				"?pid",
				profileId,
				"?item",
				itemId,
				"?status",
				status.ToString().ToLower()
			});
			ulong val = ulong.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00007B34 File Offset: 0x00005D34
		public DALResultVoid DeleteDefaultItems(ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteDefaultItems(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00007B7C File Offset: 0x00005D7C
		public DALResultVoid AddDefaultItem(ulong profileId, ulong itemId, ulong slotIds, ulong attached_to, string config)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL AddDefaultItem( ?pid, ?item, ?slot, ?attached, ?config)", new object[]
			{
				"?pid",
				profileId,
				"?item",
				itemId,
				"?slot",
				slotIds,
				"?attached",
				attached_to,
				"?config",
				config
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00007C08 File Offset: 0x00005E08
		public DALResultVoid ClearDefaultProfileItems()
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL ClearDefaultProfileItems()", new object[0]);
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00007C40 File Offset: 0x00005E40
		public DALResultVoid DeleteProfileItem(ulong profile_id, ulong profile_item_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteProfileItem(?pid, ?profile_id)", new object[]
			{
				"?pid",
				profile_item_id,
				"?profile_id",
				profile_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00007C98 File Offset: 0x00005E98
		public DALResult<ulong> ExtendProfileItem(ulong profileId, ulong profile_item_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT ExtendProfileItem(?id, ?profile_id)", new object[]
			{
				"?id",
				profile_item_id,
				"?profile_id",
				profileId
			});
			ulong val = ulong.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00007D08 File Offset: 0x00005F08
		public DALResultVoid DebugUnlockAllItems(ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DebugUnlockAllItems(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00007D50 File Offset: 0x00005F50
		public DALResultVoid DebugResetProfileItems(ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DebugResetProfileItems(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0400004B RID: 75
		private DAL m_dal;

		// Token: 0x0400004C RID: 76
		private ItemSerializer m_item_serializer = new ItemSerializer();

		// Token: 0x0400004D RID: 77
		private EquipItemSerializer m_profile_item_serializer = new EquipItemSerializer();

		// Token: 0x0400004E RID: 78
		private ProfileDefaultItemSerializer m_profile_default_item_serializer = new ProfileDefaultItemSerializer();
	}
}
