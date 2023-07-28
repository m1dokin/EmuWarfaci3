using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;

namespace MasterServer.Users
{
	// Token: 0x0200074F RID: 1871
	[Service]
	[Singleton]
	internal class CorruptedUsersCheckingService : ServiceModule, ICorruptedUsersCheckingService
	{
		// Token: 0x060026A0 RID: 9888 RVA: 0x000A3B09 File Offset: 0x000A1F09
		public CorruptedUsersCheckingService(IUserRepository userRepository, ISessionInfoService sessionInfoService, IJobSchedulerService jobSchedulerService)
		{
			this.m_userRepository = userRepository;
			this.m_sessionInfoService = sessionInfoService;
			this.m_jobSchedulerService = jobSchedulerService;
		}

		// Token: 0x060026A1 RID: 9889 RVA: 0x000A3B26 File Offset: 0x000A1F26
		public override void Init()
		{
			base.Init();
			this.m_jobSchedulerService.AddJob("corrupted_users_check");
		}

		// Token: 0x060026A2 RID: 9890 RVA: 0x000A3B40 File Offset: 0x000A1F40
		public void PerformCheck(TimeSpan untouchedForCheck)
		{
			List<UserInfo.User> usersWithoutTouch = this.m_userRepository.GetUsersWithoutTouch((UserInfo.User user) => user.UntouchedFor > untouchedForCheck);
			if (!usersWithoutTouch.Any<UserInfo.User>())
			{
				return;
			}
			Log.Info<int>("[CorruptedUsersCheckingService] {0} users are suspected to be corrupted", usersWithoutTouch.Count);
			IEnumerable<ProfileInfo> profileInfo = this.m_sessionInfoService.GetProfileInfo(from user in usersWithoutTouch
			select user.Nickname);
			List<UserInfo.User> list = (from user in usersWithoutTouch
			join profile in profileInfo on user.Nickname equals profile.Nickname
			select new
			{
				user,
				profile
			} into <>__TranspIdent11
			where (!string.IsNullOrEmpty(<>__TranspIdent11.profile.OnlineID) && <>__TranspIdent11.profile.LoginTime != <>__TranspIdent11.user.LoginTime) || <>__TranspIdent11.profile.Status == UserStatus.Offline
			select <>__TranspIdent11.user).ToList<UserInfo.User>();
			Log.Info<int>("[CorruptedUsersCheckingService] {0} users are to be removed from server", list.Count);
			foreach (UserInfo.User user2 in list)
			{
				this.m_userRepository.UserLogout(user2, ELogoutType.ChannelSwitch);
			}
		}

		// Token: 0x040013E3 RID: 5091
		private readonly IUserRepository m_userRepository;

		// Token: 0x040013E4 RID: 5092
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x040013E5 RID: 5093
		private readonly IJobSchedulerService m_jobSchedulerService;
	}
}
