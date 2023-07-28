using System;
using HK2Net;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x02000415 RID: 1045
	[Contract]
	internal interface IDebugRatingService
	{
		// Token: 0x0600168D RID: 5773
		void SetRatingWinStreak(ulong profileId, uint winStreak);
	}
}
