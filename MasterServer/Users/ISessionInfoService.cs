using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x020007F1 RID: 2033
	[Contract]
	internal interface ISessionInfoService
	{
		// Token: 0x060029A9 RID: 10665
		SessionInfo GetSessionInfo(string online_id);

		// Token: 0x060029AA RID: 10666
		SessionInfo GetSessionInfo(string online_id, int retries);

		// Token: 0x060029AB RID: 10667
		Task<SessionInfo> GetSessionInfoByOnlineIdAsync(string onlineId);

		// Token: 0x060029AC RID: 10668
		Task<SessionInfo> GetSessionInfoByOnlineIdAsync(string onlineId, int retries);

		// Token: 0x060029AD RID: 10669
		ProfileInfo GetProfileInfo(string nickname);

		// Token: 0x060029AE RID: 10670
		IEnumerable<ProfileInfo> GetProfileInfo(IEnumerable<string> nicknames);

		// Token: 0x060029AF RID: 10671
		ProfileInfo GetProfileInfo(ulong profileId);

		// Token: 0x060029B0 RID: 10672
		IEnumerable<ProfileInfo> GetProfileInfo(IEnumerable<ulong> profiles);

		// Token: 0x060029B1 RID: 10673
		void GetProfileInfo(IEnumerable<ulong> profileIds, ProfileInfoCallbackMulti clb);

		// Token: 0x060029B2 RID: 10674
		void GetProfileInfo(string nickname, ProfileInfoCallback clb);

		// Token: 0x060029B3 RID: 10675
		void GetProfileInfo(IEnumerable<string> nicknames, ProfileInfoCallbackMulti clb);

		// Token: 0x060029B4 RID: 10676
		Task<ProfileInfo> GetProfileInfoAsync(string nickname);

		// Token: 0x060029B5 RID: 10677
		Task<List<ProfileInfo>> GetProfileInfoAsync(IEnumerable<string> nicknames);

		// Token: 0x060029B6 RID: 10678
		Task<ProfileInfo> GetProfileInfoAsync(ulong profileId);

		// Token: 0x060029B7 RID: 10679
		Task<List<ProfileInfo>> GetProfileInfoAsync(IEnumerable<ulong> profileIds);

		// Token: 0x060029B8 RID: 10680
		void GetProfileInfo(IEnumerable<string> nicknames, IEnumerable<ulong> profileIds, ProfileInfoCallbackMulti clb);

		// Token: 0x060029B9 RID: 10681
		Task<List<ProfileInfo>> GetProfileInfoAsync(IEnumerable<string> nicknames, IEnumerable<ulong> profileIds);

		// Token: 0x060029BA RID: 10682
		void UpdateProfileStatus(UserInfo.User user);

		// Token: 0x060029BB RID: 10683
		Task<object> UpdateProfileStatusAsync(UserInfo.User user);
	}
}
