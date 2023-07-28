using System;
using HK2Net;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem
{
	// Token: 0x02000083 RID: 131
	[Contract]
	public interface IDebugFirstWinOfDayByModeService
	{
		// Token: 0x060001E5 RID: 485
		void ResetPvpModesFirstWin(ulong profileId);
	}
}
