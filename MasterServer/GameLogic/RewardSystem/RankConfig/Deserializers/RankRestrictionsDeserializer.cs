using System;
using System.Collections.Generic;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.RewardSystem.RankConfig.Deserializers
{
	// Token: 0x020000D3 RID: 211
	public class RankRestrictionsDeserializer
	{
		// Token: 0x0600036D RID: 877 RVA: 0x0000F938 File Offset: 0x0000DD38
		public Dictionary<Resources.ChannelType, ChannelRankRestriction> Deserialize(IEnumerable<ConfigSection> channelsRankRestrictions, uint globalMaxRank)
		{
			Dictionary<Resources.ChannelType, ChannelRankRestriction> dictionary = new Dictionary<Resources.ChannelType, ChannelRankRestriction>();
			foreach (ConfigSection configSection in channelsRankRestrictions)
			{
				uint channelMinRank = 1U;
				uint channelMaxRank = globalMaxRank;
				Resources.ChannelType key = Utils.ParseEnum<Resources.ChannelType>(configSection.Name);
				if (configSection.HasValue("MinRank"))
				{
					configSection.Get("MinRank", out channelMinRank);
				}
				if (configSection.HasValue("MaxRank"))
				{
					configSection.Get("MaxRank", out channelMaxRank);
				}
				ChannelRankRestriction value = new ChannelRankRestriction(channelMinRank, channelMaxRank);
				dictionary.Add(key, value);
			}
			return dictionary;
		}
	}
}
