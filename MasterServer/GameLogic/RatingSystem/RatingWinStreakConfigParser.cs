using System;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.RatingSystem.Exceptions;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x0200009B RID: 155
	public class RatingWinStreakConfigParser
	{
		// Token: 0x0600025D RID: 605 RVA: 0x0000C4EC File Offset: 0x0000A8EC
		public RatingWinStreakConfig Parse(ConfigSection winStreakConfigData)
		{
			if (winStreakConfigData == null)
			{
				throw new InvalidRatingWinStreakConfigException("Rating win streak config is missing from rating_curve.xml");
			}
			bool enabled;
			uint bonusAmount;
			uint startFromStreak;
			uint applyBelowRating;
			try
			{
				winStreakConfigData.Get("enabled", out enabled);
				winStreakConfigData.Get("bonus_amount", out bonusAmount);
				winStreakConfigData.Get("start_from_streak", out startFromStreak);
				winStreakConfigData.Get("apply_below_rating", out applyBelowRating);
			}
			catch (Exception ex)
			{
				throw new RatingWinStreakConfigAttributeMissingException(ex);
			}
			return new RatingWinStreakConfig
			{
				Enabled = enabled,
				BonusAmount = bonusAmount,
				StartFromStreak = startFromStreak,
				ApplyBelowRating = applyBelowRating
			};
		}
	}
}
