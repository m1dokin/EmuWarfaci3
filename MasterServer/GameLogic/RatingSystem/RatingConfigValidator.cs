using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.GameLogic.RatingSystem.Exceptions;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000ED RID: 237
	public class RatingConfigValidator
	{
		// Token: 0x060003DD RID: 989 RVA: 0x00010D32 File Offset: 0x0000F132
		public void Validate(RatingConfig ratingConfig)
		{
			this.ValidateStepValue(ratingConfig);
			this.ValidateTopRatingCapacity(ratingConfig);
			this.ValidateZeroRatingLevel(ratingConfig);
			this.ValidateDistanceBetweenRatingLevels(ratingConfig);
			this.ValidateDistinctRequiredRatingsPoints(ratingConfig);
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00010D58 File Offset: 0x0000F158
		private void ValidateZeroRatingLevel(RatingConfig ratingConfig)
		{
			IEnumerable<RatingLevelConfig> ratingLevelConfigs = ratingConfig.RatingLevelConfigs;
			RatingLevelConfig ratingLevelConfig = ratingLevelConfigs.ElementAt(0);
			if (ratingLevelConfig.PointsRequired != 0U)
			{
				throw new InvalidRatingConfigException("Zero level rating is mandatory and should have points_required equal to 0");
			}
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00010D8C File Offset: 0x0000F18C
		private void ValidateTopRatingCapacity(RatingConfig ratingConfig)
		{
			if (ratingConfig.TopRatingCapacity > 10U * ratingConfig.Step)
			{
				string message = string.Format("Top rating capacity can't exceed {0} steps. Step: {1}", 10U, ratingConfig.Step);
				throw new InvalidRatingConfigException(message);
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00010DD4 File Offset: 0x0000F1D4
		private void ValidateDistanceBetweenRatingLevels(RatingConfig ratingConfig)
		{
			IEnumerable<RatingLevelConfig> ratingLevelConfigs = ratingConfig.RatingLevelConfigs;
			uint num = 10U * ratingConfig.Step;
			int num2 = ratingLevelConfigs.Count<RatingLevelConfig>();
			uint num3 = 0U;
			for (int i = 0; i < num2; i++)
			{
				RatingLevelConfig ratingLevelConfig = ratingLevelConfigs.ElementAt(i);
				uint pointsRequired = ratingLevelConfig.PointsRequired;
				uint num4 = pointsRequired - num3;
				if (num4 > num)
				{
					string message = string.Format("Distance between rating levels {0} and {1} invalid: {2}. Can't be more than {3} steps. Step: {4}", new object[]
					{
						i - 1,
						i,
						num4,
						10U,
						ratingConfig.Step
					});
					throw new InvalidRatingConfigException(message);
				}
				num3 = pointsRequired;
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00010E88 File Offset: 0x0000F288
		private void ValidateDistinctRequiredRatingsPoints(RatingConfig ratingConfig)
		{
			IEnumerable<RatingLevelConfig> ratingLevelConfigs = ratingConfig.RatingLevelConfigs;
			IEnumerable<uint> source = (from x in ratingLevelConfigs
			select x.PointsRequired).Distinct<uint>();
			if (source.Count<uint>() != ratingLevelConfigs.Count<RatingLevelConfig>())
			{
				string message = string.Format("Rating levels with equal points_required values are present in config", new object[0]);
				throw new InvalidRatingConfigException(message);
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00010EF0 File Offset: 0x0000F2F0
		private void ValidateStepValue(RatingConfig ratingConfig)
		{
			if (ratingConfig.Step <= 0U)
			{
				string message = string.Format("Step value could not be <= 0", new object[0]);
				throw new InvalidRatingConfigException(message);
			}
		}

		// Token: 0x0400019E RID: 414
		public const uint MAX_STEPS_BETWEEN_RATING_LEVELS = 10U;
	}
}
