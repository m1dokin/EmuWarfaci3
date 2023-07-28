using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.GameLogic.GameInterface;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x0200057D RID: 1405
	[ConsoleCmdAttributes(CmdName = "notification_add_debug", ArgsSize = 2, Help = "profileId, size")]
	public class NotificationAddDebugCmd : IConsoleCmd
	{
		// Token: 0x06001E4A RID: 7754 RVA: 0x0007AE64 File Offset: 0x00079264
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 3)
			{
				byte[] item = new byte[int.Parse(args[2])];
				NotificationService notificationService = (NotificationService)ServicesManager.GetService<INotificationService>();
				foreach (ulong profileId in GameInterfaceCmd.GetProfiles(args[1]))
				{
					notificationService.AddNotifications<byte[]>(profileId, ENotificationType.Message, new List<byte[]>
					{
						item
					}, TimeSpan.FromSeconds(10.0), EDeliveryType.SendOnCheckPoint, EConfirmationType.None).Wait();
				}
				Log.Info("Command finished");
			}
			else
			{
				Log.Error("Not enough arguments");
			}
		}
	}
}
