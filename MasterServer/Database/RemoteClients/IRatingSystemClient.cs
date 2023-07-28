using System;
using System.Collections.Generic;
using MasterServer.DAL.RatingSystem;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x0200020F RID: 527
	internal interface IRatingSystemClient
	{
		// Token: 0x06000B71 RID: 2929
		RatingInfo GetRating(ulong profileId);

		// Token: 0x06000B72 RID: 2930
		RatingInfo AddRatingPoints(ulong profileId, int ratingPoints, int wins, string seasonId);

		// Token: 0x06000B73 RID: 2931
		IEnumerable<RatingInfo> GetTopRatingPlayers(string seasonId, uint playersCount);

		// Token: 0x06000B74 RID: 2932
		RatingSeasonInfo GetRatingSeasonInfo();

		// Token: 0x06000B75 RID: 2933
		void UpdateSeason(string seasonId, string announcementEndDate, string gamesEndDate, bool active);
	}
}
