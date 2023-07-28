using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000011 RID: 17
	public class AnnouncementSerializer : IDBSerializer<Announcement>
	{
		// Token: 0x06000025 RID: 37 RVA: 0x00002700 File Offset: 0x00000B00
		public void Deserialize(IDataReaderEx reader, out Announcement ret)
		{
			ret = new Announcement();
			ret.ID = ulong.Parse(reader["id"].ToString());
			ret.StartTimeUTC = TimeUtils.UTCTimestampToUTCTime(ulong.Parse(reader["start_time"].ToString()));
			ret.EndTimeUTC = TimeUtils.UTCTimestampToUTCTime(ulong.Parse(reader["end_time"].ToString()));
			ret.IsSystem = (int.Parse(reader["priority"].ToString()) > 0);
			ret.RepeatTimes = uint.Parse(reader["repeat_times"].ToString());
			ret.Target = ulong.Parse(reader["target"].ToString());
			ret.Channel = reader["channel"].ToString();
			ret.Server = reader["server"].ToString();
			ret.Message = reader["message"].ToString();
			ret.Place = (EAnnouncementPlace)Enum.Parse(typeof(EAnnouncementPlace), reader["place"].ToString(), true);
		}
	}
}
