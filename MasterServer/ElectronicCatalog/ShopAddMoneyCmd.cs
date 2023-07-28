using System;
using System.Text;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200022C RID: 556
	[ConsoleCmdAttributes(CmdName = "shop_add_money", ArgsSize = 3)]
	internal class ShopAddMoneyCmd : IConsoleCmd
	{
		// Token: 0x06000BDF RID: 3039 RVA: 0x0002D454 File Offset: 0x0002B854
		public ShopAddMoneyCmd(ICatalogService catalogService)
		{
			this.m_catalogService = catalogService;
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x0002D464 File Offset: 0x0002B864
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			ulong num2 = ulong.Parse(args[2]);
			ulong num3 = ulong.Parse(args[3]);
			this.m_catalogService.AddMoney(num, (Currency)num2, num3, string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Customer id: {0}\n", num);
			stringBuilder.AppendFormat("Currency id: {0}\n", num2);
			stringBuilder.AppendFormat("Money:       {0}\n", num3);
			Log.Info<string>("{0}", stringBuilder.ToString());
		}

		// Token: 0x04000588 RID: 1416
		private readonly ICatalogService m_catalogService;
	}
}
