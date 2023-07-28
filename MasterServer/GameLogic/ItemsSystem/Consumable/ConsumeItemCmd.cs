using System;
using System.Linq;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;

namespace MasterServer.GameLogic.ItemsSystem.Consumable
{
	// Token: 0x0200033E RID: 830
	[ConsoleCmdAttributes(CmdName = "consume_item", ArgsSize = 3, Help = "consume n consumables. Params: <userId> <itemName> <quantity>")]
	internal class ConsumeItemCmd : IConsoleCmd
	{
		// Token: 0x060012A4 RID: 4772 RVA: 0x0004AE44 File Offset: 0x00049244
		public ConsumeItemCmd(IDebugItemService itemService, IDALService dalService)
		{
			this.m_itemService = itemService;
			this.m_dalService = dalService;
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x0004AE5C File Offset: 0x0004925C
		public void ExecuteCmd(string[] args)
		{
			ulong userId = ulong.Parse(args[1]);
			string itemName = args[2];
			ushort quantity = ushort.Parse(args[3]);
			ulong profileId = (from sp in this.m_dalService.ProfileSystem.GetUserProfiles(userId)
			select sp.ProfileID).FirstOrDefault<ulong>();
			Log.Info<ushort>("Consumbles left {0}", this.m_itemService.ConsumeItem(userId, profileId, itemName, quantity).ItemsLeft);
		}

		// Token: 0x0400089B RID: 2203
		private readonly IDebugItemService m_itemService;

		// Token: 0x0400089C RID: 2204
		private readonly IDALService m_dalService;
	}
}
