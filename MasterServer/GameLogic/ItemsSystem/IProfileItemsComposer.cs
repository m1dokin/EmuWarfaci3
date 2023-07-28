using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000376 RID: 886
	public interface IProfileItemsComposer
	{
		// Token: 0x06001427 RID: 5159
		void Compose(ulong profileId, EquipOptions options, List<SEquipItem> composedEquip);
	}
}
