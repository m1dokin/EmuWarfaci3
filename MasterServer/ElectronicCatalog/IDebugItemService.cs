using System;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000237 RID: 567
	[Contract]
	internal interface IDebugItemService
	{
		// Token: 0x06000C2C RID: 3116
		ConsumeItemResponse ConsumeItem(ulong userId, ulong profileId, string itemName, ushort quantity);
	}
}
