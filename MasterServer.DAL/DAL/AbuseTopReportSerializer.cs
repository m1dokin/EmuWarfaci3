using System;

namespace MasterServer.DAL
{
	// Token: 0x02000003 RID: 3
	public class AbuseTopReportSerializer : IDBSerializer<SAbuseTopReport>
	{
		// Token: 0x06000003 RID: 3 RVA: 0x000020B8 File Offset: 0x000004B8
		public void Deserialize(IDataReaderEx reader, out SAbuseTopReport ret)
		{
			ret = default(SAbuseTopReport);
			ret.ProfileId = ulong.Parse(reader["id"].ToString());
			ret.Nickname = reader["nickname"].ToString();
			ret.RankId = int.Parse(reader["rank_id"].ToString());
			ret.TotalUserReports = ulong.Parse(reader["total_user_reports"].ToString());
			ret.TotalAutoReports = ulong.Parse(reader["total_auto_reports"].ToString());
		}
	}
}
