using System;

namespace MasterServer.Users
{
	// Token: 0x02000804 RID: 2052
	// (Invoke) Token: 0x06002A1C RID: 10780
	public delegate void OnUserStatusHandler(UserStatus prev_status, UserStatus new_status, string onlineId);
}
