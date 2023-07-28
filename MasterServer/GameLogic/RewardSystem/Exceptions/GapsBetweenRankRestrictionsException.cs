using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RewardSystem.Exceptions
{
	// Token: 0x020000DA RID: 218
	public class GapsBetweenRankRestrictionsException : ChannelRankRestrictionConfigException
	{
		// Token: 0x0600038C RID: 908 RVA: 0x0000FE61 File Offset: 0x0000E261
		public GapsBetweenRankRestrictionsException(Resources.ChannelType prevChannel, int prevChannelMaxRank, Resources.ChannelType nextChannel, int nextChannelMinRank, int gapSize) : base(GapsBetweenRankRestrictionsException.CreateErrorMessage(prevChannel, prevChannelMaxRank, nextChannel, nextChannelMinRank, gapSize))
		{
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0000FE78 File Offset: 0x0000E278
		private static string CreateErrorMessage(Resources.ChannelType prevChannel, int prevChannelMaxRank, Resources.ChannelType nextChannel, int nextChannelMinRank, int gapSize)
		{
			return string.Format("There are gaps between {0} MaxRank({1}) and {2} MinRank({3}) in channel restrictions. Gap size: {4}", new object[]
			{
				prevChannel,
				prevChannelMaxRank,
				nextChannel,
				nextChannelMinRank,
				gapSize
			});
		}
	}
}
