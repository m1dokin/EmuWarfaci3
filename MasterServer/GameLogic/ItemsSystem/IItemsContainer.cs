using System;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200035F RID: 863
	public interface IItemsContainer
	{
		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06001356 RID: 4950
		string Name { get; }

		// Token: 0x06001357 RID: 4951
		bool HasItemNamed(string name);
	}
}
