using System;
using HK2Net;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000270 RID: 624
	[Contract]
	public interface IClanInfoUpdater
	{
		// Token: 0x06000D79 RID: 3449
		void RepairAndSendClanInfo(string onlineId, ulong profileId);
	}
}
