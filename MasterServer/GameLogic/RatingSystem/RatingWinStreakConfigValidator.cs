using System;
using MasterServer.GameLogic.RatingSystem.Exceptions;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x0200009D RID: 157
	public class RatingWinStreakConfigValidator
	{
		// Token: 0x06000268 RID: 616 RVA: 0x0000C6CC File Offset: 0x0000AACC
		public void Validate(RatingWinStreakConfig winStreakConfig)
		{
			this.ValidateStartFromStreak(winStreakConfig.StartFromStreak);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000C6DC File Offset: 0x0000AADC
		private void ValidateStartFromStreak(uint startFromStreak)
		{
			if (startFromStreak < 2U)
			{
				string message = string.Format("start_from_streak value can't be lower than {0}. current: {1}", 2U, startFromStreak);
				throw new InvalidRatingWinStreakConfigException(message);
			}
		}

		// Token: 0x0400010C RID: 268
		public const uint MIN_START_FROM_STREAK_VALUE = 2U;
	}
}
