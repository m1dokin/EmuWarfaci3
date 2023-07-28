using System;

namespace MasterServer.DAL.RatingSystem
{
	// Token: 0x0200008D RID: 141
	public class RatingInfoSerializer : IDBSerializer<RatingInfo>
	{
		// Token: 0x060001AB RID: 427 RVA: 0x000055C4 File Offset: 0x000039C4
		public void Deserialize(IDataReaderEx reader, out RatingInfo ret)
		{
			ret = default(RatingInfo);
			RatingInfo ratingInfo = ret;
			ratingInfo.ProfileId = ulong.Parse(reader["profile_id"].ToString());
			ratingInfo.RatingPoints = uint.Parse(reader["points"].ToString());
			ratingInfo.WinStreak = uint.Parse(reader["win_streak"].ToString());
			ratingInfo.SeasonId = reader["season_id"].ToString();
			ret = ratingInfo;
		}
	}
}
