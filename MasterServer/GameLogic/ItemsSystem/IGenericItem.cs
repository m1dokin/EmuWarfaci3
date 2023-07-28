using System;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000360 RID: 864
	public interface IGenericItem
	{
		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06001358 RID: 4952
		int? Amount { get; }

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06001359 RID: 4953
		int Durability { get; }

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x0600135A RID: 4954
		bool IsExpirable { get; }

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x0600135B RID: 4955
		TimeSpan Expiration { get; }

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600135C RID: 4956
		bool IsRegular { get; }

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x0600135D RID: 4957
		string Name { get; }
	}
}
