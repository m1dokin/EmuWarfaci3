using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services.Regions;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Matchmaking.Data;
using MasterServer.XMPP;
using Network.Amqp;
using Network.Amqp.Interfaces;
using Network.Amqp.RabbitMq;

namespace MasterServer.Matchmaking.MessagesBuilders
{
	// Token: 0x0200050A RID: 1290
	internal abstract class MatchmakingMessageBuilder
	{
		// Token: 0x06001BE6 RID: 7142 RVA: 0x00070F34 File Offset: 0x0006F334
		protected MatchmakingMessageBuilder(IMissionSystem missionSystem, IMMMissionDTOFactory mmMissionDtoFactory, IMatchmakingSystem matchmakingSystem, IMMEntityDTOFactory mmEntityDtoFactory, IGameRoomManager gameRoomManager, IServerPresenceNotifier serverPresenceNotifier, IMatchmakingConfigProvider matchmakingConfigProvider, IRegionsService regionsService, IGameModesSystem gameModesSystem)
		{
			this.m_missionSystem = missionSystem;
			this.m_mmMissionDTOFactory = mmMissionDtoFactory;
			this.m_matchmakingSystem = matchmakingSystem;
			this.m_mmEntityDTOFactory = mmEntityDtoFactory;
			this.m_gameRoomManager = gameRoomManager;
			this.m_serverPresenceNotifier = serverPresenceNotifier;
			this.m_matchmakingConfigProvider = matchmakingConfigProvider;
			this.m_regionsService = regionsService;
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06001BE7 RID: 7143
		protected abstract GameRoomType RoomType { get; }

		// Token: 0x06001BE8 RID: 7144 RVA: 0x00070F8C File Offset: 0x0006F38C
		public IMessage<GlobalMatchmakingData> BuildMessage()
		{
			float load = this.m_serverPresenceNotifier.LoadStats.Load;
			GlobalMatchmakingData body = new GlobalMatchmakingData
			{
				ReplyTo = Resources.MMQueuesNames.ReceivingRoomUpdatesQueueName,
				Load = (double)load,
				Entities = this.GetMMEntities(),
				Rooms = this.GetMMRooms(load),
				Missions = this.GetMMMissions(),
				RegionsDistances = this.GetMMRegionDistances()
			};
			return new Message<GlobalMatchmakingData>(body)
			{
				Properties = 
				{
					DeliveryMode = DeliveryMode.NonPersistent,
					Expiration = this.m_matchmakingConfigProvider.Get().QueueInterval.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)
				}
			};
		}

		// Token: 0x06001BE9 RID: 7145 RVA: 0x00071040 File Offset: 0x0006F440
		protected virtual bool RoomSuits(IGameRoom room)
		{
			MissionContext ctx = null;
			bool started = false;
			DateTime sessionStartTime = DateTime.MinValue;
			bool allowManualJoin = true;
			room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				SessionExtension extension = room.GetExtension<SessionExtension>();
				started = extension.Started;
				if (started)
				{
					sessionStartTime = extension.SessionStartTime;
				}
				ctx = r.GetState<MissionState>(AccessMode.ReadOnly).Mission;
				allowManualJoin = r.GetState<CoreState>(AccessMode.ReadOnly).AllowManualJoin;
			});
			GameModeSetting gameModeSetting = this.m_gameModesSystem.GetGameModeSetting(ctx);
			int num;
			gameModeSetting.GetSetting(room.Type, ERoomSetting.RESTRICT_MM_AFTER_START_SEC, out num);
			DateTime utcNow = DateTime.UtcNow;
			bool flag = utcNow >= sessionStartTime && utcNow < sessionStartTime.Add(TimeSpan.FromSeconds((double)num));
			return allowManualJoin && (!started || flag);
		}

		// Token: 0x06001BEA RID: 7146
		protected abstract bool EntitySuits(MMEntityInfo entity);

		// Token: 0x06001BEB RID: 7147
		protected abstract bool MissionSuits(MissionContextBase mission);

		// Token: 0x06001BEC RID: 7148 RVA: 0x0007110C File Offset: 0x0006F50C
		private IEnumerable<MMMissionDTO> GetMMMissions()
		{
			return from m in this.m_missionSystem.GetMatchmakingMissions().Where(new Func<MissionContextBase, bool>(this.MissionSuits))
			select this.m_mmMissionDTOFactory.Create(m, this.RoomType);
		}

		// Token: 0x06001BED RID: 7149 RVA: 0x0007113C File Offset: 0x0006F53C
		private IEnumerable<MMEntityDTO> GetMMEntities()
		{
			return from e in this.m_matchmakingSystem.GetQueue().Values.SelectMany((MMEntityPool pool) => pool.GetEntities()).Where(new Func<MMEntityInfo, bool>(this.EntitySuits))
			select this.m_mmEntityDTOFactory.Create(e);
		}

		// Token: 0x06001BEE RID: 7150 RVA: 0x000711A0 File Offset: 0x0006F5A0
		private IEnumerable<MMRoomDTO> GetMMRooms(float load)
		{
			if (load >= ServerPresenceNotifier.MAX_LOAD)
			{
				return Enumerable.Empty<MMRoomDTO>();
			}
			List<MMRoomDTO> list = new List<MMRoomDTO>();
			List<IGameRoom> rooms = this.m_gameRoomManager.GetRooms(new Predicate<IGameRoom>(this.RoomSuits));
			foreach (IGameRoom room in rooms)
			{
				try
				{
					MMRoomDTO item = new MMRoomDTO(room);
					list.Add(item);
				}
				catch (RoomClosedException)
				{
				}
			}
			return list;
		}

		// Token: 0x06001BEF RID: 7151 RVA: 0x00071248 File Offset: 0x0006F648
		private IEnumerable<MMRegionDistanceDTO> GetMMRegionDistances()
		{
			return from d in this.m_regionsService.RegionsDistances
			select new MMRegionDistanceDTO(d);
		}

		// Token: 0x04000D5A RID: 3418
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000D5B RID: 3419
		private readonly IMMMissionDTOFactory m_mmMissionDTOFactory;

		// Token: 0x04000D5C RID: 3420
		private readonly IMatchmakingSystem m_matchmakingSystem;

		// Token: 0x04000D5D RID: 3421
		private readonly IMMEntityDTOFactory m_mmEntityDTOFactory;

		// Token: 0x04000D5E RID: 3422
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000D5F RID: 3423
		private readonly IServerPresenceNotifier m_serverPresenceNotifier;

		// Token: 0x04000D60 RID: 3424
		private readonly IMatchmakingConfigProvider m_matchmakingConfigProvider;

		// Token: 0x04000D61 RID: 3425
		private readonly IRegionsService m_regionsService;

		// Token: 0x04000D62 RID: 3426
		private readonly IGameModesSystem m_gameModesSystem;
	}
}
