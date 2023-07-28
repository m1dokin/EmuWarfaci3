using System;

namespace MasterServer.DAL
{
	// Token: 0x0200001D RID: 29
	public class AbuseReportSerializer : IDBSerializer<SAbuseReport>
	{
		// Token: 0x0600004C RID: 76 RVA: 0x00002D6C File Offset: 0x0000116C
		public void Deserialize(IDataReaderEx reader, out SAbuseReport ret)
		{
			ret = default(SAbuseReport);
			ret.From = ulong.Parse(reader["from_pid"].ToString());
			ret.To = ulong.Parse(reader["to_pid"].ToString());
			ret.Type = reader["type"].ToString();
			ret.Timestamp = DateTime.Parse(reader["timestamp"].ToString());
		}
	}
}
