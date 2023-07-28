using System;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x02000805 RID: 2053
	[Contract]
	public interface IUserStatusProxy
	{
		// Token: 0x140000B4 RID: 180
		// (add) Token: 0x06002A1F RID: 10783
		// (remove) Token: 0x06002A20 RID: 10784
		event OnUserStatusHandler OnUserStatus;
	}
}
