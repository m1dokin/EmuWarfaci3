using System;

namespace MasterServer.DAL
{
	// Token: 0x02000094 RID: 148
	public class SponsorPointsSerializer : IDBSerializer<SSponsorPoints>
	{
		// Token: 0x060001BD RID: 445 RVA: 0x00005704 File Offset: 0x00003B04
		public void Deserialize(IDataReaderEx reader, out SSponsorPoints ret)
		{
			ret = default(SSponsorPoints);
			ret.SponsorID = uint.Parse(reader["sponsor_id"].ToString());
			ret.NextUnlockItemId = ulong.Parse(reader["next_unlock_item_id"].ToString());
			ret.RankInfo = default(SRankInfo);
			ret.RankInfo.Points = (ulong)uint.Parse(reader["points"].ToString());
			ret.RankInfo.RankId = (int)byte.Parse(reader["stage_id"].ToString());
			ret.RankInfo.RankStart = ulong.Parse(reader["stage_start"].ToString());
			ret.RankInfo.NextRankStart = ulong.Parse(reader["next_stage_start"].ToString());
		}
	}
}
