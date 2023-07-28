using System;
using System.Collections.Generic;

namespace MasterServer.Users
{
	// Token: 0x0200073C RID: 1852
	public class AuthResponse
	{
		// Token: 0x040013B7 RID: 5047
		public bool Authenticated;

		// Token: 0x040013B8 RID: 5048
		public List<SUserAccessLevel> Permissions;
	}
}
