using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RewardSystem.Exceptions
{
	// Token: 0x020000D9 RID: 217
	public class InvalidMinRankRestrictionException : ChannelRankRestrictionConfigException
	{
		// Token: 0x0600038A RID: 906 RVA: 0x0000FE2B File Offset: 0x0000E22B
		public InvalidMinRankRestrictionException(Resources.ChannelType channelType, uint minRank) : base(InvalidMinRankRestrictionException.CreateErrorMessage(channelType, minRank))
		{
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0000FE3C File Offset: 0x0000E23C
		private static string CreateErrorMessage(Resources.ChannelType channelType, uint minRank)
		{
			return string.Format("MinRank restriction for channel {0} should be equal to 1. Now: {1}", channelType, minRank);
		}
	}
}
