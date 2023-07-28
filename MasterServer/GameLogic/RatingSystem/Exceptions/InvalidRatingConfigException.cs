using System;

namespace MasterServer.GameLogic.RatingSystem.Exceptions
{
	// Token: 0x020000EC RID: 236
	internal class InvalidRatingConfigException : RatingServiceException
	{
		// Token: 0x060003DB RID: 987 RVA: 0x00010D21 File Offset: 0x0000F121
		public InvalidRatingConfigException(string message) : base(message)
		{
		}
	}
}
