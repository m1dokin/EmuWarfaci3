using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000047 RID: 71
	public class PendingNotificationSerializer : IDBSerializer<SPendingNotification>
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00003C30 File Offset: 0x00002030
		public void Deserialize(IDataReaderEx reader, out SPendingNotification ret)
		{
			ret = default(SPendingNotification);
			ret.ID = ulong.Parse(reader["id"].ToString());
			ret.ProfileId = ulong.Parse(reader["profile_id"].ToString());
			ret.Type = uint.Parse(reader["type"].ToString());
			ret.ConfirmationType = uint.Parse(reader["confirmation_type"].ToString());
			ret.Data = (byte[])reader["data"];
			ret.Message = reader["description"].ToString();
			ulong utc = ulong.Parse(reader["expation_time"].ToString());
			ret.ExpirationTimeUTC = TimeUtils.UTCTimestampToUTCTime(utc);
		}
	}
}
