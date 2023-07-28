using System;
using MasterServer.Core;
using MasterServer.Database;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000580 RID: 1408
	[ConsoleCmdAttributes(CmdName = "notifications_check", ArgsSize = 0, Help = "profileId")]
	internal class NotificationsCheckCmd : IConsoleCmd
	{
		// Token: 0x06001E4F RID: 7759 RVA: 0x0007B0E4 File Offset: 0x000794E4
		public NotificationsCheckCmd(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x06001E50 RID: 7760 RVA: 0x0007B0F3 File Offset: 0x000794F3
		public void ExecuteCmd(string[] args)
		{
			Log.Info("Checking expired notifications");
			this.m_dalService.NotificationSystem.ClearExpiredNotifications();
		}

		// Token: 0x04000EC5 RID: 3781
		private readonly IDALService m_dalService;
	}
}
