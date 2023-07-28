using System;
using HK2Net;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler.MetaGameActions
{
	// Token: 0x0200031C RID: 796
	[Contract]
	public interface IMetaGameAction
	{
		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x0600121F RID: 4639
		string Name { get; }

		// Token: 0x06001220 RID: 4640
		void Execute(ulong profileId, string action);
	}
}
