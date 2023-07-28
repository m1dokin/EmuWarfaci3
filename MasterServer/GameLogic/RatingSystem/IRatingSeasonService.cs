using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000DF RID: 223
	[Contract]
	internal interface IRatingSeasonService
	{
		// Token: 0x14000014 RID: 20
		// (add) Token: 0x0600039D RID: 925
		// (remove) Token: 0x0600039E RID: 926
		event Action<ulong, Rating> ProfileRatingReseted;

		// Token: 0x0600039F RID: 927
		RatingSeason GetRatingSeason();

		// Token: 0x060003A0 RID: 928
		void SetupSeason(RatingSeasonRuleConfig ratingSeasonConfig, bool seasonActive);

		// Token: 0x060003A1 RID: 929
		Rating GetPlayerRating(ulong profileId);

		// Token: 0x060003A2 RID: 930
		bool SetPlayerRatingPoints(ulong profileId, uint ratingPointsToSet);

		// Token: 0x060003A3 RID: 931
		void UpdateSeasonForPlayer(ulong profileId);

		// Token: 0x060003A4 RID: 932
		IEnumerable<ulong> GetTopRatingPlayers(uint playersCount);
	}
}
