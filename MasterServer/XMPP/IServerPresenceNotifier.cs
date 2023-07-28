using System;
using HK2Net;

namespace MasterServer.XMPP
{
	// Token: 0x02000810 RID: 2064
	[Contract]
	internal interface IServerPresenceNotifier
	{
		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06002A49 RID: 10825
		// (set) Token: 0x06002A4A RID: 10826
		bool PresenceEnabled { get; set; }

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06002A4B RID: 10827
		ServerLoadStats LoadStats { get; }
	}
}
