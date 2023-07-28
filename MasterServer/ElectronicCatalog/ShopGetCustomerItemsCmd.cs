using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200022A RID: 554
	[ConsoleCmdAttributes(CmdName = "shop_get_customer_items", ArgsSize = 1)]
	internal class ShopGetCustomerItemsCmd : IConsoleCmd
	{
		// Token: 0x06000BDB RID: 3035 RVA: 0x0002D30C File Offset: 0x0002B70C
		public ShopGetCustomerItemsCmd(ICatalogService catalogService)
		{
			this.m_catalogService = catalogService;
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x0002D31C File Offset: 0x0002B71C
		public void ExecuteCmd(string[] args)
		{
			ulong customerId = ulong.Parse(args[1]);
			Dictionary<ulong, CustomerItem> customerItems = this.m_catalogService.GetCustomerItems(customerId);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CustomerItem arg in customerItems.Values)
			{
				stringBuilder.AppendFormat("{0}\n", arg);
			}
			Log.Info<string>("{0}", stringBuilder.ToString());
		}

		// Token: 0x04000586 RID: 1414
		private readonly ICatalogService m_catalogService;
	}
}
