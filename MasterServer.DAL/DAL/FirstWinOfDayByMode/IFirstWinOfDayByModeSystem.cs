using System;

namespace MasterServer.DAL.FirstWinOfDayByMode
{
	// Token: 0x02000032 RID: 50
	public interface IFirstWinOfDayByModeSystem
	{
		// Token: 0x0600007F RID: 127
		DALResult<bool> SetPvpModeFirstWin(ulong profileId, string mode, DateTime nextOccurrence);

		// Token: 0x06000080 RID: 128
		DALResultMulti<PvpModeWinNextOccurrence> GetPvpModesWinNextOccurrence(ulong profileId);

		// Token: 0x06000081 RID: 129
		DALResultVoid ResetPvpModesFirstWin(ulong profileId);
	}
}
