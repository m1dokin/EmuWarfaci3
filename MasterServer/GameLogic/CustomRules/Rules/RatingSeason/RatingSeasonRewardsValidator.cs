using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core.Configs;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason.Exceptions;
using MasterServer.GameLogic.RatingSystem;
using MasterServer.GameLogic.SpecialProfileRewards;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x020000A3 RID: 163
	internal class RatingSeasonRewardsValidator
	{
		// Token: 0x0600027D RID: 637 RVA: 0x0000CA0A File Offset: 0x0000AE0A
		public RatingSeasonRewardsValidator(ISpecialProfileRewardService specialRewards, IConfigProvider<RatingConfig> ratingConfigProvider)
		{
			this.m_specialRewards = specialRewards;
			this.m_ratingConfigProvider = ratingConfigProvider;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000CA20 File Offset: 0x0000AE20
		public void Validate(Dictionary<uint, RatingSeasonReward> rewards)
		{
			this.ValidateRewardNotConfiguredForZeroRatingLevel(rewards);
			this.ValidateAllRatingLevelsInRewardsExist(rewards);
			this.ValidateAllRatingLevelsExceptZeroLevelCoveredWithRatingAchievedReward(rewards);
			this.ValidateSpecialRewardsExist(rewards);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000CA40 File Offset: 0x0000AE40
		private void ValidateRewardNotConfiguredForZeroRatingLevel(Dictionary<uint, RatingSeasonReward> rewards)
		{
			RatingSeasonReward arg;
			if (rewards.TryGetValue(0U, out arg))
			{
				string message = string.Format("Reward nodes for 0 rating level should not be present in {0} reward configs: {1}", "rating_season_rule", arg);
				throw new RatingSeasonRewardsValidationException(message);
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000CA74 File Offset: 0x0000AE74
		private void ValidateAllRatingLevelsInRewardsExist(Dictionary<uint, RatingSeasonReward> rewards)
		{
			RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
			IEnumerable<RatingLevelConfig> ratingLevelConfigs = ratingConfig.RatingLevelConfigs;
			IEnumerable<uint> source = from x in ratingLevelConfigs
			where x.Level > 0U
			select x.Level;
			IEnumerable<uint> source2 = from x in rewards
			select x.Key;
			if (source.Count<uint>() < source2.Count<uint>())
			{
				string message = string.Format("{0} has rewards for unexisting rating level(s)", "rating_season_rule");
				throw new RatingSeasonRewardsValidationException(message);
			}
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000CB28 File Offset: 0x0000AF28
		private void ValidateAllRatingLevelsExceptZeroLevelCoveredWithRatingAchievedReward(Dictionary<uint, RatingSeasonReward> rewards)
		{
			RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
			IEnumerable<RatingLevelConfig> ratingLevelConfigs = ratingConfig.RatingLevelConfigs;
			IEnumerable<uint> first = from x in ratingLevelConfigs
			where x.Level > 0U
			select x.Level;
			IEnumerable<uint> second = from x in rewards
			where x.Value.HasRatingAchievedReward
			select x.Key;
			IEnumerable<uint> enumerable = first.Except(second);
			if (enumerable.Any<uint>())
			{
				string message = string.Format("Following rating levels missing rating achieved reward: {0}", string.Join<uint>(", ", enumerable));
				throw new RatingSeasonRewardsValidationException(message);
			}
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000CC0C File Offset: 0x0000B00C
		private void ValidateSpecialRewardsExist(Dictionary<uint, RatingSeasonReward> rewards)
		{
			foreach (KeyValuePair<uint, RatingSeasonReward> keyValuePair in rewards)
			{
				RatingSeasonReward value = keyValuePair.Value;
				this.ValidateSpecialRewardPresence(value.RatingAchievedRewardName);
				this.ValidateSpecialRewardItemsAmount(value.RatingAchievedRewardName);
				if (value.HasSeasonResultReward)
				{
					this.ValidateSpecialRewardPresence(value.SeasonResultRewardName);
					this.ValidateSpecialRewardItemsAmount(value.SeasonResultRewardName);
				}
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000CCA0 File Offset: 0x0000B0A0
		private void ValidateSpecialRewardItemsAmount(string rewardSetName)
		{
			RewardSet rewardSet = this.m_specialRewards.GetRewardSet(rewardSetName);
			if (rewardSet.Count() > 8)
			{
				string message = string.Format("Special reward set '{0}' contains more items than possible: {1}. Maximum: {2}", rewardSetName, rewardSet.Count(), 8);
				throw new RatingSeasonRewardsValidationException(message);
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000CCEC File Offset: 0x0000B0EC
		private void ValidateSpecialRewardPresence(string rewardSetName)
		{
			if (this.m_specialRewards.GetRewardSet(rewardSetName) == null)
			{
				string message = string.Format("Special reward set not found: {0}", rewardSetName);
				throw new RatingSeasonRewardsValidationException(message);
			}
		}

		// Token: 0x04000112 RID: 274
		public const int UI_LIMIT_MAX_ITEMS_SUPPORTED_PER_RATING_REWARD = 8;

		// Token: 0x04000113 RID: 275
		private readonly ISpecialProfileRewardService m_specialRewards;

		// Token: 0x04000114 RID: 276
		private readonly IConfigProvider<RatingConfig> m_ratingConfigProvider;
	}
}
