using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003CD RID: 973
	[ConsoleCmdAttributes(CmdName = "cleanup_notifications", Help = "delete all notification for profile")]
	internal class CleanUpNotificationCmd : ConsoleCommand<CleanUpNotificationParams>
	{
		// Token: 0x06001569 RID: 5481 RVA: 0x0005A110 File Offset: 0x00058510
		public CleanUpNotificationCmd(IDebugNotificationService notificationService)
		{
			this.m_notificationService = notificationService;
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x0005A11F File Offset: 0x0005851F
		protected override void Execute(CleanUpNotificationParams param)
		{
			this.m_notificationService.DeleteNotificationsForProfile(param.ProfileId);
		}

		// Token: 0x04000A4B RID: 2635
		private readonly IDebugNotificationService m_notificationService;
	}
}
