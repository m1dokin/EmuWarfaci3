using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x0200073D RID: 1853
	[Contract]
	public interface IAuthService
	{
		// Token: 0x06002637 RID: 9783
		List<SUserAccessLevel> GetAccessLevel(ulong userId);

		// Token: 0x06002638 RID: 9784
		List<SUserAccessLevel> GetAccessLevel();

		// Token: 0x06002639 RID: 9785
		bool SetAccessLevel(SUserAccessLevel userAccesslevel);

		// Token: 0x0600263A RID: 9786
		bool RemoveAccessLevel(ulong id, ulong userId);
	}
}
