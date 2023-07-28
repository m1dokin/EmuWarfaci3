using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Matchmaking.Data;
using MasterServer.Users;
using NLog;
using Util.Common;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000676 RID: 1654
	[Service]
	[Singleton]
	internal class MatchmakingSystem : ServiceModule, IMatchmakingSystem
	{
		// Token: 0x060022CD RID: 8909 RVA: 0x000910A4 File Offset: 0x0008F4A4
		public MatchmakingSystem(IMatchmakingMissionsProvider matchmakingMissionsProvider, ILogService logService, IUserRepository userRepository, IMatchmakingConfigProvider matchmakingConfigProvider, IGameRoomManager gameRoomManager, IGameRoomActivator gameRoomActivator, MasterServer.GameLogic.MissionSystem.IMissionSystem missionSystem, IGameRoomOfferService gameRoomOfferService, IUserProxyRepository userProxyRepository, IClanService clanService)
		{
			this.m_matchmakingMissionsProvider = matchmakingMissionsProvider;
			this.m_logService = logService;
			this.m_userRepository = userRepository;
			this.m_matchmakingConfigProvider = matchmakingConfigProvider;
			this.m_gameRoomManager = gameRoomManager;
			this.m_gameRoomActivator = gameRoomActivator;
			this.m_missionSystem = missionSystem;
			this.m_gameRoomOfferService = gameRoomOfferService;
			this.m_userProxyRepository = userProxyRepository;
			this.m_clanService = clanService;
		}

		// Token: 0x14000096 RID: 150
		// (add) Token: 0x060022CE RID: 8910 RVA: 0x00091184 File Offset: 0x0008F584
		// (remove) Token: 0x060022CF RID: 8911 RVA: 0x000911BC File Offset: 0x0008F5BC
		public event UnQueueEntityDeleg OnUnQueueEntity = delegate(MMEntityInfo A_0, EUnQueueReason A_1)
		{
		};

		// Token: 0x14000097 RID: 151
		// (add) Token: 0x060022D0 RID: 8912 RVA: 0x000911F4 File Offset: 0x0008F5F4
		// (remove) Token: 0x060022D1 RID: 8913 RVA: 0x0009122C File Offset: 0x0008F62C
		public event Action<IEnumerable<MMResultEntity>> OnEntitiesFailed = delegate(IEnumerable<MMResultEntity> A_0)
		{
		};

		// Token: 0x14000098 RID: 152
		// (add) Token: 0x060022D2 RID: 8914 RVA: 0x00091264 File Offset: 0x0008F664
		// (remove) Token: 0x060022D3 RID: 8915 RVA: 0x0009129C File Offset: 0x0008F69C
		public event Action<IEnumerable<MMResultEntity>, IGameRoom> OnEntitiesSucceded = delegate(IEnumerable<MMResultEntity> A_0, IGameRoom A_1)
		{
		};

		// Token: 0x060022D4 RID: 8916 RVA: 0x000912D2 File Offset: 0x0008F6D2
		public override void Start()
		{
			base.Start();
			this.m_userRepository.UserLoggedOut += this.OnUserLoggedOut;
		}

		// Token: 0x060022D5 RID: 8917 RVA: 0x000912F1 File Offset: 0x0008F6F1
		public override void Stop()
		{
			this.m_userRepository.UserLoggedOut -= this.OnUserLoggedOut;
			base.Stop();
		}

		// Token: 0x060022D6 RID: 8918 RVA: 0x00091310 File Offset: 0x0008F710
		public MatchmakingConfig GetConfig()
		{
			return this.m_matchmakingConfigProvider.Get();
		}

		// Token: 0x060022D7 RID: 8919 RVA: 0x00091320 File Offset: 0x0008F720
		public void QueueEntity(MMEntityInfo entityInfo)
		{
			MMSettings settings = entityInfo.Settings;
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				foreach (MMEntityPool mmentityPool in this.m_playersPoolsBackBuffer.Values)
				{
					foreach (MMPlayerInfo mmplayerInfo in entityInfo.Players)
					{
						MMEntityInfo entityByInitiatorId = mmentityPool.GetEntityByInitiatorId(mmplayerInfo.User.ProfileID);
						if (entityByInitiatorId != null)
						{
							mmentityPool.RemoveEntity(entityByInitiatorId.Id);
						}
					}
				}
				if (settings.RoomType.IsPvpMode())
				{
					if (settings.RoomType != GameRoomType.PvP_Rating)
					{
						IEnumerable<string> autostartMissions = this.m_matchmakingMissionsProvider.AutostartMissions;
						settings.RoomType = ((!this.m_matchmakingConfigProvider.Get().IsAutostartEnabled || !autostartMissions.Any<string>()) ? GameRoomType.PvP_Public : GameRoomType.PvP_AutoStart);
					}
				}
				else
				{
					settings.RoomType = ((!this.m_matchmakingConfigProvider.Get().IsPveAutostartEnabled) ? GameRoomType.PvE_Private : GameRoomType.PvE_AutoStart);
				}
				this.GetPlayersPoolUnsafe(settings.RoomType).AddEntity(entityInfo);
				MmRoomHelper.SendMatchmakingStartedQuery(entityInfo);
			}
			int groupSize = entityInfo.Players.Count<MMPlayerInfo>();
			foreach (MMPlayerInfo mmplayerInfo2 in entityInfo.Players)
			{
				this.m_logService.Event.MatchmakingStartedLog(mmplayerInfo2.User.UserID, mmplayerInfo2.User.ProfileID, entityInfo.Id, groupSize, settings.RoomType, ProfileProgressionInfo.ClassToClassChar[mmplayerInfo2.PlayerCurrentClass].ToString(), this.GetPlayerClanName(mmplayerInfo2.User.ProfileID));
			}
		}

		// Token: 0x060022D8 RID: 8920 RVA: 0x000915AC File Offset: 0x0008F9AC
		public void UnQueueEntity(ulong initiatorProfileId, EUnQueueReason reason)
		{
			MMEntityInfo mmentityInfo = null;
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				foreach (MMEntityPool mmentityPool in this.m_playersPoolsBackBuffer.Values)
				{
					mmentityInfo = mmentityPool.GetEntityByInitiatorId(initiatorProfileId);
					if (mmentityInfo != null)
					{
						mmentityPool.RemoveEntity(mmentityInfo.Id);
						break;
					}
				}
			}
			if (mmentityInfo != null && reason != EUnQueueReason.LogOut)
			{
				MmRoomHelper.SendMatchmakingCanceledQuery(mmentityInfo);
				this.EntityUnqueued(mmentityInfo, reason);
			}
		}

		// Token: 0x060022D9 RID: 8921 RVA: 0x00091670 File Offset: 0x0008FA70
		public void ResetMapsSettings(ulong initiatorProfileId)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				foreach (MMEntityPool mmentityPool in this.m_playersPoolsBackBuffer.Values)
				{
					MMEntityInfo entityByInitiatorId = mmentityPool.GetEntityByInitiatorId(initiatorProfileId);
					if (entityByInitiatorId != null)
					{
						entityByInitiatorId.Settings.GameMode = string.Empty;
						entityByInitiatorId.Settings.MissionId = string.Empty;
						return;
					}
				}
			}
			MatchmakingSystem.Log.Warn("Couldn't find entity for {0} profile to reset it's maps settings", initiatorProfileId);
		}

		// Token: 0x060022DA RID: 8922 RVA: 0x00091740 File Offset: 0x0008FB40
		public void OnMatchmakingResult(MatchmakingResult result)
		{
			this.UnqueueEntities(result.FailedEntities);
			this.OnEntitiesFailed(result.FailedEntities);
			MmRoomHelper.SendMatchmakingFailedQuery(result.FailedEntities);
			this.LogMatchmakingFinished(result.FailedEntities, 0UL, string.Empty);
			List<ulong> list = new List<ulong>();
			foreach (MMResultEntity mmresultEntity in result.RoomUpdates.SelectMany((MMResultRoomUpdate roomUpdate) => roomUpdate.Entities))
			{
				list.AddRange(from player in mmresultEntity.Players
				select player.ProfileId);
			}
			Dictionary<ulong, UserInfo.User> usersCache = this.m_userProxyRepository.GetUserOrProxyByProfileId(list.Distinct<ulong>(), true).ToDictionary((UserInfo.User x) => x.ProfileID, (UserInfo.User x) => x);
			foreach (MMResultRoomUpdate mmresultRoomUpdate in result.RoomUpdates)
			{
				IGameRoom room = this.GetRoom(mmresultRoomUpdate);
				if (room != null)
				{
					this.ApplyRoomUpdate(mmresultRoomUpdate, room, usersCache);
				}
				else
				{
					MatchmakingSystem.Log.Warn("[MatchmakingSystem.OnMatchmakingResult] Room update was received for closed room");
					this.OnEntitiesFailed(mmresultRoomUpdate.Entities);
				}
			}
		}

		// Token: 0x060022DB RID: 8923 RVA: 0x00091900 File Offset: 0x0008FD00
		public IEnumerable<string> GetAcceptedMissions(MMEntityInfo mmEntity)
		{
			if (!string.IsNullOrEmpty(mmEntity.Settings.MissionId))
			{
				return new List<string>
				{
					mmEntity.Settings.MissionId
				};
			}
			string gameMode = mmEntity.Settings.GameMode;
			IEnumerable<string> enumerable = (!mmEntity.Settings.RoomType.IsPvpRatingMode()) ? this.m_matchmakingMissionsProvider.AutostartMissions : this.m_matchmakingMissionsProvider.RatingGameMissions;
			if (string.IsNullOrEmpty(gameMode))
			{
				return enumerable;
			}
			return from x in enumerable
			where this.m_missionSystem.GetMission(x).gameMode == gameMode
			select x;
		}

		// Token: 0x060022DC RID: 8924 RVA: 0x000919B0 File Offset: 0x0008FDB0
		private void LogMatchmakingFinished(IEnumerable<MMResultEntity> entities, ulong roomId, string mapName)
		{
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				foreach (MMResultEntity mmresultEntity in entities)
				{
					foreach (MMResultPlayerInfo mmresultPlayerInfo in mmresultEntity.Players)
					{
						logGroup.MatchmakingFinishedLog(mmresultPlayerInfo.UserId, mmresultPlayerInfo.ProfileId, mmresultEntity.EntityId, mmresultEntity.Players.Count<MMResultPlayerInfo>(), mmresultEntity.RoomType, DateTime.UtcNow - TimeUtils.UTCTimestampToUTCTime(mmresultEntity.StartTime), roomId, this.GetPlayerClanName(mmresultPlayerInfo.ProfileId), mapName);
					}
				}
			}
		}

		// Token: 0x060022DD RID: 8925 RVA: 0x00091AB8 File Offset: 0x0008FEB8
		private IGameRoom GetRoom(MMResultRoomUpdate roomUpdate)
		{
			if (roomUpdate.RoomId == 0UL)
			{
				try
				{
					return this.m_gameRoomActivator.OpenRoom(roomUpdate.RoomType, delegate(IGameRoom r)
					{
						r.RoomName = roomUpdate.RoomName;
						MissionExtension extension = r.GetExtension<MissionExtension>();
						if (extension.SetMission(roomUpdate.MissionId) != GameRoomRetCode.OK)
						{
							MatchmakingSystem.Log.Warn("Room open (matchmaking) failed mission uid {0}", roomUpdate.MissionId);
							throw new DiscardRoomException();
						}
					});
				}
				catch (Exception exception)
				{
					MatchmakingSystem.Log.Error(exception, "[MatchmakingSystem.GetRoom] Failed to open game room on matchmaking", new object[0]);
				}
			}
			return this.m_gameRoomManager.GetRoom(roomUpdate.RoomId);
		}

		// Token: 0x060022DE RID: 8926 RVA: 0x00091B50 File Offset: 0x0008FF50
		private bool JoinToRoom(string initiatorNickname, UserInfo.User[] users, string token, IGameRoom room)
		{
			List<UserInfo.User> list = (from x in users
			where x.IsOnline
			select x).ToList<UserInfo.User>();
			if (users.Length > list.Count)
			{
				string argument = string.Join<ulong>(",", from x in users
				where !x.IsOnline
				select x.ProfileID);
				MatchmakingSystem.Log.Info<string, ulong>("[MmRoomHelper] There are offline users: {0} while joining to room {1}", argument, room.ID);
				if (!list.Any<UserInfo.User>())
				{
					return false;
				}
			}
			return this.m_gameRoomOfferService.OfferRoom(initiatorNickname, list, token, room);
		}

		// Token: 0x060022DF RID: 8927 RVA: 0x00091C18 File Offset: 0x00090018
		private void ApplyRoomUpdate(MMResultRoomUpdate update, IGameRoom room, Dictionary<ulong, UserInfo.User> usersCache)
		{
			List<MMResultEntity> list = new List<MMResultEntity>();
			foreach (MMResultEntity mmresultEntity in update.Entities)
			{
				try
				{
					UserInfo.User[] users = (from player in mmresultEntity.Players
					select usersCache[player.ProfileId]).ToArray<UserInfo.User>();
					if (this.JoinToRoom(mmresultEntity.Initiator, users, mmresultEntity.EntityId, room))
					{
						list.Add(mmresultEntity);
						this.UnqueueEntity(mmresultEntity.EntityId);
					}
				}
				catch (RoomClosedException exception)
				{
					MatchmakingSystem.Log.Error(exception, "[MatchmakingSystem.ApplyRoomUpdate] Joining entity to room failed\nentity: {0}\nroom_update: {1}", new object[]
					{
						mmresultEntity,
						update
					});
					return;
				}
				catch (Exception exception2)
				{
					MatchmakingSystem.Log.Error(exception2, "[MatchmakingSystem.ApplyRoomUpdate] Joining entity to room failed\nentity: {0}\nroom_update: {1}", new object[]
					{
						mmresultEntity,
						update
					});
				}
			}
			int playerCount = 0;
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				if (r.IsEmpty)
				{
					MatchmakingSystem.Log.Warn<ulong, MMResultRoomUpdate>("[MatchmakingSystem.ApplyRoomUpdate] No one added to room {0}. Room update: {1}", r.ID, update);
					return;
				}
				r.MMGeneration += 1UL;
				playerCount = r.PlayerCount;
			});
			this.OnEntitiesSucceded(list, room);
			this.OnEntitiesFailed(update.Entities.Except(list).ToArray<MMResultEntity>());
			MatchmakingSystem.Log.Debug<ulong, int, int>("JoinPlayersToRooms added - room: {0}; joined: {1}; total: {2}", update.RoomId, list.SelectMany((MMResultEntity e) => e.Players).Count<MMResultPlayerInfo>(), playerCount);
			MmRoomHelper.SendMatchmakingSuccessQuery(list);
			string missionName = string.Empty;
			room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				MissionContext mission = r.GetExtension<MissionExtension>().Mission;
				missionName = (mission.name ?? string.Empty);
			});
			this.LogMatchmakingFinished(list, room.ID, missionName);
		}

		// Token: 0x060022E0 RID: 8928 RVA: 0x00091E14 File Offset: 0x00090214
		public IDictionary<GameRoomType, MMEntityPool> GetQueue()
		{
			object thisLock = this.m_thisLock;
			IDictionary<GameRoomType, MMEntityPool> result;
			lock (thisLock)
			{
				result = this.m_playersPoolsBackBuffer.ToDictionary((KeyValuePair<GameRoomType, MMEntityPool> kvp) => kvp.Key, (KeyValuePair<GameRoomType, MMEntityPool> kvp) => kvp.Value.Clone());
			}
			return result;
		}

		// Token: 0x060022E1 RID: 8929 RVA: 0x00091E98 File Offset: 0x00090298
		private void EntityUnqueued(MMEntityInfo removedEntity, EUnQueueReason reason)
		{
			this.OnUnQueueEntity(removedEntity, reason);
			int groupSize = removedEntity.Players.Count<MMPlayerInfo>();
			foreach (MMPlayerInfo mmplayerInfo in removedEntity.Players)
			{
				this.m_logService.Event.MatchmakingAbortedLog(mmplayerInfo.User.UserID, mmplayerInfo.User.ProfileID, removedEntity.Id, groupSize, removedEntity.Settings.RoomType, DateTime.UtcNow - removedEntity.Settings.StartTimeUtc, this.GetPlayerClanName(mmplayerInfo.User.ProfileID));
			}
		}

		// Token: 0x060022E2 RID: 8930 RVA: 0x00091F64 File Offset: 0x00090364
		private MMEntityPool GetPlayersPoolUnsafe(GameRoomType roomType)
		{
			MMEntityPool mmentityPool;
			if (!this.m_playersPoolsBackBuffer.TryGetValue(roomType, out mmentityPool))
			{
				mmentityPool = new MMEntityPool(this);
				if (roomType.IsAutoStartMode())
				{
					if (roomType.IsPvpRatingMode())
					{
						List<string> missionList = this.m_matchmakingMissionsProvider.RatingGameMissions.ToList<string>();
						mmentityPool.SetMissionList(missionList);
					}
					else if (roomType.IsPvpAutoStartMode())
					{
						List<string> missionList2 = this.m_matchmakingMissionsProvider.AutostartMissions.ToList<string>();
						mmentityPool.SetMissionList(missionList2);
					}
				}
				else
				{
					List<string> missionList3 = this.m_matchmakingMissionsProvider.PvpMissions.ToList<string>();
					mmentityPool.SetMissionList(missionList3);
				}
				this.m_playersPoolsBackBuffer.Add(roomType, mmentityPool);
			}
			return mmentityPool;
		}

		// Token: 0x060022E3 RID: 8931 RVA: 0x0009200C File Offset: 0x0009040C
		private void UnqueueEntity(string entityId)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				foreach (MMEntityPool mmentityPool in this.m_playersPoolsBackBuffer.Values)
				{
					mmentityPool.RemoveEntity(entityId);
				}
			}
		}

		// Token: 0x060022E4 RID: 8932 RVA: 0x0009209C File Offset: 0x0009049C
		private void UnqueueEntities(IEnumerable<MMResultEntity> entities)
		{
			object thisLock = this.m_thisLock;
			lock (thisLock)
			{
				foreach (MMResultEntity mmresultEntity in entities)
				{
					foreach (MMEntityPool mmentityPool in this.m_playersPoolsBackBuffer.Values)
					{
						mmentityPool.RemoveEntity(mmresultEntity.EntityId);
					}
				}
			}
		}

		// Token: 0x060022E5 RID: 8933 RVA: 0x00092170 File Offset: 0x00090570
		private void OnUserLoggedOut(UserInfo.User user, ELogoutType logoutType)
		{
			this.UnQueueEntity(user.ProfileID, EUnQueueReason.LogOut);
		}

		// Token: 0x060022E6 RID: 8934 RVA: 0x00092180 File Offset: 0x00090580
		private string GetPlayerClanName(ulong profileId)
		{
			ClanInfo clanInfoByPid = this.m_clanService.GetClanInfoByPid(profileId);
			return (clanInfoByPid != null) ? clanInfoByPid.Name : string.Empty;
		}

		// Token: 0x04001170 RID: 4464
		private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

		// Token: 0x04001174 RID: 4468
		private readonly ILogService m_logService;

		// Token: 0x04001175 RID: 4469
		private readonly IUserRepository m_userRepository;

		// Token: 0x04001176 RID: 4470
		private readonly IMatchmakingConfigProvider m_matchmakingConfigProvider;

		// Token: 0x04001177 RID: 4471
		private readonly IMatchmakingMissionsProvider m_matchmakingMissionsProvider;

		// Token: 0x04001178 RID: 4472
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04001179 RID: 4473
		private readonly IGameRoomActivator m_gameRoomActivator;

		// Token: 0x0400117A RID: 4474
		private readonly MasterServer.GameLogic.MissionSystem.IMissionSystem m_missionSystem;

		// Token: 0x0400117B RID: 4475
		private readonly IGameRoomOfferService m_gameRoomOfferService;

		// Token: 0x0400117C RID: 4476
		private readonly IUserProxyRepository m_userProxyRepository;

		// Token: 0x0400117D RID: 4477
		private readonly IClanService m_clanService;

		// Token: 0x0400117E RID: 4478
		private readonly Dictionary<GameRoomType, MMEntityPool> m_playersPoolsBackBuffer = new Dictionary<GameRoomType, MMEntityPool>();

		// Token: 0x0400117F RID: 4479
		private readonly object m_thisLock = new object();
	}
}
