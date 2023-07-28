using System;

namespace MasterServer.GameLogic.RatingSystem.Exceptions
{
	// Token: 0x02000098 RID: 152
	internal class RatingWinStreakConfigAttributeMissingException : InvalidRatingWinStreakConfigException
	{
		// Token: 0x0600024F RID: 591 RVA: 0x0000C430 File Offset: 0x0000A830
		public RatingWinStreakConfigAttributeMissingException(Exception ex) : base("Mandatory attribute in rating win streak config is missing", ex)
		{
		}
	}
}
