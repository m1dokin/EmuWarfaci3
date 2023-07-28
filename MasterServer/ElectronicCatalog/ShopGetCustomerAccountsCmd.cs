using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200022B RID: 555
	[ConsoleCmdAttributes(CmdName = "shop_get_customer_accounts", ArgsSize = 1)]
	internal class ShopGetCustomerAccountsCmd : IConsoleCmd
	{
		// Token: 0x06000BDD RID: 3037 RVA: 0x0002D3AC File Offset: 0x0002B7AC
		public ShopGetCustomerAccountsCmd(ICatalogService catalogService)
		{
			this.m_catalogService = catalogService;
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x0002D3BC File Offset: 0x0002B7BC
		public void ExecuteCmd(string[] args)
		{
			ulong customerId = ulong.Parse(args[1]);
			List<CustomerAccount> customerAccounts = this.m_catalogService.GetCustomerAccounts(customerId);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CustomerAccount customerAccount in customerAccounts)
			{
				stringBuilder.AppendFormat("{0}\n", customerAccount.ToString());
			}
			Log.Info<string>("{0}", stringBuilder.ToString());
		}

		// Token: 0x04000587 RID: 1415
		private readonly ICatalogService m_catalogService;
	}
}
