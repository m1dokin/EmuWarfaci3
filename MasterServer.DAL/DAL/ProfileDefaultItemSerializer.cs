using System;

namespace MasterServer.DAL
{
	// Token: 0x02000077 RID: 119
	public class ProfileDefaultItemSerializer : IDBSerializer<SEquipItem>
	{
		// Token: 0x0600014D RID: 333 RVA: 0x00004BF4 File Offset: 0x00002FF4
		public void Deserialize(IDataReaderEx reader, out SEquipItem ret)
		{
			ret = new SEquipItem();
			ret.ItemID = ulong.Parse(reader["item"].ToString());
			ret.ProfileItemID = ulong.Parse(reader["id"].ToString());
			ret.Config = reader["config"].ToString();
			ret.SlotIDs = ulong.Parse(reader["slot_ids"].ToString());
			ret.ProfileID = 0UL;
			ret.AttachedTo = 0UL;
			ret.Status = EProfileItemStatus.DEFAULT;
			ret.CatalogID = 0UL;
		}
	}
}
