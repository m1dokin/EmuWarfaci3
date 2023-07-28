using System;

namespace MasterServer.Users
{
	// Token: 0x02000807 RID: 2055
	public static class UserStatuses
	{
		// Token: 0x06002A21 RID: 10785 RVA: 0x000B5E42 File Offset: 0x000B4242
		public static bool Check(UserStatus status, UserStatus bit)
		{
			return (status & bit) == bit;
		}

		// Token: 0x06002A22 RID: 10786 RVA: 0x000B5E4A File Offset: 0x000B424A
		public static bool IsPreGame(UserStatus status)
		{
			return UserStatuses.Check(status, UserStatus.InLobby) || UserStatuses.Check(status, UserStatus.InGameRoom);
		}

		// Token: 0x06002A23 RID: 10787 RVA: 0x000B5E63 File Offset: 0x000B4263
		public static bool IsInGame(UserStatus status)
		{
			return UserStatuses.Check(status, UserStatus.InGame);
		}

		// Token: 0x06002A24 RID: 10788 RVA: 0x000B5E6D File Offset: 0x000B426D
		public static bool IsAway(UserStatus status)
		{
			return UserStatuses.Check(status, UserStatus.Away);
		}
	}
}
