using System;
using System.Collections.Generic;
using System.Threading;
using MasterServer.Core.Configuration;

namespace MasterServer.Core
{
	// Token: 0x0200013F RID: 319
	public class LogGroup : IDisposable
	{
		// Token: 0x06000587 RID: 1415 RVA: 0x000166DC File Offset: 0x00014ADC
		public LogGroup(int group, IEnumerable<ILogSync> sync)
		{
			this.m_group = group;
			this.m_sync = sync;
			ConfigSection section = Resources.CommonSettings.GetSection("log");
			if (section == null || !section.Get("timestamp_format", out LogGroup.m_timestampFormat))
			{
				LogGroup.m_timestampFormat = "yyyy-MM-ddTHH:mm:ss.fffzz";
			}
		}

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06000588 RID: 1416 RVA: 0x00016734 File Offset: 0x00014B34
		// (remove) Token: 0x06000589 RID: 1417 RVA: 0x0001676C File Offset: 0x00014B6C
		public event Action<string> OnEvent;

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x0600058A RID: 1418 RVA: 0x000167A4 File Offset: 0x00014BA4
		protected static string TimestampFormat
		{
			get
			{
				if (string.IsNullOrEmpty(LogGroup.m_timestampFormat))
				{
					ConfigSection section = Resources.CommonSettings.GetSection("log");
					string value;
					if (section == null || !section.Get("timestamp_format", out value))
					{
						value = "yyyy-MM-ddTHH:mm:ss.fffzz";
					}
					Interlocked.CompareExchange<string>(ref LogGroup.m_timestampFormat, value, null);
				}
				return LogGroup.m_timestampFormat;
			}
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x00016800 File Offset: 0x00014C00
		public void Dispose()
		{
			if (this.m_group != 0)
			{
				foreach (ILogSync logSync in this.m_sync)
				{
					logSync.EndGroup(this.m_group);
				}
			}
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x0001686C File Offset: 0x00014C6C
		protected void WriteToLogs(string messageCategory, string message)
		{
			foreach (ILogSync logSync in this.m_sync)
			{
				try
				{
					logSync.WriteToLog(this.m_group, message);
					if (this.OnEvent != null)
					{
						this.OnEvent(messageCategory);
					}
				}
				catch (Exception e)
				{
					Log.Error<Type>("Log syncing for {0} synchronizer failed.", logSync.GetType());
					Log.Error(e);
				}
			}
		}

		// Token: 0x04000330 RID: 816
		public static readonly string RECORD_SEPARATOR = ",";

		// Token: 0x04000331 RID: 817
		public const string ABUSE_REPORT = "abuse_report";

		// Token: 0x04000332 RID: 818
		public const string ADMIN_COMMAND_REPORT = "admin_command";

		// Token: 0x04000333 RID: 819
		public const string ANTI_ADDICTION_RULE = "anti_addiction_rule";

		// Token: 0x04000334 RID: 820
		public const string ANTICHEAT_REPORT = "anticheat_report";

		// Token: 0x04000335 RID: 821
		public const string ANTICHEAT_IMMEDIATE_REPORT = "anticheat_immediate_report";

		// Token: 0x04000336 RID: 822
		public const string AUTO_REPAIR_EQUIPMENT_RESULT = "auto_repair_equipment_res";

		// Token: 0x04000337 RID: 823
		public const string BLOCK_PURCHASE = "block_purchase";

		// Token: 0x04000338 RID: 824
		public const string CATALOG_OFFER = "catalog_offer";

		// Token: 0x04000339 RID: 825
		public const string CATALOG_ITEM = "catalog_item";

		// Token: 0x0400033A RID: 826
		public const string CLAN_INVITE_MEMBER = "clan_invite_member";

		// Token: 0x0400033B RID: 827
		public const string CLAN_CREATE = "clan_create";

		// Token: 0x0400033C RID: 828
		public const string CLAN_SET_ROLE = "clan_set_role";

		// Token: 0x0400033D RID: 829
		public const string CLAN_KICK = "clan_kick";

		// Token: 0x0400033E RID: 830
		public const string CLAN_LEAVE = "clan_leave";

		// Token: 0x0400033F RID: 831
		public const string CLAN_DELETE = "clan_delete";

		// Token: 0x04000340 RID: 832
		public const string CLASS_UNLOCKED = "class_unlocked";

		// Token: 0x04000341 RID: 833
		public const string CHANNEL_SWITCH = "channel_switch";

		// Token: 0x04000342 RID: 834
		public const string CHARACTER_ACHIEVED = "character_achieved";

		// Token: 0x04000343 RID: 835
		public const string CHARACTER_ALARM = "character_alarm";

		// Token: 0x04000344 RID: 836
		public const string CHARACTER_BAN = "character_ban";

		// Token: 0x04000345 RID: 837
		public const string CHARACTER_CREATION = "character_creation";

		// Token: 0x04000346 RID: 838
		public const string CHARACTER_DELETION = "character_deletion";

		// Token: 0x04000347 RID: 839
		public const string CHARACTER_KICK = "character_kick";

		// Token: 0x04000348 RID: 840
		public const string CHARACTER_LEVEL_UP = "character_levelup";

		// Token: 0x04000349 RID: 841
		public const string CHARACTER_LOGIN = "character_login";

		// Token: 0x0400034A RID: 842
		public const string CHARACTER_LOGOUT = "character_logout";

		// Token: 0x0400034B RID: 843
		public const string CHARACTER_MUTE = "character_mute";

		// Token: 0x0400034C RID: 844
		public const string CHARACTER_UNBAN = "character_unban";

		// Token: 0x0400034D RID: 845
		public const string CHARACTER_UNMUTE = "character_unmute";

		// Token: 0x0400034E RID: 846
		public const string CONSECUTIVE_LOGIN_BONUS_ACTIVATION = "consecutive_login_bonus_activation";

		// Token: 0x0400034F RID: 847
		public const string CONTRACT_ACTIVATE = "contract_activate";

		// Token: 0x04000350 RID: 848
		public const string CONTRACT_COMPLETE = "contract_complete";

		// Token: 0x04000351 RID: 849
		public const string CONTRACT_FAIL = "contract_fail";

		// Token: 0x04000352 RID: 850
		public const string CUSTOM_RULE = "custom_rule";

		// Token: 0x04000353 RID: 851
		public const string CUSTOM_RULE_TRIGGERED = "custom_rule_triggered";

		// Token: 0x04000354 RID: 852
		public const string CROWN_MONEY_GIVEN = "crown_money_given";

		// Token: 0x04000355 RID: 853
		public const string DEFAULT_PROFILE = "default_profile";

		// Token: 0x04000356 RID: 854
		public const string FRIEND_REMOVE = "friend_remove";

		// Token: 0x04000357 RID: 855
		public const string FRIEND_INVITE = "friend_invite";

		// Token: 0x04000358 RID: 856
		public const string GAME_INTERFACE_CMD = "game_interface_cmd";

		// Token: 0x04000359 RID: 857
		public const string IN_GAME_EVENT_REWARD = "in_game_event_reward";

		// Token: 0x0400035A RID: 858
		public const string ITEM_CACHE = "item_cache";

		// Token: 0x0400035B RID: 859
		public const string ITEM_CONSUMED = "item_consumed";

		// Token: 0x0400035C RID: 860
		public const string ITEM_DESTROY = "item_destroy";

		// Token: 0x0400035D RID: 861
		public const string ITEM_EQUIP = "item_equip";

		// Token: 0x0400035E RID: 862
		public const string ITEM_EXPIRE = "item_expire";

		// Token: 0x0400035F RID: 863
		public const string ITEM_GIVEN = "item_given";

		// Token: 0x04000360 RID: 864
		public const string ITEM_REPAIR = "item_repair";

		// Token: 0x04000361 RID: 865
		public const string ITEM_REIMBURSEMENT = "item_reimbursement";

		// Token: 0x04000362 RID: 866
		public const string ITEM_LOCKED = "item_locked";

		// Token: 0x04000363 RID: 867
		public const string ITEM_UNLOCKED = "item_unlocked";

		// Token: 0x04000364 RID: 868
		public const string ITEM_USE = "item_use";

		// Token: 0x04000365 RID: 869
		public const string KICK_VOTING_RESULT = "kick_voting_result";

		// Token: 0x04000366 RID: 870
		public const string LOSE_CONNECTION = "lose_connection";

		// Token: 0x04000367 RID: 871
		public const string MAP_VOTING_RESULT = "map_voting_result";

		// Token: 0x04000368 RID: 872
		public const string MATCHMAKING_STARTED = "matchmaking_started";

		// Token: 0x04000369 RID: 873
		public const string MATCHMAKING_FINISHED = "matchmaking_finished";

		// Token: 0x0400036A RID: 874
		public const string MATCHMAKING_ABORTED = "matchmaking_aborted";

		// Token: 0x0400036B RID: 875
		public const string MISSION_UNLOCKED = "mission_unlocked";

		// Token: 0x0400036C RID: 876
		public const string PAYMENT_SPEND_MONEY_FAILED = "payment_spend_money_failed";

		// Token: 0x0400036D RID: 877
		public const string PAYMENT_SPEND_MONEY_OFFER_FAILED = "payment_spend_money_offer_failed";

		// Token: 0x0400036E RID: 878
		public const string PAYMENT_MONEY_OFFER_SPENT = "payment_money_offer_spent";

		// Token: 0x0400036F RID: 879
		public const string PAYMENT_MONEY_SPENT = "payment_money_spent";

		// Token: 0x04000370 RID: 880
		public const string PAYMENT_SPEND_MONEY_REQUEST = "payment_spend_money_request";

		// Token: 0x04000371 RID: 881
		public const string PAYMENT_SPEND_MONEY_OFFER_REQUEST = "payment_spend_money_offer_request";

		// Token: 0x04000372 RID: 882
		public const string PLAYER_CHAT = "player_chat";

		// Token: 0x04000373 RID: 883
		public const string PLAYERS_PERFORMANCE_PROGRESS = "players_performance_progress";

		// Token: 0x04000374 RID: 884
		public const string PLAYER_INVITE_FAIL = "players_invite_fail";

		// Token: 0x04000375 RID: 885
		public const string RANK_UPDATED = "rank_updated";

		// Token: 0x04000376 RID: 886
		public const string RATING_CHANGED = "rating_changed";

		// Token: 0x04000377 RID: 887
		public const string RATING_SEASON_REWARD_GIVEN = "rating_season_reward_given";

		// Token: 0x04000378 RID: 888
		public const string ROOM_AUTOBALANCE_RESULT = "room_autobalance_result";

		// Token: 0x04000379 RID: 889
		public const string ROOM_JOINED = "room_joined";

		// Token: 0x0400037A RID: 890
		public const string ROOM_LEFT = "room_left";

		// Token: 0x0400037B RID: 891
		public const string ROOM_CLOSE = "room_close";

		// Token: 0x0400037C RID: 892
		public const string ROOM_RECONNECT = "room_reconnect";

		// Token: 0x0400037D RID: 893
		public const string SESSION_DETAILS_EX = "session_details_ex";

		// Token: 0x0400037E RID: 894
		public const string SESSION_DETAILS_OBSERVER_EX = "session_details_observer_ex";

		// Token: 0x0400037F RID: 895
		public const string SESSION_ENDED = "session_ended";

		// Token: 0x04000380 RID: 896
		public const string SESSION_STARTED = "session_started";

		// Token: 0x04000381 RID: 897
		public const string SESSION_TOTAL_EX = "session_total_ex";

		// Token: 0x04000382 RID: 898
		public const string SHOP_MONEY_CHANGED = "shop_money_changed";

		// Token: 0x04000383 RID: 899
		public const string SHOP_OFFER_BOUGHT = "shop_offer_bought";

		// Token: 0x04000384 RID: 900
		public const string SHOP_OFFER_BOUGHT_FAILED = "shop_offer_bought_failed";

		// Token: 0x04000385 RID: 901
		public const string SHOP_OFFER_CONFIRM = "shop_offer_confirm";

		// Token: 0x04000386 RID: 902
		public const string SHOP_OFFER_REFUND = "shop_offer_refund";

		// Token: 0x04000387 RID: 903
		public const string SKILL_CHANGED = "skill_changed";

		// Token: 0x04000388 RID: 904
		public const string SPONSOR_POINTS = "sponsor_points";

		// Token: 0x04000389 RID: 905
		public const string SURRENDER_VOTING_RESULT = "surrender_voting_result";

		// Token: 0x0400038A RID: 906
		public const string TOP_PRIZE_TOKENS_RESET = "top_prize_tokens_reset";

		// Token: 0x0400038B RID: 907
		public const string TUTORIAL_PASSED = "tutorial_passed";

		// Token: 0x0400038C RID: 908
		public const string TUTORIAL_UNLOCKED = "tutorial_unlocked";

		// Token: 0x0400038D RID: 909
		public const string TUTORIAL_STATS = "tutorial_stats";

		// Token: 0x0400038E RID: 910
		public const string UI_USER_CHOICE = "ui_user_choice";

		// Token: 0x0400038F RID: 911
		public const string UNBLOCK_PURCHASE = "unblock_purchase";

		// Token: 0x04000390 RID: 912
		public const string VOUCHER_CONSUMED = "voucher_consumed";

		// Token: 0x04000391 RID: 913
		private readonly int m_group;

		// Token: 0x04000392 RID: 914
		private readonly IEnumerable<ILogSync> m_sync;

		// Token: 0x04000393 RID: 915
		private static string m_timestampFormat;

		// Token: 0x02000140 RID: 320
		public enum ProduceType
		{
			// Token: 0x04000395 RID: 917
			Buy,
			// Token: 0x04000396 RID: 918
			Reward,
			// Token: 0x04000397 RID: 919
			SpecialReward,
			// Token: 0x04000398 RID: 920
			Achievement,
			// Token: 0x04000399 RID: 921
			GameInterface,
			// Token: 0x0400039A RID: 922
			CrownReward,
			// Token: 0x0400039B RID: 923
			Leaderboard,
			// Token: 0x0400039C RID: 924
			Repair,
			// Token: 0x0400039D RID: 925
			Extend,
			// Token: 0x0400039E RID: 926
			Reimbursement,
			// Token: 0x0400039F RID: 927
			Contract,
			// Token: 0x040003A0 RID: 928
			RandomBox,
			// Token: 0x040003A1 RID: 929
			ProfileProgression,
			// Token: 0x040003A2 RID: 930
			Bundle,
			// Token: 0x040003A3 RID: 931
			VendorUnlock,
			// Token: 0x040003A4 RID: 932
			Voucher,
			// Token: 0x040003A5 RID: 933
			RatingSeasonResult,
			// Token: 0x040003A6 RID: 934
			RatingAchieved
		}
	}
}
