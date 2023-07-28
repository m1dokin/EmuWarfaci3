using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.XMPP
{
	// Token: 0x0200080B RID: 2059
	[Contract]
	internal interface ICommunicationStatsService
	{
		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06002A30 RID: 10800
		int TotalOnlineUsers { get; }

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06002A31 RID: 10801
		Dictionary<string, int> ServerOnlineUsers { get; }

		// Token: 0x06002A32 RID: 10802
		void StatsUpdate(int total_online);
	}
}
