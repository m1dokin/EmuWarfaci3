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
	// Token: 0x0200050C RID: 1292
	[Service]
	[Singleton]
	internal class PveGameMessageBuilder : MatchmakingMessageBuilder, IMessageBuilder
	{
		// Token: 0x06001BFB RID: 7163 RVA: 0x000713D0 File Offset: 0x0006F7D0
		public PveGameMessageBuilder(IServerPresenceNotifier serverPresenceNotifier, IMatchmakingSystem matchmakingSystem, IMMEntityDTOFactory mmEntityDtoFactory, IMMMissionDTOFactory mmMissionDtoFactory, IGameRoomManager gameRoomManager, IMissionSystem missionSystem, IMatchmakingConfigProvider matchmakingConfigProvider, IRegionsService regionsService, IGameModesSystem gameModesSystem) : base(missionSystem, mmMissionDtoFactory, matchmakingSystem, mmEntityDtoFactory, gameRoomManager, serverPresenceNotifier, matchmakingConfigProvider, regionsService, gameModesSystem)
		{
			this.m_matchmakingConfigProvider = matchmakingConfigProvider;
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06001BFC RID: 7164 RVA: 0x000713FA File Offset: 0x0006F7FA
		public string QueueName
		{
			get
			{
				return Resources.MMQueuesNames.SendingPublicGamesQueueName;
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06001BFD RID: 7165 RVA: 0x00071401 File Offset: 0x0006F801
		public bool IsQueueAvailableByChannelType
		{
			get
			{
				return Resources.Channel == Resources.ChannelType.PVE;
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06001BFE RID: 7166 RVA: 0x0007140B File Offset: 0x0006F80B
		protected override GameRoomType RoomType
		{
			get
			{
				return GameRoomType.PvE_AutoStart;
			}
		}

		// Token: 0x06001BFF RID: 7167 RVA: 0x00071410 File Offset: 0x0006F810
		protected override bool RoomSuits(IGameRoom room)
		{
			return !room.IsPvpMode() && base.RoomSuits(room) && room.IsAutoStartMode() == this.m_matchmakingConfigProvider.Get().IsPveAutostartEnabled;
		}

		// Token: 0x06001C00 RID: 7168 RVA: 0x00071451 File Offset: 0x0006F851
		protected override bool EntitySuits(MMEntityInfo entity)
		{
			return entity.Settings.RoomType.IsPveMode();
		}

		// Token: 0x06001C01 RID: 7169 RVA: 0x00071463 File Offset: 0x0006F863
		protected override bool MissionSuits(MissionContextBase mission)
		{
			return mission.IsPveMode();
		}

		// Token: 0x04000D66 RID: 3430
		private readonly IMatchmakingConfigProvider m_matchmakingConfigProvider;
	}
}
