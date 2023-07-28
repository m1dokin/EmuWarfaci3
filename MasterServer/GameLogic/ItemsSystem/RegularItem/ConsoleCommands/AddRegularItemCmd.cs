using System;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;

namespace MasterServer.GameLogic.ItemsSystem.RegularItem.ConsoleCommands
{
	// Token: 0x0200006E RID: 110
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "add_regular_item", Help = "Adds specified number of specified regular item copies to player's inventory")]
	internal class AddRegularItemCmd : ConsoleCommand<AddRegularItemCmdParams>
	{
		// Token: 0x060001AB RID: 427 RVA: 0x0000B001 File Offset: 0x00009401
		public AddRegularItemCmd(ICatalogService catalogService)
		{
			this.m_catalogService = catalogService;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000B010 File Offset: 0x00009410
		protected override void Execute(AddRegularItemCmdParams param)
		{
			CatalogItem item;
			if (!this.m_catalogService.TryGetCatalogItem(param.ItemName, out item))
			{
				Log.Info<string>("Invalid item name '{0}'", param.ItemName);
				return;
			}
			OfferItem item2 = new OfferItem
			{
				Item = item
			};
			AddCustomerItemResponse addCustomerItemResponse = new AddCustomerItemResponse
			{
				Status = TransactionStatus.OK
			};
			int num = 0;
			while ((long)num < (long)((ulong)param.Count) && addCustomerItemResponse.Status != TransactionStatus.LIMIT_REACHED)
			{
				addCustomerItemResponse = this.m_catalogService.AddCustomerItem(param.UserId, item2, true);
				if (addCustomerItemResponse.Status == TransactionStatus.OK)
				{
					num++;
				}
			}
			Log.Info<ulong, int, string>("User {0} got {1} copies of regular item '{2}'", param.UserId, num, param.ItemName);
		}

		// Token: 0x040000C7 RID: 199
		private readonly ICatalogService m_catalogService;
	}
}
