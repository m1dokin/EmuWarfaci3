using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.MissionAccessLimitation;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameLogic.RatingSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoom.RoomExtensions;
using MasterServer.GameRoom.RoomExtensions.Reconnect;
using MasterServer.GameRoom.RoomExtensions.Vote;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Matchmaking;
using MasterServer.ServerInfo;
using MasterServer.Telemetry;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004BC RID: 1212
	[Service]
	[Singleton]
	internal class RoomExtensionFactory : IRoomExtensionFactory
	{
		// Token: 0x06001A3A RID: 6714 RVA: 0x0006B8AC File Offset: 0x00069CAC
		public RoomExtensionFactory(IHabitat habitat, IMissionAccessLimitationService limitationService, IGameModesSystem gameModesSystem, ISessionStorage sessionStorage, ILogService logService, IMissionSystem missionSystem, IClanService clanService, ITelemetryService telemetryService, IRankSystem rankSystem, IGameRoomServer gameRoomServer, IServerInfo serverInfo, IOnlineClient onlineClient, IQueryManager queryManager, IMapVoting mapVotingTracker, IMatchmakingMissionsProvider matchmakingMissionsProvider, IClientVersionsManagementService clientVersionsManagementService, IUserRepository userRepository, IAutoTeamBalanceLogicFactory balanceLogicFactory, IClassPresenceService classPresenceService, ITimerFactory timerFactory, IConfigProvider<RatingRoomConfig> ratingConfigProvider, IConfigProvider<ReconnectConfig> reconnectConfigProvider, IGameRoomOfferService gameRoomOfferService, IVoteConfigContainer voteConfigContainer, IPunishmentService punishmentService, IRatingGameBanService ratingGameBanService)
		{
			this.m_limitationService = limitationService;
			this.m_gameModesSystem = gameModesSystem;
			this.m_sessionStorage = sessionStorage;
			this.m_logService = logService;
			this.m_missionSystem = missionSystem;
			this.m_clanService = clanService;
			this.m_telemetryService = telemetryService;
			this.m_rankSystem = rankSystem;
			this.m_gameRoomServer = gameRoomServer;
			this.m_serverInfo = serverInfo;
			this.m_onlineClient = onlineClient;
			this.m_queryManager = queryManager;
			this.m_mapVotingtracker = mapVotingTracker;
			this.m_matchmakingMissionsProvider = matchmakingMissionsProvider;
			this.m_clientVersionsManagementService = clientVersionsManagementService;
			this.m_userRepository = userRepository;
			this.m_autoTeamBalanceLogicFactory = balanceLogicFactory;
			this.m_classPresenceService = classPresenceService;
			this.m_timerFactory = timerFactory;
			this.m_ratingConfigProvider = ratingConfigProvider;
			this.m_reconnectConfigProvider = reconnectConfigProvider;
			this.m_gameRoomOfferService = gameRoomOfferService;
			this.m_voteConfigContainer = voteConfigContainer;
			this.m_punishmentService = punishmentService;
			this.m_ratingGameBanService = ratingGameBanService;
			IEnumerable<ServiceInfo> servicesByContract = habitat.GetServicesByContract<IRoomExtension>();
			IEnumerable<ServiceInfo> servicesByContract2 = habitat.GetServicesByContract<IRoomState>();
			IEnumerable<KeyValuePair<RoomExtensionAttribute, Type>> roomExtensions = ReflectionUtils.MapAttributes<RoomExtensionAttribute>(from si in servicesByContract
			select si.ServiceType);
			IEnumerable<KeyValuePair<RoomStateAttribute, Type>> roomStates = ReflectionUtils.MapAttributes<RoomStateAttribute>(from si in servicesByContract2
			select si.ServiceType);
			this.LoadRoomExtensions(roomExtensions, roomStates);
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x0006B9F8 File Offset: 0x00069DF8
		public RoomExtensionsData GetRoomExtensions(GameRoomType type)
		{
			List<IRoomExtension> list = new List<IRoomExtension>
			{
				new CoreStateExtension(),
				new StateSyncExtension(this.m_onlineClient),
				new ServerExtension(this.m_gameRoomServer, this.m_serverInfo),
				new SessionExtension(this.m_queryManager),
				new CustomParamsExtension(this.m_limitationService, this.m_gameModesSystem, this.m_sessionStorage),
				new KickExtension(this.m_clientVersionsManagementService, this.m_userRepository),
				new LogRoomEvents(this.m_logService, this.m_clanService),
				new MissionExtension(this.m_limitationService, this.m_missionSystem, this.m_gameModesSystem),
				new PlayerClanInfo(this.m_clanService),
				new ReservationsExtension(),
				new RoomTracker(this.m_telemetryService, this.m_rankSystem),
				new AutoKickExtension()
			};
			ReconnectConfig reconnectConfig = this.m_reconnectConfigProvider.Get();
			if (reconnectConfig.ReconnectableRoomTypes.Any((GameRoomType t) => t.HasFlag(type)))
			{
				list.Add(new ReconnectExtension(this.m_reconnectConfigProvider, this.m_gameRoomOfferService));
			}
			if (type.HasFlag(GameRoomType.PvE_Private))
			{
				list.Add(new KickVoteExtension(this.m_queryManager, this.m_logService, this.m_punishmentService, this.m_voteConfigContainer));
				list.Add(new StartRules());
				list.Add(new RoomMasterExtension());
				list.Add(new TeamExtension(this.m_autoTeamBalanceLogicFactory.GetTeamBalancer(GameRoomType.PvE_Private), this.m_logService));
				list.Add(new RegionRoomMasterExtension());
			}
			else if (type.HasFlag(GameRoomType.PvE_AutoStart))
			{
				list.Add(new KickVoteExtension(this.m_queryManager, this.m_logService, this.m_punishmentService, this.m_voteConfigContainer));
				list.Add(new AutoStartExtension(this.m_gameModesSystem));
				list.Add(new AutoStartKickExtension(this.m_limitationService));
				list.Add(new MapVotingExtension(this.m_queryManager, this.m_logService, this.m_mapVotingtracker, this.m_missionSystem, this.m_matchmakingMissionsProvider, this.m_gameModesSystem));
				list.Add(new TeamExtension(this.m_autoTeamBalanceLogicFactory.GetTeamBalancer(GameRoomType.PvE_AutoStart), this.m_logService));
				list.Add(new RegionAutostartExtension());
			}
			else if (type.HasFlag(GameRoomType.PvP_Public))
			{
				list.Add(new KickVoteExtension(this.m_queryManager, this.m_logService, this.m_punishmentService, this.m_voteConfigContainer));
				list.Add(new StartRules());
				list.Add(new RoomMasterExtension());
				list.Add(new TeamExtension(this.m_autoTeamBalanceLogicFactory.GetTeamBalancer(GameRoomType.PvP_Public), this.m_logService));
				list.Add(new PlayTimeExtension(this.m_sessionStorage));
				list.Add(new RegionRoomMasterExtension());
			}
			else if (type.HasFlag(GameRoomType.PvP_AutoStart))
			{
				list.Add(new KickVoteExtension(this.m_queryManager, this.m_logService, this.m_punishmentService, this.m_voteConfigContainer));
				list.Add(new TeamExtension(this.m_autoTeamBalanceLogicFactory.GetTeamBalancer(GameRoomType.PvP_AutoStart), this.m_logService));
				list.Add(new AutoStartExtension(this.m_gameModesSystem));
				list.Add(new MapVotingExtension(this.m_queryManager, this.m_logService, this.m_mapVotingtracker, this.m_missionSystem, this.m_matchmakingMissionsProvider, this.m_gameModesSystem));
				list.Add(new PlayTimeExtension(this.m_sessionStorage));
				list.Add(new RegionAutostartExtension());
			}
			else if (type.HasFlag(GameRoomType.PvP_Rating))
			{
				list.Add(new TeamExtension(this.m_autoTeamBalanceLogicFactory.GetTeamBalancer(GameRoomType.PvP_Rating), this.m_logService));
				list.Add(new AutoStartExtension(this.m_gameModesSystem));
				list.Add(new PlayTimeExtension(this.m_sessionStorage));
				list.Add(new RatingRoomExtension(this.m_timerFactory, this.m_ratingConfigProvider, this.m_ratingGameBanService));
				list.Add(new RegionAutostartExtension());
				list.Add(new SurrenderVoteExtension(this.m_queryManager, this.m_logService, this.m_voteConfigContainer));
			}
			else if (type.HasFlag(GameRoomType.PvP_ClanWar))
			{
				list.Add(new KickVoteExtension(this.m_queryManager, this.m_logService, this.m_punishmentService, this.m_voteConfigContainer));
				list.Add(new StartRules());
				list.Add(new RoomMasterExtension());
				list.Add(new ClanWarExtension());
				list.Add(new TeamExtension(this.m_autoTeamBalanceLogicFactory.GetTeamBalancer(GameRoomType.PvP_ClanWar), this.m_logService));
				list.Add(new RegionRoomMasterExtension());
			}
			if (Resources.DebugQueriesEnabled)
			{
				list.Add(new DebugRoomExtension());
			}
			return this.GetExtensionsWithStates(list);
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x0006BF24 File Offset: 0x0006A324
		private RoomExtensionsData GetExtensionsWithStates(IEnumerable<IRoomExtension> extensions)
		{
			RoomExtensionsData roomExtensionsData = new RoomExtensionsData();
			foreach (IRoomExtension roomExtension in extensions)
			{
				roomExtensionsData.Add(roomExtension, new List<IRoomState>());
				foreach (Type type in this.m_roomExtensionsToStates[roomExtension.GetType()])
				{
					roomExtensionsData[roomExtension].Add((IRoomState)Activator.CreateInstance(type));
				}
			}
			return roomExtensionsData;
		}

		// Token: 0x06001A3D RID: 6717 RVA: 0x0006BFEC File Offset: 0x0006A3EC
		private void LoadRoomExtensions(IEnumerable<KeyValuePair<RoomExtensionAttribute, Type>> roomExtensions, IEnumerable<KeyValuePair<RoomStateAttribute, Type>> roomStates)
		{
			foreach (KeyValuePair<RoomExtensionAttribute, Type> keyValuePair in roomExtensions)
			{
				this.m_roomExtensionsToStates.Add(keyValuePair.Value, new List<Type>());
			}
			foreach (KeyValuePair<RoomStateAttribute, Type> keyValuePair2 in roomStates)
			{
				foreach (Type key in keyValuePair2.Key.OwnerExtensions)
				{
					List<Type> list = this.m_roomExtensionsToStates[key];
					list.Add(keyValuePair2.Value);
				}
			}
		}

		// Token: 0x04000C77 RID: 3191
		private readonly IMissionAccessLimitationService m_limitationService;

		// Token: 0x04000C78 RID: 3192
		private readonly IGameModesSystem m_gameModesSystem;

		// Token: 0x04000C79 RID: 3193
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000C7A RID: 3194
		private readonly ILogService m_logService;

		// Token: 0x04000C7B RID: 3195
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000C7C RID: 3196
		private readonly IClanService m_clanService;

		// Token: 0x04000C7D RID: 3197
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x04000C7E RID: 3198
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000C7F RID: 3199
		private readonly IGameRoomServer m_gameRoomServer;

		// Token: 0x04000C80 RID: 3200
		private readonly IServerInfo m_serverInfo;

		// Token: 0x04000C81 RID: 3201
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04000C82 RID: 3202
		private readonly IQueryManager m_queryManager;

		// Token: 0x04000C83 RID: 3203
		private readonly IMatchmakingMissionsProvider m_matchmakingMissionsProvider;

		// Token: 0x04000C84 RID: 3204
		private readonly IClientVersionsManagementService m_clientVersionsManagementService;

		// Token: 0x04000C85 RID: 3205
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000C86 RID: 3206
		private readonly IMapVoting m_mapVotingtracker;

		// Token: 0x04000C87 RID: 3207
		private readonly IAutoTeamBalanceLogicFactory m_autoTeamBalanceLogicFactory;

		// Token: 0x04000C88 RID: 3208
		private readonly IClassPresenceService m_classPresenceService;

		// Token: 0x04000C89 RID: 3209
		private readonly ITimerFactory m_timerFactory;

		// Token: 0x04000C8A RID: 3210
		private readonly IConfigProvider<RatingRoomConfig> m_ratingConfigProvider;

		// Token: 0x04000C8B RID: 3211
		private readonly IConfigProvider<ReconnectConfig> m_reconnectConfigProvider;

		// Token: 0x04000C8C RID: 3212
		private readonly IGameRoomOfferService m_gameRoomOfferService;

		// Token: 0x04000C8D RID: 3213
		private readonly IVoteConfigContainer m_voteConfigContainer;

		// Token: 0x04000C8E RID: 3214
		private readonly IRatingGameBanService m_ratingGameBanService;

		// Token: 0x04000C8F RID: 3215
		private readonly IPunishmentService m_punishmentService;

		// Token: 0x04000C90 RID: 3216
		private readonly Dictionary<Type, List<Type>> m_roomExtensionsToStates = new Dictionary<Type, List<Type>>();
	}
}
