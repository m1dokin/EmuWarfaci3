using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200017B RID: 379
	[Contract]
	public interface IOnlineClient
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060006D0 RID: 1744
		string XmppHost { get; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060006D1 RID: 1745
		string OnlineID { get; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060006D2 RID: 1746
		string TargetRoute { get; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060006D3 RID: 1747
		string Server { get; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060006D4 RID: 1748
		int ServerPort { get; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x060006D5 RID: 1749
		EConnectionState ConnectionState { get; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x060006D6 RID: 1750
		List<SOnlineServer> OnlineServers { get; }

		// Token: 0x060006D7 RID: 1751
		void ServiceConnection();

		// Token: 0x060006D8 RID: 1752
		ECompressType GetDefaultCompression();

		// Token: 0x060006D9 RID: 1753
		void SetDefaultCompression(ECompressType compr);

		// Token: 0x060006DA RID: 1754
		int GetSendDelay();

		// Token: 0x060006DB RID: 1755
		void SetSendDelay(int delay);

		// Token: 0x060006DC RID: 1756
		string GetJid(string user, string resource);

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x060006DD RID: 1757
		// (remove) Token: 0x060006DE RID: 1758
		event ConnectionStateDeleg ConnectionStateChanged;

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x060006DF RID: 1759
		// (remove) Token: 0x060006E0 RID: 1760
		event UserStatusDeleg UserStatusChanged;

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x060006E1 RID: 1761
		// (remove) Token: 0x060006E2 RID: 1762
		event OnlineQueryStatsDeleg OnlineQueryStats;

		// Token: 0x060006E3 RID: 1763
		void SetOnlineServers(List<SOnlineServer> servers);
	}
}
