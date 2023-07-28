using System;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x02000745 RID: 1861
	[Contract]
	internal interface IClassPresenceService
	{
		// Token: 0x0600265A RID: 9818
		void ClassPresenceRecieved(ClassPresenceData data);

		// Token: 0x140000A3 RID: 163
		// (add) Token: 0x0600265B RID: 9819
		// (remove) Token: 0x0600265C RID: 9820
		event ClassPresenceDeleg ClassPresenceReceived;
	}
}
