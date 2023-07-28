using System;
using System.Collections.Generic;
using System.Net;
using MasterServer.DAL;
using MasterServer.GameLogic.Achievements;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002DD RID: 733
	public interface IGameInterfaceContext : IDisposable
	{
		// Token: 0x06000FAE RID: 4014
		[AccessLevel(AccessLevel.Admin)]
		int TotalOnlineUsers();

		// Token: 0x06000FAF RID: 4015
		[AccessLevel(AccessLevel.Admin)]
		string GetServerStatus();

		// Token: 0x06000FB0 RID: 4016
		[AccessLevel(AccessLevel.Admin)]
		void Quit();

		// Token: 0x06000FB1 RID: 4017
		[AccessLevel(AccessLevel.Admin)]
		IPAddress GetUserIPByProfileId(ulong profileId);

		// Token: 0x06000FB2 RID: 4018
		[AccessLevel(AccessLevel.Admin)]
		Dictionary<string, int> ServerOnlineUsers();

		// Token: 0x06000FB3 RID: 4019
		[AccessLevel(AccessLevel.Admin)]
		bool GiveItem(ulong userId, string itemName, string expire, string message, string reason, bool notify);

		// Token: 0x06000FB4 RID: 4020
		[AccessLevel(AccessLevel.Admin)]
		bool GiveItem(ulong userId, string itemName, OfferType offerType, string parameter, string message, string reason, bool notify, bool ignoreLimit);

		// Token: 0x06000FB5 RID: 4021
		[AccessLevel(AccessLevel.Admin)]
		bool RemoveItem(ulong userId, string itemName, bool all, string reason);

		// Token: 0x06000FB6 RID: 4022
		[AccessLevel(AccessLevel.Admin)]
		bool RemoveItem(ulong userId, ulong customerItemId, string reason);

		// Token: 0x06000FB7 RID: 4023
		[AccessLevel(AccessLevel.Admin)]
		bool RemovePermanentItem(ulong userId, string itemName, string reason);

		// Token: 0x06000FB8 RID: 4024
		[AccessLevel(AccessLevel.Admin)]
		bool RemoveProfileItem(ulong profileId, ulong profileItemId, string reason);

		// Token: 0x06000FB9 RID: 4025
		[AccessLevel(AccessLevel.Admin)]
		bool NotifyMoneyGiven(ulong userId, Currency curr, ulong money);

		// Token: 0x06000FBA RID: 4026
		[AccessLevel(AccessLevel.Admin)]
		TransactionStatus GiveMoney(ulong userId, Currency curr, ulong money, string message, bool notify, string transactionId, string reason);

		// Token: 0x06000FBB RID: 4027
		[AccessLevel(AccessLevel.Admin)]
		bool SpendMoney(ulong userId, Currency curr, ulong money, string reason);

		// Token: 0x06000FBC RID: 4028
		[AccessLevel(AccessLevel.Admin)]
		int GiveCoins(ulong userId, ushort coins, string message, string reason, bool notify);

		// Token: 0x06000FBD RID: 4029
		[AccessLevel(AccessLevel.Admin)]
		int GiveConsumable(ulong userId, string item, ushort count, string message, string reason, bool notify);

		// Token: 0x06000FBE RID: 4030
		[AccessLevel(AccessLevel.Admin)]
		bool UnlockItem(ulong profileId, string item);

		// Token: 0x06000FBF RID: 4031
		[AccessLevel(AccessLevel.Admin)]
		bool UnlockAllItems(ulong profileId);

		// Token: 0x06000FC0 RID: 4032
		[AccessLevel(AccessLevel.Admin)]
		bool BanPlayer(ulong profileId, TimeSpan banTime, string message);

		// Token: 0x06000FC1 RID: 4033
		[AccessLevel(AccessLevel.Admin)]
		bool CancelBanPlayer(ulong profileId);

		// Token: 0x06000FC2 RID: 4034
		[AccessLevel(AccessLevel.Admin)]
		bool MutePlayer(ulong profileId, TimeSpan muteTime);

		// Token: 0x06000FC3 RID: 4035
		[AccessLevel(AccessLevel.Admin)]
		bool CancelMutePlayer(ulong profileId);

		// Token: 0x06000FC4 RID: 4036
		[AccessLevel(AccessLevel.Admin)]
		bool KickPlayer(ulong profileId);

		// Token: 0x06000FC5 RID: 4037
		[AccessLevel(AccessLevel.Admin)]
		bool ForceLogout(ulong profileId);

		// Token: 0x06000FC6 RID: 4038
		[AccessLevel(AccessLevel.Admin)]
		bool SendPlainTextNotification(ulong profileId, string notification, TimeSpan expiration);

		// Token: 0x06000FC7 RID: 4039
		[AccessLevel(AccessLevel.Admin)]
		string GetAnnouncements();

		// Token: 0x06000FC8 RID: 4040
		[AccessLevel(AccessLevel.Admin)]
		string GetActiveAnnouncements();

		// Token: 0x06000FC9 RID: 4041
		[AccessLevel(AccessLevel.Admin)]
		Announcement GetAnnouncement(ulong id);

		// Token: 0x06000FCA RID: 4042
		[AccessLevel(AccessLevel.Admin)]
		bool AddAnnouncement(Announcement announcement);

		// Token: 0x06000FCB RID: 4043
		[AccessLevel(AccessLevel.Admin)]
		bool ModifyAnnouncement(Announcement announcement);

		// Token: 0x06000FCC RID: 4044
		[AccessLevel(AccessLevel.Admin)]
		bool DeleteAnnouncement(ulong id);

		// Token: 0x06000FCD RID: 4045
		[AccessLevel(AccessLevel.Admin)]
		ulong GetUserID(ulong profileId);

		// Token: 0x06000FCE RID: 4046
		[AccessLevel(AccessLevel.Admin)]
		ulong GetProfileID(ulong userId);

		// Token: 0x06000FCF RID: 4047
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		ulong GetProfileIDByNickname(string nick);

		// Token: 0x06000FD0 RID: 4048
		[AccessLevel(AccessLevel.Admin)]
		string GetProfileItems(ulong profileId);

		// Token: 0x06000FD1 RID: 4049
		[AccessLevel(AccessLevel.Admin)]
		string GetDefaultItems();

		// Token: 0x06000FD2 RID: 4050
		[AccessLevel(AccessLevel.Admin)]
		string GetProfileUnlockedItems(ulong profileId);

		// Token: 0x06000FD3 RID: 4051
		[AccessLevel(AccessLevel.Admin)]
		string GetProfileAchievements(ulong profileId);

		// Token: 0x06000FD4 RID: 4052
		[AccessLevel(AccessLevel.Admin)]
		string GetAllAchievements();

		// Token: 0x06000FD5 RID: 4053
		[AccessLevel(AccessLevel.Admin)]
		string GetProfileSponsorPoints(ulong profileId);

		// Token: 0x06000FD6 RID: 4054
		[AccessLevel(AccessLevel.Admin)]
		string GetProfilePersistentSettings(ulong profileId);

		// Token: 0x06000FD7 RID: 4055
		[AccessLevel(AccessLevel.Admin)]
		string GetProfileContract(ulong profileId);

		// Token: 0x06000FD8 RID: 4056
		[AccessLevel(AccessLevel.Admin)]
		ClanInfo GetProfileClan(ulong profileId);

		// Token: 0x06000FD9 RID: 4057
		[AccessLevel(AccessLevel.Admin)]
		void ResetProfile(ulong userId, bool full);

		// Token: 0x06000FDA RID: 4058
		[AccessLevel(AccessLevel.Admin)]
		void FlushUserProfile(ulong userId);

		// Token: 0x06000FDB RID: 4059
		[AccessLevel(AccessLevel.Admin)]
		ulong AddExp(ulong profileId, long amount, LevelChangeReason reason);

		// Token: 0x06000FDC RID: 4060
		[AccessLevel(AccessLevel.Admin)]
		Dictionary<Currency, ulong> GetMoney(ulong profileId);

		// Token: 0x06000FDD RID: 4061
		[AccessLevel(AccessLevel.Admin)]
		SProfileInfo GetProfileInfo(ulong profileId);

		// Token: 0x06000FDE RID: 4062
		[AccessLevel(AccessLevel.Admin)]
		SProfileInfoEx GetProfileInfoEx(ulong profileId);

		// Token: 0x06000FDF RID: 4063
		[AccessLevel(AccessLevel.Admin)]
		Dictionary<ulong, SItem> GetAllItems();

		// Token: 0x06000FE0 RID: 4064
		[AccessLevel(AccessLevel.Admin)]
		IEnumerable<SFriend> GetFriends(ulong profileId);

		// Token: 0x06000FE1 RID: 4065
		[AccessLevel(AccessLevel.Admin)]
		bool SetAccessLevel(SUserAccessLevel level);

		// Token: 0x06000FE2 RID: 4066
		[AccessLevel(AccessLevel.Admin)]
		List<SUserAccessLevel> GetAccessLevel(ulong userId);

		// Token: 0x06000FE3 RID: 4067
		[AccessLevel(AccessLevel.Admin)]
		List<SUserAccessLevel> GetAccessLevel();

		// Token: 0x06000FE4 RID: 4068
		[AccessLevel(AccessLevel.Admin)]
		bool RemoveAccessLevel(ulong id, ulong user_id);

		// Token: 0x06000FE5 RID: 4069
		[AccessLevel(AccessLevel.Admin)]
		ClanInfo GetClanInfo(ulong clanId);

		// Token: 0x06000FE6 RID: 4070
		[AccessLevel(AccessLevel.Admin)]
		ClanInfo GetClanInfoByName(string clanName);

		// Token: 0x06000FE7 RID: 4071
		[AccessLevel(AccessLevel.Admin)]
		string GetClanDesc(ulong clanId);

		// Token: 0x06000FE8 RID: 4072
		[AccessLevel(AccessLevel.Admin)]
		IEnumerable<ClanMember> GetClanMembers(ulong clanId);

		// Token: 0x06000FE9 RID: 4073
		[AccessLevel(AccessLevel.Admin)]
		int CreateClan(ulong profileId, string clanName, string desc);

		// Token: 0x06000FEA RID: 4074
		[AccessLevel(AccessLevel.Admin)]
		bool RemoveClan(ulong initiatorId);

		// Token: 0x06000FEB RID: 4075
		[AccessLevel(AccessLevel.Admin)]
		bool RemoveClanMember(ulong profileId);

		// Token: 0x06000FEC RID: 4076
		[AccessLevel(AccessLevel.Admin)]
		int AddClanMember(ulong clanId, ulong profileId);

		// Token: 0x06000FED RID: 4077
		[AccessLevel(AccessLevel.Admin)]
		bool SetClanRole(ulong initiatorId, ulong targetId, EClanRole role);

		// Token: 0x06000FEE RID: 4078
		[AccessLevel(AccessLevel.Admin)]
		IEnumerable<SAbuseHistory> GetAbuseReportsByDate(DateTime startTime, DateTime endTime, sbyte reportSource, uint count);

		// Token: 0x06000FEF RID: 4079
		[AccessLevel(AccessLevel.Admin)]
		uint GetAbusesCount(DateTime startTime, DateTime endTime);

		// Token: 0x06000FF0 RID: 4080
		[AccessLevel(AccessLevel.Admin)]
		IEnumerable<SAbuseHistory> GetAbuseReportsFromUser(ulong profile_id);

		// Token: 0x06000FF1 RID: 4081
		[AccessLevel(AccessLevel.Admin)]
		IEnumerable<SAbuseHistory> GetAbuseReportsToUser(ulong profile_id);

		// Token: 0x06000FF2 RID: 4082
		[AccessLevel(AccessLevel.Admin)]
		IEnumerable<SAbuseTopReport> GetTopAbuseReports(uint count);

		// Token: 0x06000FF3 RID: 4083
		[AccessLevel(AccessLevel.Admin)]
		string MakeRemoteScreenShot(ulong profileId, bool frontBuffer, int count, float scaleW, float scaleH);

		// Token: 0x06000FF4 RID: 4084
		[AccessLevel(AccessLevel.Admin)]
		string GetPlayerStat(ulong profileId);

		// Token: 0x06000FF5 RID: 4085
		[AccessLevel(AccessLevel.Admin)]
		string GetPlayerStatFromTelem(ulong profileId);

		// Token: 0x06000FF6 RID: 4086
		[AccessLevel(AccessLevel.Admin)]
		void UnlockTutorial(ulong profileId, string tutorial, bool silent, string ev);

		// Token: 0x06000FF7 RID: 4087
		[AccessLevel(AccessLevel.Admin)]
		void UnlockClass(ulong profileId, string pclass, bool silent, string ev);

		// Token: 0x06000FF8 RID: 4088
		[AccessLevel(AccessLevel.Admin)]
		IEnumerable<string> UnlockMission(ulong profileId, string mission, bool silent);

		// Token: 0x06000FF9 RID: 4089
		[AccessLevel(AccessLevel.Admin)]
		void UnlockAchievement(ulong profileId, uint achievementId);

		// Token: 0x06000FFA RID: 4090
		[AccessLevel(AccessLevel.Admin)]
		AchievementLockStatus LockHiddenAchievement(ulong profileId, uint achievementId);

		// Token: 0x06000FFB RID: 4091
		[AccessLevel(AccessLevel.Admin)]
		string GetProfileProgression(ulong profileId);

		// Token: 0x06000FFC RID: 4092
		[AccessLevel(AccessLevel.Admin)]
		string GetUserTags(ulong userId);

		// Token: 0x06000FFD RID: 4093
		[AccessLevel(AccessLevel.Admin)]
		void SetUserTags(ulong userId, string tags);

		// Token: 0x06000FFE RID: 4094
		[AccessLevel(AccessLevel.Admin)]
		void RemoveUserTags(ulong userId);

		// Token: 0x06000FFF RID: 4095
		[AccessLevel(AccessLevel.Admin)]
		string CustomRuleList();

		// Token: 0x06001000 RID: 4096
		[AccessLevel(AccessLevel.Admin)]
		ulong CustomRuleAdd(string config);

		// Token: 0x06001001 RID: 4097
		[AccessLevel(AccessLevel.Admin)]
		string CustomRuleEnable(IEnumerable<ulong> ruleId, bool enable, bool cleanUp);

		// Token: 0x06001002 RID: 4098
		[AccessLevel(AccessLevel.Admin)]
		string CustomRuleRemove(ulong ruleId);

		// Token: 0x06001003 RID: 4099
		[AccessLevel(AccessLevel.Admin)]
		string RecoverVouchers();

		// Token: 0x06001004 RID: 4100
		[AccessLevel(AccessLevel.Admin)]
		string ReloadOffers();

		// Token: 0x06001005 RID: 4101
		[AccessLevel(AccessLevel.Admin)]
		void LoadOffers();

		// Token: 0x06001006 RID: 4102
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		bool SetObserver(ulong profileId, bool enable);

		// Token: 0x06001007 RID: 4103
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		string CreateRoom(string masterId, RoomReference roomRef, CreateRoomParam param);

		// Token: 0x06001008 RID: 4104
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		string GetRoomInfo(string masterId, RoomReference roomRef);

		// Token: 0x06001009 RID: 4105
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		string AddPlayerToRoom(string masterId, RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> playersInfos);

		// Token: 0x0600100A RID: 4106
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		string RemovePlayerFromRoom(string master, RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> playersInfo);

		// Token: 0x0600100B RID: 4107
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		string StartRoom(string masterId, RoomReference roomRef, int team1Score, int team2Score);

		// Token: 0x0600100C RID: 4108
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		string PauseGameSession(string masterId, RoomReference roomRef);

		// Token: 0x0600100D RID: 4109
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		string ResumeGameSession(string masterId, RoomReference roomRef);

		// Token: 0x0600100E RID: 4110
		[AccessLevel(AccessLevel.Moderator | AccessLevel.Admin)]
		string StopGameSession(string masterId, RoomReference roomRef);

		// Token: 0x0600100F RID: 4111
		[AccessLevel(AccessLevel.Admin)]
		string AddFriend(ulong senderProfileID, ulong targetProfileID);

		// Token: 0x06001010 RID: 4112
		[AccessLevel(AccessLevel.Admin)]
		void RemoveFriend(ulong senderProfileID, ulong targetProfileID);

		// Token: 0x06001011 RID: 4113
		[AccessLevel(AccessLevel.Admin)]
		string GetPendingFriends(ulong profileID);

		// Token: 0x06001012 RID: 4114
		[AccessLevel(AccessLevel.Admin)]
		void RespondToInvitation(ulong profileID, ulong notifID, bool accept);

		// Token: 0x06001013 RID: 4115
		[AccessLevel(AccessLevel.Admin)]
		string InviteClanMember(ulong senderProfileID, ulong targetProfileID);

		// Token: 0x06001014 RID: 4116
		[AccessLevel(AccessLevel.Admin)]
		bool LeaveClan(ulong profileID);

		// Token: 0x06001015 RID: 4117
		[AccessLevel(AccessLevel.Admin)]
		string GetPendingClans(ulong profileID);

		// Token: 0x06001016 RID: 4118
		[AccessLevel(AccessLevel.Admin)]
		void SetSupportedClientVersions(params string[] versions);

		// Token: 0x06001017 RID: 4119
		[AccessLevel(AccessLevel.Admin)]
		void AddSupportedClientVersions(params string[] versions);

		// Token: 0x06001018 RID: 4120
		[AccessLevel(AccessLevel.Admin)]
		void RemoveSupportedClientVersions(params string[] versions);

		// Token: 0x06001019 RID: 4121
		[AccessLevel(AccessLevel.Admin)]
		IEnumerable<string> GetSupportedClientVersions();

		// Token: 0x0600101A RID: 4122
		[AccessLevel(AccessLevel.Admin)]
		void BroadcastClientVersionsReload();

		// Token: 0x0600101B RID: 4123
		[AccessLevel(AccessLevel.Admin)]
		void BlockPurchase(ulong userId, string notification);

		// Token: 0x0600101C RID: 4124
		[AccessLevel(AccessLevel.Admin)]
		void UnblockPurchase(ulong userId, string notification);

		// Token: 0x0600101D RID: 4125
		[AccessLevel(AccessLevel.Admin)]
		string TestAccessLevelAdmin();

		// Token: 0x0600101E RID: 4126
		[AccessLevel(AccessLevel.Basic)]
		string TestAccessLevelBasic();

		// Token: 0x0600101F RID: 4127
		[AccessLevel(AccessLevel.Moderator)]
		string TestAccessLevelModerator();

		// Token: 0x06001020 RID: 4128
		[AccessLevel(AccessLevel.Debug)]
		string TestAccessLevelDebug();

		// Token: 0x06001021 RID: 4129
		[AccessLevel(AccessLevel.Admin)]
		string GetRatingSeason();

		// Token: 0x06001022 RID: 4130
		[AccessLevel(AccessLevel.Admin)]
		uint GetProfileRatingPoints(ulong profileId);

		// Token: 0x06001023 RID: 4131
		[AccessLevel(AccessLevel.Admin)]
		bool SetProfileRatingPoints(ulong profileId, uint ratingPointsToSet);

		// Token: 0x06001024 RID: 4132
		[AccessLevel(AccessLevel.Admin)]
		IEnumerable<ulong> GetTopRatingPlayers(uint playersCount);

		// Token: 0x06001025 RID: 4133
		[AccessLevel(AccessLevel.Admin)]
		bool RatingGameBan(ulong profileId, TimeSpan banTimeOut, string msg);

		// Token: 0x06001026 RID: 4134
		[AccessLevel(AccessLevel.Admin)]
		bool RatingGameUnban(ulong profileId);

		// Token: 0x06001027 RID: 4135
		[AccessLevel(AccessLevel.Admin)]
		bool ValidateAuthorizationToken(ulong userId, string tokenStr);
	}
}
