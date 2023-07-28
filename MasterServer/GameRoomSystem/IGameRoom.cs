using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Common;
using MasterServer.DAL;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.GameRoom.Commands.Debug;
using MasterServer.GameRoom.RoomExtensions.Reconnect;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000497 RID: 1175
	internal interface IGameRoom : IDisposable
	{
		// Token: 0x17000259 RID: 601
		// (get) Token: 0x0600188B RID: 6283
		ulong ID { get; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x0600188C RID: 6284
		// (set) Token: 0x0600188D RID: 6285
		string RoomName { get; set; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x0600188E RID: 6286
		// (set) Token: 0x0600188F RID: 6287
		ulong MMGeneration { get; set; }

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06001890 RID: 6288
		string SessionID { get; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06001891 RID: 6289
		MissionType MissionType { get; }

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06001892 RID: 6290
		string MissionName { get; }

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06001893 RID: 6291
		string MissionKey { get; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06001894 RID: 6292
		string MissionDifficulty { get; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06001895 RID: 6293
		DateTime CreationTime { get; }

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06001896 RID: 6294
		RoomReference Reference { get; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06001897 RID: 6295
		int MaxPlayers { get; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06001898 RID: 6296
		// (set) Token: 0x06001899 RID: 6297
		int MinReadyPlayers { get; set; }

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x0600189A RID: 6298
		int MaxTeamSize { get; }

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x0600189B RID: 6299
		// (set) Token: 0x0600189C RID: 6300
		int TeamsReadyPlayersDiff { get; set; }

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600189D RID: 6301
		int PlayerCount { get; }

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x0600189E RID: 6302
		int PlayerCountWithReserved { get; }

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x0600189F RID: 6303
		bool IsEmpty { get; }

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x060018A0 RID: 6304
		int AllowedInventorySlots { get; }

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060018A1 RID: 6305
		// (set) Token: 0x060018A2 RID: 6306
		bool TeamBalance { get; set; }

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x060018A3 RID: 6307
		// (set) Token: 0x060018A4 RID: 6308
		bool Locked { get; set; }

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x060018A5 RID: 6309
		bool Autobalance { get; }

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x060018A6 RID: 6310
		bool NoTeamsMode { get; }

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x060018A7 RID: 6311
		// (set) Token: 0x060018A8 RID: 6312
		bool Private { get; set; }

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x060018A9 RID: 6313
		bool CanStart { get; }

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x060018AA RID: 6314
		// (set) Token: 0x060018AB RID: 6315
		bool AllowManualJoin { get; set; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x060018AC RID: 6316
		bool GameRunning { get; }

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x060018AD RID: 6317
		string RegionId { get; }

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060018AE RID: 6318
		GameRoomType Type { get; }

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060018AF RID: 6319
		IEnumerable<RoomPlayer> Players { get; }

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x060018B0 RID: 6320
		IEnumerable<RoomPlayer> ReservedPlayers { get; }

		// Token: 0x060018B1 RID: 6321
		bool IsPveMode();

		// Token: 0x060018B2 RID: 6322
		bool IsPvpMode();

		// Token: 0x060018B3 RID: 6323
		bool IsClanWarMode();

		// Token: 0x060018B4 RID: 6324
		bool IsAutoStartMode();

		// Token: 0x060018B5 RID: 6325
		bool IsPublicPvPMode();

		// Token: 0x060018B6 RID: 6326
		bool IsPveAutoStartMode();

		// Token: 0x060018B7 RID: 6327
		bool IsPvpRatingMode();

		// Token: 0x060018B8 RID: 6328
		T GetExtension<T>() where T : IRoomExtension;

		// Token: 0x060018B9 RID: 6329
		bool TryGetExtension<T>(out T ext) where T : IRoomExtension;

		// Token: 0x060018BA RID: 6330
		T GetState<T>(AccessMode acc) where T : IRoomState;

		// Token: 0x060018BB RID: 6331
		T TryGetState<T>(AccessMode acc) where T : IRoomState;

		// Token: 0x060018BC RID: 6332
		void transaction(AccessMode acc, Action<IGameRoom> action);

		// Token: 0x060018BD RID: 6333
		XmlElement transaction(string onlineId, XmlDocument factory, AccessMode acc, Action<IGameRoom> action);

		// Token: 0x060018BE RID: 6334
		void CheckAccessMode(AccessMode atLeast);

		// Token: 0x060018BF RID: 6335
		bool HasPlayer(ulong profileId);

		// Token: 0x060018C0 RID: 6336
		bool HasReservation(ulong profileId);

		// Token: 0x060018C1 RID: 6337
		RoomPlayer GetPlayer(ulong profileId);

		// Token: 0x060018C2 RID: 6338
		RoomPlayer GetPlayer(ulong profileId, AccessMode mode);

		// Token: 0x060018C3 RID: 6339
		RoomPlayer GetPlayer(string nickname, AccessMode mode);

		// Token: 0x060018C4 RID: 6340
		bool TryGetPlayer(ulong profileId, out RoomPlayer player);

		// Token: 0x060018C5 RID: 6341
		bool TryGetPlayer(ulong profileId, AccessMode mode, out RoomPlayer player);

		// Token: 0x060018C6 RID: 6342
		GameRoomRetCode AddPlayer(ulong profileId, int teamId, int classId, RoomPlayer.EStatus roomStatus, GameRoomPlayerAddReason reason);

		// Token: 0x060018C7 RID: 6343
		GameRoomRetCode AddPlayer(ulong profileId, string groupId, int teamId, int classId, TimeSpan quickplaySearchTime, RoomPlayer.EStatus roomStatus, GameRoomPlayerAddReason reason);

		// Token: 0x060018C8 RID: 6344
		bool ReservePlaceForPlayer(UserInfo.User user, string groupID);

		// Token: 0x060018C9 RID: 6345
		bool ReservePlaceForPlayers(params RoomPlayer[] player);

		// Token: 0x060018CA RID: 6346
		void RemovePlayer(ulong profileId, GameRoomPlayerRemoveReason reason);

		// Token: 0x060018CB RID: 6347
		void RemoveAllPlayers();

		// Token: 0x060018CC RID: 6348
		void RemoveAllReservations();

		// Token: 0x060018CD RID: 6349
		void RemoveReservation(ulong profileId, ReservationRemovedReason reason);

		// Token: 0x060018CE RID: 6350
		GameRoomRetCode CanJoin(UserInfo.User user);

		// Token: 0x060018CF RID: 6351
		void SwitchTeam(ulong profileId, int team);

		// Token: 0x060018D0 RID: 6352
		void SetTeamColor(int team, uint color);

		// Token: 0x060018D1 RID: 6353
		void SwapTeams();

		// Token: 0x060018D2 RID: 6354
		string GetUsersBuildType();

		// Token: 0x060018D3 RID: 6355
		void SignalPlayersChanged();

		// Token: 0x060018D4 RID: 6356
		void PlayerJoinedSession(ulong profileId);

		// Token: 0x060018D5 RID: 6357
		void CheckPlayerRank(SProfileInfo profile, SRankInfo rank);

		// Token: 0x060018D6 RID: 6358
		void Close();

		// Token: 0x060018D7 RID: 6359
		float CalculateRoomRating();

		// Token: 0x060018D8 RID: 6360
		void UpdatePlayerStatus(ulong profileId, UserStatus status);

		// Token: 0x060018D9 RID: 6361
		void SetObserver(ulong profileId, bool enable);

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x060018DA RID: 6362
		bool AllowReconnect { get; }

		// Token: 0x060018DB RID: 6363
		ReconnectResult ReconnectPlayer(ulong profileId);

		// Token: 0x060018DC RID: 6364
		XmlElement SerializeStateChanges(RoomUpdate.Target target, RoomUpdate.Kind kind, DoubleBuffer<Type, IRoomState>.Snapshot snapshot, XmlDocument factory);

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x060018DD RID: 6365
		// (remove) Token: 0x060018DE RID: 6366
		event TrPlayerAddCheckDeleg tr_player_add_check;

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x060018DF RID: 6367
		// (remove) Token: 0x060018E0 RID: 6368
		event TrOnPlayerAddedDeleg tr_player_added;

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x060018E1 RID: 6369
		// (remove) Token: 0x060018E2 RID: 6370
		event TrOnPlayerRemovedDeleg tr_player_removed;

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x060018E3 RID: 6371
		// (remove) Token: 0x060018E4 RID: 6372
		event TrOnPlayersChangedDeleg tr_players_changed;

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x060018E5 RID: 6373
		// (remove) Token: 0x060018E6 RID: 6374
		event TrOnPlayerStatusDeleg tr_player_status;

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x060018E7 RID: 6375
		// (remove) Token: 0x060018E8 RID: 6376
		event TrOnPlayerJoinedSession tr_player_joined_session;

		// Token: 0x14000053 RID: 83
		// (add) Token: 0x060018E9 RID: 6377
		// (remove) Token: 0x060018EA RID: 6378
		event Action<ReservationRemovedReason> tr_player_reservation_removed;

		// Token: 0x14000054 RID: 84
		// (add) Token: 0x060018EB RID: 6379
		// (remove) Token: 0x060018EC RID: 6380
		event OnPlayerAddedDeleg PlayerAdded;

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x060018ED RID: 6381
		// (remove) Token: 0x060018EE RID: 6382
		event OnPlayerRemovedDeleg PlayerRemoved;

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x060018EF RID: 6383
		// (remove) Token: 0x060018F0 RID: 6384
		event OnPlayerChangedDeleg PlayerChanged;

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x060018F1 RID: 6385
		// (remove) Token: 0x060018F2 RID: 6386
		event OnRoomClosed RoomClosed;

		// Token: 0x060018F3 RID: 6387
		XmlElement CreateRoomElement(XmlDocument factory);

		// Token: 0x060018F4 RID: 6388
		XmlElement FullStateSnapshot(RoomUpdate.Target target, XmlDocument factory);

		// Token: 0x060018F5 RID: 6389
		XmlElement FullStateSnapshot(RoomUpdate.Target target, XmlDocument factory, ulong profileId);

		// Token: 0x060018F6 RID: 6390
		void AddFakePlayers(DebugRoomPlayersParams p, int startId, IProfileProgressionService profileProgressionService, SkillType skillType);

		// Token: 0x060018F7 RID: 6391
		void RemoveFakePlayers(IEnumerable<ulong> playerIds);
	}
}
