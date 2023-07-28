using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000378 RID: 888
	[Contract]
	public interface IProfileItemsComposerFactory
	{
		// Token: 0x0600142B RID: 5163
		IEnumerable<KeyValuePair<int, IProfileItemsComposer>> GetComposers();
	}
}
