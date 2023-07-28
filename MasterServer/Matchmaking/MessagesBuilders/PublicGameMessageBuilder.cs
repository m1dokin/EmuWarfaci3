using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services.Regions;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Matchmaking.Data;
using MasterServer.XMPP;

namespace MasterServer.Matchmaking.MessagesBuilders
{
	// Token: 0x0200050B RID: 1291
	[Service]
	[Singleton]
	internal class PublicGameMessageBuilder : MatchmakingMessageBuilder, IMessageBuilder
	{
		// Token: 0x06001BF4 RID: 7156 RVA: 0x00071314 File Offset: 0x0006F714
		public PublicGameMessageBuilder(IServerPresenceNotifier serverPresenceNotifier, IMatchmakingSystem matchmakingSystem, IMMEntityDTOFactory mmEntityDtoFactory, IMMMissionDTOFactory mmMissionDtoFactory, IGameRoomManager gameRoomManager, IMissionSystem missionSystem, IMatchmakingConfigProvider matchmakingConfigProvider, IRegionsService regionsService, IGameModesSystem gameModesSystem) : base(missionSystem, mmMissionDtoFactory, matchmakingSystem, mmEntityDtoFactory, gameRoomManager, serverPresenceNotifier, matchmakingConfigProvider, regionsService, gameModesSystem)
		{
			this.m_matchmakingConfigProvider = matchmakingConfigProvider;
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06001BF5 RID: 7157 RVA: 0x0007133E File Offset: 0x0006F73E
		public string QueueName
		{
			get
			{
				return Resources.MMQueuesNames.SendingPublicGamesQueueName;
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06001BF6 RID: 7158 RVA: 0x00071345 File Offset: 0x0006F745
		public bool IsQueueAvailableByChannelType
		{
			get
			{
				return Resources.Channel != Resources.ChannelType.PVE;
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06001BF7 RID: 7159 RVA: 0x00071352 File Offset: 0x0006F752
		protected override GameRoomType RoomType
		{
			get
			{
				return GameRoomType.PvP_AutoStart;
			}
		}

		// Token: 0x06001BF8 RID: 7160 RVA: 0x00071358 File Offset: 0x0006F758
		protected override bool RoomSuits(IGameRoom room)
		{
			return base.RoomSuits(room) && !room.IsPvpRatingMode() && room.IsAutoStartMode() == this.m_matchmakingConfigProvider.Get().IsAutostartEnabled;
		}

		// Token: 0x06001BF9 RID: 7161 RVA: 0x0007139A File Offset: 0x0006F79A
		protected override bool EntitySuits(MMEntityInfo entity)
		{
			return entity.Settings.RoomType.IsPvpAutoStartMode() && !entity.Settings.RoomType.IsPvpRatingMode();
		}

		// Token: 0x06001BFA RID: 7162 RVA: 0x000713C7 File Offset: 0x0006F7C7
		protected override bool MissionSuits(MissionContextBase mission)
		{
			return mission.IsPvPMode();
		}

		// Token: 0x04000D65 RID: 3429
		private readonly IMatchmakingConfigProvider m_matchmakingConfigProvider;
	}
}
