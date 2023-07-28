using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000069 RID: 105
	[Contract]
	internal interface IItemRepairDescriptionRepository
	{
		// Token: 0x0600018E RID: 398
		bool GetRepairItemDesc(ulong itemId, ulong catalogId, out SRepairItemDesc repairDesc);

		// Token: 0x0600018F RID: 399
		bool GetRepairItemDesc(ulong itemId, ulong catalogId, IEnumerable<StoreOffer> storeOffers, Dictionary<ulong, SRepairItemDesc> boxedItemRepairDesc, out SRepairItemDesc repairDesc);

		// Token: 0x06000190 RID: 400
		bool GetBoxedItemRepairDesc(ulong itemId, out SRepairItemDesc repairInfo);

		// Token: 0x06000191 RID: 401
		Dictionary<ulong, SRepairItemDesc> ParseBoxedItemRepairData(IEnumerable<StoreOffer> eStoreList);
	}
}
