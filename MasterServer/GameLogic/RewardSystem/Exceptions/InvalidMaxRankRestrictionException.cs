using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.RewardSystem.Exceptions
{
	// Token: 0x020000DC RID: 220
	public class InvalidMaxRankRestrictionException : ChannelRankRestrictionConfigException
	{
		// Token: 0x06000390 RID: 912 RVA: 0x0000FF29 File Offset: 0x0000E329
		public InvalidMaxRankRestrictionException(Resources.ChannelType channelType, uint globalMaxRank) : base(InvalidMaxRankRestrictionException.CreateErrorMessage(channelType, globalMaxRank))
		{
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0000FF38 File Offset: 0x0000E338
		private static string CreateErrorMessage(Resources.ChannelType channelType, uint globalMaxRank)
		{
			return string.Format("MaxRank restriction for channel {0} should be equal to experience curve max rank({1})", channelType, globalMaxRank);
		}
	}
}
