using System;

namespace MasterServer.ElectronicCatalog.Exceptions
{
	// Token: 0x0200023A RID: 570
	internal class ItemServiceDalException : ItemServiceException
	{
		// Token: 0x06000C4E RID: 3150 RVA: 0x0002FFD4 File Offset: 0x0002E3D4
		public ItemServiceDalException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
