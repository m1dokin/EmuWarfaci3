using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200062E RID: 1582
	[Contract]
	internal interface ISessionSummaryService
	{
		// Token: 0x14000091 RID: 145
		// (add) Token: 0x060021FB RID: 8699
		// (remove) Token: 0x060021FC RID: 8700
		event Action<SessionSummary> SessionSummaryFinalized;

		// Token: 0x060021FD RID: 8701
		void Contribute(string sessionId, string hint, Action<SessionSummary> contribution);
	}
}
