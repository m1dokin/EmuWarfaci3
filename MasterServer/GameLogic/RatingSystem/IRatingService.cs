using System;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x02000416 RID: 1046
	[Contract]
	internal interface IRatingService
	{
		// Token: 0x14000049 RID: 73
		// (add) Token: 0x0600168E RID: 5774
		// (remove) Token: 0x0600168F RID: 5775
		event Action<ulong, ulong, Rating, Rating, string, ILogGroup> ProfileRatingChanged;

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06001690 RID: 5776
		uint MaxRatingLevel { get; }

		// Token: 0x06001691 RID: 5777
		Rating GetRating(ulong profileId);

		// Token: 0x06001692 RID: 5778
		void AddRatingPoints(ulong profileId, int ratingPointsToAdd, int winsToAdd, string sessionId);

		// Token: 0x06001693 RID: 5779
		void SetRatingPoints(ulong profileId, uint ratingPoints);

		// Token: 0x06001694 RID: 5780
		void ResetRating(ulong profileId, string seasonId);
	}
}
