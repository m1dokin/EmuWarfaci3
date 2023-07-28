using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000228 RID: 552
	[ConsoleCmdAttributes(CmdName = "shop_get_catalog", ArgsSize = 0)]
	internal class ShopGetItemsCmd : IConsoleCmd
	{
		// Token: 0x06000BD7 RID: 3031 RVA: 0x0002D191 File Offset: 0x0002B591
		public ShopGetItemsCmd(ICatalogService catalogService)
		{
			this.m_catalogService = catalogService;
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x0002D1A0 File Offset: 0x0002B5A0
		public void ExecuteCmd(string[] args)
		{
			Dictionary<string, CatalogItem> catalogItems = this.m_catalogService.GetCatalogItems();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CatalogItem catalogItem in catalogItems.Values)
			{
				stringBuilder.AppendFormat("{0}\n", catalogItem.ToString());
			}
			Log.Info<string>("{0}", stringBuilder.ToString());
		}

		// Token: 0x04000584 RID: 1412
		private readonly ICatalogService m_catalogService;
	}
}
