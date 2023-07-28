using System;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020000BA RID: 186
	public static class NotificationTypeExtentions
	{
		// Token: 0x060002F8 RID: 760 RVA: 0x0000E0CC File Offset: 0x0000C4CC
		public static bool IsInvite(this ENotificationType type)
		{
			return type.HasFlag(ENotificationType.ClanInvite) || type.HasFlag(ENotificationType.FriendInvite);
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000E0FE File Offset: 0x0000C4FE
		public static bool IsAchievement(this ENotificationType type)
		{
			return type.HasFlag(ENotificationType.Achievement);
		}
	}
}
