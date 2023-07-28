using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x0200020D RID: 525
	public interface IProfileSystemClient
	{
		// Token: 0x06000B33 RID: 2867
		IEnumerable<SAuthProfile> GetUserProfiles(ulong userId);

		// Token: 0x06000B34 RID: 2868
		void CreateProfile(ulong profileId, ulong userId, string nickname);

		// Token: 0x06000B35 RID: 2869
		ulong CreateProfile(ulong userId, string nickname, string head);

		// Token: 0x06000B36 RID: 2870
		void DeleteProfile(ulong userId, ulong profileId, string nickname);

		// Token: 0x06000B37 RID: 2871
		ulong CreateUser(string csb, string nickname, string pwd, string mail);

		// Token: 0x06000B38 RID: 2872
		void FlushProfileCache(ulong userId, ulong profileId);

		// Token: 0x06000B39 RID: 2873
		void FlushProfileFriendsCache(ulong commonFriendPID);

		// Token: 0x06000B3A RID: 2874
		void FlushCatalogCache(ulong userId);

		// Token: 0x06000B3B RID: 2875
		void SetProfileCurClass(ulong profileId, uint curClass);

		// Token: 0x06000B3C RID: 2876
		bool SetProfileRankInfo(ulong profileId, ulong old_experience, SRankInfo info);

		// Token: 0x06000B3D RID: 2877
		SProfileInfo GetProfileInfo(ulong profileId);

		// Token: 0x06000B3E RID: 2878
		ulong GetProfileIDByNickname(string nickname);

		// Token: 0x06000B3F RID: 2879
		SProfileInfo GetProfileByNickname(string nickname);

		// Token: 0x06000B40 RID: 2880
		ulong GetLastSeenDate(ulong profileId);

		// Token: 0x06000B41 RID: 2881
		void UpdateLastSeenDate(ulong profileId);

		// Token: 0x06000B42 RID: 2882
		void UpdateLastSeenDate(ulong profileId, DateTime lastSeenDate);

		// Token: 0x06000B43 RID: 2883
		bool UpdateProfileNickname(ulong profileId, string newNickname);

		// Token: 0x06000B44 RID: 2884
		void UpdateProfileHead(ulong profileId, string head);

		// Token: 0x06000B45 RID: 2885
		void UpdateMuteTime(ulong profileId, DateTime muteTime);

		// Token: 0x06000B46 RID: 2886
		void UpdateBanTime(ulong profileId, DateTime banTime);

		// Token: 0x06000B47 RID: 2887
		void SetProfileBanner(ulong profileId, SBannerInfo banner);

		// Token: 0x06000B48 RID: 2888
		IEnumerable<SPersistentSettings> GetPersistentSettings(ulong profileId);

		// Token: 0x06000B49 RID: 2889
		void SetPersistentSettings(ulong profileId, string group, string value);

		// Token: 0x06000B4A RID: 2890
		void ClearPersistentSettings(ulong profileId, string group);

		// Token: 0x06000B4B RID: 2891
		void ClearPersistentSettingsFull(ulong profileId);

		// Token: 0x06000B4C RID: 2892
		IEnumerable<SFriend> GetFriends(ulong profileId);

		// Token: 0x06000B4D RID: 2893
		EAddMemberResult AddFriend(ulong profileId1, ulong profileId2, uint limit);

		// Token: 0x06000B4E RID: 2894
		void RemoveFriend(ulong profileId1, ulong profileId2);

		// Token: 0x06000B4F RID: 2895
		TimeSpan UpdateTimeToRank(ulong profileId);
	}
}
