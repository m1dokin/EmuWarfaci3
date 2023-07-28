using System;
using System.Net;
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
	// Token: 0x0200013E RID: 318
	public enum LogKey
	{
		// Token: 0x04000233 RID: 563
		[LogKey(typeof(string))]
		abuse_comment,
		// Token: 0x04000234 RID: 564
		[LogKey(typeof(string))]
		abuse_reason,
		// Token: 0x04000235 RID: 565
		[LogKey(typeof(ulong))]
		abuser_pid,
		// Token: 0x04000236 RID: 566
		[LogKey(typeof(ulong))]
		abuser_uid,
		// Token: 0x04000237 RID: 567
		[LogKey(typeof(EAchevementStatus))]
		achievement_completion,
		// Token: 0x04000238 RID: 568
		[LogKey(typeof(uint))]
		achievement_id,
		// Token: 0x04000239 RID: 569
		[LogKey(typeof(int))]
		achievement_progress,
		// Token: 0x0400023A RID: 570
		[LogKey(typeof(ulong))]
		added_rank_points,
		// Token: 0x0400023B RID: 571
		[LogKey(typeof(uint))]
		added_sponsor_points,
		// Token: 0x0400023C RID: 572
		[LogKey(typeof(string))]
		alarm_reason,
		// Token: 0x0400023D RID: 573
		[LogKey(typeof(string))]
		anticheat_calls,
		// Token: 0x0400023E RID: 574
		[LogKey(typeof(string))]
		anticheat_description,
		// Token: 0x0400023F RID: 575
		[LogKey(typeof(string))]
		anticheat_score,
		// Token: 0x04000240 RID: 576
		[LogKey(typeof(string))]
		anticheat_type,
		// Token: 0x04000241 RID: 577
		[LogKey(typeof(string))]
		auth_token,
		// Token: 0x04000242 RID: 578
		[LogKey(typeof(BanReportSource))]
		ban_source,
		// Token: 0x04000243 RID: 579
		[LogKey(typeof(uint))]
		butt_kill_count,
		// Token: 0x04000244 RID: 580
		[LogKey(typeof(ulong))]
		catalog_item_id,
		// Token: 0x04000245 RID: 581
		channel_id,
		// Token: 0x04000246 RID: 582
		[LogKey(typeof(uint))]
		checkpoint,
		// Token: 0x04000247 RID: 583
		[LogKey(typeof(string))]
		choice_from,
		// Token: 0x04000248 RID: 584
		[LogKey(typeof(string))]
		choice_id,
		// Token: 0x04000249 RID: 585
		[LogKey(typeof(int))]
		choice_result,
		// Token: 0x0400024A RID: 586
		[LogKey(typeof(ulong))]
		clan_id,
		// Token: 0x0400024B RID: 587
		[LogKey(typeof(string))]
		clan_name,
		// Token: 0x0400024C RID: 588
		[LogKey(typeof(int))]
		clan_members_count,
		// Token: 0x0400024D RID: 589
		[LogKey(typeof(ulong))]
		clan_initiator_profileId,
		// Token: 0x0400024E RID: 590
		[LogKey(typeof(ulong))]
		clan_target_profileId,
		// Token: 0x0400024F RID: 591
		[LogKey(typeof(EClanRole))]
		clan_role,
		// Token: 0x04000250 RID: 592
		[LogKey(typeof(EClanCreationStatus))]
		clan_create_status,
		// Token: 0x04000251 RID: 593
		[LogKey(typeof(EInviteStatus))]
		clan_invite_member_result,
		// Token: 0x04000252 RID: 594
		[LogKey(typeof(uint))]
		clan_points_added,
		// Token: 0x04000253 RID: 595
		[LogKey(typeof(ProfileProgressionInfo.PlayerClass))]
		class_id,
		// Token: 0x04000254 RID: 596
		[LogKey(typeof(uint))]
		claymore_kill_count,
		// Token: 0x04000255 RID: 597
		[LogKey(typeof(TutorialEvent))]
		completion_status,
		// Token: 0x04000256 RID: 598
		[LogKey(typeof(TimeSpan))]
		contract_time,
		// Token: 0x04000257 RID: 599
		[LogKey(typeof(bool))]
		clb_expired,
		// Token: 0x04000258 RID: 600
		[LogKey(typeof(bool))]
		clbs_outdated,
		// Token: 0x04000259 RID: 601
		[LogKey(typeof(string))]
		clb_reward,
		// Token: 0x0400025A RID: 602
		[LogKey(typeof(int))]
		clb_reward_id,
		// Token: 0x0400025B RID: 603
		[LogKey(typeof(int))]
		clb_streak_id,
		// Token: 0x0400025C RID: 604
		[LogKey(typeof(string))]
		client_version,
		// Token: 0x0400025D RID: 605
		[LogKey(typeof(uint))]
		cpu_vendor,
		// Token: 0x0400025E RID: 606
		[LogKey(typeof(uint))]
		cpu_family,
		// Token: 0x0400025F RID: 607
		[LogKey(typeof(uint))]
		cpu_model,
		// Token: 0x04000260 RID: 608
		[LogKey(typeof(uint))]
		cpu_num_cores,
		// Token: 0x04000261 RID: 609
		[LogKey(typeof(uint))]
		cpu_speed,
		// Token: 0x04000262 RID: 610
		[LogKey(typeof(uint))]
		cpu_stepping,
		// Token: 0x04000263 RID: 611
		[LogKey(typeof(ulong))]
		crown_money,
		// Token: 0x04000264 RID: 612
		[LogKey(typeof(long))]
		crown_money_diff,
		// Token: 0x04000265 RID: 613
		[LogKey(typeof(ulong))]
		crown_money_original,
		// Token: 0x04000266 RID: 614
		[LogKey(typeof(ulong))]
		cry_money,
		// Token: 0x04000267 RID: 615
		[LogKey(typeof(long))]
		cry_money_diff,
		// Token: 0x04000268 RID: 616
		[LogKey(typeof(ulong))]
		cry_money_original,
		// Token: 0x04000269 RID: 617
		[LogKey(typeof(uint))]
		curve_coef,
		// Token: 0x0400026A RID: 618
		[LogKey(typeof(uint))]
		death_count,
		// Token: 0x0400026B RID: 619
		[LogKey(typeof(string))]
		description,
		// Token: 0x0400026C RID: 620
		[LogKey(typeof(ELogoutType))]
		disconnect_reason,
		// Token: 0x0400026D RID: 621
		[LogKey(typeof(uint))]
		discount,
		// Token: 0x0400026E RID: 622
		[LogKey(typeof(string))]
		dynamic_experience_multiplier,
		// Token: 0x0400026F RID: 623
		[LogKey(typeof(string))]
		dynamic_money_multiplier,
		// Token: 0x04000270 RID: 624
		[LogKey(typeof(string))]
		dynamic_sponsor_points_multiplier,
		// Token: 0x04000271 RID: 625
		[LogKey(typeof(string))]
		dynamic_crown_multiplier,
		// Token: 0x04000272 RID: 626
		[LogKey(typeof(string))]
		dynamic_multiplier_providers,
		// Token: 0x04000273 RID: 627
		[LogKey(typeof(string))]
		exec_command,
		// Token: 0x04000274 RID: 628
		[LogKey(typeof(string))]
		event_name,
		// Token: 0x04000275 RID: 629
		[LogKey(typeof(uint))]
		frag_kill_count,
		// Token: 0x04000276 RID: 630
		[LogKey(typeof(uint))]
		first_checkpoint,
		// Token: 0x04000277 RID: 631
		[LogKey(typeof(bool))]
		first_win,
		// Token: 0x04000278 RID: 632
		[LogKey(typeof(EInviteStatus))]
		friend_invite_status,
		// Token: 0x04000279 RID: 633
		[LogKey(typeof(TimeSpan))]
		game_lead_time,
		// Token: 0x0400027A RID: 634
		[LogKey(typeof(ulong))]
		game_money,
		// Token: 0x0400027B RID: 635
		[LogKey(typeof(ulong))]
		game_money_before_autorepair,
		// Token: 0x0400027C RID: 636
		[LogKey(typeof(ulong))]
		game_money_after_autorepair,
		// Token: 0x0400027D RID: 637
		[LogKey(typeof(long))]
		game_money_diff,
		// Token: 0x0400027E RID: 638
		[LogKey(typeof(ulong))]
		game_money_original,
		// Token: 0x0400027F RID: 639
		[LogKey(typeof(string))]
		game_interface_cmd,
		// Token: 0x04000280 RID: 640
		[LogKey(typeof(string))]
		give_money_transaction_id,
		// Token: 0x04000281 RID: 641
		[LogKey(typeof(uint))]
		gpu_device_id,
		// Token: 0x04000282 RID: 642
		[LogKey(typeof(uint))]
		gpu_vendor_id,
		// Token: 0x04000283 RID: 643
		[LogKey(typeof(string))]
		group_id,
		// Token: 0x04000284 RID: 644
		[LogKey(typeof(int))]
		group_size,
		// Token: 0x04000285 RID: 645
		[LogKey(typeof(string))]
		hardware_id,
		// Token: 0x04000286 RID: 646
		[LogKey(typeof(uint))]
		headshot_kill_count,
		// Token: 0x04000287 RID: 647
		[LogKey(typeof(TimeSpan))]
		ingame_playtime,
		// Token: 0x04000288 RID: 648
		[LogKey(typeof(string))]
		ip_address,
		// Token: 0x04000289 RID: 649
		[LogKey(typeof(int))]
		item_durability,
		// Token: 0x0400028A RID: 650
		[LogKey(typeof(string))]
		item_expiration_time,
		// Token: 0x0400028B RID: 651
		[LogKey(typeof(ulong))]
		item_id,
		// Token: 0x0400028C RID: 652
		[LogKey(typeof(string))]
		item_name,
		// Token: 0x0400028D RID: 653
		[LogKey(typeof(int))]
		item_number_current,
		// Token: 0x0400028E RID: 654
		[LogKey(typeof(int))]
		item_number_previous,
		// Token: 0x0400028F RID: 655
		[LogKey(typeof(int))]
		item_max_amount,
		// Token: 0x04000290 RID: 656
		[LogKey(typeof(ulong))]
		item_slot,
		// Token: 0x04000291 RID: 657
		[LogKey(typeof(int))]
		item_total_durability,
		// Token: 0x04000292 RID: 658
		[LogKey(typeof(string))]
		item_type,
		// Token: 0x04000293 RID: 659
		[LogKey(typeof(string))]
		item_repair_cost,
		// Token: 0x04000294 RID: 660
		[LogKey(typeof(ulong))]
		items_consumed,
		// Token: 0x04000295 RID: 661
		[LogKey(typeof(ulong))]
		items_left,
		// Token: 0x04000296 RID: 662
		[LogKey(typeof(ulong))]
		items_quantity,
		// Token: 0x04000297 RID: 663
		[LogKey(typeof(EInvitationStatus))]
		invite_status,
		// Token: 0x04000298 RID: 664
		[LogKey(typeof(ulong))]
		invite_initiator,
		// Token: 0x04000299 RID: 665
		[LogKey(typeof(ulong))]
		invite_target,
		// Token: 0x0400029A RID: 666
		[LogKey(typeof(ulong))]
		initiator,
		// Token: 0x0400029B RID: 667
		[LogKey(typeof(bool))]
		is_autobalanced,
		// Token: 0x0400029C RID: 668
		[LogKey(typeof(bool))]
		is_auto_repair_on,
		// Token: 0x0400029D RID: 669
		[LogKey(typeof(ulong))]
		target,
		// Token: 0x0400029E RID: 670
		[LogKey(typeof(string))]
		join_source,
		// Token: 0x0400029F RID: 671
		[LogKey(typeof(ulong))]
		key_items_amount_before_reset,
		// Token: 0x040002A0 RID: 672
		[LogKey(typeof(string))]
		key_item_name,
		// Token: 0x040002A1 RID: 673
		[LogKey(typeof(uint))]
		kill_count,
		// Token: 0x040002A2 RID: 674
		[LogKey(typeof(uint))]
		last_checkpoint,
		// Token: 0x040002A3 RID: 675
		[LogKey(typeof(LevelChangeReason))]
		levelup_reason,
		// Token: 0x040002A4 RID: 676
		[LogKey(typeof(TimeSpan))]
		levelup_time,
		// Token: 0x040002A5 RID: 677
		log_category,
		// Token: 0x040002A6 RID: 678
		log_datetime,
		// Token: 0x040002A7 RID: 679
		[LogKey(typeof(DateTime))]
		autorepair_datetime,
		// Token: 0x040002A8 RID: 680
		[LogKey(typeof(string))]
		maps_in_voting,
		// Token: 0x040002A9 RID: 681
		[LogKey(typeof(TimeSpan))]
		match_time,
		// Token: 0x040002AA RID: 682
		[LogKey(typeof(uint))]
		melee_kill_count,
		// Token: 0x040002AB RID: 683
		[LogKey(typeof(string))]
		message,
		// Token: 0x040002AC RID: 684
		[LogKey(typeof(string))]
		message_type,
		// Token: 0x040002AD RID: 685
		[LogKey(typeof(string))]
		mission_difficulty,
		// Token: 0x040002AE RID: 686
		[LogKey(typeof(string))]
		mission_game_mode,
		// Token: 0x040002AF RID: 687
		[LogKey(typeof(string))]
		mission_type,
		// Token: 0x040002B0 RID: 688
		[LogKey(typeof(string))]
		mission_level_graph,
		// Token: 0x040002B1 RID: 689
		[LogKey(typeof(string))]
		mission_name,
		// Token: 0x040002B2 RID: 690
		[LogKey(typeof(string))]
		mission_setting,
		// Token: 0x040002B3 RID: 691
		[LogKey(typeof(int))]
		mission_sublevels,
		// Token: 0x040002B4 RID: 692
		[LogKey(typeof(string))]
		mission_sublevel_name,
		// Token: 0x040002B5 RID: 693
		[LogKey(typeof(string))]
		mission_sublevel_flow,
		// Token: 0x040002B6 RID: 694
		[LogKey(typeof(string))]
		mission_type_experience_multiplier,
		// Token: 0x040002B7 RID: 695
		[LogKey(typeof(string))]
		mission_type_money_multiplier,
		// Token: 0x040002B8 RID: 696
		[LogKey(typeof(string))]
		mission_type_sponsor_points_multiplier,
		// Token: 0x040002B9 RID: 697
		[LogKey(typeof(uint))]
		new_rating,
		// Token: 0x040002BA RID: 698
		[LogKey(typeof(uint))]
		new_rating_points,
		// Token: 0x040002BB RID: 699
		[LogKey(typeof(uint))]
		new_rating_win_streak,
		// Token: 0x040002BC RID: 700
		[LogKey(typeof(string))]
		nickname,
		// Token: 0x040002BD RID: 701
		[LogKey(typeof(bool))]
		observer,
		// Token: 0x040002BE RID: 702
		[LogKey(typeof(int))]
		old_rank_id,
		// Token: 0x040002BF RID: 703
		[LogKey(typeof(ulong))]
		old_rank_points,
		// Token: 0x040002C0 RID: 704
		[LogKey(typeof(uint))]
		old_rating,
		// Token: 0x040002C1 RID: 705
		[LogKey(typeof(uint))]
		old_rating_points,
		// Token: 0x040002C2 RID: 706
		[LogKey(typeof(OfferType))]
		offer_type,
		// Token: 0x040002C3 RID: 707
		[LogKey(typeof(string))]
		offer_status,
		// Token: 0x040002C4 RID: 708
		[LogKey(typeof(ulong))]
		order_id,
		// Token: 0x040002C5 RID: 709
		[LogKey(typeof(uint))]
		os_64,
		// Token: 0x040002C6 RID: 710
		[LogKey(typeof(uint))]
		os_ver,
		// Token: 0x040002C7 RID: 711
		[LogKey(typeof(PaymentResult))]
		payment_result,
		// Token: 0x040002C8 RID: 712
		[LogKey(typeof(uint))]
		physical_memory,
		// Token: 0x040002C9 RID: 713
		[LogKey(typeof(TimeSpan))]
		player_time_in_room,
		// Token: 0x040002CA RID: 714
		[LogKey(typeof(bool))]
		played_first_time,
		// Token: 0x040002CB RID: 715
		[LogKey(typeof(TimeSpan))]
		playtime,
		// Token: 0x040002CC RID: 716
		[LogKey(typeof(TimeSpan))]
		playtime_since_last_step,
		// Token: 0x040002CD RID: 717
		[LogKey(typeof(LogGroup.ProduceType))]
		produce_type,
		// Token: 0x040002CE RID: 718
		[LogKey(typeof(DateTime))]
		profile_ban_time,
		// Token: 0x040002CF RID: 719
		[LogKey(typeof(DateTime))]
		profile_create_time,
		// Token: 0x040002D0 RID: 720
		[LogKey(typeof(ulong))]
		profile_id,
		// Token: 0x040002D1 RID: 721
		[LogKey(typeof(ulong))]
		profile_item_id,
		// Token: 0x040002D2 RID: 722
		[LogKey(typeof(string))]
		profile_jid,
		// Token: 0x040002D3 RID: 723
		[LogKey(typeof(string))]
		profile_jid_to,
		// Token: 0x040002D4 RID: 724
		[LogKey(typeof(DateTime))]
		profile_login_time,
		// Token: 0x040002D5 RID: 725
		[LogKey(typeof(DateTime))]
		profile_mute_time,
		// Token: 0x040002D6 RID: 726
		[LogKey(typeof(string))]
		profile_tags,
		// Token: 0x040002D7 RID: 727
		[LogKey(typeof(CrownRewardThreshold.PerformanceCategory))]
		performanceCategory,
		// Token: 0x040002D8 RID: 728
		[LogKey(typeof(int))]
		rank_id,
		// Token: 0x040002D9 RID: 729
		[LogKey(typeof(ulong))]
		rank_points,
		// Token: 0x040002DA RID: 730
		[LogKey(typeof(uint))]
		rating,
		// Token: 0x040002DB RID: 731
		[LogKey(typeof(string))]
		rating_reward_set,
		// Token: 0x040002DC RID: 732
		[LogKey(typeof(string))]
		rating_season_id,
		// Token: 0x040002DD RID: 733
		[LogKey(typeof(string))]
		reason,
		// Token: 0x040002DE RID: 734
		[LogKey(typeof(ReconnectResult))]
		reconnect_result,
		// Token: 0x040002DF RID: 735
		[LogKey(typeof(GameRoomPlayerRemoveReason))]
		remove_reason,
		// Token: 0x040002E0 RID: 736
		[LogKey(typeof(ulong))]
		repair_cost,
		// Token: 0x040002E1 RID: 737
		[LogKey(typeof(RepairStatus))]
		repair_status,
		// Token: 0x040002E2 RID: 738
		[LogKey(typeof(string))]
		reward_set,
		// Token: 0x040002E3 RID: 739
		[LogKey(typeof(DateTime))]
		reward_time,
		// Token: 0x040002E4 RID: 740
		[LogKey(typeof(int))]
		refund_status,
		// Token: 0x040002E5 RID: 741
		[LogKey(typeof(string))]
		region_id,
		// Token: 0x040002E6 RID: 742
		[LogKey(typeof(ulong))]
		room_id,
		// Token: 0x040002E7 RID: 743
		[LogKey(typeof(TimeSpan))]
		room_life_time,
		// Token: 0x040002E8 RID: 744
		[LogKey(typeof(string))]
		room_name,
		// Token: 0x040002E9 RID: 745
		[LogKey(typeof(GameRoomType))]
		room_type,
		// Token: 0x040002EA RID: 746
		[LogKey(typeof(RoomPlayer.EStatus))]
		room_player_status,
		// Token: 0x040002EB RID: 747
		[LogKey(typeof(ulong))]
		rule_id,
		// Token: 0x040002EC RID: 748
		[LogKey(typeof(bool))]
		rule_enabled,
		// Token: 0x040002ED RID: 749
		[LogKey(typeof(string))]
		rule_config,
		// Token: 0x040002EE RID: 750
		[LogKey(typeof(uint))]
		session_coin_spent,
		// Token: 0x040002EF RID: 751
		[LogKey(typeof(string))]
		session_id,
		// Token: 0x040002F0 RID: 752
		[LogKey(typeof(string))]
		session_player_class,
		// Token: 0x040002F1 RID: 753
		[LogKey(typeof(uint))]
		session_player_deaths,
		// Token: 0x040002F2 RID: 754
		[LogKey(typeof(TimeSpan))]
		session_player_time,
		// Token: 0x040002F3 RID: 755
		[LogKey(typeof(int))]
		session_players,
		// Token: 0x040002F4 RID: 756
		[LogKey(typeof(DateTime))]
		session_start_time,
		// Token: 0x040002F5 RID: 757
		[LogKey(typeof(int))]
		session_status,
		// Token: 0x040002F6 RID: 758
		[LogKey(typeof(TimeSpan))]
		session_time,
		// Token: 0x040002F7 RID: 759
		[LogKey(typeof(ulong))]
		shop_offer_id,
		// Token: 0x040002F8 RID: 760
		[LogKey(typeof(TransactionStatus))]
		shop_tr_status,
		// Token: 0x040002F9 RID: 761
		[LogKey(typeof(uint))]
		skill,
		// Token: 0x040002FA RID: 762
		[LogKey(typeof(uint))]
		skill_points,
		// Token: 0x040002FB RID: 763
		[LogKey(typeof(SkillType))]
		skill_type,
		// Token: 0x040002FC RID: 764
		[LogKey(typeof(uint))]
		slide_kill_count,
		// Token: 0x040002FD RID: 765
		[LogKey(typeof(uint))]
		sponsor_id,
		// Token: 0x040002FE RID: 766
		[LogKey(typeof(ulong))]
		sponsor_points_current,
		// Token: 0x040002FF RID: 767
		[LogKey(typeof(int))]
		stat_id_performance,
		// Token: 0x04000300 RID: 768
		[LogKey(typeof(int))]
		stat_id_headshots_points,
		// Token: 0x04000301 RID: 769
		[LogKey(typeof(int))]
		stat_id_melee_points,
		// Token: 0x04000302 RID: 770
		[LogKey(typeof(int))]
		stat_id_explosions_points,
		// Token: 0x04000303 RID: 771
		[LogKey(typeof(int))]
		stat_id_average_multiplier,
		// Token: 0x04000304 RID: 772
		[LogKey(typeof(int))]
		stat_id_time,
		// Token: 0x04000305 RID: 773
		[LogKey(typeof(string))]
		sub_mode,
		// Token: 0x04000306 RID: 774
		[LogKey(typeof(int))]
		supplier_id,
		// Token: 0x04000307 RID: 775
		[LogKey(typeof(string))]
		tags_filter,
		// Token: 0x04000308 RID: 776
		[LogKey(typeof(int))]
		team_1_kills,
		// Token: 0x04000309 RID: 777
		[LogKey(typeof(int))]
		team_2_kills,
		// Token: 0x0400030A RID: 778
		[LogKey(typeof(int))]
		team_1_score,
		// Token: 0x0400030B RID: 779
		[LogKey(typeof(int))]
		team_2_score,
		// Token: 0x0400030C RID: 780
		[LogKey(typeof(string))]
		team_1_skill,
		// Token: 0x0400030D RID: 781
		[LogKey(typeof(string))]
		team_2_skill,
		// Token: 0x0400030E RID: 782
		[LogKey(typeof(int))]
		team_1_size,
		// Token: 0x0400030F RID: 783
		[LogKey(typeof(int))]
		team_2_size,
		// Token: 0x04000310 RID: 784
		[LogKey(typeof(TimeSpan))]
		total_online_time,
		// Token: 0x04000311 RID: 785
		[LogKey(typeof(ProfileProgressionInfo.Tutorial))]
		tutorial_id,
		// Token: 0x04000312 RID: 786
		[LogKey(typeof(int))]
		tutorial_mission,
		// Token: 0x04000313 RID: 787
		[LogKey(typeof(string))]
		tutorial_step,
		// Token: 0x04000314 RID: 788
		[LogKey(typeof(ulong))]
		user_id,
		// Token: 0x04000315 RID: 789
		[LogKey(typeof(string))]
		voucher_data,
		// Token: 0x04000316 RID: 790
		[LogKey(typeof(ulong))]
		voucher_id,
		// Token: 0x04000317 RID: 791
		[LogKey(typeof(string))]
		voucher_type,
		// Token: 0x04000318 RID: 792
		[LogKey(typeof(VoucherStatus))]
		voucher_status,
		// Token: 0x04000319 RID: 793
		[LogKey(typeof(int))]
		vote_result,
		// Token: 0x0400031A RID: 794
		[LogKey(typeof(int))]
		votes_for,
		// Token: 0x0400031B RID: 795
		[LogKey(typeof(int))]
		votes_against,
		// Token: 0x0400031C RID: 796
		zzz_unit_test_1,
		// Token: 0x0400031D RID: 797
		[LogKey(typeof(string))]
		zzz_unit_test_2,
		// Token: 0x0400031E RID: 798
		[LogKey(typeof(object))]
		zzz_unit_test_3,
		// Token: 0x0400031F RID: 799
		[LogKey(typeof(DateTime))]
		zzz_unit_test_datetime,
		// Token: 0x04000320 RID: 800
		[LogKey(typeof(TimeSpan))]
		zzz_unit_test_timespan,
		// Token: 0x04000321 RID: 801
		[LogKey(typeof(AuthenticationSchemes))]
		zzz_unit_test_enum,
		// Token: 0x04000322 RID: 802
		[LogKey(typeof(bool))]
		zzz_unit_test_bool,
		// Token: 0x04000323 RID: 803
		[LogKey(typeof(byte))]
		zzz_unit_test_byte,
		// Token: 0x04000324 RID: 804
		[LogKey(typeof(sbyte))]
		zzz_unit_test_sbyte,
		// Token: 0x04000325 RID: 805
		[LogKey(typeof(short))]
		zzz_unit_test_short,
		// Token: 0x04000326 RID: 806
		[LogKey(typeof(ushort))]
		zzz_unit_test_ushort,
		// Token: 0x04000327 RID: 807
		[LogKey(typeof(int))]
		zzz_unit_test_int,
		// Token: 0x04000328 RID: 808
		[LogKey(typeof(uint))]
		zzz_unit_test_uint,
		// Token: 0x04000329 RID: 809
		[LogKey(typeof(long))]
		zzz_unit_test_long,
		// Token: 0x0400032A RID: 810
		[LogKey(typeof(ulong))]
		zzz_unit_test_ulong,
		// Token: 0x0400032B RID: 811
		[LogKey(typeof(string))]
		zzz_unit_test_string,
		// Token: 0x0400032C RID: 812
		[LogKey(typeof(float))]
		zzz_unit_test_float,
		// Token: 0x0400032D RID: 813
		[LogKey(typeof(double))]
		zzz_unit_test_double,
		// Token: 0x0400032E RID: 814
		[LogKey(typeof(decimal))]
		zzz_unit_test_decimal
	}
}
