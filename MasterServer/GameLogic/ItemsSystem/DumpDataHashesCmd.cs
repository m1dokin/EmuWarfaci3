using System;
using System.Text;
using MasterServer.Core;
using MasterServer.ElectronicCatalog;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200034A RID: 842
	[ConsoleCmdAttributes(CmdName = "dump_data_hashes", ArgsSize = 0)]
	internal class DumpDataHashesCmd : IConsoleCmd
	{
		// Token: 0x060012E6 RID: 4838 RVA: 0x0004C438 File Offset: 0x0004A838
		public void ExecuteCmd(string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IItemCache service = ServicesManager.GetService<IItemCache>();
			stringBuilder.AppendFormat("Items: {0}\n", service.GetItemsHash());
			IShopService service2 = ServicesManager.GetService<IShopService>();
			stringBuilder.AppendFormat("ShopOffers: {0}\n", service2.OffersHash);
			Log.Info(stringBuilder.ToString());
		}
	}
}
