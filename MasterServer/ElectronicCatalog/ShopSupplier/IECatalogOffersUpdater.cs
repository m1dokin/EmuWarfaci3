using System;
using HK2Net;
using MasterServer.Database;

namespace MasterServer.ElectronicCatalog.ShopSupplier
{
	// Token: 0x02000252 RID: 594
	[Contract]
	public interface IECatalogOffersUpdater
	{
		// Token: 0x06000D1A RID: 3354
		void Update(IDBUpdateService updater);
	}
}
