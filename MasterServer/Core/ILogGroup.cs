using System;
using MasterServer.DAL;
using MasterServer.DAL.VoucherSystem;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.GameLogic.StatsTracking;
using MasterServer.GameRoom.RoomExtensions.Reconnect;
using MasterServer.GameRoomSystem;
using MasterServer.Platform.Payment;
using MasterServer.Users;

namespace MasterServer.Core
{
	// Token: 0x0200013D RID: 317
	internal interface ILogGroup : IDisposable
	{
		// Token: 0x06000528 RID: 1320
		[LogCategory("abuse_report", "Send by MS when profile send abuse report")]
		void AbuseReportLog([LogParam(LogKey.user_id)] ulong senderUserId, [LogParam(LogKey.profile_id)] ulong senderProfileId, [LogParam(LogKey.ip_address)] string senderIp, [LogParam(LogKey.abuser_uid)] ulong receiverUserId, [LogParam(LogKey.abuser_pid)] ulong receiverProfileId, [LogParam(LogKey.abuse_reason)] string type, [LogParam(LogKey.abuse_comment)] string comment);

		// Token: 0x06000529 RID: 1321
		[LogCategory("admin_command", "Sent by MS when admin command is executed")]
		void AdminCommandLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.exec_command)] string command);

		// Token: 0x0600052A RID: 1322
		[LogCategory("anticheat_report", "Send by MS when player possible is a cheater")]
		void AntiCheatReport([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.anticheat_type)] string anticheatType, [LogParam(LogKey.anticheat_score)] string anticheatScore, [LogParam(LogKey.anticheat_calls)] string anticheatCalls, [LogParam(LogKey.anticheat_description)] string anticheatDescription);

		// Token: 0x0600052B RID: 1323
		[LogCategory("anticheat_immediate_report", "Send by MS when player possible is a cheater immediately")]
		void AntiCheatImmediateReport([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.anticheat_type)] string anticheatType, [LogParam(LogKey.anticheat_score)] string anticheatScore, [LogParam(LogKey.anticheat_description)] string anticheatDescription);

		// Token: 0x0600052C RID: 1324
		[LogCategory("auto_repair_equipment_res", "Sent by MS when AutoRepairEquipmentService had tried to repair items")]
		void AutoRepairEquipmentResult([LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.autorepair_datetime)] DateTime autorepairDateTime, [LogParam(LogKey.game_money_before_autorepair)] ulong gameMoneyBeforeSession, [LogParam(LogKey.game_money_after_autorepair)] ulong gameMoneyAfterSession, [LogParam(LogKey.is_auto_repair_on)] bool isAutoRepairOn, [LogParam(LogKey.repair_status)] RepairStatus repairStatus, [LogParam(LogKey.repair_cost)] ulong repairCost);

		// Token: 0x0600052D RID: 1325
		[LogCategory("block_purchase", "Sent by MS when hard currecncy purchase is blocked")]
		void BlockPurchase([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId);

		// Token: 0x0600052E RID: 1326
		[LogCategory("catalog_item", "Sent by MS when: \\\\ \\- MS starts and LogServerData.enable is set to 1 \\\\ \\- default profile data is changed and LogServerData.enable is set to 1 \\\\ \\- LogServerData.enable is changed to 1")]
		void CatalogItemsLog([LogParam(LogKey.catalog_item_id)] ulong catalogItemId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.item_max_amount)] int maxAmount, [LogParam(LogKey.item_type)] string itemType);

		// Token: 0x0600052F RID: 1327
		[LogCategory("clan_invite_member", "Sent by MS when: \\\\ \\- when user  invite other user to his clan")]
		void ClanInviteMemberLog([LogParam(LogKey.clan_initiator_profileId)] ulong profileId, [LogParam(LogKey.clan_target_profileId)] ulong targetProfileId, [LogParam(LogKey.clan_id)] ulong clanId, [LogParam(LogKey.clan_invite_member_result)] EInviteStatus status);

		// Token: 0x06000530 RID: 1328
		[LogCategory("clan_create", "Sent by MS when: \\\\ \\- user creates new clan")]
		void ClanCreateLog([LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.clan_id)] ulong clanId, [LogParam(LogKey.clan_name)] string clanName, [LogParam(LogKey.clan_create_status)] EClanCreationStatus status);

		// Token: 0x06000531 RID: 1329
		[LogCategory("clan_set_role", "Sent by MS when: \\\\ \\- user promote another user from same clan")]
		void ClanSetRoleLog([LogParam(LogKey.clan_initiator_profileId)] ulong initiatorId, [LogParam(LogKey.clan_target_profileId)] ulong targetId, [LogParam(LogKey.clan_role)] EClanRole clanRole, [LogParam(LogKey.clan_id)] ulong clanId);

		// Token: 0x06000532 RID: 1330
		[LogCategory("clan_kick", "Sent by MS when: \\\\ \\- user kick another user from same clan")]
		void ClanKickLog([LogParam(LogKey.clan_initiator_profileId)] ulong initiatorId, [LogParam(LogKey.clan_target_profileId)] ulong targetId, [LogParam(LogKey.clan_id)] ulong clanId);

		// Token: 0x06000533 RID: 1331
		[LogCategory("clan_leave", "Sent by MS when: \\\\ \\- user leaves clan")]
		void ClanLeaveLog([LogParam(LogKey.profile_id)] ulong initiatorId, [LogParam(LogKey.clan_members_count)] int count, [LogParam(LogKey.clan_id)] ulong clanId);

		// Token: 0x06000534 RID: 1332
		[LogCategory("clan_delete", "Sent by MS when: \\\\ \\- when clan is delete from DB")]
		void ClanDeleteLog([LogParam(LogKey.clan_id)] ulong clanId);

		// Token: 0x06000535 RID: 1333
		[LogCategory("catalog_offer", "Sent by MS when: \\\\ \\- MS starts and LogServerData.enable is set to 1 \\\\ \\- default profile data is changed and LogServerData.enable is set to 1 \\\\ \\- LogServerData.enable is changed to 1")]
		void CatalogOfferLog([LogParam(LogKey.supplier_id)] int supplierId, [LogParam(LogKey.shop_offer_id)] ulong shopOfferId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.offer_type)] OfferType offerType, [LogParam(LogKey.game_money)] ulong priceGameMoney, [LogParam(LogKey.cry_money)] ulong priceCryMoney, [LogParam(LogKey.crown_money)] ulong priceCrownMoney, [LogParam(LogKey.key_item_name)] string priceKeyItemName, [LogParam(LogKey.offer_status)] string offerStatus, [LogParam(LogKey.discount)] uint discount, [LogParam(LogKey.game_money_original)] ulong priceOriginalGameMoney, [LogParam(LogKey.cry_money_original)] ulong priceOriginalCryMoney, [LogParam(LogKey.crown_money_original)] ulong priceOriginalCrownMoney, [LogParam(LogKey.item_total_durability)] int totalDurabilityPoints, [LogParam(LogKey.item_expiration_time)] string expirationTime, [LogParam(LogKey.items_quantity)] ulong quantity, [LogParam(LogKey.item_repair_cost)] string repairCost, [LogParam(LogKey.rank_id)] int rank);

		// Token: 0x06000536 RID: 1334
		[LogCategory("channel_switch", "Send by MS when player switch between game channels")]
		void ChannelSwitch([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.profile_jid)] string fromJid, [LogParam(LogKey.auth_token)] string token, [LogParam(LogKey.profile_tags)] string tags);

		// Token: 0x06000537 RID: 1335
		[LogCategory("character_achieved", "Sent by MS when character gets progress in achievement")]
		void CharacterAchievedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.mission_name)] string missionName, [LogParam(LogKey.mission_setting)] string missionSetting, [LogParam(LogKey.achievement_id)] uint achievementId, [LogParam(LogKey.achievement_progress)] int achievementProgress, [LogParam(LogKey.achievement_completion)] EAchevementStatus achievementCompletion);

		// Token: 0x06000538 RID: 1336
		[LogCategory("character_alarm", "Send by MS when profile validation failed, reason can be achievements, profile or clan")]
		void CharacterAlarmLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.alarm_reason)] string reason);

		// Token: 0x06000539 RID: 1337
		[LogCategory("character_ban", "Send by MS when profile was banned")]
		void CharacterBanLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.profile_ban_time)] DateTime banExpirationTime, [LogParam(LogKey.ban_source)] BanReportSource source);

		// Token: 0x0600053A RID: 1338
		[LogCategory("character_creation", "Send by MS when new profile is created")]
		void CharacterCreationLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname);

		// Token: 0x0600053B RID: 1339
		[LogCategory("character_deletion", "Send by MS when new profile is deleted, can be done with help of _online_reset_profile_full_ command")]
		void CharacterDeletionLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname);

		// Token: 0x0600053C RID: 1340
		[LogCategory("character_kick", "Sent by MS when character is kicked from game room.")]
		void CharacterKickLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.remove_reason)] GameRoomPlayerRemoveReason reason, [LogParam(LogKey.region_id)] string regionId, [LogParam(LogKey.mission_type)] string missionType, [LogParam(LogKey.mission_name)] string missionName);

		// Token: 0x0600053D RID: 1341
		[LogCategory("character_levelup", "Send by MS when profile achieves next rank")]
		void CharacterLevelUpLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.rank_points)] ulong rankPoints, [LogParam(LogKey.levelup_time)] TimeSpan levelUpTime, [LogParam(LogKey.total_online_time)] TimeSpan totalOnlineTime, [LogParam(LogKey.levelup_reason)] LevelChangeReason levelUpReason);

		// Token: 0x0600053E RID: 1342
		[LogCategory("character_login", "Send by MS when already created profile successfully login")]
		void CharacterLoginLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.rank_points)] ulong rankPoints, [LogParam(LogKey.rating)] uint rating, [LogParam(LogKey.rating_season_id)] string ratingSeasonId, [LogParam(LogKey.profile_create_time)] DateTime profileCreationTime, [LogParam(LogKey.total_online_time)] TimeSpan totalOnlineTime, [LogParam(LogKey.game_money)] ulong gameMoney, [LogParam(LogKey.cry_money)] ulong cryMoney, [LogParam(LogKey.crown_money)] ulong crownMoney, [LogParam(LogKey.profile_jid)] string jid, [LogParam(LogKey.auth_token)] string token, [LogParam(LogKey.profile_tags)] string tags, [LogParam(LogKey.hardware_id)] string hardwareId, [LogParam(LogKey.cpu_vendor)] uint cpuVendor, [LogParam(LogKey.cpu_family)] uint cpuFamily, [LogParam(LogKey.cpu_model)] uint cpuModel, [LogParam(LogKey.cpu_stepping)] uint cpuStepping, [LogParam(LogKey.cpu_speed)] uint cpuSpeed, [LogParam(LogKey.cpu_num_cores)] uint cpuNumCores, [LogParam(LogKey.gpu_vendor_id)] uint gpuVendorId, [LogParam(LogKey.gpu_device_id)] uint gpuDeviceId, [LogParam(LogKey.physical_memory)] uint physicalMemory, [LogParam(LogKey.client_version)] string clientVersion, [LogParam(LogKey.region_id)] string regionId, [LogParam(LogKey.os_64)] uint os64, [LogParam(LogKey.os_ver)] uint osVer);

		// Token: 0x0600053F RID: 1343
		[LogCategory("character_logout", "Send by MS when player gracefully close game client")]
		void CharacterLogoutLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_login_time)] DateTime loginTime, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.rank_points)] ulong rankPoints, [LogParam(LogKey.session_time)] TimeSpan sessionDuration, [LogParam(LogKey.ingame_playtime)] TimeSpan inGamePlaytime, [LogParam(LogKey.game_money)] ulong gameMoney, [LogParam(LogKey.cry_money)] ulong cryMoney, [LogParam(LogKey.crown_money)] ulong crownMoney, [LogParam(LogKey.profile_jid)] string jid, [LogParam(LogKey.auth_token)] string token, [LogParam(LogKey.profile_tags)] string tags, [LogParam(LogKey.disconnect_reason)] ELogoutType logoutType);

		// Token: 0x06000540 RID: 1344
		[LogCategory("character_mute", "Send by MS when profile was muted")]
		void CharacterMuteLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.profile_mute_time)] DateTime muteExpirationTime);

		// Token: 0x06000541 RID: 1345
		[LogCategory("character_unban", "Send by MS when profile was unbanned")]
		void CharacterUnBanLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId);

		// Token: 0x06000542 RID: 1346
		[LogCategory("character_unmute", "Send by MS when profile was unmuted")]
		void CharacterUnMuteLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId);

		// Token: 0x06000543 RID: 1347
		[LogCategory("contract_activate", "Notifies about contract activation")]
		void ContractActivateLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.item_expiration_time)] string expirationTime, [LogParam(LogKey.item_name)] string name);

		// Token: 0x06000544 RID: 1348
		[LogCategory("contract_complete", "Notifies about contract completion where _contract_time_ is time from purchase to completion")]
		void ContractCompleteLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.item_name)] string name, [LogParam(LogKey.contract_time)] TimeSpan completionTime);

		// Token: 0x06000545 RID: 1349
		[LogCategory("contract_fail", "Notifies about contract expire where _contract_time_ is time from purchase to expiration")]
		void ContractFailLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.item_name)] string name, [LogParam(LogKey.contract_time)] TimeSpan completionTime);

		// Token: 0x06000546 RID: 1350
		[LogCategory("custom_rule", "Sent by MS on startup and all changes to custom rules set to log current rules configuration")]
		void CustomRuleLog([LogParam(LogKey.rule_id)] ulong ruleId, [LogParam(LogKey.rule_enabled)] bool enabled, [LogParam(LogKey.rule_config)] string config);

		// Token: 0x06000547 RID: 1351
		[LogCategory("custom_rule_triggered", "Sent by MS when some custom rule is triggered")]
		void CustomRuleTriggeredLog([LogParam(LogKey.rule_id)] ulong ruleId, [LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.profile_tags)] string tags);

		// Token: 0x06000548 RID: 1352
		[LogCategory("consecutive_login_bonus_activation", "Sent by MS consecutive login bonus rule on activation")]
		void ConsecutiveLoginBonusActivationLog([LogParam(LogKey.rule_id)] ulong ruleId, [LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.clb_expired)] bool activationExpired, [LogParam(LogKey.clbs_outdated)] bool prevRewardOutdated, [LogParam(LogKey.clb_reward)] string reward, [LogParam(LogKey.clb_streak_id)] int currStreak, [LogParam(LogKey.clb_reward_id)] int currReward);

		// Token: 0x06000549 RID: 1353
		[LogCategory("default_profile", "Sent by MS when: \\\\ \\- MS starts and LogServerData.enable is set to 1 \\\\ \\- default profile data is changed and LogServerData.enable is set to 1 \\\\ \\- LogServerData.enable is changed to 1")]
		void DefaultProfileLog([LogParam(LogKey.item_id)] ulong itemId, [LogParam(LogKey.catalog_item_id)] ulong catalogId, [LogParam(LogKey.item_slot)] ulong slotIDs);

		// Token: 0x0600054A RID: 1354
		[LogCategory("game_interface_cmd", "Sent by MS when GI command is executed")]
		void GameInterfaceCmd([LogParam(LogKey.game_interface_cmd)] string command);

		// Token: 0x0600054B RID: 1355
		[LogCategory("item_cache", "Sent by MS when: \\\\ \\- MS starts and LogServerData.enable is set to 1 \\\\ \\- default profile data is changed and LogServerData.enable is set to 1 \\\\ \\- LogServerData.enable is changed to 1")]
		void ItemCacheLog([LogParam(LogKey.item_id)] ulong itemId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.item_type)] string itemType, [LogParam(LogKey.tags_filter)] string tagsFilter);

		// Token: 0x0600054C RID: 1356
		[LogCategory("item_consumed", "Send by MS when player consumes its item, currently only coins are reported")]
		void ItemConsumedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.item_type)] string itemType, [LogParam(LogKey.items_consumed)] ulong quantityConsumed, [LogParam(LogKey.items_left)] ulong quantityLeft, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.mission_type)] string missionType, [LogParam(LogKey.checkpoint)] uint checkpoint);

		// Token: 0x0600054D RID: 1357
		[LogCategory("item_destroy", "Send by MS when profile item was destroyed")]
		void ItemDestroyLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.profile_item_id)] ulong profileItemId, [LogParam(LogKey.catalog_item_id)] ulong catalogItemId, [LogParam(LogKey.item_type)] string itemType, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.item_durability)] int durabilityPoints, [LogParam(LogKey.reason)] string reason);

		// Token: 0x0600054E RID: 1358
		[LogCategory("item_equip", "Sent by MS when character equip/unequip item")]
		void ItemEquipLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.profile_item_id)] ulong profileItemId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.item_slot)] ulong slotIds);

		// Token: 0x0600054F RID: 1359
		[LogCategory("item_expire", "Send by MS when item was expired for profile")]
		void ItemExpireLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.profile_item_id)] ulong profileItemId, [LogParam(LogKey.item_name)] string itemName);

		// Token: 0x06000550 RID: 1360
		[LogCategory("item_given", "Send by MS when item was given to profile")]
		void ItemGivenLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.item_id)] ulong itemId, [LogParam(LogKey.profile_item_id)] ulong profileItemId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.produce_type)] LogGroup.ProduceType produceType);

		// Token: 0x06000551 RID: 1361
		[LogCategory("item_reimbursement", "Sent by MS when: \\\\ \\- MS starts and LogServerData.enable is set to 1 \\\\ \\- default profile data is changed and LogServerData.enable is set to 1 \\\\ \\- LogServerData.enable is changed to 1")]
		void ItemReimbursementLog([LogParam(LogKey.item_name)] string name, [LogParam(LogKey.game_money)] ulong gameMoney, [LogParam(LogKey.cry_money)] ulong cryMoney, [LogParam(LogKey.crown_money)] ulong crownMoney);

		// Token: 0x06000552 RID: 1362
		[LogCategory("item_repair", "Send by MS when profile item was repaired")]
		void ItemRepairLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.profile_item_id)] ulong profileItemId, [LogParam(LogKey.shop_tr_status)] TransactionStatus transactionStatus, [LogParam(LogKey.item_durability)] int durabilityPoints, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.game_money_diff)] long gameMoneyDiff, [LogParam(LogKey.cry_money_diff)] long cryMoneyDiff, [LogParam(LogKey.crown_money_diff)] long crownMoneyDiff);

		// Token: 0x06000553 RID: 1363
		[LogCategory("item_locked", "Send by MS when item was locked for profile")]
		void ItemLockedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.item_id)] ulong itemId, [LogParam(LogKey.item_name)] string itemName);

		// Token: 0x06000554 RID: 1364
		[LogCategory("item_unlocked", "Send by MS when item was unlocked for profile")]
		void ItemUnlockedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.item_id)] ulong itemId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.produce_type)] LogGroup.ProduceType produceType);

		// Token: 0x06000555 RID: 1365
		[LogCategory("item_use", "Send by MS when item's durability was changed")]
		void ItemUseLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.profile_item_id)] ulong profileItemId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.item_type)] string itemType, [LogParam(LogKey.item_durability)] int durabilityPoints);

		// Token: 0x06000556 RID: 1366
		[LogCategory("lose_connection", "Send by MS when player's client crashed")]
		void CharacterLostConnectionLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.profile_jid)] string jid);

		// Token: 0x06000557 RID: 1367
		[LogCategory("matchmaking_started", "Sent by MS when character starts matchmaking")]
		void MatchmakingStartedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.group_id)] string groupId, [LogParam(LogKey.group_size)] int groupSize, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.session_player_class)] string playerClass, [LogParam(LogKey.clan_name)] string clanName);

		// Token: 0x06000558 RID: 1368
		[LogCategory("matchmaking_finished", "Sent by MS when character ends matchmaking")]
		void MatchmakingFinishedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.group_id)] string groupId, [LogParam(LogKey.group_size)] int groupSize, [LogParam(LogKey.room_type)] GameRoomType eType, [LogParam(LogKey.match_time)] TimeSpan matchTime, [LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.clan_name)] string clanName, [LogParam(LogKey.mission_name)] string mapName);

		// Token: 0x06000559 RID: 1369
		[LogCategory("matchmaking_aborted", "Sent by MS when character aborts matchmaking")]
		void MatchmakingAbortedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.group_id)] string groupId, [LogParam(LogKey.group_size)] int groupSize, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.match_time)] TimeSpan matchTime, [LogParam(LogKey.clan_name)] string clanName);

		// Token: 0x0600055A RID: 1370
		[LogCategory("payment_spend_money_request", "Sent by MS when request for spending external money is going to be sent")]
		void PaymentSpendMoneyRequestLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.cry_money_diff)] long cryMoney);

		// Token: 0x0600055B RID: 1371
		[LogCategory("payment_money_spent", "Sent by MS when external money has been spent")]
		void PaymentMoneySpentLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.cry_money_diff)] long cryMoney, [LogParam(LogKey.payment_result)] PaymentResult result);

		// Token: 0x0600055C RID: 1372
		[LogCategory("payment_spend_money_failed", "Sent by MS when request for spending external money has been failed")]
		void PaymentSpendMoneyFailedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.cry_money_diff)] long cryMoney);

		// Token: 0x0600055D RID: 1373
		[LogCategory("payment_spend_money_offer_request", "Sent by MS when request for spending external money through the offer is going to be sent")]
		void PaymentSpendMoneyOfferRequestLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.shop_offer_id)] ulong storeId, [LogParam(LogKey.cry_money_diff)] long cryMoney);

		// Token: 0x0600055E RID: 1374
		[LogCategory("payment_money_offer_spent", "Sent by MS when external money through the offer has been spent")]
		void PaymentMoneyOfferSpentLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.shop_offer_id)] ulong storeId, [LogParam(LogKey.cry_money_diff)] long cryMoney, [LogParam(LogKey.payment_result)] PaymentResult result, [LogParam(LogKey.offer_status)] string offerStatus, [LogParam(LogKey.discount)] uint discount);

		// Token: 0x0600055F RID: 1375
		[LogCategory("payment_spend_money_offer_failed", "Sent by MS when request for spending external money through the offer has been failed")]
		void PaymentSpendMoneyOfferFailedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.shop_offer_id)] ulong storeId, [LogParam(LogKey.cry_money_diff)] long cryMoney);

		// Token: 0x06000560 RID: 1376
		[LogCategory("player_chat", "Send by CommunicationServer when player sends message in lobby chat (global, room, team, private message). Receiver's jid can be empty if message is not private")]
		void PlayerChatLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.message_type)] string messageType, [LogParam(LogKey.message)] string message, [LogParam(LogKey.room_name)] string roomName, [LogParam(LogKey.profile_jid)] string senderJid, [LogParam(LogKey.profile_jid_to)] string receiverJid);

		// Token: 0x06000561 RID: 1377
		[LogCategory("rank_updated", "Send by MS when profile's rank is updated")]
		void RankUpdatedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.added_rank_points)] ulong addedRankPoints, [LogParam(LogKey.old_rank_points)] ulong oldRankPoints, [LogParam(LogKey.old_rank_id)] int oldRankId, [LogParam(LogKey.rank_points)] ulong rankPoints, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.levelup_reason)] LevelChangeReason levelUpReason);

		// Token: 0x06000562 RID: 1378
		[LogCategory("rating_changed", "Send by MS when player rating updated")]
		void RatingChangedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.rating_season_id)] string ratingSeasonId, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.old_rating_points)] uint oldRatingPoints, [LogParam(LogKey.old_rating)] uint oldRating, [LogParam(LogKey.new_rating_points)] uint newRatingPoints, [LogParam(LogKey.new_rating)] uint newRating, [LogParam(LogKey.new_rating_win_streak)] uint newWinStreak);

		// Token: 0x06000563 RID: 1379
		[LogCategory("rating_season_reward_given", "Send by MS when player got rating season reward")]
		void RatingSeasonRewardGivenLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.rating)] uint rating, [LogParam(LogKey.rating_season_id)] string ratingSeasonId, [LogParam(LogKey.rating_reward_set)] string ratingRewardSet, [LogParam(LogKey.produce_type)] LogGroup.ProduceType ratingRewardProduceType);

		// Token: 0x06000564 RID: 1380
		[LogCategory("room_joined", "Send by MS when player either creates or joins the room (quick play, follow, invite)")]
		void RoomJoinedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.group_id)] string groupId, [LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.room_name)] string roomName, [LogParam(LogKey.join_source)] string joinSource, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.clan_name)] string clanName);

		// Token: 0x06000565 RID: 1381
		[LogCategory("room_left", "Send by MS when player lefts the room (graceful, kick, kick by timeout)")]
		void RoomLeftLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.group_id)] string groupId, [LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.remove_reason)] GameRoomPlayerRemoveReason reason, [LogParam(LogKey.room_player_status)] RoomPlayer.EStatus playerStatus, [LogParam(LogKey.room_name)] string roomName, [LogParam(LogKey.player_time_in_room)] TimeSpan timeInRoom, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.clan_name)] string clanName);

		// Token: 0x06000566 RID: 1382
		[LogCategory("room_close", "Send by MS when room is closing")]
		void RoomCloseLog([LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.room_name)] string roomName, [LogParam(LogKey.room_life_time)] TimeSpan roomLifeTime);

		// Token: 0x06000567 RID: 1383
		[LogCategory("room_autobalance_result", "Send by MS when autobalance finish assigning teams")]
		void RoomAutoBalanceResult([LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.team_1_size)] int team1Size, [LogParam(LogKey.team_1_skill)] string team1Skill, [LogParam(LogKey.team_2_size)] int team2Size, [LogParam(LogKey.team_2_skill)] string team2Skill);

		// Token: 0x06000568 RID: 1384
		[LogCategory("skill_changed", "Send by MS when player skill is changed")]
		void SkillChangedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.skill_type)] SkillType skillType, [LogParam(LogKey.skill)] uint skill, [LogParam(LogKey.skill_points)] uint skill_points, [LogParam(LogKey.curve_coef)] uint curveCoef);

		// Token: 0x06000569 RID: 1385
		[LogCategory("room_reconnect", "Send by MS after player tried to reconnect")]
		void RoomReconnectLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.reconnect_result)] ReconnectResult reconnectResult);

		// Token: 0x0600056A RID: 1386
		[LogCategory("session_details_ex", "Sent by MS when session is ended")]
		void SessionDetailsExLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.session_start_time)] DateTime sessionStartTime, [LogParam(LogKey.session_player_time)] TimeSpan playerTime, [LogParam(LogKey.session_time)] TimeSpan sessionTime, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.sub_mode)] string subMode, [LogParam(LogKey.added_rank_points)] ulong addedRankPoints, [LogParam(LogKey.rank_points)] ulong rankPoints, [LogParam(LogKey.game_money_diff)] long gainedMoney, [LogParam(LogKey.game_money)] ulong gameMoney, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.room_name)] string roomName, [LogParam(LogKey.sponsor_id)] uint sponsorId, [LogParam(LogKey.added_sponsor_points)] uint gainedSponsorPoint, [LogParam(LogKey.mission_name)] string missionName, [LogParam(LogKey.mission_setting)] string missionSetting, [LogParam(LogKey.mission_difficulty)] string missionDifficulty, [LogParam(LogKey.mission_type)] string missionType, [LogParam(LogKey.session_status)] int sessionStatus, [LogParam(LogKey.mission_game_mode)] string missionGameMode, [LogParam(LogKey.session_player_class)] string playerClass, [LogParam(LogKey.session_coin_spent)] uint spentCoin, [LogParam(LogKey.kill_count)] uint killCount, [LogParam(LogKey.death_count)] uint deathCount, [LogParam(LogKey.skill)] uint skill, [LogParam(LogKey.headshot_kill_count)] uint headshotCount, [LogParam(LogKey.melee_kill_count)] uint meleeCount, [LogParam(LogKey.frag_kill_count)] uint fragCount, [LogParam(LogKey.claymore_kill_count)] uint claymoreCount, [LogParam(LogKey.butt_kill_count)] uint buttCount, [LogParam(LogKey.slide_kill_count)] uint slideCount, [LogParam(LogKey.dynamic_experience_multiplier)] string dynamicExperienceMultiplier, [LogParam(LogKey.dynamic_money_multiplier)] string dynamicMoneyMultiplier, [LogParam(LogKey.dynamic_sponsor_points_multiplier)] string dynamicSponsorPointsMultiplier, [LogParam(LogKey.dynamic_crown_multiplier)] string dynamicCrownMultiplier, [LogParam(LogKey.dynamic_multiplier_providers)] string dynamicMultiplierProviders, [LogParam(LogKey.first_checkpoint)] uint firstCheckpoint, [LogParam(LogKey.last_checkpoint)] uint lastCheckpoint, [LogParam(LogKey.skill_type)] SkillType skillType, [LogParam(LogKey.first_win)] bool firstWin, [LogParam(LogKey.clan_id)] ulong clanId, [LogParam(LogKey.clan_name)] string clanName, [LogParam(LogKey.clan_points_added)] uint gainedClanPoints);

		// Token: 0x0600056B RID: 1387
		[LogCategory("session_details_observer_ex", "Sent by MS when session is ended")]
		void SessionDetailsObserverExLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.session_start_time)] DateTime sessionStartTime, [LogParam(LogKey.session_player_time)] TimeSpan playerTime, [LogParam(LogKey.session_time)] TimeSpan sessionTime, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.sub_mode)] string subMode, [LogParam(LogKey.rank_points)] ulong rankPoints, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.room_name)] string roomName, [LogParam(LogKey.mission_name)] string missionName, [LogParam(LogKey.mission_setting)] string missionSetting, [LogParam(LogKey.mission_difficulty)] string missionDifficulty, [LogParam(LogKey.mission_type)] string missionType, [LogParam(LogKey.mission_game_mode)] string missionGameMode);

		// Token: 0x0600056C RID: 1388
		[LogCategory("session_ended", "Send by MS when the room has finished to play current game session")]
		void SessionEndedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.mission_type)] string missionType, [LogParam(LogKey.observer)] bool observer);

		// Token: 0x0600056D RID: 1389
		[LogCategory("session_started", "Send by MS when the room has started to play new game session")]
		void SessionStartedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.group_id)] string groupId, [LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.mission_type)] string missionType, [LogParam(LogKey.observer)] bool observer, [LogParam(LogKey.game_lead_time)] TimeSpan gameLeadTime, [LogParam(LogKey.session_player_class)] string playerClass, [LogParam(LogKey.region_id)] string regionId, [LogParam(LogKey.clan_name)] string clanName);

		// Token: 0x0600056E RID: 1390
		[LogCategory("session_total_ex", "Sent by MS when session is ended")]
		void SessionTotalExLog([LogParam(LogKey.room_type)] GameRoomType roomType, [LogParam(LogKey.sub_mode)] string subMode, [LogParam(LogKey.session_start_time)] DateTime sessionStartTime, [LogParam(LogKey.session_time)] TimeSpan sessionTime, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.room_name)] string roomName, [LogParam(LogKey.session_players)] int playersCount, [LogParam(LogKey.mission_sublevels)] int missionSublevelsCount, [LogParam(LogKey.mission_level_graph)] string missionLevelGraph, [LogParam(LogKey.mission_game_mode)] string missionGameMode, [LogParam(LogKey.mission_name)] string missionName, [LogParam(LogKey.mission_setting)] string missionSetting, [LogParam(LogKey.mission_difficulty)] string missionDifficulty, [LogParam(LogKey.mission_type)] string missionType, [LogParam(LogKey.session_player_deaths)] uint totalDeathCount, [LogParam(LogKey.session_coin_spent)] uint totalSpentCoin, [LogParam(LogKey.mission_type_experience_multiplier)] string missionTypeExperienceMultiplier, [LogParam(LogKey.mission_type_money_multiplier)] string missionTypeMoneyMultiplier, [LogParam(LogKey.mission_type_sponsor_points_multiplier)] string missionTypeSponsorPointsMultiplier, [LogParam(LogKey.is_autobalanced)] bool isAutobalanced, [LogParam(LogKey.team_1_score)] int team1Score, [LogParam(LogKey.team_2_score)] int team2Score, [LogParam(LogKey.team_1_kills)] int team1Kills, [LogParam(LogKey.team_2_kills)] int team2Kills);

		// Token: 0x0600056F RID: 1391
		[LogCategory("shop_money_changed", "Send by MS when player's money balance was changed (except payment debug commands)")]
		void ShopMoneyChangedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.game_money_diff)] long gameMoneyDiff, [LogParam(LogKey.cry_money_diff)] long cryMoneyDiff, [LogParam(LogKey.crown_money_diff)] long crownMoneyDiff, [LogParam(LogKey.produce_type)] LogGroup.ProduceType produceType, [LogParam(LogKey.shop_tr_status)] TransactionStatus transactionStatus, [LogParam(LogKey.give_money_transaction_id)] string giveMoneyTransactionId, [LogParam(LogKey.reason)] string reason);

		// Token: 0x06000570 RID: 1392
		[LogCategory("shop_offer_bought", "Send by MS when player purchases or failed to buy offer")]
		void ShopOfferBoughtLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.shop_tr_status)] TransactionStatus transactionStatus, [LogParam(LogKey.game_money_diff)] long spentGameMoney, [LogParam(LogKey.cry_money_diff)] long spentCryMoney, [LogParam(LogKey.crown_money_diff)] long spentCrownMoney, [LogParam(LogKey.key_item_name)] string priceKeyItemName, [LogParam(LogKey.game_money)] ulong gameMoney, [LogParam(LogKey.cry_money)] ulong cryMoney, [LogParam(LogKey.crown_money)] ulong crownMoney, [LogParam(LogKey.offer_status)] string offerStatus, [LogParam(LogKey.discount)] uint discount, [LogParam(LogKey.shop_offer_id)] ulong shopOfferId, [LogParam(LogKey.offer_type)] OfferType offerType, [LogParam(LogKey.catalog_item_id)] ulong catalogItemId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.item_total_durability)] int totalDurabilityPoints, [LogParam(LogKey.item_expiration_time)] string expirationTime, [LogParam(LogKey.item_type)] string itemType, [LogParam(LogKey.items_left)] ulong itemsLeft, [LogParam(LogKey.items_quantity)] ulong itemsQuantity, [LogParam(LogKey.produce_type)] LogGroup.ProduceType gameInterface, [LogParam(LogKey.profile_item_id)] ulong profileItemId = 0UL, [LogParam(LogKey.reason)] string reason = "-");

		// Token: 0x06000571 RID: 1393
		[LogCategory("shop_offer_bought_failed", "Sent by MS when character fails to buy item in ecatalog")]
		void ShopOfferBoughtFailedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.nickname)] string nickname, [LogParam(LogKey.rank_id)] int rankId, [LogParam(LogKey.ip_address)] string ip, [LogParam(LogKey.shop_tr_status)] TransactionStatus transactionStatus, [LogParam(LogKey.game_money_diff)] long spentGameMoney, [LogParam(LogKey.cry_money_diff)] long spentCryMoney, [LogParam(LogKey.crown_money_diff)] long spentCrownMoney, [LogParam(LogKey.game_money)] ulong gameMoney, [LogParam(LogKey.cry_money)] ulong cryMoney, [LogParam(LogKey.crown_money)] ulong crownMoney, [LogParam(LogKey.shop_offer_id)] ulong shopOfferId, [LogParam(LogKey.offer_type)] OfferType offerType, [LogParam(LogKey.catalog_item_id)] ulong catalogItemId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.item_total_durability)] int totalDurabilityPoints, [LogParam(LogKey.item_expiration_time)] string expirationTime, [LogParam(LogKey.items_quantity)] ulong itemsQuantity, [LogParam(LogKey.produce_type)] LogGroup.ProduceType gameInterface);

		// Token: 0x06000572 RID: 1394
		[LogCategory("shop_offer_confirm", "Sent by MS when character confirm storage item")]
		void ConfirmStorageItemLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.supplier_id)] int supplierId, [LogParam(LogKey.offer_type)] OfferType offerType, [LogParam(LogKey.catalog_item_id)] ulong catalogItemId, [LogParam(LogKey.item_name)] string itemName, [LogParam(LogKey.game_money_diff)] long gameMoneyPrice, [LogParam(LogKey.cry_money_diff)] long cryMoneyPrice, [LogParam(LogKey.crown_money_diff)] long crownMoneyPrice, [LogParam(LogKey.shop_tr_status)] TransactionStatus transactionStatus);

		// Token: 0x06000573 RID: 1395
		[LogCategory("shop_offer_refund", "Sent by MS when character confirm storage item")]
		void RefundStorageItemLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.supplier_id)] int supplierId, [LogParam(LogKey.order_id)] ulong orderId, [LogParam(LogKey.refund_status)] int refundStatus, [LogParam(LogKey.description)] string description);

		// Token: 0x06000574 RID: 1396
		[LogCategory("sponsor_points", "Send by MS when profile's sponsor points are updated")]
		void SponsorPointsLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.sponsor_id)] uint sponsorId, [LogParam(LogKey.added_sponsor_points)] uint gainedSponsorPoint, [LogParam(LogKey.item_number_previous)] int itemNumberPrevious, [LogParam(LogKey.sponsor_points_current)] ulong sponsorPointsCurrent, [LogParam(LogKey.item_number_current)] int itemNumberCurrent);

		// Token: 0x06000575 RID: 1397
		[LogCategory("tutorial_passed", "Sent by MS when tutorial passed")]
		void TutorialPassedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.tutorial_id)] ProfileProgressionInfo.Tutorial tutorialId, [LogParam(LogKey.rank_id)] int rankId);

		// Token: 0x06000576 RID: 1398
		[LogCategory("tutorial_unlocked", "Sent by MS when tutorial unlocked")]
		void TutorialUnlockedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.tutorial_id)] ProfileProgressionInfo.Tutorial tutorialId, [LogParam(LogKey.rank_id)] int rankId);

		// Token: 0x06000577 RID: 1399
		[LogCategory("class_unlocked", "Sent by MS when class unlocked")]
		void ClassUnlockedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.class_id)] ProfileProgressionInfo.PlayerClass classId, [LogParam(LogKey.rank_id)] int rankId);

		// Token: 0x06000578 RID: 1400
		[LogCategory("mission_unlocked", "Sent by MS when mission unlocked")]
		void MissionUnlockedLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.mission_type)] string missionType, [LogParam(LogKey.rank_id)] int rankId);

		// Token: 0x06000579 RID: 1401
		[LogCategory("tutorial_stats", "Sent by MS when player attempts to pass any of the tutorial missions")]
		void TutorialStatsLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.completion_status)] TutorialEvent toLower, [LogParam(LogKey.tutorial_mission)] int tutorialId, [LogParam(LogKey.tutorial_step)] string tutorialStep, [LogParam(LogKey.playtime)] TimeSpan playtime, [LogParam(LogKey.playtime_since_last_step)] TimeSpan playtimeSinceLastStep, [LogParam(LogKey.played_first_time)] bool playedFirstTime);

		// Token: 0x0600057A RID: 1402
		[LogCategory("crown_money_given", "Sent by MS when player receive crown for mission from crown reward service")]
		void CrownMoneyGivenLog([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.mission_type)] string missionType, [LogParam(LogKey.performanceCategory)] CrownRewardThreshold.PerformanceCategory performanceCategory, [LogParam(LogKey.crown_money)] ulong crownMoney, [LogParam(LogKey.reward_time)] DateTime rewardTime);

		// Token: 0x0600057B RID: 1403
		[LogCategory("players_performance_progress", "Sent by MS when to report players progress per sublevel")]
		void PlayersPerformanceProgressLog([LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.mission_name)] string missionName, [LogParam(LogKey.mission_difficulty)] string missionDifficulty, [LogParam(LogKey.mission_sublevels)] int missionSublevelsCount, [LogParam(LogKey.mission_sublevel_name)] string subLevelName, [LogParam(LogKey.mission_sublevel_flow)] string subLevelFlow, [LogParam(LogKey.stat_id_performance)] int value_0, [LogParam(LogKey.stat_id_headshots_points)] int value_1, [LogParam(LogKey.stat_id_melee_points)] int value_2, [LogParam(LogKey.stat_id_explosions_points)] int value_3, [LogParam(LogKey.stat_id_average_multiplier)] int value_4, [LogParam(LogKey.stat_id_time)] int value_5);

		// Token: 0x0600057C RID: 1404
		[LogCategory("players_invite_fail", "Sent by MS when invite/follow failed")]
		void PlayerInviteFailedLog([LogParam(LogKey.invite_initiator)] ulong from, [LogParam(LogKey.invite_target)] ulong to, [LogParam(LogKey.invite_status)] EInvitationStatus status);

		// Token: 0x0600057D RID: 1405
		[LogCategory("friend_remove", "Sent by MS when user remove someone from friends")]
		void FriendRemoveLog([LogParam(LogKey.initiator)] ulong from, [LogParam(LogKey.target)] ulong to);

		// Token: 0x0600057E RID: 1406
		[LogCategory("friend_invite", "Sent by MS when user invites someone to friends")]
		void FriendInviteLog([LogParam(LogKey.invite_initiator)] ulong from, [LogParam(LogKey.invite_target)] ulong to, [LogParam(LogKey.friend_invite_status)] EInviteStatus status);

		// Token: 0x0600057F RID: 1407
		[LogCategory("ui_user_choice", "Sent by MS when it receives UI event from client")]
		void UiUserChoice([LogParam(LogKey.user_id)] ulong fromUser, [LogParam(LogKey.profile_id)] ulong fromProfile, [LogParam(LogKey.choice_from)] string choiceFrom, [LogParam(LogKey.choice_id)] string choiceId, [LogParam(LogKey.choice_result)] int choiceResult);

		// Token: 0x06000580 RID: 1408
		[LogCategory("unblock_purchase", "Sent by MS when hard currecncy purchase is unblocked")]
		void UnblockPurchase([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.profile_id)] ulong profileId);

		// Token: 0x06000581 RID: 1409
		[LogCategory("in_game_event_reward", "Sent by MS when in-game event for reward is received")]
		void InGameEventReward([LogParam(LogKey.event_name)] string eventName, [LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.profile_id)] ulong toProfile, [LogParam(LogKey.reward_set)] string rewardSet);

		// Token: 0x06000582 RID: 1410
		[LogCategory("top_prize_tokens_reset", "Sent by MS when top prize tokens count (of any kind) for the specific user is reset to zero")]
		void TopPrizeTokensReset([LogParam(LogKey.profile_id)] ulong profileId, [LogParam(LogKey.key_item_name)] string tokenName, [LogParam(LogKey.key_items_amount_before_reset)] ulong tokenCount);

		// Token: 0x06000583 RID: 1411
		[LogCategory("map_voting_result", "Sent by MS when map change voting in autostart room is finished")]
		void MapVotingResult([LogParam(LogKey.room_id)] ulong roomId, [LogParam(LogKey.room_name)] string roomName, [LogParam(LogKey.maps_in_voting)] string mapsInVoting);

		// Token: 0x06000584 RID: 1412
		[LogCategory("voucher_consumed", "Sent by MS when voucher was processed ")]
		void VoucherConsumed([LogParam(LogKey.user_id)] ulong userId, [LogParam(LogKey.voucher_id)] ulong voucherId, [LogParam(LogKey.voucher_type)] string voucherType, [LogParam(LogKey.voucher_status)] VoucherStatus status, [LogParam(LogKey.voucher_data)] string voucherData);

		// Token: 0x06000585 RID: 1413
		[LogCategory("kick_voting_result", "Sent by MS when kick voting is ended")]
		void KickVotingResult([LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.initiator)] ulong initiatorId, [LogParam(LogKey.target)] ulong targetId, [LogParam(LogKey.vote_result)] int voteResult, [LogParam(LogKey.votes_for)] int votesFor, [LogParam(LogKey.votes_against)] int votesAgainst);

		// Token: 0x06000586 RID: 1414
		[LogCategory("surrender_voting_result", "Sent by MS when surrender voting is ended")]
		void SurrenderVotingResult([LogParam(LogKey.session_id)] string sessionId, [LogParam(LogKey.vote_result)] int voteResult, [LogParam(LogKey.votes_for)] int votesFor, [LogParam(LogKey.votes_against)] int votesAgainst);
	}
}
