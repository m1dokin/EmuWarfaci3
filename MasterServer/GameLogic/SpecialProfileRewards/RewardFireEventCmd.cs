using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.GameLogic.GameInterface;
using MasterServer.GameLogic.NotificationSystem;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020000F2 RID: 242
	[ConsoleCmdAttributes(CmdName = "reward_fire_event", ArgsSize = 2, Help = "Fire special reward event for given profiles")]
	internal class RewardFireEventCmd : IConsoleCmd
	{
		// Token: 0x060003F9 RID: 1017 RVA: 0x0001140A File Offset: 0x0000F80A
		public RewardFireEventCmd(ISpecialProfileRewardService profileRewardService, ILogService logService, INotificationService notificationService)
		{
			this.m_profileRewardService = profileRewardService;
			this.m_logService = logService;
			this.m_notificationService = notificationService;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00011428 File Offset: 0x0000F828
		public void ExecuteCmd(string[] args)
		{
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				foreach (ulong profileId in GameInterfaceCmd.GetProfiles(args[1]))
				{
					List<SNotification> notifications = this.m_profileRewardService.ProcessEvent(args[2], profileId, logGroup);
					this.m_notificationService.AddNotifications(profileId, notifications, EDeliveryType.SendNow).Wait();
				}
			}
		}

		// Token: 0x040001AB RID: 427
		private readonly ISpecialProfileRewardService m_profileRewardService;

		// Token: 0x040001AC RID: 428
		private readonly ILogService m_logService;

		// Token: 0x040001AD RID: 429
		private readonly INotificationService m_notificationService;
	}
}
