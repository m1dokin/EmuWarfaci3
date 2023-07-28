using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200032F RID: 815
	[Contract]
	internal interface IItemStatsValidator
	{
		// Token: 0x06001258 RID: 4696
		void Validate(IEnumerable<StoreOffer> offers);
	}
}
