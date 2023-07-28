using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000066 RID: 102
	public class CatalogItemSerializer : IDBSerializer<CatalogItem>
	{
		// Token: 0x060000F9 RID: 249 RVA: 0x00004648 File Offset: 0x00002A48
		public void Deserialize(IDataReaderEx reader, out CatalogItem ret)
		{
			ret = default(CatalogItem);
			ret.ID = ulong.Parse(reader["catalog_id"].ToString());
			ret.Name = reader["item_name"].ToString();
			ret.Active = ParseUtils.ParseBool(reader["active"].ToString());
			ret.Type = reader["type"].ToString();
			ret.Stackable = ParseUtils.ParseBool(reader["stackable"].ToString());
			ret.MaxAmount = int.Parse(reader["max_amount"].ToString());
		}
	}
}
