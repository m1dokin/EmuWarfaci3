using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.ItemsSystem.WinModels
{
	// Token: 0x02000336 RID: 822
	[Contract]
	public interface IWinModel : IDisposable
	{
		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06001287 RID: 4743
		TopPrizeWinModel WinModel { get; }

		// Token: 0x06001288 RID: 4744
		void Init();

		// Token: 0x06001289 RID: 4745
		int AddPrizeToken(ulong userId, string tokenName);

		// Token: 0x0600128A RID: 4746
		void ResetPrizeTokensCount(ulong userId, ulong profileId, string tokenName);

		// Token: 0x0600128B RID: 4747
		Dictionary<string, ulong> GetCollectedPrizeTokensCount(ulong userId);
	}
}
