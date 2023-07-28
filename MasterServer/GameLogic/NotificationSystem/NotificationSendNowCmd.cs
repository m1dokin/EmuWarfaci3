using System;
using MasterServer.Core;
using MasterServer.GameLogic.GameInterface;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x0200057E RID: 1406
	[ConsoleCmdAttributes(CmdName = "notification_send_now", ArgsSize = 2, Help = "profileId, message")]
	internal class NotificationSendNowCmd : IConsoleCmd
	{
		// Token: 0x06001E4B RID: 7755 RVA: 0x0007AF24 File Offset: 0x00079324
		public NotificationSendNowCmd(INotificationService notificationService)
		{
			this.m_notificationService = notificationService;
		}

		// Token: 0x06001E4C RID: 7756 RVA: 0x0007AF34 File Offset: 0x00079334
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 3)
			{
				string data = Uri.UnescapeDataString(args[2]);
				foreach (ulong profileId in GameInterfaceCmd.GetProfiles(args[1]))
				{
					this.m_notificationService.AddNotification<string>(profileId, ENotificationType.Message, data, TimeSpan.Zero, EDeliveryType.SendNow, EConfirmationType.None).Wait();
				}
				Log.Info("Notification added");
			}
			else
			{
				Log.Error("Not enough arguments");
			}
		}

		// Token: 0x04000EC2 RID: 3778
		private readonly INotificationService m_notificationService;
	}
}
