using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DedicatedPoolServer.Model;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.Matchmaking;
using MasterServer.ServerInfo;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004B2 RID: 1202
	[RoomExtension]
	internal class MapVotingExtension : RoomExtensionBase
	{
		// Token: 0x06001984 RID: 6532 RVA: 0x000679D4 File Offset: 0x00065DD4
		public MapVotingExtension(IQueryManager queryManager, ILogService logService, IMapVoting mapVotingTracker, IMissionSystem missionSystem, IMatchmakingMissionsProvider matchmakingMissionsProvider, IGameModesSystem gameModesSystem)
		{
			this.m_queryManager = queryManager;
			this.m_logService = logService;
			this.m_mapVotingTracker = mapVotingTracker;
			this.m_missionSystem = missionSystem;
			this.m_matchmakingMissionsProvider = matchmakingMissionsProvider;
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x00067A2C File Offset: 0x00065E2C
		private void OnConfigGameRoomMapVotingChanged(ConfigEventArgs args)
		{
			if (string.Equals(args.Name, "enabled", StringComparison.InvariantCultureIgnoreCase))
			{
				int num;
				if (!int.TryParse(args.sValue, out num) || num < 0 || num > 1)
				{
					throw new ApplicationException("Attribute 'enabled' from MapVoting section in module_configuration.xml contains incorrect data. Allowed values are 0 or 1");
				}
				this.m_mapVotingParams.Enabled = (num != 0);
			}
			else if (string.Equals(args.Name, "new_maps", StringComparison.InvariantCultureIgnoreCase))
			{
				int num2;
				if (!int.TryParse(args.sValue, out num2) || num2 < 0)
				{
					throw new ApplicationException("Attribute 'new_maps' from MapVoting section in module_configuration.xml contains incorrect data. Allowed new_maps >= 0");
				}
				this.m_mapVotingParams.NewMaps = num2;
			}
			else if (string.Equals(args.Name, "voting_time_sec", StringComparison.InvariantCultureIgnoreCase))
			{
				if (args.TimeSpanValue.Equals(default(TimeSpan)) || args.TimeSpanValue.TotalMilliseconds < 0.0)
				{
					throw new ApplicationException("Attribute 'voting_time_sec' from MapVoting section in module_configuration.xml contains incorrect data. Allowed voting_time >= 0");
				}
				this.m_mapVotingParams.VotingTime = args.TimeSpanValue;
				this.Validate();
			}
			else if (string.Equals(args.Name, "mode", StringComparison.InvariantCultureIgnoreCase))
			{
				this.m_mapVotingParams.Mode = new HashSet<string[]>(this.GetCustomSettings(this.m_configGameRoomMapVotingSection.Get("mode")));
			}
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x00067B90 File Offset: 0x00065F90
		private void GetResources(MissionContext mission)
		{
			GameModeSetting gameModeSetting = this.m_gameModesSystem.GetGameModeSetting(mission);
			int num;
			if (!gameModeSetting.GetSetting(base.Room.Type, ERoomSetting.AUTOSTART_POST_SESSION_TIMEOUT_SEC, out num))
			{
				throw new ApplicationException(string.Format("Can't get 'AutostartPostSessionTimeoutSec' for room type '{0}'", base.Room.Type));
			}
			this.m_postSessionTimeout = TimeSpan.FromSeconds((double)num);
			int num2;
			if (!int.TryParse(this.m_configGameRoomMapVotingSection.Get("enabled"), out num2))
			{
				throw new ApplicationException("Attribute 'enabled' from MapVoting section in module_configuration.xml is missed or contains incorrect data");
			}
			int newMaps;
			if (!int.TryParse(this.m_configGameRoomMapVotingSection.Get("new_maps"), out newMaps))
			{
				throw new ApplicationException("Attribute 'new_maps' from MapVoting section in module_configuration.xml is missed or contains incorrect data");
			}
			TimeSpan votingTime;
			if (!this.m_configGameRoomMapVotingSection.TryGet("voting_time_sec", out votingTime, default(TimeSpan)))
			{
				throw new ApplicationException("Attribute 'voting_time_sec' from MapVoting section in module_configuration.xml is missed or contains incorrect data");
			}
			HashSet<string[]> mode = new HashSet<string[]>(this.GetCustomSettings(this.m_configGameRoomMapVotingSection.Get("mode")));
			this.m_mapVotingParams = new MapVotingParams(mode, num2 != 0, newMaps, votingTime);
			this.Validate();
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x00067CA4 File Offset: 0x000660A4
		private IEnumerable<string[]> GetCustomSettings(string customSettings)
		{
			if (base.Room.Type == GameRoomType.PvE_AutoStart)
			{
				return new List<string[]>();
			}
			string[] array = customSettings.Trim(new char[]
			{
				' '
			}).Split(new char[]
			{
				','
			});
			if (this.IsStringArrayEmpty(array))
			{
				throw new ApplicationException("Attribute 'mode' from MapVotingSection in module_configuration.xml is missed");
			}
			List<string[]> list = (from g in array
			select g.ToLower().Trim(new char[]
			{
				' '
			}).Split(new char[]
			{
				'-'
			})).ToList<string[]>();
			if (list.Any((string[] mode) => mode.Any((string s) => this.m_gameModesSystem.GetGameModeRestriction(s) == null)))
			{
				throw new ApplicationException("Unknown game mode in attribute 'mode' from MapVotingSection in module_configuration.xml");
			}
			list.RemoveAll(new Predicate<string[]>(this.IsStringArrayEmpty));
			return list;
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x00067D61 File Offset: 0x00066161
		private bool IsStringArrayEmpty(string[] arr)
		{
			return arr.Length == 0 || (arr.Length == 1 && string.IsNullOrEmpty(arr[0]));
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x00067D82 File Offset: 0x00066182
		private void Validate()
		{
			if (this.m_postSessionTimeout < this.m_mapVotingParams.VotingTime)
			{
				throw new ApplicationException("AutostartPostSessionTimeoutSec is less than 'vote_time' from MapVoting section in module_configuration.xml");
			}
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x00067DAA File Offset: 0x000661AA
		private void SetupExtension(MissionContext mission)
		{
			this.m_rotatingMode = ((!base.Room.Type.IsPveMode()) ? this.SetupForPvpRoom(mission) : this.SetupForPveRoom(mission));
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x00067DDA File Offset: 0x000661DA
		private IMapRotatingMode SetupForPveRoom(MissionContext mission)
		{
			this.GetResources(mission);
			return new MapRotatingModePve(this.m_missionSystem, this.m_matchmakingMissionsProvider, this.m_mapVotingParams);
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x00067DFA File Offset: 0x000661FA
		private IMapRotatingMode SetupForPvpRoom(MissionContext mission)
		{
			this.GetResources(mission);
			return new MapRotatingModePvp(this.m_missionSystem, this.m_matchmakingMissionsProvider, this.m_mapVotingParams);
		}

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x0600198D RID: 6541 RVA: 0x00067E1C File Offset: 0x0006621C
		private bool IsActive
		{
			get
			{
				AutoStart autoStart = base.Room.TryGetState<AutoStart>(AccessMode.ReadOnly);
				return autoStart != null && !autoStart.CanManualStart && this.m_rotatingMode.IsVoteAvailable;
			}
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x00067E58 File Offset: 0x00066258
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			this.m_configGameRoomMapVotingSection = Resources.ModuleSettings.GetSection(string.Format("GameRoom.{0}.MapVoting", (!base.Room.Type.IsPveMode()) ? "Pvp" : "Pve"));
			this.m_configGameRoomMapVotingSection.OnConfigChanged += this.OnConfigGameRoomMapVotingChanged;
			MissionExtension extension = base.Room.GetExtension<MissionExtension>();
			extension.TrSetMissionInfoEnded += this.OnTrSetMissionInfoEnded;
			SessionExtension extension2 = base.Room.GetExtension<SessionExtension>();
			extension2.tr_session_started += this.OnGameSessionStarted;
			extension2.tr_session_ended += this.OnGameSessionFinished;
			ServerExtension extension3 = base.Room.GetExtension<ServerExtension>();
			extension3.tr_server_changed += this.OnServerStateChanged;
			base.Room.tr_player_removed += this.OnPlayerRemovedFromRoom;
			base.Room.tr_player_joined_session += this.OnPlayerEnteredSession;
			base.Room.tr_player_status += this.OnPlayerStatusChanged;
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x00067F74 File Offset: 0x00066374
		public override void Close()
		{
			this.DisposeVotingTimer();
			this.m_configGameRoomMapVotingSection.OnConfigChanged -= this.OnConfigGameRoomMapVotingChanged;
			MissionExtension extension = base.Room.GetExtension<MissionExtension>();
			extension.TrSetMissionInfoEnded -= this.OnTrSetMissionInfoEnded;
			ServerExtension extension2 = base.Room.GetExtension<ServerExtension>();
			extension2.tr_server_changed -= this.OnServerStateChanged;
			SessionExtension extension3 = base.Room.GetExtension<SessionExtension>();
			extension3.tr_session_started -= this.OnGameSessionStarted;
			extension3.tr_session_ended -= this.OnGameSessionFinished;
			base.Room.tr_player_removed -= this.OnPlayerRemovedFromRoom;
			base.Room.tr_player_joined_session -= this.OnPlayerEnteredSession;
			base.Room.tr_player_status -= this.OnPlayerStatusChanged;
			base.Close();
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x00068055 File Offset: 0x00066455
		public IDictionary<string, int> DumpVotingState()
		{
			if (this.m_state == MapVotingExtension.VotingState.InProgress)
			{
				return this.m_rotatingMode.DumpVotingState();
			}
			return null;
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x00068070 File Offset: 0x00066470
		public void CountVote(ulong profileId, string missionUid)
		{
			if (this.m_state == MapVotingExtension.VotingState.InProgress)
			{
				RoomPlayer player = base.Room.GetPlayer(profileId);
				if (player == null)
				{
					return;
				}
				object @lock = this.m_lock;
				lock (@lock)
				{
					if (this.m_state != MapVotingExtension.VotingState.InProgress)
					{
						Log.Warning<ulong, ulong>("Vote from profileId: {0} in roomId: {1} arrived after vote has finished", profileId, base.Room.ID);
						this.m_mapVotingTracker.ReportVotesAfterVotingFinished();
						return;
					}
					if (!this.m_playersAllowedToVote.Contains(player.OnlineID))
					{
						throw new ApplicationException(string.Format("User with OnlineId: {0} ProfileId: {1} RoomId: {2} is trying to take part in voting but has no rights on it. Current voting state {3}", new object[]
						{
							player.OnlineID,
							profileId,
							base.Room.ID,
							this.m_state
						}));
					}
				}
				if (this.m_rotatingMode.TryCountVote(profileId, missionUid))
				{
					this.SendBroadcastRequest("map_voting_state", new object[]
					{
						this.m_rotatingMode.DumpVotingState()
					});
				}
				else
				{
					Log.Info<string, ulong, ulong>("Choice of map: {0} for roomId: {1} from profileId: {2} is not present in set of maps for vote", missionUid, base.Room.ID, profileId);
				}
			}
			else
			{
				Log.Warning<ulong, ulong>("Vote from profileId: {0} in roomId: {1} arrived after vote has finished", profileId, base.Room.ID);
				this.m_mapVotingTracker.ReportVotesAfterVotingFinished();
			}
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x000681D4 File Offset: 0x000665D4
		private void OnTrSetMissionInfoEnded(MissionContext mission)
		{
			this.SetupExtension(mission);
			this.m_rotatingMode.OnSetMissionEnded(mission);
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x000681EC File Offset: 0x000665EC
		private void OnGameSessionStarted(string sessionId)
		{
			if (this.IsActive)
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					this.m_state = MapVotingExtension.VotingState.InProgress;
					this.m_playersAllowedToVote = new HashSet<string>(from p in base.Room.Players
					select p.OnlineID);
					this.SendBroadcastRequest("map_voting_started", new object[]
					{
						this.m_rotatingMode.GetRotatingMaps,
						(int)this.m_mapVotingParams.VotingTime.TotalSeconds
					});
				}
				Log.Info(string.Format("Map voting has started in room: {0}", base.Room.ID));
			}
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x000682CC File Offset: 0x000666CC
		private void OnPlayerEnteredSession(ulong profileId)
		{
			if (this.IsActive)
			{
				RoomPlayer player = base.Room.GetPlayer(profileId);
				object @lock = this.m_lock;
				lock (@lock)
				{
					if (!this.m_playersAllowedToVote.Contains(player.OnlineID))
					{
						this.m_queryManager.BroadcastRequest("map_voting_started", "k01.", new string[]
						{
							player.OnlineID
						}.ToList<string>(), new object[]
						{
							this.m_rotatingMode.GetRotatingMaps,
							(int)this.m_mapVotingParams.VotingTime.TotalSeconds
						});
						this.m_playersAllowedToVote.Add(player.OnlineID);
					}
				}
			}
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x000683A0 File Offset: 0x000667A0
		private void OnPlayerRemovedFromRoom(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			if (this.IsActive)
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					this.m_playersAllowedToVote.Remove(player.OnlineID);
				}
			}
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x000683FC File Offset: 0x000667FC
		private void OnPlayerStatusChanged(ulong profileId, UserStatus oldStatus, UserStatus newStatus)
		{
			if (UserStatuses.IsInGame(oldStatus) && !UserStatuses.IsInGame(newStatus) && this.IsActive)
			{
				RoomPlayer player = base.Room.GetPlayer(profileId);
				if (player != null)
				{
					object @lock = this.m_lock;
					lock (@lock)
					{
						this.m_playersAllowedToVote.Remove(player.OnlineID);
					}
				}
			}
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x00068480 File Offset: 0x00066880
		private void OnGameSessionFinished(string sessionId, bool abnormal)
		{
			if (abnormal)
			{
				this.TerminateMapVoting(sessionId);
				return;
			}
			if (this.IsActive)
			{
				this.ChangeGameRoomMission();
			}
		}

		// Token: 0x06001998 RID: 6552 RVA: 0x000684A1 File Offset: 0x000668A1
		private void OnServerStateChanged(ServerEntity server)
		{
			if (this.IsActive && this.m_serverStatus != server.Status)
			{
				this.m_serverStatus = server.Status;
				if (this.m_serverStatus == EGameServerStatus.PostGame)
				{
					this.StartVotingTimer();
				}
			}
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x000684E0 File Offset: 0x000668E0
		private void StartVotingTimer()
		{
			Log.Info<ulong>("Map Voting count down has been started in room with roomId: {0}", base.Room.ID);
			object @lock = this.m_lock;
			bool flag = false;
			try
			{
				Monitor.Enter(@lock, ref flag);
				this.DisposeVotingTimer();
				bool abnormallyTerminated = false;
				this.m_timer = new SafeTimer(delegate(object x)
				{
					this.FinishVoting(abnormallyTerminated);
				}, null, (long)this.m_mapVotingParams.VotingTime.TotalMilliseconds, -1L);
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(@lock);
				}
			}
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x00068578 File Offset: 0x00066978
		private void DisposeVotingTimer()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_timer != null)
				{
					this.m_timer.Dispose();
					this.m_timer = null;
				}
			}
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x000685D4 File Offset: 0x000669D4
		private void FinishVoting(bool abnormallyTerminated)
		{
			this.m_state = MapVotingExtension.VotingState.Finished;
			string text = (!abnormallyTerminated) ? this.m_rotatingMode.ChooseWinner() : base.Room.MissionKey;
			this.SendBroadcastRequest("map_vote_finished", new object[]
			{
				text
			});
			Log.Info<ulong>("Map Voting count down has been finished in room with roomId: {0}", base.Room.ID);
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x00068634 File Offset: 0x00066A34
		private void ChangeGameRoomMission()
		{
			string chosenMap = this.m_rotatingMode.ChooseWinner();
			IDictionary<string, int> results = this.m_rotatingMode.DumpVotingState();
			base.Room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				StringBuilder stringBuilder = new StringBuilder();
				MissionContext mission = this.m_missionSystem.GetMission(this.Room.MissionKey);
				if (mission == null)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.AppendFormat("Current room(ID: {0}) mission with key {1} was not found in mission system.", this.Room.ID, this.Room.MissionKey);
					stringBuilder2.AppendFormat(" Additional info - current room type: {0}", this.Room.Type);
					throw new NullReferenceException(stringBuilder2.ToString());
				}
				int value = results.FirstOrDefault((KeyValuePair<string, int> k) => string.Equals(k.Key, this.Room.MissionKey)).Value;
				stringBuilder.AppendFormat("{0};{1};", mission.missionName, value);
				foreach (KeyValuePair<string, int> keyValuePair in from kv in results
				where !string.Equals(kv.Key, this.Room.MissionKey)
				orderby kv.Value
				select kv)
				{
					mission = this.m_missionSystem.GetMission(keyValuePair.Key);
					if (mission == null)
					{
						StringBuilder stringBuilder3 = new StringBuilder();
						stringBuilder3.AppendFormat("Voting candidate mission with key {0} was not found in mission system.", keyValuePair.Key);
						stringBuilder3.AppendFormat(" Voting candidate missions are: {0}.", string.Join<KeyValuePair<string, int>>(",", results));
						stringBuilder3.AppendFormat(" Additional info - current room type: {0}, room id: {1}", this.Room.Type, this.Room.ID);
						throw new NullReferenceException(stringBuilder3.ToString());
					}
					stringBuilder.AppendFormat("{0};{1};", mission.missionName, keyValuePair.Value);
				}
				this.m_logService.Event.MapVotingResult(r.ID, r.RoomName, stringBuilder.ToString());
				MissionExtension extension = this.Room.GetExtension<MissionExtension>();
				extension.SetMission(chosenMap);
				MissionContext mission2 = this.m_missionSystem.GetMission(chosenMap);
				if (mission2 == null)
				{
					StringBuilder stringBuilder4 = new StringBuilder();
					stringBuilder4.AppendFormat("Voting winner mission with key {0} was not found in mission system.", chosenMap);
					stringBuilder4.AppendFormat(" Additional info - current room type: {0}, room id: {1}", this.Room.Type, this.Room.ID);
					throw new NullReferenceException(stringBuilder4.ToString());
				}
				Log.Info(string.Format("Map voting has finished in room: {0}. New mission name:{1} uid: {2}", r.ID, mission2.missionName, chosenMap));
			});
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x00068688 File Offset: 0x00066A88
		private void TerminateMapVoting(string sessionId)
		{
			bool abnormallyTerminated = true;
			this.DisposeVotingTimer();
			this.FinishVoting(abnormallyTerminated);
			Log.Info<ulong, string>("Map voting in room: {0} is terminated due to abnormally terminated session {1}", base.Room.ID, sessionId);
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x000686BC File Offset: 0x00066ABC
		private void SendBroadcastRequest(string queryName, params object[] args)
		{
			object @lock = this.m_lock;
			List<string> recievers;
			lock (@lock)
			{
				recievers = this.m_playersAllowedToVote.ToList<string>();
			}
			this.m_queryManager.BroadcastRequest(queryName, "k01.", recievers, args);
		}

		// Token: 0x04000C33 RID: 3123
		private TimeSpan m_postSessionTimeout;

		// Token: 0x04000C34 RID: 3124
		private MapVotingParams m_mapVotingParams;

		// Token: 0x04000C35 RID: 3125
		private ConfigSection m_configGameRoomMapVotingSection;

		// Token: 0x04000C36 RID: 3126
		private readonly object m_lock = new object();

		// Token: 0x04000C37 RID: 3127
		private readonly IQueryManager m_queryManager;

		// Token: 0x04000C38 RID: 3128
		private readonly ILogService m_logService;

		// Token: 0x04000C39 RID: 3129
		private readonly IMapVoting m_mapVotingTracker;

		// Token: 0x04000C3A RID: 3130
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000C3B RID: 3131
		private readonly IMatchmakingMissionsProvider m_matchmakingMissionsProvider;

		// Token: 0x04000C3C RID: 3132
		private readonly IGameModesSystem m_gameModesSystem;

		// Token: 0x04000C3D RID: 3133
		private MapVotingExtension.VotingState m_state;

		// Token: 0x04000C3E RID: 3134
		private EGameServerStatus m_serverStatus;

		// Token: 0x04000C3F RID: 3135
		private HashSet<string> m_playersAllowedToVote = new HashSet<string>();

		// Token: 0x04000C40 RID: 3136
		private SafeTimer m_timer;

		// Token: 0x04000C41 RID: 3137
		private IMapRotatingMode m_rotatingMode;

		// Token: 0x02000524 RID: 1316
		private enum VotingState
		{
			// Token: 0x04000D9B RID: 3483
			NotStarted,
			// Token: 0x04000D9C RID: 3484
			InProgress,
			// Token: 0x04000D9D RID: 3485
			Finished
		}
	}
}
