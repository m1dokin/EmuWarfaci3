using System;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200023B RID: 571
	internal class ItemServiceException : Exception
	{
		// Token: 0x06000C4F RID: 3151 RVA: 0x00009A78 File Offset: 0x00007E78
		public ItemServiceException(string message) : base(message)
		{
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x00009A81 File Offset: 0x00007E81
		public ItemServiceException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
