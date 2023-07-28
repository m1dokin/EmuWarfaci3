using System;

namespace MasterServer.DAL
{
	// Token: 0x02000089 RID: 137
	public class SProfileInfoSerializer : IDBSerializer<SProfileInfo>
	{
		// Token: 0x060001A0 RID: 416 RVA: 0x00005230 File Offset: 0x00003630
		public void Deserialize(IDataReaderEx reader, out SProfileInfo ret)
		{
			ret = default(SProfileInfo);
			ret.Id = ulong.Parse(reader["id"].ToString());
			ret.UserID = ulong.Parse(reader["user_id"].ToString());
			ret.Nickname = reader["nickname"].ToString();
			ret.Gender = reader["gender"].ToString();
			ret.Height = float.Parse(reader["height"].ToString());
			ret.Fatness = float.Parse(reader["fatness"].ToString());
			ret.CurrentClass = uint.Parse(reader["current_class"].ToString());
			ret.Head = reader["head"].ToString();
			ret.MissionPassed = byte.Parse(reader["mission_passed"].ToString());
			ret.RankInfo = default(SRankInfo);
			ret.RankInfo.Points = ulong.Parse(reader["experience"].ToString());
			ret.RankInfo.RankId = (int)byte.Parse(reader["rank_id"].ToString());
			ret.RankInfo.RankStart = ulong.Parse(reader["rank_start"].ToString());
			ret.RankInfo.NextRankStart = ulong.Parse(reader["next_rank_start"].ToString());
			ret.Banner.Badge = uint.Parse(reader["banner_badge"].ToString());
			ret.Banner.Mark = uint.Parse(reader["banner_mark"].ToString());
			ret.Banner.Stripe = uint.Parse(reader["banner_stripe"].ToString());
			ret.LastSeenTimeUTC = ulong.Parse(reader["last_seen"].ToString());
			ret.LastRankedTimeUTC = ulong.Parse(reader["last_ranked"].ToString());
			ret.CreateTimeUTC = ulong.Parse(reader["create_time"].ToString());
			ret.BanTimeUTC = ulong.Parse(reader["ban_expire"].ToString());
			ret.MuteTimeUTC = ulong.Parse(reader["mute_expire"].ToString());
		}
	}
}
