using System;
using System.Threading.Tasks;
using MasterServer.Core;
using MasterServer.GameLogic.GameInterface;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020000B3 RID: 179
	[ConsoleCmdAttributes(CmdName = "notification_add", Help = "Create new notification for profile")]
	internal class AddNotificationCmd : ConsoleCommand<AddNotificationCmdParams>
	{
		// Token: 0x060002DC RID: 732 RVA: 0x0000DEA7 File Offset: 0x0000C2A7
		public AddNotificationCmd(INotificationService notificationService)
		{
			this.m_notificationService = notificationService;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000DEB8 File Offset: 0x0000C2B8
		protected override void Execute(AddNotificationCmdParams param)
		{
			SNotification snotification = NotificationFactory.CreateNotification<string>(param.Type, param.Message, GameInterfaceCmd.GetTimeSpan(param.Expiration), param.ConfirmationType);
			Task task = this.m_notificationService.AddNotifications(param.ProfileId, new SNotification[]
			{
				snotification
			}, param.DeliveryType);
			task.Wait();
		}

		// Token: 0x0400013B RID: 315
		private readonly INotificationService m_notificationService;
	}
}
