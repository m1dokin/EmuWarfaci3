using System;

namespace MasterServer.DAL.RatingSystem
{
	// Token: 0x02000091 RID: 145
	public interface IRatingSystem
	{
		// Token: 0x060001B0 RID: 432
		DALResult<RatingInfo> GetRating(ulong profileId);

		// Token: 0x060001B1 RID: 433
		DALResult<RatingInfo> AddRatingPoints(ulong profileId, int ratingPoints, int wins, string seasonId);

		// Token: 0x060001B2 RID: 434
		DALResultMulti<RatingInfo> GetTopRatingPlayers(string seasonId, uint playersCount);

		// Token: 0x060001B3 RID: 435
		DALResult<RatingSeasonInfo> GetRatingSeasonInfo();

		// Token: 0x060001B4 RID: 436
		DALResultVoid UpdateSeason(string seasonId, string announcementEndDate, string gamesEndDate, bool active);
	}
}
