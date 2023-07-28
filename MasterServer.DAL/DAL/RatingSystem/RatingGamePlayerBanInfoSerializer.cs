using System;
using Util.Common;

namespace MasterServer.DAL.RatingSystem
{
	// Token: 0x0200008B RID: 139
	public class RatingGamePlayerBanInfoSerializer : IDBSerializer<RatingGamePlayerBanInfo>
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x0000552C File Offset: 0x0000392C
		public void Deserialize(IDataReaderEx reader, out RatingGamePlayerBanInfo ret)
		{
			ret = new RatingGamePlayerBanInfo();
			ulong utc = ulong.Parse(reader["unban_time"].ToString());
			ret.UnbanTime = TimeUtils.UTCTimestampToUTCTime(utc);
			TimeSpan timeSpan = ret.UnbanTime - DateTime.UtcNow;
			ret.BanTimeout = ((!(ret.UnbanTime > DateTime.UtcNow)) ? 0UL : ((ulong)timeSpan.TotalSeconds));
		}
	}
}
