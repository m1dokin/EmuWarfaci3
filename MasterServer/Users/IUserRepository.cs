using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.GameInterface;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.Users
{
	// Token: 0x020007EA RID: 2026
	[Contract]
	internal interface IUserRepository
	{
		// Token: 0x0600296B RID: 10603
		void UserPreLogin(UserInfo.User user, ELoginType loginType, DateTime lastSeen);

		// Token: 0x0600296C RID: 10604
		bool UserLogin(UserInfo.User user, ELoginType loginType);

		// Token: 0x0600296D RID: 10605
		void UserLogout(UserInfo.User user, ELogoutType logoutType);

		// Token: 0x0600296E RID: 10606
		UserInfo.User GetUser(ulong profileId);

		// Token: 0x0600296F RID: 10607
		UserInfo.User GetUser(string nickname);

		// Token: 0x06002970 RID: 10608
		UserInfo.User GetUserByOnlineId(string onlineId);

		// Token: 0x06002971 RID: 10609
		UserInfo.User GetUserByUserId(ulong userId);

		// Token: 0x06002972 RID: 10610
		List<UserInfo.User> GetUsersWithoutTouch();

		// Token: 0x06002973 RID: 10611
		List<UserInfo.User> GetUsersWithoutTouch(Predicate<UserInfo.User> pred);

		// Token: 0x06002974 RID: 10612
		bool IsOnline(string onlineId);

		// Token: 0x06002975 RID: 10613
		bool IsOnline(ulong profileId);

		// Token: 0x06002976 RID: 10614
		void SetUserInfo(UserInfo.User user);

		// Token: 0x06002977 RID: 10615
		void DumpUsers();

		// Token: 0x06002978 RID: 10616
		void RemoveAllUsers();

		// Token: 0x06002979 RID: 10617
		int GetOnlineUsersCount();

		// Token: 0x0600297A RID: 10618
		IEnumerable<Tuple<string, int>> GetOnlineUsersCountPerRegion();

		// Token: 0x0600297B RID: 10619
		int GetOnlineUsersLimit();

		// Token: 0x0600297C RID: 10620
		int GetJoinedUsersLimit();

		// Token: 0x0600297D RID: 10621
		bool IsOnlineUsersLimitReached();

		// Token: 0x0600297E RID: 10622
		bool IsJoinUsersLimitReached();

		// Token: 0x140000AF RID: 175
		// (add) Token: 0x0600297F RID: 10623
		// (remove) Token: 0x06002980 RID: 10624
		event UserLoggingInDeleg UserLoggingIn;

		// Token: 0x140000B0 RID: 176
		// (add) Token: 0x06002981 RID: 10625
		// (remove) Token: 0x06002982 RID: 10626
		event UserLoginDeleg UserLoggedIn;

		// Token: 0x140000B1 RID: 177
		// (add) Token: 0x06002983 RID: 10627
		// (remove) Token: 0x06002984 RID: 10628
		event UserLogoutDeleg UserLoggedOut;

		// Token: 0x140000B2 RID: 178
		// (add) Token: 0x06002985 RID: 10629
		// (remove) Token: 0x06002986 RID: 10630
		event UserInfoChangedDeleg UserInfoChanged;

		// Token: 0x06002987 RID: 10631
		ulong UnmangleUserId(ulong userId);

		// Token: 0x06002988 RID: 10632
		ulong MangleUserId(ulong userId, ulong platformId);

		// Token: 0x06002989 RID: 10633
		bool IsSameBootstrap(ulong userId1, ulong userId2);

		// Token: 0x0600298A RID: 10634
		bool IsBootstrap(ulong userId, ulong platformId);

		// Token: 0x0600298B RID: 10635
		UserInfo.User Make(ulong uid, ulong pid, string nick, string jid, DateTime loginTime, ProfileProgressionInfo profileProgression, string token, string ip, string buildType, string regionId, AccessLevel accessLevel, ulong exp, int rank, int ping, SBannerInfo banner, ClientVersion version);

		// Token: 0x0600298C RID: 10636
		UserInfo.User Make(ulong pid, string jid, string token, string ip, string buildType, string regionId, SProfileInfo profileInfo, AccessLevel accessLevel, DateTime loginTime, ProfileProgressionInfo profileProgression, ClientVersion version);

		// Token: 0x0600298D RID: 10637
		UserInfo.User Make(SProfileInfo profileInfo, string onlineId, ProfileProgressionInfo profileProgression);

		// Token: 0x0600298E RID: 10638
		UserInfo.User Make(SProfileInfo profileInfo, ProfileProgressionInfo profileProgression);

		// Token: 0x0600298F RID: 10639
		UserInfo.User Make(ProfileInfo profileInfo, SProfileInfo sProfileInfo, ProfileProgressionInfo profileProgression);

		// Token: 0x06002990 RID: 10640
		UserInfo.User MakeFake(ulong uid, ulong pid, string jid, int rank = 0, ulong experience = 0UL, string regionId = "");
	}
}
