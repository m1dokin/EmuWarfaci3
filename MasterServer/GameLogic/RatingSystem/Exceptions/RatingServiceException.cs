using System;

namespace MasterServer.GameLogic.RatingSystem.Exceptions
{
	// Token: 0x02000412 RID: 1042
	internal class RatingServiceException : ApplicationException
	{
		// Token: 0x06001674 RID: 5748 RVA: 0x0000C40A File Offset: 0x0000A80A
		protected RatingServiceException(string message) : base(message)
		{
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x0000C413 File Offset: 0x0000A813
		protected RatingServiceException(string message, Exception ex) : base(message, ex)
		{
		}
	}
}
