using System;

namespace MasterServer.DAL.Exceptions
{
	// Token: 0x02000031 RID: 49
	public class DalException : Exception
	{
		// Token: 0x0600007D RID: 125 RVA: 0x000034B8 File Offset: 0x000018B8
		public DalException(string message) : base(message)
		{
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000034C1 File Offset: 0x000018C1
		public DalException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
