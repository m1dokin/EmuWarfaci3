using System;

namespace MasterServer.DAL
{
	// Token: 0x0200000E RID: 14
	public class LeaderboardEntrySerializer : IDBSerializer<LeaderboardEntry>
	{
		// Token: 0x0600001D RID: 29 RVA: 0x000023D4 File Offset: 0x000007D4
		public void Deserialize(IDataReaderEx reader, out LeaderboardEntry ret)
		{
			ret = default(LeaderboardEntry);
			ret.Rank = int.Parse(reader["rank"].ToString());
			ret.ProfileID = ulong.Parse(reader["profile_id"].ToString());
			ret.Nickname = reader["nickname"].ToString();
			ret.Class = int.Parse(reader["class"].ToString());
			ret.Experience = ulong.Parse(reader["total"].ToString());
		}
	}
}
