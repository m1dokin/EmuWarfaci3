using System;

namespace MasterServer.Users
{
	// Token: 0x02000801 RID: 2049
	internal class UserLoginContext : IDisposable
	{
		// Token: 0x06002A05 RID: 10757 RVA: 0x000B5A04 File Offset: 0x000B3E04
		internal UserLoginContext(IUserRepository userRepository, UserInfo.User user, ELoginType loginType, DateTime lastSeen)
		{
			this.m_userRepository = userRepository;
			this.m_user = user;
			this.m_lastSeen = lastSeen;
			this.m_loginType = loginType;
			this.m_userRepository.UserPreLogin(this.m_user, this.m_loginType, this.m_lastSeen);
		}

		// Token: 0x06002A06 RID: 10758 RVA: 0x000B5A51 File Offset: 0x000B3E51
		public bool Commit()
		{
			this.m_committed = this.m_userRepository.UserLogin(this.m_user, this.m_loginType);
			return this.m_committed;
		}

		// Token: 0x06002A07 RID: 10759 RVA: 0x000B5A76 File Offset: 0x000B3E76
		public void Dispose()
		{
			if (!this.m_committed)
			{
				this.m_userRepository.UserLogout(this.m_user, ELogoutType.ChannelSwitch);
			}
		}

		// Token: 0x0400165F RID: 5727
		private readonly IUserRepository m_userRepository;

		// Token: 0x04001660 RID: 5728
		private readonly UserInfo.User m_user;

		// Token: 0x04001661 RID: 5729
		private readonly DateTime m_lastSeen;

		// Token: 0x04001662 RID: 5730
		private readonly ELoginType m_loginType;

		// Token: 0x04001663 RID: 5731
		private bool m_committed;
	}
}
