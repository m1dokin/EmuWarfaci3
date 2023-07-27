using System;
using MasterServer.DAL.RatingSystem;
using MasterServer.Database;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000020 RID: 32
	internal class RatingSystem : IRatingSystem
	{
		// Token: 0x0600016E RID: 366 RVA: 0x0000DE10 File Offset: 0x0000C010
		public RatingSystem(DAL dal)
		{
			this.m_dal = dal;
			this.m_ratingInfoSerializer = new RatingInfoSerializer();
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000DE2C File Offset: 0x0000C02C
		public DALResult<RatingInfo> GetRating(ulong profileId)
		{
			CacheProxy.Options<RatingInfo> options = new CacheProxy.Options<RatingInfo>
			{
				db_serializer = this.m_ratingInfoSerializer
			};
			options.query("CALL GetPvpRatingPoints(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Get<RatingInfo>(options);
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000DE80 File Offset: 0x0000C080
		public DALResult<RatingInfo> AddRatingPoints(ulong profileId, int ratingPoints, int wins, string seasonId)
		{
			CacheProxy.Options<RatingInfo> options = new CacheProxy.Options<RatingInfo>
			{
				db_serializer = this.m_ratingInfoSerializer,
				db_transaction = true
			};
			options.query("CALL AddPvpRatingPoints(?pid, ?new_points, ?new_wins, ?new_season_id)", new object[]
			{
				"?pid",
				profileId,
				"?new_points",
				ratingPoints,
				"?new_wins",
				wins,
				"?new_season_id",
				seasonId
			});
			return this.m_dal.CacheProxy.Get<RatingInfo>(options);
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000DF0C File Offset: 0x0000C10C
		public DALResultMulti<RatingInfo> GetTopRatingPlayers(string seasonId, uint playersCount)
		{
			CacheProxy.Options<RatingInfo> options = new CacheProxy.Options<RatingInfo>
			{
				db_serializer = this.m_ratingInfoSerializer,
				db_mode = DBAccessMode.Slave
			};
			options.query("CALL GetTopRatingPlayers(?sid, ?player_count)", new object[]
			{
				"?sid",
				seasonId,
				"?player_count",
				playersCount
			});
			return this.m_dal.CacheProxy.GetStream<RatingInfo>(options);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000DF74 File Offset: 0x0000C174
		public DALResult<RatingSeasonInfo> GetRatingSeasonInfo()
		{
			DALStats dalstats = new DALStats();
			RatingSeasonInfo ratingSeasonInfo;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				object obj = mySqlAccessor.ExecuteScalar(string.Format("CALL GetGameStateValue('{0}')", "rating_season_id"), new object[0]);
				object obj2 = mySqlAccessor.ExecuteScalar(string.Format("CALL GetGameStateValue('{0}')", "is_rating_season_active"), new object[0]);
				object obj3 = mySqlAccessor.ExecuteScalar(string.Format("CALL GetGameStateValue('{0}')", "rating_season_announcement_end"), new object[0]);
				object obj4 = mySqlAccessor.ExecuteScalar(string.Format("CALL GetGameStateValue('{0}')", "rating_season_games_end"), new object[0]);
				string seasonId = (obj == null) ? string.Empty : obj.ToString();
				uint value = (obj2 == null) ? 0U : uint.Parse(obj2.ToString());
				DateTime announcementEndDate = DateTime.MinValue;
				if (obj3 != null)
				{
					announcementEndDate = RatingSeasonDateParser.Parse(obj3.ToString());
				}
				DateTime gamesEndDate = DateTime.MinValue;
				if (obj4 != null)
				{
					gamesEndDate = RatingSeasonDateParser.Parse(obj4.ToString());
				}
				ratingSeasonInfo = default(RatingSeasonInfo);
				RatingSeasonInfo ratingSeasonInfo2 = ratingSeasonInfo;
				ratingSeasonInfo2.SeasonId = seasonId;
				ratingSeasonInfo2.IsActive = Convert.ToBoolean(value);
				ratingSeasonInfo2.AnnouncementEndDate = announcementEndDate;
				ratingSeasonInfo2.GamesEndDate = gamesEndDate;
				ratingSeasonInfo = ratingSeasonInfo2;
			}
			return new DALResult<RatingSeasonInfo>(ratingSeasonInfo, dalstats);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000E0D8 File Offset: 0x0000C2D8
		public DALResultVoid UpdateSeason(string seasonId, string announcementEndDate, string gamesEndDate, bool active)
		{
			RatingSystem.<UpdateSeason>c__AnonStorey1 <UpdateSeason>c__AnonStorey = new RatingSystem.<UpdateSeason>c__AnonStorey1();
			<UpdateSeason>c__AnonStorey.seasonId = seasonId;
			<UpdateSeason>c__AnonStorey.announcementEndDate = announcementEndDate;
			<UpdateSeason>c__AnonStorey.gamesEndDate = gamesEndDate;
			<UpdateSeason>c__AnonStorey.active = active;
			DALStats dalstats = new DALStats();
			using (MySqlAccessorTransaction acc = new MySqlAccessorTransaction(dalstats))
			{
				acc.Transaction(delegate()
				{
					acc.ExecuteScalar(string.Format("CALL SetGameStateValue('{0}', ?val)", "rating_season_id"), new object[]
					{
						"?val",
						<UpdateSeason>c__AnonStorey.seasonId
					});
					acc.ExecuteScalar(string.Format("CALL SetGameStateValue('{0}', ?val)", "rating_season_announcement_end"), new object[]
					{
						"?val",
						<UpdateSeason>c__AnonStorey.announcementEndDate
					});
					acc.ExecuteScalar(string.Format("CALL SetGameStateValue('{0}', ?val)", "rating_season_games_end"), new object[]
					{
						"?val",
						<UpdateSeason>c__AnonStorey.gamesEndDate
					});
					acc.ExecuteScalar(string.Format("CALL SetGameStateValue('{0}', ?val)", "is_rating_season_active"), new object[]
					{
						"?val",
						(!<UpdateSeason>c__AnonStorey.active) ? 0 : 1
					});
				});
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x0400006C RID: 108
		private const string SeasonId = "rating_season_id";

		// Token: 0x0400006D RID: 109
		private const string SeasonStatus = "is_rating_season_active";

		// Token: 0x0400006E RID: 110
		private const string SeasonAnnouncementEndDate = "rating_season_announcement_end";

		// Token: 0x0400006F RID: 111
		private const string SeasonGamesEndDate = "rating_season_games_end";

		// Token: 0x04000070 RID: 112
		private readonly DAL m_dal;

		// Token: 0x04000071 RID: 113
		private readonly RatingInfoSerializer m_ratingInfoSerializer;
	}
}
