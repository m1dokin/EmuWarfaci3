using System;

namespace MasterServer.DAL
{
	// Token: 0x0200007D RID: 125
	public interface IProfileSystem
	{
		// Token: 0x0600016B RID: 363
		DALResultMulti<SAuthProfile> GetUserProfiles(ulong user_id);

		// Token: 0x0600016C RID: 364
		DALResult<ulong> CreateProfile(ulong user_id, string nickname, string head);

		// Token: 0x0600016D RID: 365
		DALResultVoid CreateProfile(ulong profile_id, ulong user_id, string nickname);

		// Token: 0x0600016E RID: 366
		DALResultVoid DeleteProfile(ulong user_id, ulong profile_id, string nickname);

		// Token: 0x0600016F RID: 367
		DALResult<ulong> CreateUser(string csb, string nickname, string pwd, string mail);

		// Token: 0x06000170 RID: 368
		DALResultVoid SetProfileCurClass(ulong profile_id, uint currClass);

		// Token: 0x06000171 RID: 369
		DALResult<bool> SetProfileRankInfo(ulong profile_id, ulong old_experience, SRankInfo new_rank_info);

		// Token: 0x06000172 RID: 370
		DALResult<SProfileInfo> GetProfileInfo(ulong profile_id);

		// Token: 0x06000173 RID: 371
		DALResultVoid SetProfileBanner(ulong profile_id, SBannerInfo banner);

		// Token: 0x06000174 RID: 372
		DALResult<SProfileInfo> GetProfileByNickname(string nickname);

		// Token: 0x06000175 RID: 373
		DALResult<ulong> GetLastSeenDate(ulong profile_id);

		// Token: 0x06000176 RID: 374
		DALResultVoid UpdateLastSeenDate(ulong profile_id);

		// Token: 0x06000177 RID: 375
		DALResultVoid UpdateLastSeenDate(ulong profileId, DateTime lastSeenDate);

		// Token: 0x06000178 RID: 376
		DALResult<bool> UpdateProfileNickname(ulong profileId, string newNickname);

		// Token: 0x06000179 RID: 377
		DALResultVoid UpdateProfileHead(ulong profileId, string head);

		// Token: 0x0600017A RID: 378
		DALResultMulti<SPersistentSettings> GetPersistentSettings(ulong profile_id);

		// Token: 0x0600017B RID: 379
		DALResultVoid SetPersistentSettings(ulong profile_id, string group, string value);

		// Token: 0x0600017C RID: 380
		DALResultVoid ClearPersistentSettings(ulong profile_id, string group);

		// Token: 0x0600017D RID: 381
		DALResultVoid ClearPersistentSettingsFull(ulong profile_id);

		// Token: 0x0600017E RID: 382
		DALResultVoid UpdateMuteTime(ulong profile_id, DateTime mute_time);

		// Token: 0x0600017F RID: 383
		DALResultVoid UpdateBanTime(ulong profile_id, DateTime ban_time);

		// Token: 0x06000180 RID: 384
		DALResultMulti<SFriend> GetFriends(ulong profile_id);

		// Token: 0x06000181 RID: 385
		DALResult<EAddMemberResult> AddFriend(ulong profile_id_1, ulong profile_id_2, uint limit);

		// Token: 0x06000182 RID: 386
		DALResultVoid RemoveFriend(ulong profile_id_1, ulong profile_id_2);

		// Token: 0x06000183 RID: 387
		DALResult<TimeSpan> UpdateTimeToRank(ulong profile_id);
	}
}
