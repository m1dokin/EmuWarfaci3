using System;
using MasterServer.Common;

namespace MasterServer.Users
{
	// Token: 0x02000761 RID: 1889
	internal static class UserMakerHelper
	{
		// Token: 0x06002740 RID: 10048 RVA: 0x000A5C74 File Offset: 0x000A4074
		internal static TUser With<TUser>(this TUser user, params Action<TUser>[] modifiers) where TUser : UserInfo.User
		{
			modifiers.SafeForEach(delegate(Action<TUser> m)
			{
				m(user);
			});
			return user;
		}
	}
}
