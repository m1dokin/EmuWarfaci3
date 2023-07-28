using System;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason.Exceptions
{
	// Token: 0x020000A1 RID: 161
	internal class RatingSeasonRewardsParseException : RatingSeasonException
	{
		// Token: 0x06000279 RID: 633 RVA: 0x0000C9F7 File Offset: 0x0000ADF7
		public RatingSeasonRewardsParseException(string message) : base(message)
		{
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000CA00 File Offset: 0x0000AE00
		public RatingSeasonRewardsParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
