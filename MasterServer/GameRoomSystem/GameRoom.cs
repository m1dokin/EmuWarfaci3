using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.MissionAccessLimitation;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.GameRoom.Commands.Debug;
using MasterServer.GameRoom.RoomExtensions;
using MasterServer.GameRoom.RoomExtensions.Reconnect;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004B9 RID: 1209
	internal class GameRoom : GameRoomBase, IGameRoom, IDisposable
	{
		// Token: 0x060019C6 RID: 6598 RVA: 0x00069D44 File Offset: 0x00068144
		public GameRoom(IGameRoomManagerRegistry gameRoomManager, IRankSystem rankSystem, IUserRepository userRepository, IClientVersionsManagementService clientVersionsManagementService, IMissionAccessLimitationService limitationService, IRoomPlayerFactory roomPlayerFactory, ISkillService skillService, RoomExtensionsData extensions, ulong id, GameRoomType type) : this(gameRoomManager, rankSystem, userRepository, clientVersionsManagementService, limitationService, roomPlayerFactory, skillService, extensions, id, RoomReference.EmptyReference, type)
		{
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x00069D70 File Offset: 0x00068170
		public GameRoom(IGameRoomManagerRegistry gameRoomManager, IRankSystem rankSystem, IUserRepository userRepository, IClientVersionsManagementService clientVersionsManagementService, IMissionAccessLimitationService limitationService, IRoomPlayerFactory roomPlayerFactory, ISkillService skillService, RoomExtensionsData extensions, ulong id, RoomReference reference, GameRoomType type) : base(type, extensions)
		{
			base.ID = id;
			this.Reference = reference;
			this.m_limitationService = limitationService;
			this.CreationTime = DateTime.UtcNow;
			this.m_gameRoomManager = gameRoomManager;
			this.m_rankSystem = rankSystem;
			this.m_userRepository = userRepository;
			this.m_clientVersionsManagementService = clientVersionsManagementService;
			this.m_roomPlayerFactory = roomPlayerFactory;
			this.m_skillService = skillService;
			this.m_rand = new Random((int)DateTime.Now.Ticks);
			foreach (IRoomExtension roomExtension in this.m_extensions.Values)
			{
				roomExtension.Init(this);
			}
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x00069E44 File Offset: 0x00068244
		public void AddFakePlayers(DebugRoomPlayersParams p, int startId, IProfileProgressionService profileProgressionService, SkillType skillType)
		{
			for (int i = 0; i < (int)p.Quantity; i++)
			{
				ulong num = (ulong)((long)startId++);
				SProfileProgression defaultProgression = p.DefaultProgression;
				defaultProgression.ProfileId = num;
				ProfileProgressionInfo progression = new ProfileProgressionInfo(defaultProgression, null, profileProgressionService);
				int desiredTeamId = (int)((p.TeamId == 0) ? ((i >= this.MaxPlayers / 2) ? 2 : 1) : p.TeamId);
				GameRoomRetCode gameRoomRetCode = this.AddFakePlayer(num, DebugCommandsJidHelper.GetFakeJid(num).ToString(), p, progression, i, desiredTeamId, skillType);
				if (gameRoomRetCode != GameRoomRetCode.OK)
				{
					Log.Warning<ulong, ulong, GameRoomRetCode>("Can't add fake player {0} to room {1} because {2}.", num, base.ID, gameRoomRetCode);
				}
			}
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x00069EEA File Offset: 0x000682EA
		public void RemoveFakePlayers(IEnumerable<ulong> playerIds)
		{
			playerIds.ForEach(delegate(ulong pid)
			{
				this.RemovePlayer(pid, GameRoomPlayerRemoveReason.Left);
			});
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x00069F00 File Offset: 0x00068300
		private GameRoomRetCode AddFakePlayer(ulong userId, string onlineId, DebugRoomPlayersParams playerParams, ProfileProgressionInfo progression, int playerIndex, int desiredTeamId, SkillType skillType)
		{
			if (!Resources.DebugQueriesEnabled)
			{
				throw new InvalidOperationException("This operation can not be performed on non-debug environment");
			}
			int num = this.m_rand.Next((int)playerParams.RankMin, (int)playerParams.RankMax);
			float num2;
			if (!playerParams.IsDefaultSkillMax)
			{
				num2 = playerParams.SkillMax;
			}
			else
			{
				num2 = (float)this.m_skillService.GetMaxChannelSkillByType(skillType);
			}
			float num3 = (float)this.m_rand.Next((int)(playerParams.SkillMin * 100f), (int)(num2 * 100f)) / 100f;
			ulong num4 = (ulong)((long)this.m_rand.Next((int)playerParams.GroupMin, (int)playerParams.GroupMax));
			byte classID = playerParams.ClassIds[playerIndex % playerParams.ClassIds.Length];
			FakeRoomPlayer fakeRoomPlayer = new FakeRoomPlayer(desiredTeamId)
			{
				ProfileID = userId,
				UserID = userId,
				OnlineID = onlineId,
				Nickname = "fake_user_" + userId,
				TeamID = 0,
				ClassID = (int)classID,
				GroupID = ((num4 != 0UL) ? num4.ToString() : Guid.NewGuid().ToString()),
				RoomStatus = RoomPlayer.EStatus.NotReady,
				UserStatus = (UserStatus.Online | UserStatus.InGameRoom),
				RegionId = playerParams.RegionId,
				Rank = num,
				Skill = new Skill(skillType, (double)num3, 0.0),
				Experience = this.m_rankSystem.GetExperience(num),
				Banner = default(SBannerInfo),
				SessionsPlayedInRoom = 0,
				JoinTime = DateTime.UtcNow,
				GameWaitStartTime = DateTime.UtcNow,
				LastSessionEndTime = DateTime.MinValue,
				QuickPlaySearchTime = TimeSpan.Zero,
				ProfileProgression = progression
			};
			GameRoomRetCode gameRoomRetCode = this.CanJoin(fakeRoomPlayer);
			if (gameRoomRetCode != GameRoomRetCode.OK)
			{
				return gameRoomRetCode;
			}
			return this.AddPlayer(fakeRoomPlayer, GameRoomPlayerAddReason.RoomBrowser);
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x060019CB RID: 6603 RVA: 0x0006A0FC File Offset: 0x000684FC
		// (set) Token: 0x060019CC RID: 6604 RVA: 0x0006A104 File Offset: 0x00068504
		public RoomReference Reference { get; private set; }

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x060019CD RID: 6605 RVA: 0x0006A10D File Offset: 0x0006850D
		// (set) Token: 0x060019CE RID: 6606 RVA: 0x0006A115 File Offset: 0x00068515
		public DateTime CreationTime { get; private set; }

		// Token: 0x060019CF RID: 6607 RVA: 0x0006A11E File Offset: 0x0006851E
		public override void Close()
		{
			base.Close();
			if (this.RoomClosed != null)
			{
				this.RoomClosed(this);
			}
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x0006A140 File Offset: 0x00068540
		public override void Dispose()
		{
			base.Dispose();
			this.tr_player_add_check = null;
			this.tr_player_added = null;
			this.tr_player_removed = null;
			this.tr_players_changed = null;
			this.tr_player_status = null;
			this.tr_player_joined_session = null;
			this.PlayerAdded = null;
			this.PlayerRemoved = null;
			this.PlayerChanged = null;
			this.RoomClosed = null;
		}

		// Token: 0x14000058 RID: 88
		// (add) Token: 0x060019D1 RID: 6609 RVA: 0x0006A19C File Offset: 0x0006859C
		// (remove) Token: 0x060019D2 RID: 6610 RVA: 0x0006A1D4 File Offset: 0x000685D4
		public event TrPlayerAddCheckDeleg tr_player_add_check;

		// Token: 0x14000059 RID: 89
		// (add) Token: 0x060019D3 RID: 6611 RVA: 0x0006A20C File Offset: 0x0006860C
		// (remove) Token: 0x060019D4 RID: 6612 RVA: 0x0006A244 File Offset: 0x00068644
		public event TrOnPlayerAddedDeleg tr_player_added;

		// Token: 0x1400005A RID: 90
		// (add) Token: 0x060019D5 RID: 6613 RVA: 0x0006A27C File Offset: 0x0006867C
		// (remove) Token: 0x060019D6 RID: 6614 RVA: 0x0006A2B4 File Offset: 0x000686B4
		public event TrOnPlayerRemovedDeleg tr_player_removed;

		// Token: 0x1400005B RID: 91
		// (add) Token: 0x060019D7 RID: 6615 RVA: 0x0006A2EC File Offset: 0x000686EC
		// (remove) Token: 0x060019D8 RID: 6616 RVA: 0x0006A324 File Offset: 0x00068724
		public event TrOnPlayersChangedDeleg tr_players_changed;

		// Token: 0x1400005C RID: 92
		// (add) Token: 0x060019D9 RID: 6617 RVA: 0x0006A35C File Offset: 0x0006875C
		// (remove) Token: 0x060019DA RID: 6618 RVA: 0x0006A394 File Offset: 0x00068794
		public event TrOnPlayerStatusDeleg tr_player_status;

		// Token: 0x1400005D RID: 93
		// (add) Token: 0x060019DB RID: 6619 RVA: 0x0006A3CC File Offset: 0x000687CC
		// (remove) Token: 0x060019DC RID: 6620 RVA: 0x0006A404 File Offset: 0x00068804
		public event TrOnPlayerJoinedSession tr_player_joined_session;

		// Token: 0x1400005E RID: 94
		// (add) Token: 0x060019DD RID: 6621 RVA: 0x0006A43C File Offset: 0x0006883C
		// (remove) Token: 0x060019DE RID: 6622 RVA: 0x0006A474 File Offset: 0x00068874
		public event Action<ReservationRemovedReason> tr_player_reservation_removed;

		// Token: 0x1400005F RID: 95
		// (add) Token: 0x060019DF RID: 6623 RVA: 0x0006A4AC File Offset: 0x000688AC
		// (remove) Token: 0x060019E0 RID: 6624 RVA: 0x0006A4E4 File Offset: 0x000688E4
		public event OnPlayerAddedDeleg PlayerAdded;

		// Token: 0x14000060 RID: 96
		// (add) Token: 0x060019E1 RID: 6625 RVA: 0x0006A51C File Offset: 0x0006891C
		// (remove) Token: 0x060019E2 RID: 6626 RVA: 0x0006A554 File Offset: 0x00068954
		public event OnPlayerRemovedDeleg PlayerRemoved;

		// Token: 0x14000061 RID: 97
		// (add) Token: 0x060019E3 RID: 6627 RVA: 0x0006A58C File Offset: 0x0006898C
		// (remove) Token: 0x060019E4 RID: 6628 RVA: 0x0006A5C4 File Offset: 0x000689C4
		public event OnPlayerChangedDeleg PlayerChanged;

		// Token: 0x14000062 RID: 98
		// (add) Token: 0x060019E5 RID: 6629 RVA: 0x0006A5FC File Offset: 0x000689FC
		// (remove) Token: 0x060019E6 RID: 6630 RVA: 0x0006A634 File Offset: 0x00068A34
		public event OnRoomClosed RoomClosed;

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x060019E7 RID: 6631 RVA: 0x0006A66A File Offset: 0x00068A6A
		// (set) Token: 0x060019E8 RID: 6632 RVA: 0x0006A678 File Offset: 0x00068A78
		public string RoomName
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).RoomName;
			}
			set
			{
				base.GetState<CoreState>(AccessMode.ReadWrite).RoomName = value;
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x060019E9 RID: 6633 RVA: 0x0006A687 File Offset: 0x00068A87
		// (set) Token: 0x060019EA RID: 6634 RVA: 0x0006A695 File Offset: 0x00068A95
		public ulong MMGeneration
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).MMGeneration;
			}
			set
			{
				base.GetState<CoreState>(AccessMode.ReadWrite).MMGeneration = value;
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x060019EB RID: 6635 RVA: 0x0006A6A4 File Offset: 0x00068AA4
		// (set) Token: 0x060019EC RID: 6636 RVA: 0x0006A6B2 File Offset: 0x00068AB2
		public bool Private
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).Private;
			}
			set
			{
				base.GetState<CoreState>(AccessMode.ReadWrite).Private = value;
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x060019ED RID: 6637 RVA: 0x0006A6C1 File Offset: 0x00068AC1
		// (set) Token: 0x060019EE RID: 6638 RVA: 0x0006A6CF File Offset: 0x00068ACF
		public bool Locked
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).Locked;
			}
			set
			{
				base.GetState<CoreState>(AccessMode.ReadWrite).Locked = value;
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x060019EF RID: 6639 RVA: 0x0006A6DE File Offset: 0x00068ADE
		// (set) Token: 0x060019F0 RID: 6640 RVA: 0x0006A6EC File Offset: 0x00068AEC
		public int MinReadyPlayers
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).MinReadyPlayers;
			}
			set
			{
				base.GetState<CoreState>(AccessMode.ReadWrite).MinReadyPlayers = value;
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x060019F1 RID: 6641 RVA: 0x0006A6FB File Offset: 0x00068AFB
		// (set) Token: 0x060019F2 RID: 6642 RVA: 0x0006A709 File Offset: 0x00068B09
		public int TeamsReadyPlayersDiff
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).TeamsReadyPlayersDiff;
			}
			set
			{
				base.GetState<CoreState>(AccessMode.ReadWrite).TeamsReadyPlayersDiff = value;
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x060019F3 RID: 6643 RVA: 0x0006A718 File Offset: 0x00068B18
		// (set) Token: 0x060019F4 RID: 6644 RVA: 0x0006A726 File Offset: 0x00068B26
		public bool TeamBalance
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).TeamBalance;
			}
			set
			{
				base.GetState<CoreState>(AccessMode.ReadWrite).TeamBalance = value;
			}
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x060019F5 RID: 6645 RVA: 0x0006A735 File Offset: 0x00068B35
		public int PlayerCount
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).Players.Count;
			}
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x060019F6 RID: 6646 RVA: 0x0006A748 File Offset: 0x00068B48
		public int PlayerCountWithReserved
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).ReservedPlayers.Count + this.PlayerCount;
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x060019F7 RID: 6647 RVA: 0x0006A762 File Offset: 0x00068B62
		public bool IsEmpty
		{
			get
			{
				return this.PlayerCountWithReserved == 0;
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x060019F8 RID: 6648 RVA: 0x0006A76D File Offset: 0x00068B6D
		public IEnumerable<RoomPlayer> Players
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).Players.Values;
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x060019F9 RID: 6649 RVA: 0x0006A780 File Offset: 0x00068B80
		public IEnumerable<RoomPlayer> ReservedPlayers
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).ReservedPlayers.Values;
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x060019FA RID: 6650 RVA: 0x0006A793 File Offset: 0x00068B93
		public bool CanStart
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).CanStart;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x060019FB RID: 6651 RVA: 0x0006A7A1 File Offset: 0x00068BA1
		// (set) Token: 0x060019FC RID: 6652 RVA: 0x0006A7AF File Offset: 0x00068BAF
		public bool AllowManualJoin
		{
			get
			{
				return base.GetState<CoreState>(AccessMode.ReadOnly).AllowManualJoin;
			}
			set
			{
				base.GetState<CoreState>(AccessMode.ReadWrite).AllowManualJoin = value;
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x060019FD RID: 6653 RVA: 0x0006A7BE File Offset: 0x00068BBE
		public int AllowedInventorySlots
		{
			get
			{
				return base.GetState<CustomParams>(AccessMode.ReadOnly).InventorySlots;
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x060019FE RID: 6654 RVA: 0x0006A7CC File Offset: 0x00068BCC
		public bool Autobalance
		{
			get
			{
				return base.GetState<CustomParams>(AccessMode.ReadOnly).Autobalance;
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x060019FF RID: 6655 RVA: 0x0006A7DA File Offset: 0x00068BDA
		public string MissionKey
		{
			get
			{
				return base.GetExtension<MissionExtension>().MissionKey;
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06001A00 RID: 6656 RVA: 0x0006A7E7 File Offset: 0x00068BE7
		public int MaxPlayers
		{
			get
			{
				return base.GetExtension<MissionExtension>().MaxPlayers;
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06001A01 RID: 6657 RVA: 0x0006A7F4 File Offset: 0x00068BF4
		public int MaxTeamSize
		{
			get
			{
				CustomParams state = base.GetState<CustomParams>(AccessMode.ReadOnly);
				return (!this.NoTeamsMode) ? ((int)Math.Ceiling((double)this.MaxPlayers / 2.0)) : state.MaxPlayers;
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06001A02 RID: 6658 RVA: 0x0006A838 File Offset: 0x00068C38
		public bool NoTeamsMode
		{
			get
			{
				return base.GetExtension<MissionExtension>().NoTeamsMode;
			}
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06001A03 RID: 6659 RVA: 0x0006A845 File Offset: 0x00068C45
		public MissionType MissionType
		{
			get
			{
				return base.GetExtension<MissionExtension>().MissionType;
			}
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06001A04 RID: 6660 RVA: 0x0006A852 File Offset: 0x00068C52
		public string MissionName
		{
			get
			{
				return base.GetExtension<MissionExtension>().Mission.name;
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06001A05 RID: 6661 RVA: 0x0006A864 File Offset: 0x00068C64
		public string MissionDifficulty
		{
			get
			{
				return base.GetExtension<MissionExtension>().MissionDifficulty;
			}
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06001A06 RID: 6662 RVA: 0x0006A871 File Offset: 0x00068C71
		public bool GameRunning
		{
			get
			{
				return base.GetExtension<ServerExtension>().GameRunning;
			}
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06001A07 RID: 6663 RVA: 0x0006A87E File Offset: 0x00068C7E
		public string SessionID
		{
			get
			{
				return base.GetExtension<SessionExtension>().SessionID;
			}
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06001A08 RID: 6664 RVA: 0x0006A88B File Offset: 0x00068C8B
		public string RegionId
		{
			get
			{
				return base.GetState<RegionState>(AccessMode.ReadOnly).RegionId;
			}
		}

		// Token: 0x06001A09 RID: 6665 RVA: 0x0006A89C File Offset: 0x00068C9C
		public GameRoomRetCode CanJoin(UserInfo.User user)
		{
			if (!this.m_clientVersionsManagementService.Validate(user.Version))
			{
				Log.Warning<ulong, ulong>("Build version mismatch for player {0} while joining room {1}", user.ProfileID, base.ID);
				return GameRoomRetCode.BUILD_VERSION_MISMATCH;
			}
			RoomPlayer roomPlayer = this.m_roomPlayerFactory.GetRoomPlayer(user, base.Type);
			return this.CanJoin(roomPlayer);
		}

		// Token: 0x06001A0A RID: 6666 RVA: 0x0006A8F4 File Offset: 0x00068CF4
		private GameRoomRetCode CanJoin(RoomPlayer roomPlayer)
		{
			CoreState state = base.GetState<CoreState>(AccessMode.ReadOnly);
			if (state.Players.ContainsKey(roomPlayer.ProfileID))
			{
				return GameRoomRetCode.OK;
			}
			if (this.MaxPlayers > 0 && this.PlayerCount >= this.MaxPlayers)
			{
				Log.Warning<ulong, ulong>("Room {0} is full. Player {1} can't join.", base.ID, roomPlayer.ProfileID);
				return GameRoomRetCode.FULL;
			}
			if (this.tr_player_add_check != null)
			{
				foreach (Delegate @delegate in this.tr_player_add_check.GetInvocationList())
				{
					TrPlayerAddCheckDeleg trPlayerAddCheckDeleg = (TrPlayerAddCheckDeleg)@delegate;
					GameRoomRetCode gameRoomRetCode = trPlayerAddCheckDeleg(roomPlayer);
					if (gameRoomRetCode != GameRoomRetCode.OK)
					{
						Log.Warning<ulong, ulong, string>("Player {0} can't join to {1} . Reason: {2}", roomPlayer.ProfileID, base.ID, gameRoomRetCode.ToString());
						return gameRoomRetCode;
					}
				}
			}
			if (base.IsPveMode() && !string.IsNullOrEmpty(this.MissionType.Name) && !roomPlayer.ProfileProgression.IsMissionTypeUnlocked(this.MissionType.Name))
			{
				return GameRoomRetCode.MISSION_RESTRICTED;
			}
			if (base.IsPveAutoStartMode() && !roomPlayer.CanJoinMission(this.m_limitationService, this.MissionType.Name))
			{
				return GameRoomRetCode.ITEM_NOT_AVAILABLE;
			}
			return this.m_rankSystem.CanJoinChannel(roomPlayer.Rank) ? GameRoomRetCode.OK : GameRoomRetCode.RANK_RESTRICTED;
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x0006AA4C File Offset: 0x00068E4C
		public bool ReservePlaceForPlayer(UserInfo.User user, string groupID)
		{
			RoomPlayer roomPlayer = this.m_roomPlayerFactory.GetRoomPlayer(user, base.Type);
			roomPlayer.GroupID = groupID;
			return this.ReservePlaceForPlayers(new RoomPlayer[]
			{
				roomPlayer
			});
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x0006AA84 File Offset: 0x00068E84
		public bool ReservePlaceForPlayers(params RoomPlayer[] players)
		{
			ReservationsExtension extension = base.GetExtension<ReservationsExtension>();
			ulong[] array = (from player in players
			select player.ProfileID).ToArray<ulong>();
			bool flag = extension.TryAddReservations(array);
			if (flag)
			{
				try
				{
					this.AddReservedPlayers(players);
				}
				catch
				{
					foreach (ulong profileId in array)
					{
						this.RemoveReservation(profileId, ReservationRemovedReason.CleanUp);
					}
					throw;
				}
			}
			return flag;
		}

		// Token: 0x06001A0D RID: 6669 RVA: 0x0006AB1C File Offset: 0x00068F1C
		private void AddReservedPlayers(IEnumerable<RoomPlayer> players)
		{
			TeamExtension extension = base.GetExtension<TeamExtension>();
			foreach (RoomPlayer roomPlayer in players)
			{
				RoomPlayer player = this.GetPlayer(roomPlayer.ProfileID);
				if (player != null)
				{
					Log.Warning<ulong, ulong>("Player {0} tried to join the room {1} twice", roomPlayer.ProfileID, base.ID);
				}
				else
				{
					if (extension.ChooseTeam(roomPlayer) == 0)
					{
						throw new ApplicationException(string.Format("Team balancing error in room {0} {1}, for {2}", base.ID, base.Type, roomPlayer.OnlineID));
					}
					CoreState state = base.GetState<CoreState>(AccessMode.ReadWrite);
					state.ReservedPlayers[roomPlayer.ProfileID] = roomPlayer;
					this.m_gameRoomManager.RegisterPlayer(this, roomPlayer.ProfileID);
				}
			}
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x0006AC04 File Offset: 0x00069004
		public GameRoomRetCode AddPlayer(ulong profileId, int teamId, int classId, RoomPlayer.EStatus roomStatus, GameRoomPlayerAddReason reason)
		{
			return this.AddPlayer(profileId, string.Empty, teamId, classId, TimeSpan.Zero, roomStatus, reason);
		}

		// Token: 0x06001A0F RID: 6671 RVA: 0x0006AC20 File Offset: 0x00069020
		public GameRoomRetCode AddPlayer(ulong profileId, string groupId, int teamId, int classId, TimeSpan quickplaySearchTime, RoomPlayer.EStatus roomStatus, GameRoomPlayerAddReason reason)
		{
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			if (user == null)
			{
				throw new RoomGenericException(base.ID, string.Format("User {0} is not present on the server", profileId));
			}
			return this.AddPlayer(user, groupId, teamId, classId, quickplaySearchTime, roomStatus, reason);
		}

		// Token: 0x06001A10 RID: 6672 RVA: 0x0006AC70 File Offset: 0x00069070
		private GameRoomRetCode AddPlayer(UserInfo.User user, string groupId, int teamId, int classId, TimeSpan quickplaySearchTime, RoomPlayer.EStatus roomStatus, GameRoomPlayerAddReason reason)
		{
			RoomPlayer roomPlayer = this.m_roomPlayerFactory.GetRoomPlayer(user, base.Type);
			roomPlayer.TeamID = teamId;
			roomPlayer.GroupID = groupId;
			roomPlayer.ClassID = classId;
			roomPlayer.RoomStatus = roomStatus;
			roomPlayer.GameWaitStartTime = DateTime.UtcNow;
			roomPlayer.QuickPlaySearchTime = quickplaySearchTime;
			if (base.IsPveMode() && !string.IsNullOrEmpty(this.MissionType.Name) && !user.ProfileProgression.IsMissionTypeUnlocked(this.MissionType.Name))
			{
				roomPlayer.RoomStatus = RoomPlayer.EStatus.CantBeReady;
			}
			return this.AddPlayer(user, roomPlayer, reason);
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x0006AD10 File Offset: 0x00069110
		private GameRoomRetCode AddPlayer(UserInfo.User user, RoomPlayer player, GameRoomPlayerAddReason reason)
		{
			GameRoomRetCode gameRoomRetCode = this.CanJoin(user);
			if (gameRoomRetCode != GameRoomRetCode.OK)
			{
				this.RemoveReservation(user.ProfileID, ReservationRemovedReason.PlayerAdded);
			}
			return (gameRoomRetCode == GameRoomRetCode.OK) ? this.AddPlayer(player, reason) : gameRoomRetCode;
		}

		// Token: 0x06001A12 RID: 6674 RVA: 0x0006AD4C File Offset: 0x0006914C
		private GameRoomRetCode AddPlayer(RoomPlayer player, GameRoomPlayerAddReason reason)
		{
			RoomPlayer player2 = this.GetPlayer(player.ProfileID);
			if (player2 != null)
			{
				Log.Warning<ulong, ulong>("Player {0} tried to join the room {1} twice", player.ProfileID, base.ID);
				return GameRoomRetCode.OK;
			}
			CoreState state = base.GetState<CoreState>(AccessMode.ReadWrite);
			state.Players.Add(player.ProfileID, player);
			if (this.tr_player_added != null)
			{
				this.tr_player_added(player.ProfileID, reason);
			}
			this.RemoveReservation(player.ProfileID, ReservationRemovedReason.PlayerAdded);
			this.m_gameRoomManager.RegisterPlayer(this, player.ProfileID);
			Log.Verbose(Log.Group.GameRoom, "Player {0} added to room {1}", new object[]
			{
				player.ProfileID,
				base.ID
			});
			return GameRoomRetCode.OK;
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x0006AE08 File Offset: 0x00069208
		public void RemoveReservation(ulong profileId, ReservationRemovedReason reason)
		{
			CoreState state = base.GetState<CoreState>(AccessMode.ReadWrite);
			if (state.ReservedPlayers.ContainsKey(profileId))
			{
				ReservationsExtension extension = base.GetExtension<ReservationsExtension>();
				extension.RemoveReservation(profileId);
				state.ReservedPlayers.Remove(profileId);
				this.m_gameRoomManager.UnregisterPlayer(this, profileId);
				this.tr_player_reservation_removed.SafeInvokeEach(reason);
			}
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x0006AE64 File Offset: 0x00069264
		public void RemovePlayer(ulong profileId, GameRoomPlayerRemoveReason reason)
		{
			Log.Verbose(Log.Group.GameRoom, "Removing player {0} from room {1}: {2}", new object[]
			{
				profileId,
				base.ID,
				reason
			});
			CoreState state = base.GetState<CoreState>(AccessMode.ReadWrite);
			this.RemoveReservation(profileId, ReservationRemovedReason.CleanUp);
			RoomPlayer roomPlayer;
			if (!state.Players.TryGetValue(profileId, out roomPlayer))
			{
				return;
			}
			state.Players.Remove(profileId);
			this.m_gameRoomManager.UnregisterPlayer(this, roomPlayer.ProfileID);
			if (this.tr_player_removed != null)
			{
				this.tr_player_removed(roomPlayer, reason);
			}
			Log.Verbose(Log.Group.GameRoom, "Player {0} removed from room {1}: {2}", new object[]
			{
				roomPlayer.ProfileID,
				base.ID,
				reason
			});
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x0006AF34 File Offset: 0x00069334
		public void RemoveAllPlayers()
		{
			List<RoomPlayer> list = this.Players.ToList<RoomPlayer>();
			foreach (RoomPlayer roomPlayer in list)
			{
				this.RemovePlayer(roomPlayer.ProfileID, GameRoomPlayerRemoveReason.Left);
			}
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x0006AFA0 File Offset: 0x000693A0
		public void RemoveAllReservations()
		{
			List<RoomPlayer> list = this.ReservedPlayers.ToList<RoomPlayer>();
			foreach (RoomPlayer roomPlayer in list)
			{
				this.RemoveReservation(roomPlayer.ProfileID, ReservationRemovedReason.CleanUp);
			}
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x0006B00C File Offset: 0x0006940C
		public void UpdatePlayerStatus(ulong profile_id, UserStatus status)
		{
			Log.Verbose(Log.Group.GameRoom, "Updating player's status {0} for room {1}: {2}", new object[]
			{
				profile_id,
				base.ID,
				status
			});
			CoreState state = base.GetState<CoreState>(AccessMode.ReadWrite);
			RoomPlayer roomPlayer;
			if (state.Players.TryGetValue(profile_id, out roomPlayer))
			{
				UserStatus userStatus = roomPlayer.UserStatus;
				roomPlayer.UserStatus = status;
				if (userStatus != status && this.tr_player_status != null)
				{
					this.tr_player_status(roomPlayer.ProfileID, userStatus, status);
				}
				if (status == UserStatus.Offline || status.HasFlag(UserStatus.Logout))
				{
					this.RemovePlayer(profile_id, GameRoomPlayerRemoveReason.Left);
				}
			}
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x0006B0C0 File Offset: 0x000694C0
		public bool HasPlayer(ulong profile_id)
		{
			CoreState state = base.GetState<CoreState>(AccessMode.ReadOnly);
			return state.Players.ContainsKey(profile_id);
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x0006B0E4 File Offset: 0x000694E4
		public bool HasReservation(ulong profile_id)
		{
			CoreState state = base.GetState<CoreState>(AccessMode.ReadOnly);
			return state.ReservedPlayers.ContainsKey(profile_id);
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x0006B105 File Offset: 0x00069505
		public RoomPlayer GetPlayer(ulong profile_id)
		{
			return this.GetPlayer(profile_id, base.CurrentAccessMode);
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x0006B114 File Offset: 0x00069514
		public RoomPlayer GetPlayer(ulong profile_id, AccessMode mode)
		{
			CoreState state = base.GetState<CoreState>(mode);
			RoomPlayer result;
			state.Players.TryGetValue(profile_id, out result);
			return result;
		}

		// Token: 0x06001A1C RID: 6684 RVA: 0x0006B13C File Offset: 0x0006953C
		public RoomPlayer GetPlayer(string nickname, AccessMode mode)
		{
			CoreState state = base.GetState<CoreState>(mode);
			return state.Players.Values.FirstOrDefault((RoomPlayer p) => p.Nickname == nickname);
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x0006B17A File Offset: 0x0006957A
		public bool TryGetPlayer(ulong profileId, out RoomPlayer player)
		{
			return this.TryGetPlayer(profileId, base.CurrentAccessMode, out player);
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x0006B18C File Offset: 0x0006958C
		public bool TryGetPlayer(ulong profileId, AccessMode mode, out RoomPlayer player)
		{
			CoreState state = base.GetState<CoreState>(mode);
			return state.Players.TryGetValue(profileId, out player);
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x0006B1AE File Offset: 0x000695AE
		public void SignalPlayersChanged()
		{
			base.CheckAccessMode(AccessMode.ReadWrite);
			if (this.tr_players_changed != null)
			{
				this.tr_players_changed();
			}
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x0006B1D0 File Offset: 0x000695D0
		public void SwitchTeam(ulong profile_id, int team)
		{
			RoomPlayer player = this.GetPlayer(profile_id, AccessMode.ReadWrite);
			if (player == null)
			{
				throw new ApplicationException("No such player");
			}
			TeamExtension extension = base.GetExtension<TeamExtension>();
			extension.SwitchTeam(player, team);
			Log.Verbose(Log.Group.GameRoom, "Player {0} in room {1} is now in team {2}", new object[]
			{
				profile_id,
				base.ID,
				player.TeamID
			});
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x0006B23C File Offset: 0x0006963C
		public void SwapTeams()
		{
			CoreState state = base.GetState<CoreState>(AccessMode.ReadWrite);
			state.TeamsSwapped = !state.TeamsSwapped;
			Log.Verbose(Log.Group.GameRoom, "Room {0} teams swapped", new object[]
			{
				base.ID
			});
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x0006B280 File Offset: 0x00069680
		public uint GetTeamColor(int team)
		{
			CoreState state = base.GetState<CoreState>(AccessMode.ReadOnly);
			return state.TeamColors[team - 1];
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x0006B2A0 File Offset: 0x000696A0
		public void SetTeamColor(int team, uint color)
		{
			CoreState state = base.GetState<CoreState>(AccessMode.ReadWrite);
			state.TeamColors[team - 1] = color;
		}

		// Token: 0x06001A24 RID: 6692 RVA: 0x0006B2C0 File Offset: 0x000696C0
		public void PlayerJoinedSession(ulong profileId)
		{
			if (this.tr_player_joined_session != null)
			{
				this.tr_player_joined_session(profileId);
			}
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x0006B2DC File Offset: 0x000696DC
		protected override void PostStateChanged(IRoomState new_state, IRoomState old_state)
		{
			CoreState state = (CoreState)new_state;
			CoreState old = (CoreState)old_state;
			try
			{
				this.NotifyPlayerChanges(state, old);
			}
			catch (Exception e)
			{
				Log.Error("Error on GameRoom notifications intercepted");
				Log.Error(e);
			}
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x0006B32C File Offset: 0x0006972C
		public override XmlElement CreateRoomElement(XmlDocument factory)
		{
			XmlElement xmlElement = base.CreateRoomElement(factory);
			xmlElement.SetAttribute("room_id", base.ID.ToString());
			xmlElement.SetAttribute("room_type", ((int)base.Type).ToString());
			return xmlElement;
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x0006B380 File Offset: 0x00069780
		protected override void SendBroadcastUpdate(RoomUpdate.Target target, Set<string> recepients, DoubleBuffer<Type, IRoomState>.Snapshot snapshot)
		{
			StateSyncExtension extension = base.GetExtension<StateSyncExtension>();
			if (target == RoomUpdate.Target.Client)
			{
				extension.SyncStateToClients(recepients, snapshot);
			}
			else if (target == RoomUpdate.Target.Server)
			{
				extension.SyncStateToServer(recepients[0], snapshot);
			}
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x0006B3BC File Offset: 0x000697BC
		private void NotifyPlayerChanges(CoreState state, CoreState old)
		{
			foreach (RoomPlayer roomPlayer in state.Players.Values)
			{
				RoomPlayer roomPlayer2;
				if (!old.Players.TryGetValue(roomPlayer.ProfileID, out roomPlayer2))
				{
					if (this.PlayerAdded != null)
					{
						this.PlayerAdded(this, roomPlayer.ProfileID);
					}
				}
				else if (!roomPlayer.Equals(roomPlayer2))
				{
					roomPlayer.Revision++;
					if (this.PlayerChanged != null)
					{
						this.PlayerChanged(this, roomPlayer, roomPlayer2);
					}
				}
			}
			foreach (RoomPlayer roomPlayer3 in old.Players.Values)
			{
				GameRoomPlayerRemoveReason reason;
				if (this.PlayerRemoved != null && !state.Players.ContainsKey(roomPlayer3.ProfileID) && state.RoomLeftPlayers.TryGetValue(roomPlayer3.ProfileID, out reason))
				{
					this.PlayerRemoved(this, roomPlayer3, reason);
				}
			}
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x0006B518 File Offset: 0x00069918
		public string GetUsersBuildType()
		{
			CoreState state = base.GetState<CoreState>(AccessMode.ReadOnly);
			List<string> list = (from p in state.Players.Values
			select new
			{
				p = p,
				user = this.m_userRepository.GetUser(p.ProfileID)
			} into <>__TranspIdent7
			select new
			{
				<>__TranspIdent7 = <>__TranspIdent7,
				buildType = ((<>__TranspIdent7.user != null) ? <>__TranspIdent7.user.BuildType : string.Empty)
			} into <>__TranspIdent8
			where <>__TranspIdent8.buildType != string.Empty
			select <>__TranspIdent8.buildType).Distinct<string>().ToList<string>();
			return (list.Count != 1) ? string.Empty : list.First<string>();
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x0006B5D8 File Offset: 0x000699D8
		public void CheckPlayerRank(SProfileInfo profile, SRankInfo rank)
		{
			if (!this.m_rankSystem.CanJoinChannel(rank.RankId))
			{
				Log.Verbose("Player '{0}' will be kicked due to channel rank restriction.", new object[]
				{
					profile.UserID
				});
				base.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					r.GetExtension<KickExtension>().KickPlayer(profile.Id, GameRoomPlayerRemoveReason.KickRankRestricted);
				});
			}
			else
			{
				base.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					r.SignalPlayersChanged();
				});
			}
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x0006B668 File Offset: 0x00069A68
		public void SetObserver(ulong profileId, bool enable)
		{
			RoomPlayer player = this.GetPlayer(profileId, AccessMode.ReadWrite);
			if (player == null)
			{
				throw new ApplicationException(string.Format("No such player - profileId: {0}", profileId));
			}
			if (player.RoomStatus == RoomPlayer.EStatus.Ready && this.GameRunning)
			{
				throw new ApplicationException(string.Format("Can't change observer status on spawned player - profileId: {0}", profileId));
			}
			player.Observer = enable;
			if (!player.Observer && player.TeamID != 0)
			{
				Log.Warning<ulong, int>("Player profileId: {0}, observer: false, already in team: {1}", player.ProfileID, player.TeamID);
				return;
			}
			TeamExtension extension = base.GetExtension<TeamExtension>();
			extension.SwitchTeam(player, -1);
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06001A2C RID: 6700 RVA: 0x0006B70A File Offset: 0x00069B0A
		public bool AllowReconnect
		{
			get
			{
				return this.m_extensions.ContainsKey(typeof(ReconnectExtension));
			}
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x0006B724 File Offset: 0x00069B24
		public ReconnectResult ReconnectPlayer(ulong profileId)
		{
			if (!this.AllowReconnect)
			{
				return ReconnectResult.InvalidReconnectInfo;
			}
			ReconnectExtension extension = base.GetExtension<ReconnectExtension>();
			return extension.Reconnect(profileId);
		}

		// Token: 0x06001A2E RID: 6702 RVA: 0x0006B74C File Offset: 0x00069B4C
		public float CalculateRoomRating()
		{
			List<double> list = (from p in this.ReservedPlayers
			select p.Skill.Value).ToList<double>();
			list.AddRange(from p in this.Players
			select p.Skill.Value);
			return (!list.Any<double>()) ? 0f : Convert.ToSingle(list.Max());
		}

		// Token: 0x04000C5D RID: 3165
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000C5E RID: 3166
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000C5F RID: 3167
		private readonly IGameRoomManagerRegistry m_gameRoomManager;

		// Token: 0x04000C60 RID: 3168
		private readonly IClientVersionsManagementService m_clientVersionsManagementService;

		// Token: 0x04000C61 RID: 3169
		private readonly IMissionAccessLimitationService m_limitationService;

		// Token: 0x04000C62 RID: 3170
		private readonly IRoomPlayerFactory m_roomPlayerFactory;

		// Token: 0x04000C63 RID: 3171
		private readonly ISkillService m_skillService;

		// Token: 0x04000C64 RID: 3172
		private readonly Random m_rand;
	}
}
