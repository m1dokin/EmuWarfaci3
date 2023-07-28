using System;

namespace MasterServer.DAL
{
	// Token: 0x02000006 RID: 6
	public class AbuseHistorySerializer : IDBSerializer<SAbuseHistory>
	{
		// Token: 0x06000011 RID: 17 RVA: 0x000021C0 File Offset: 0x000005C0
		public void Deserialize(IDataReaderEx reader, out SAbuseHistory ret)
		{
			ret = default(SAbuseHistory);
			ret.From = ulong.Parse(reader["from_pid"].ToString());
			ret.To = ulong.Parse(reader["to_pid"].ToString());
			ret.Type = reader["type"].ToString();
			ret.ReportSource = uint.Parse(reader["abuse_source"].ToString());
			ret.ReportDate = DateTime.Parse(reader["report_date"].ToString());
			ret.Message = reader["message"].ToString();
		}
	}
}
