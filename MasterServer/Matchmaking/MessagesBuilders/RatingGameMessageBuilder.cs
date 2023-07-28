using System;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services.Regions;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Matchmaking.Data;
using MasterServer.XMPP;

namespace MasterServer.Matchmaking.MessagesBuilders
{
	// Token: 0x0200050D RID: 1293
	[Service]
	[Singleton]
	internal class RatingGameMessageBuilder : MatchmakingMessageBuilder, IMessageBuilder
	{
		// Token: 0x06001C02 RID: 7170 RVA: 0x0007146C File Offset: 0x0006F86C
		public RatingGameMessageBuilder(IMissionSystem missionSystem, IMMMissionDTOFactory mmMissionDtoFactory, IMatchmakingSystem matchmakingSystem, IMMEntityDTOFactory mmEntityDtoFactory, IGameRoomManager gameRoomManager, IServerPresenceNotifier serverPresenceNotifier, IMatchmakingConfigProvider matchmakingConfigProvider, IRegionsService regionsService, IGameModesSystem gameModesSystem) : base(missionSystem, mmMissionDtoFactory, matchmakingSystem, mmEntityDtoFactory, gameRoomManager, serverPresenceNotifier, matchmakingConfigProvider, regionsService, gameModesSystem)
		{
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06001C03 RID: 7171 RVA: 0x0007148E File Offset: 0x0006F88E
		public string QueueName
		{
			get
			{
				return Resources.MMQueuesNames.SendingRatingGamesQueueName;
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06001C04 RID: 7172 RVA: 0x00071495 File Offset: 0x0006F895
		public bool IsQueueAvailableByChannelType
		{
			get
			{
				return Resources.Channel.IsOneOf(new Resources.ChannelType[]
				{
					Resources.ChannelType.PVP_Skilled,
					Resources.ChannelType.PVP_Pro
				});
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06001C05 RID: 7173 RVA: 0x000714AF File Offset: 0x0006F8AF
		protected override GameRoomType RoomType
		{
			get
			{
				return GameRoomType.PvP_Rating;
			}
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x000714B3 File Offset: 0x0006F8B3
		protected override bool RoomSuits(IGameRoom room)
		{
			return false;
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x000714B6 File Offset: 0x0006F8B6
		protected override bool EntitySuits(MMEntityInfo entity)
		{
			return entity.Settings.RoomType.IsPvpRatingMode();
		}

		// Token: 0x06001C08 RID: 7176 RVA: 0x000714C8 File Offset: 0x0006F8C8
		protected override bool MissionSuits(MissionContextBase mission)
		{
			return mission.IsAvailableForRatingGame();
		}
	}
}
