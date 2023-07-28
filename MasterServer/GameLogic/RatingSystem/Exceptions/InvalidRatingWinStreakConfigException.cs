using System;

namespace MasterServer.GameLogic.RatingSystem.Exceptions
{
	// Token: 0x02000099 RID: 153
	internal class InvalidRatingWinStreakConfigException : RatingServiceException
	{
		// Token: 0x06000250 RID: 592 RVA: 0x0000C41D File Offset: 0x0000A81D
		public InvalidRatingWinStreakConfigException(string message) : base(message)
		{
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000C426 File Offset: 0x0000A826
		public InvalidRatingWinStreakConfigException(string message, Exception ex) : base(message, ex)
		{
		}
	}
}
