using System;
using HK2Net;

namespace MasterServer.GameLogic.ItemsSystem.WinModels
{
	// Token: 0x02000331 RID: 817
	[Contract]
	public interface IWinModelFactory
	{
		// Token: 0x0600126A RID: 4714
		IWinModel GetWinModel();
	}
}
