using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x0200028A RID: 650
	[Contract]
	public interface IDebugConfigService
	{
		// Token: 0x06000E17 RID: 3607
		IEnumerable<IConfigProvider> GetConfigProviders();
	}
}
