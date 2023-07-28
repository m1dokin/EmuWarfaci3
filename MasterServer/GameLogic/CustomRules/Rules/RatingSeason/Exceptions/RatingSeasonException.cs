using System;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason.Exceptions
{
	// Token: 0x020000A2 RID: 162
	internal class RatingSeasonException : ApplicationException
	{
		// Token: 0x0600027B RID: 635 RVA: 0x0000C243 File Offset: 0x0000A643
		protected RatingSeasonException(string message) : base(message)
		{
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000C24C File Offset: 0x0000A64C
		protected RatingSeasonException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
