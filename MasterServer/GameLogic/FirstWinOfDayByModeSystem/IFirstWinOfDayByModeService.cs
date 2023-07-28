using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem
{
	// Token: 0x02000082 RID: 130
	[Contract]
	public interface IFirstWinOfDayByModeService
	{
		// Token: 0x060001E3 RID: 483
		bool SetPvpModeFirstWin(ulong profileId, string mode);

		// Token: 0x060001E4 RID: 484
		IEnumerable<string> GetPvpModesWithBonusAvailable(ulong profileId);
	}
}
