using System;
using HK2Net;

namespace MasterServer.GameLogic.ItemsSystem.RandomBoxChoiceLimitation
{
	// Token: 0x0200008C RID: 140
	[Contract]
	internal interface IRandomBoxChoiceLimitationService
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000219 RID: 537
		bool Enabled { get; }

		// Token: 0x0600021A RID: 538
		bool IsBoxAvailable(ulong profileId, string boxName);

		// Token: 0x0600021B RID: 539
		bool IsRegularItemInBox(string boxName);
	}
}
