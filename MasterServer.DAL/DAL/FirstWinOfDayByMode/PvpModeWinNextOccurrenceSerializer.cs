using System;
using Util.Common;

namespace MasterServer.DAL.FirstWinOfDayByMode
{
	// Token: 0x02000033 RID: 51
	public class PvpModeWinNextOccurrenceSerializer : IDBSerializer<PvpModeWinNextOccurrence>
	{
		// Token: 0x06000083 RID: 131 RVA: 0x000034D4 File Offset: 0x000018D4
		public void Deserialize(IDataReaderEx reader, out PvpModeWinNextOccurrence pvpModeWinNextOccurrence)
		{
			ulong utc = ulong.Parse(reader["next_occurrence"].ToString());
			pvpModeWinNextOccurrence.NextOccurence = TimeUtils.UTCTimestampToLocalTime(utc);
			pvpModeWinNextOccurrence.Mode = reader["mode"].ToString();
		}
	}
}
