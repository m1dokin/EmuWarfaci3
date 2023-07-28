using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL.RatingSystem;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000210 RID: 528
	internal class RatingSystemClient : DALCacheProxy<IDALService>, IRatingSystemClient
	{
		// Token: 0x06000B77 RID: 2935 RVA: 0x0002B408 File Offset: 0x00029808
		public RatingInfo GetRating(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<RatingInfo> options = new DALCacheProxy<IDALService>.Options<RatingInfo>
			{
				cache_domain = cache_domains.profile[profileId].pvp_rating_points,
				get_data = (() => this.m_ratingSystem.GetRating(profileId))
			};
			return base.GetData<RatingInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x0002B470 File Offset: 0x00029870
		public RatingInfo AddRatingPoints(ulong profileId, int ratingPoints, int wins, string seasonId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<RatingInfo> options = new DALCacheProxy<IDALService>.SetOptionsScalar<RatingInfo>
			{
				cache_domain = cache_domains.profile[profileId].pvp_rating_points,
				set_func = (() => this.m_ratingSystem.AddRatingPoints(profileId, ratingPoints, wins, seasonId))
			};
			return base.SetAndStore<RatingInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0002B4EC File Offset: 0x000298EC
		public IEnumerable<RatingInfo> GetTopRatingPlayers(string seasonId, uint playersCount)
		{
			DALCacheProxy<IDALService>.Options<RatingInfo> options = new DALCacheProxy<IDALService>.Options<RatingInfo>
			{
				get_data_stream = (() => this.m_ratingSystem.GetTopRatingPlayers(seasonId, playersCount))
			};
			return base.GetDataStream<RatingInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x0002B53C File Offset: 0x0002993C
		public RatingSeasonInfo GetRatingSeasonInfo()
		{
			DALCacheProxy<IDALService>.Options<RatingSeasonInfo> options = new DALCacheProxy<IDALService>.Options<RatingSeasonInfo>
			{
				cache_domain = cache_domains.rating_season,
				get_data = (() => this.m_ratingSystem.GetRatingSeasonInfo())
			};
			return base.GetData<RatingSeasonInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x0002B57C File Offset: 0x0002997C
		public void UpdateSeason(string seasonId, string announcementEndDate, string gamesEndDate, bool active)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.rating_season,
				set_func = (() => this.m_ratingSystem.UpdateSeason(seasonId, announcementEndDate, gamesEndDate, active))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x0002B5E4 File Offset: 0x000299E4
		internal void Reset(IRatingSystem ratingSystem)
		{
			this.m_ratingSystem = ratingSystem;
		}

		// Token: 0x04000561 RID: 1377
		private IRatingSystem m_ratingSystem;
	}
}
