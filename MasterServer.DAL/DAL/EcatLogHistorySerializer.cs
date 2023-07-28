using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x0200001F RID: 31
	public class EcatLogHistorySerializer : IDBSerializer<EcatLogHistory>
	{
		// Token: 0x0600004F RID: 79 RVA: 0x00002E80 File Offset: 0x00001280
		public void Deserialize(IDataReaderEx reader, out EcatLogHistory ret)
		{
			ret = default(EcatLogHistory);
			ret.id = ulong.Parse(reader["id"].ToString());
			ret.customer_id = ulong.Parse(reader["customer_id"].ToString());
			ret.realm_id = int.Parse(reader["realm_id"].ToString());
			ret.catalog_id = ulong.Parse(reader["catalog_id"].ToString());
			ret.currency = int.Parse(reader["currency"].ToString());
			ret.price = ulong.Parse(reader["price"].ToString());
			ret.time = TimeUtils.UTCTimestampToUTCTime(ulong.Parse(reader["time"].ToString()));
			ret.action = int.Parse(reader["action"].ToString());
		}
	}
}
