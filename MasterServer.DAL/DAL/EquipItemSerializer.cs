using System;

namespace MasterServer.DAL
{
	// Token: 0x02000076 RID: 118
	public class EquipItemSerializer : IDBSerializer<SEquipItem>
	{
		// Token: 0x0600014B RID: 331 RVA: 0x00004AEC File Offset: 0x00002EEC
		public void Deserialize(IDataReaderEx reader, out SEquipItem ret)
		{
			ret = new SEquipItem();
			ret.ItemID = ulong.Parse(reader["item"].ToString());
			ret.ProfileItemID = ulong.Parse(reader["id"].ToString());
			ret.ProfileID = ulong.Parse(reader["profile_id"].ToString());
			ret.AttachedTo = ulong.Parse(reader["attached_to"].ToString());
			ret.SlotIDs = ulong.Parse(reader["slot_ids"].ToString());
			ret.Config = reader["config"].ToString();
			ret.Status = (EProfileItemStatus)Enum.Parse(typeof(EProfileItemStatus), reader["status"].ToString(), true);
			ret.CatalogID = ulong.Parse(reader["catalogId"].ToString());
		}
	}
}
