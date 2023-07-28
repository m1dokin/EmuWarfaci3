using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003D0 RID: 976
	[ConsoleCmdAttributes(CmdName = "create_friend_invite_notifications", Help = "create friend invite notifications and send it to target user")]
	internal class CreateInviteNotificationCmd : ConsoleCommand<CreateInviteNotificationParams>
	{
		// Token: 0x06001570 RID: 5488 RVA: 0x0005A187 File Offset: 0x00058587
		public CreateInviteNotificationCmd(INotificationService notificationService, IDALService dalService, IUserRepository userRepository, IClanService clanService)
		{
			this.m_notificationService = notificationService;
			this.m_dalService = dalService;
			this.m_userRepository = userRepository;
			this.m_clanService = clanService;
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x0005A1AC File Offset: 0x000585AC
		protected override void Execute(CreateInviteNotificationParams param)
		{
			ulong profileIDByNickname = this.m_dalService.ProfileSystem.GetProfileIDByNickname(param.Initiator);
			ulong profileIDByNickname2 = this.m_dalService.ProfileSystem.GetProfileIDByNickname(param.Target);
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileIDByNickname2);
			List<SNotification> list = new List<SNotification>();
			SInvitationFriendData data = new SInvitationFriendData
			{
				Initiator = CommonInitiatorData.CreateInitiatorData(this.m_clanService, this.m_userRepository, profileIDByNickname),
				TargetId = profileIDByNickname2,
				TargetUserId = profileInfo.UserID,
				RecieverName = profileInfo.Nickname
			};
			int num = 0;
			while ((long)num < (long)((ulong)param.Count))
			{
				SNotification item = NotificationFactory.CreateNotification<SInvitationFriendData>(ENotificationType.FriendInvite, data, TimeSpan.FromHours(1.0), EConfirmationType.Confirmation);
				list.Add(item);
				num++;
			}
			this.m_notificationService.AddNotifications(profileIDByNickname2, list, EDeliveryType.SendOnCheckPoint).Wait();
		}

		// Token: 0x04000A4E RID: 2638
		private readonly IDALService m_dalService;

		// Token: 0x04000A4F RID: 2639
		private readonly INotificationService m_notificationService;

		// Token: 0x04000A50 RID: 2640
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000A51 RID: 2641
		private readonly IClanService m_clanService;
	}
}
