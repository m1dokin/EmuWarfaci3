using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.Platform.Nickname
{
	// Token: 0x0200068C RID: 1676
	[Contract]
	[BootstrapExplicit]
	internal interface IExternalNicknameSyncService
	{
		// Token: 0x06002326 RID: 8998
		bool SyncNickname(ulong profileId, string nickname);

		// Token: 0x14000099 RID: 153
		// (add) Token: 0x06002327 RID: 8999
		// (remove) Token: 0x06002328 RID: 9000
		event ProfileRenamedDelegate ProfileRenamed;
	}
}
