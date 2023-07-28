using System;
using System.Collections.Generic;
using MasterServer.DAL.FirstWinOfDayByMode;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000042 RID: 66
	internal interface IFirstWinOfDayByModeSystemClient
	{
		// Token: 0x0600010D RID: 269
		bool SetPvpModeFirstWin(ulong profileId, string mode, DateTime nextOccurrence);

		// Token: 0x0600010E RID: 270
		IEnumerable<PvpModeWinNextOccurrence> GetPvpModesWinNextOccurrence(ulong profileId);

		// Token: 0x0600010F RID: 271
		void ResetPvpModesFirstWin(ulong profileId);
	}
}
