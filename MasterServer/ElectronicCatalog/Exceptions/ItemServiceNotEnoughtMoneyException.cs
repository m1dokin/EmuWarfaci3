using System;

namespace MasterServer.ElectronicCatalog.Exceptions
{
	// Token: 0x0200023C RID: 572
	internal class ItemServiceNotEnoughtMoneyException : ItemServiceException
	{
		// Token: 0x06000C51 RID: 3153 RVA: 0x0002FFDE File Offset: 0x0002E3DE
		public ItemServiceNotEnoughtMoneyException(string message) : base(message)
		{
		}
	}
}
