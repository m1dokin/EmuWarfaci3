using System;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x0200057F RID: 1407
	[ConsoleCmdAttributes(CmdName = "notifications_dump", ArgsSize = 1, Help = "profileId")]
	internal class NotificationsDumpCmd : IConsoleCmd
	{
		// Token: 0x06001E4D RID: 7757 RVA: 0x0007AFD0 File Offset: 0x000793D0
		public NotificationsDumpCmd(INotificationService notificationService, IDALService dalService)
		{
			this.m_notificationService = notificationService;
			this.m_dalService = dalService;
		}

		// Token: 0x06001E4E RID: 7758 RVA: 0x0007AFE8 File Offset: 0x000793E8
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 2)
			{
				Log.Info<string>("Dump notifications for {0}", args[1]);
				foreach (SPendingNotification spendingNotification in this.m_dalService.NotificationSystem.GetPendingNotifications(ulong.Parse(args[1])))
				{
					INotificationSerializer notificationSerializer = this.m_notificationService.GetNotificationSerializer((ENotificationType)spendingNotification.Type);
					string text = notificationSerializer.Deserialize(spendingNotification.Data).ToString();
					Log.Info("ID: {0}\tType: {1}\tConfirmation: {2}\tData: {3}\tMessage: {4}", new object[]
					{
						spendingNotification.ID,
						(ENotificationType)spendingNotification.Type,
						(EConfirmationType)spendingNotification.ConfirmationType,
						text,
						spendingNotification.Message
					});
				}
			}
			else
			{
				Log.Error("Not enough arguments");
			}
		}

		// Token: 0x04000EC3 RID: 3779
		private readonly INotificationService m_notificationService;

		// Token: 0x04000EC4 RID: 3780
		private readonly IDALService m_dalService;
	}
}
