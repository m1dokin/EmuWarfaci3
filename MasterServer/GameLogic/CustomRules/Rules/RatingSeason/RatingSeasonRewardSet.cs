using System;
using System.Collections.Generic;
using System.Text;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x020000A5 RID: 165
	public class RatingSeasonRewardSet
	{
		// Token: 0x06000290 RID: 656 RVA: 0x0000CF44 File Offset: 0x0000B344
		public RatingSeasonRewardSet(Dictionary<uint, RatingSeasonReward> rewards)
		{
			Dictionary<uint, RatingSeasonReward> dictionary = this.InverseRewardLevels(rewards);
			dictionary.Add(0U, new RatingSeasonReward(string.Empty, string.Empty));
			this.m_rewards = dictionary;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000CF7C File Offset: 0x0000B37C
		public RatingSeasonReward GetRewardForRatingLevel(uint ratingLevel)
		{
			return this.m_rewards[ratingLevel];
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000CF98 File Offset: 0x0000B398
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<uint, RatingSeasonReward> keyValuePair in this.m_rewards)
			{
				uint key = keyValuePair.Key;
				RatingSeasonReward value = keyValuePair.Value;
				string value2 = string.Format("Level {0} rewards - {1}", key, value);
				stringBuilder.AppendLine(value2);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000D028 File Offset: 0x0000B428
		private Dictionary<uint, RatingSeasonReward> InverseRewardLevels(Dictionary<uint, RatingSeasonReward> rewards)
		{
			Dictionary<uint, RatingSeasonReward> dictionary = new Dictionary<uint, RatingSeasonReward>();
			uint num = (uint)(rewards.Count + 1);
			foreach (KeyValuePair<uint, RatingSeasonReward> keyValuePair in rewards)
			{
				RatingSeasonReward value = keyValuePair.Value;
				uint key = num - keyValuePair.Key;
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		// Token: 0x0400011C RID: 284
		private Dictionary<uint, RatingSeasonReward> m_rewards;
	}
}
