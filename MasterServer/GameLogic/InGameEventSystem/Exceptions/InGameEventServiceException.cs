using System;

namespace MasterServer.GameLogic.InGameEventSystem.Exceptions
{
	// Token: 0x02000308 RID: 776
	public class InGameEventServiceException : Exception
	{
		// Token: 0x060011EC RID: 4588 RVA: 0x00046EA9 File Offset: 0x000452A9
		public InGameEventServiceException()
		{
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x00046EB1 File Offset: 0x000452B1
		public InGameEventServiceException(string message) : base(message)
		{
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x00046EBA File Offset: 0x000452BA
		public InGameEventServiceException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
