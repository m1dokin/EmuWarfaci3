using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;
using MasterServer.GameLogic.RewardSystem.Exceptions;

namespace MasterServer.GameLogic.RewardSystem.RankConfig
{
	// Token: 0x020000D1 RID: 209
	public class RankRestrictionsValidator
	{
		// Token: 0x06000362 RID: 866 RVA: 0x0000F6CC File Offset: 0x0000DACC
		public void Validate(IDictionary<Resources.ChannelType, ChannelRankRestriction> channelsRankRestrictions, uint globalMaxRank)
		{
			if (channelsRankRestrictions.Any<KeyValuePair<Resources.ChannelType, ChannelRankRestriction>>())
			{
				Dictionary<Resources.ChannelType, ChannelRankRestriction> dictionary = (from x in channelsRankRestrictions
				orderby x.Value.ChannelMinRank, x.Value.ChannelMaxRank
				select x).ToDictionary((KeyValuePair<Resources.ChannelType, ChannelRankRestriction> x) => x.Key, (KeyValuePair<Resources.ChannelType, ChannelRankRestriction> y) => y.Value);
				KeyValuePair<Resources.ChannelType, ChannelRankRestriction> keyValuePair = dictionary.First<KeyValuePair<Resources.ChannelType, ChannelRankRestriction>>();
				this.ValidateLowLevelChannelRestrictions(keyValuePair.Key, keyValuePair.Value);
				KeyValuePair<Resources.ChannelType, ChannelRankRestriction> keyValuePair2 = dictionary.Last<KeyValuePair<Resources.ChannelType, ChannelRankRestriction>>();
				this.ValidateHighLevelChannelRestrictions(keyValuePair2.Key, keyValuePair2.Value, globalMaxRank);
				this.ValidateGapsAndOverlapsBetweenChannels(dictionary);
			}
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0000F7A7 File Offset: 0x0000DBA7
		private void ValidateLowLevelChannelRestrictions(Resources.ChannelType channel, ChannelRankRestriction restriction)
		{
			if (restriction.ChannelMinRank != 1U)
			{
				throw new InvalidMinRankRestrictionException(channel, restriction.ChannelMinRank);
			}
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0000F7C2 File Offset: 0x0000DBC2
		private void ValidateHighLevelChannelRestrictions(Resources.ChannelType channel, ChannelRankRestriction restriction, uint globalMaxRank)
		{
			if (restriction.ChannelMaxRank != globalMaxRank)
			{
				throw new InvalidMaxRankRestrictionException(channel, globalMaxRank);
			}
		}

		// Token: 0x06000365 RID: 869 RVA: 0x0000F7D8 File Offset: 0x0000DBD8
		private void ValidateGapsAndOverlapsBetweenChannels(IDictionary<Resources.ChannelType, ChannelRankRestriction> channelsRankRestrictions)
		{
			if (channelsRankRestrictions.Count<KeyValuePair<Resources.ChannelType, ChannelRankRestriction>>() > 1)
			{
				KeyValuePair<Resources.ChannelType, ChannelRankRestriction> keyValuePair = channelsRankRestrictions.First<KeyValuePair<Resources.ChannelType, ChannelRankRestriction>>();
				int num = (int)keyValuePair.Value.ChannelMaxRank;
				Resources.ChannelType prevChannel = keyValuePair.Key;
				foreach (KeyValuePair<Resources.ChannelType, ChannelRankRestriction> keyValuePair2 in channelsRankRestrictions.Skip(1))
				{
					Resources.ChannelType key = keyValuePair2.Key;
					ChannelRankRestriction value = keyValuePair2.Value;
					int channelMinRank = (int)value.ChannelMinRank;
					int channelMaxRank = (int)value.ChannelMaxRank;
					int num2 = channelMinRank - num - 1;
					if (num2 > 0)
					{
						throw new GapsBetweenRankRestrictionsException(prevChannel, num, key, channelMinRank, num2);
					}
					if (num2 < 0)
					{
						throw new RankRestrictionsOverlapException(prevChannel, num, key, channelMinRank, Math.Abs(num2));
					}
					num = channelMaxRank;
					prevChannel = key;
				}
			}
		}
	}
}
