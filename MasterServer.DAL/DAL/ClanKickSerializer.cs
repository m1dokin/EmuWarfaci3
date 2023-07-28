using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000016 RID: 22
	public class ClanKickSerializer : IDBSerializer<SClanKick>
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00002844 File Offset: 0x00000C44
		public void Deserialize(IDataReaderEx reader, out SClanKick ret)
		{
			ret = default(SClanKick);
			ret.clan_id = ulong.Parse(reader["clan_id"].ToString());
			ulong utc = ulong.Parse(reader["kick_date"].ToString());
			ret.kick_date = TimeUtils.UTCTimestampToLocalTime(utc);
		}
	}
}
