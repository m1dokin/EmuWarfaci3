using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RewardSystem.Exceptions
{
	// Token: 0x020000DB RID: 219
	public class RankRestrictionsOverlapException : ChannelRankRestrictionConfigException
	{
		// Token: 0x0600038E RID: 910 RVA: 0x0000FEC5 File Offset: 0x0000E2C5
		public RankRestrictionsOverlapException(Resources.ChannelType prevChannel, int prevChannelMaxRank, Resources.ChannelType nextChannel, int nextChannelMinRank, int overlappedLevels) : base(RankRestrictionsOverlapException.CreateErrorMessage(prevChannel, prevChannelMaxRank, nextChannel, nextChannelMinRank, overlappedLevels))
		{
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0000FEDC File Offset: 0x0000E2DC
		private static string CreateErrorMessage(Resources.ChannelType prevChannel, int prevChannelMaxRank, Resources.ChannelType nextChannel, int nextChannelMinRank, int overlappedLevels)
		{
			return string.Format("There are overlaps between {0} MaxRank({1}) and {2} MinRank({3}) in channel restrictions. Total overlapped ranks: {4}", new object[]
			{
				prevChannel,
				prevChannelMaxRank,
				nextChannel,
				nextChannelMinRank,
				overlappedLevels
			});
		}
	}
}
