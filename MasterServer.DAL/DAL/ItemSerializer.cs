using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000073 RID: 115
	public class ItemSerializer : IDBSerializer<SItem>
	{
		// Token: 0x06000142 RID: 322 RVA: 0x0000494C File Offset: 0x00002D4C
		public void Deserialize(IDataReaderEx reader, out SItem ret)
		{
			ret = new SItem();
			ret.ID = ulong.Parse(reader["id"].ToString());
			ret.Name = reader["name"].ToString();
			ret.Slots = reader["slots"].ToString();
			ret.Active = ParseUtils.ParseBool(reader["active"].ToString());
			ret.Locked = ParseUtils.ParseBool(reader["locked"].ToString());
			ret.ShopContent = ParseUtils.ParseBool(reader["shopcontent"].ToString());
			ret.Type = reader["type"].ToString();
		}
	}
}
