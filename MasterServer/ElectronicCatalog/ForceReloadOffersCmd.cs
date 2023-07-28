using System;
using MasterServer.Core;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200004C RID: 76
	[ConsoleCmdAttributes(CmdName = "force_reload_offer", ArgsSize = 0, Help = "Reload offers")]
	internal class ForceReloadOffersCmd : IConsoleCmd
	{
		// Token: 0x06000131 RID: 305 RVA: 0x000098D3 File Offset: 0x00007CD3
		public ForceReloadOffersCmd(IShopService shopService)
		{
			this.m_shopService = shopService;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000098E2 File Offset: 0x00007CE2
		public void ExecuteCmd(string[] args)
		{
			this.m_shopService.LoadOffers();
		}

		// Token: 0x04000091 RID: 145
		private readonly IShopService m_shopService;
	}
}
