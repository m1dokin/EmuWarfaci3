using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.CryOnlineNET;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using Newtonsoft.Json;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x02000499 RID: 1177
	[Service]
	internal class ManualRoomService : IManualRoomService
	{
		// Token: 0x06001900 RID: 6400 RVA: 0x00065688 File Offset: 0x00063A88
		public ManualRoomService(IGameRoomActivator roomActivator, IOnlineClient onlineClient, IQueryManager queryManager, IGameRoomOfferService gameRoomOfferService, IGameRoomManager gameRoomManager, IDALService dalService, IMissionSystem missionSystem, IConfigProvider<ManualRoomConfig> configProvider)
		{
			this.m_roomActivator = roomActivator;
			this.m_onlineClient = onlineClient;
			this.m_queryManager = queryManager;
			this.m_gameRoomOfferService = gameRoomOfferService;
			this.m_gameRoomManager = gameRoomManager;
			this.m_dalService = dalService;
			this.m_missionSystem = missionSystem;
			this.m_configProvider = configProvider;
		}

		// Token: 0x06001901 RID: 6401 RVA: 0x000656D8 File Offset: 0x00063AD8
		public string CreateRoom(string masterId, RoomReference roomRef, CreateRoomParam param)
		{
			param.Mission = this.FindMission(param);
			this.CheckCreateRoomParams(param);
			return this.Execute(masterId, delegate()
			{
				IGameRoom gameRoom = this.m_roomActivator.OpenRoomByRoomRef(roomRef, param);
				return string.Format("Room with id = {0} is created", gameRoom.ID);
			}, (string serverJid) => this.m_queryManager.RequestAsync("ms_gameroom_create", serverJid, new object[]
			{
				roomRef,
				param
			}));
		}

		// Token: 0x06001902 RID: 6402 RVA: 0x00065744 File Offset: 0x00063B44
		public string GetRoomInfo(string masterId, RoomReference roomRef)
		{
			return this.Execute(masterId, delegate()
			{
				string roomInfo = string.Empty;
				IGameRoom roomByRoomRef = this.m_gameRoomManager.GetRoomByRoomRef(roomRef);
				if (roomByRoomRef != null)
				{
					roomByRoomRef.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
					{
						roomInfo = JsonConvert.SerializeObject(r);
					});
				}
				else
				{
					roomInfo = string.Format("Room with ref = {0} not found", roomRef.ToString());
				}
				return roomInfo;
			}, (string serverJid) => this.m_queryManager.RequestAsync("ms_gameroom_get_info", serverJid, new object[]
			{
				roomRef
			}));
		}

		// Token: 0x06001903 RID: 6403 RVA: 0x00065788 File Offset: 0x00063B88
		public string AddPlayer(string masterId, RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> playersInfos)
		{
			return this.Execute(masterId, delegate()
			{
				this.AddPlayer(roomRef, playersInfos);
			}, (string serverJid) => this.m_queryManager.RequestAsync("ms_gameroom_add_player", serverJid, new object[]
			{
				roomRef,
				playersInfos
			}));
		}

		// Token: 0x06001904 RID: 6404 RVA: 0x000657D4 File Offset: 0x00063BD4
		public string RemovePlayer(string masterId, RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> playersInfos)
		{
			return this.Execute(masterId, delegate()
			{
				this.m_gameRoomManager.RemovePlayerByRoomRef(roomRef, playersInfos);
			}, (string serverJid) => this.m_queryManager.RequestAsync("ms_gameroom_remove_player", serverJid, new object[]
			{
				roomRef,
				playersInfos
			}));
		}

		// Token: 0x06001905 RID: 6405 RVA: 0x00065820 File Offset: 0x00063C20
		public string StartSession(string masterId, RoomReference roomRef, int team1Score, int team2Score)
		{
			return this.Execute(masterId, delegate()
			{
				this.StartSession(roomRef, team1Score, team2Score);
			}, (string serverJid) => this.m_queryManager.RequestAsync("ms_gameroom_start", serverJid, new object[]
			{
				roomRef,
				team1Score,
				team2Score
			}));
		}

		// Token: 0x06001906 RID: 6406 RVA: 0x00065874 File Offset: 0x00063C74
		public string PauseSession(string masterId, RoomReference roomRef)
		{
			return this.Execute(masterId, delegate()
			{
				this.PauseSession(roomRef);
			}, (string serverJid) => this.m_queryManager.RequestAsync("ms_gameroom_pause", serverJid, new object[]
			{
				roomRef
			}));
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x000658B8 File Offset: 0x00063CB8
		public string ResumeSession(string masterId, RoomReference roomRef)
		{
			return this.Execute(masterId, delegate()
			{
				this.ResumeSession(roomRef);
			}, (string serverJid) => this.m_queryManager.RequestAsync("ms_gameroom_resume", serverJid, new object[]
			{
				roomRef
			}));
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x000658FC File Offset: 0x00063CFC
		public string StopSession(string masterId, RoomReference roomRef)
		{
			return this.Execute(masterId, delegate()
			{
				this.StopSession(roomRef);
			}, (string serverJid) => this.m_queryManager.RequestAsync("ms_gameroom_stop", serverJid, new object[]
			{
				roomRef
			}));
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x0006593E File Offset: 0x00063D3E
		private bool Activate(string masterId, Func<string> activate, ref string result)
		{
			if (Resources.ServerName.Equals(masterId, StringComparison.InvariantCultureIgnoreCase))
			{
				result = activate();
				return true;
			}
			return false;
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x0006595C File Offset: 0x00063D5C
		private bool Activate(string masterId, Action activate, ref string result)
		{
			return this.Activate(masterId, delegate()
			{
				activate();
				return "Success";
			}, ref result);
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x0006598C File Offset: 0x00063D8C
		private string Route(string masterId, Func<string, Task<object>> route)
		{
			if (!this.m_onlineClient.OnlineServers.Exists((SOnlineServer t) => t.Resource == masterId))
			{
				throw new Exception(string.Format("Unable to find server with specified master_id: {0}", masterId));
			}
			TimeSpan timeout = this.m_configProvider.Get().Timeout;
			string jid = this.m_onlineClient.GetJid("masterserver", masterId);
			Task<object> task = route(jid);
			if (!task.Wait(timeout))
			{
				throw new TimeoutException(string.Format("Room command timeout {0}", timeout));
			}
			return task.Result.ToString();
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x00065A3C File Offset: 0x00063E3C
		private string Execute(string masterId, Func<string> activate, Func<string, Task<object>> route)
		{
			string result = string.Empty;
			if (!this.Activate(masterId, activate, ref result))
			{
				result = this.Route(masterId, route);
			}
			return result;
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x00065A68 File Offset: 0x00063E68
		private string Execute(string masterId, Action activate, Func<string, Task<object>> route)
		{
			string result = string.Empty;
			if (!this.Activate(masterId, activate, ref result))
			{
				result = this.Route(masterId, route);
			}
			return result;
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x00065A94 File Offset: 0x00063E94
		private void AddPlayer(RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> playersInfos)
		{
			foreach (PlayerInfoForRoomOffer playerInfoForRoomOffer in playersInfos)
			{
				ulong num = (!playerInfoForRoomOffer.IsNicknameUsed) ? playerInfoForRoomOffer.ProfileId : this.GetProfileIdByNickname(playerInfoForRoomOffer.Nickname);
				IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(num);
				if (roomByPlayer != null)
				{
					throw new GameRoomManagerException(string.Format("Player with ProfileId: {0} exists in room with id: {1}. Please remove him manually with gi_remove_player command", num, roomByPlayer.ID));
				}
			}
			this.m_gameRoomOfferService.OfferRoomByRoomRef(roomRef, playersInfos);
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x00065B48 File Offset: 0x00063F48
		private void StartSession(RoomReference roomRef, int team1Score, int team2Score)
		{
			this.CheckRoomGameMode(roomRef);
			this.CheckScore(roomRef, team1Score, team2Score);
			this.m_gameRoomManager.StartRoomByRoomRef(roomRef, team1Score, team2Score);
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x00065B68 File Offset: 0x00063F68
		private void PauseSession(RoomReference roomRef)
		{
			IGameRoom gameRoom = this.GetGameRoom(roomRef);
			this.CheckRoomGameMode(gameRoom);
			gameRoom.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				SessionExtension extension = r.GetExtension<SessionExtension>();
				extension.PauseSession();
				AutoKickExtension extension2 = r.GetExtension<AutoKickExtension>();
				extension2.PauseSession();
			});
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x00065BA8 File Offset: 0x00063FA8
		private void ResumeSession(RoomReference roomRef)
		{
			IGameRoom gameRoom = this.GetGameRoom(roomRef);
			this.CheckRoomGameMode(gameRoom);
			gameRoom.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				SessionExtension extension = r.GetExtension<SessionExtension>();
				extension.ResumeSession();
				AutoKickExtension extension2 = r.GetExtension<AutoKickExtension>();
				extension2.ResumeSession();
			});
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x00065BE8 File Offset: 0x00063FE8
		private void StopSession(RoomReference roomRef)
		{
			IGameRoom gameRoom = this.GetGameRoom(roomRef);
			this.CheckRoomGameMode(gameRoom);
			gameRoom.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				SessionExtension extension = r.GetExtension<SessionExtension>();
				extension.StopSession();
			});
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x00065C28 File Offset: 0x00064028
		private IGameRoom GetGameRoom(RoomReference roomRef)
		{
			IGameRoom roomByRoomRef = this.m_gameRoomManager.GetRoomByRoomRef(roomRef);
			if (roomByRoomRef == null)
			{
				throw new GameRoomManagerException(string.Format("There was no room found with roomRef: {0}", roomRef.Reference));
			}
			return roomByRoomRef;
		}

		// Token: 0x06001914 RID: 6420 RVA: 0x00065C5F File Offset: 0x0006405F
		private ulong GetProfileIdByNickname(string nick)
		{
			return this.m_dalService.ProfileSystem.GetProfileIDByNickname(nick);
		}

		// Token: 0x06001915 RID: 6421 RVA: 0x00065C74 File Offset: 0x00064074
		private string FindMission(CreateRoomParam roomParam)
		{
			if (string.IsNullOrEmpty(roomParam.Mission))
			{
				throw new Exception("Mission key can't be empty");
			}
			List<MissionContext> list = this.m_missionSystem.MatchMission(roomParam.Mission);
			if (!list.Any<MissionContext>())
			{
				throw new GameRoomManagerException(string.Format("Can't find mission with key {0}", roomParam.Mission));
			}
			if (list.Count > 1)
			{
				throw new GameRoomManagerException(string.Format("Found more then 1 mission with key {0}", roomParam.Mission));
			}
			return list[0].uid;
		}

		// Token: 0x06001916 RID: 6422 RVA: 0x00065D04 File Offset: 0x00064104
		private void CheckCreateRoomParams(CreateRoomParam param)
		{
			if (param.RoundLimit <= 0)
			{
				throw new GameRoomManagerException(string.Format("Invalid round limit value {0}, it should be greater than 0", param.RoundLimit));
			}
			if (param.PreRoundTime <= 0)
			{
				throw new GameRoomManagerException(string.Format("Invalid preround time {0}, it should be greater than 0", param.PreRoundTime));
			}
		}

		// Token: 0x06001917 RID: 6423 RVA: 0x00065D64 File Offset: 0x00064164
		private void CheckScore(RoomReference roomRef, int team1Score, int team2Score)
		{
			IGameRoom gameRoom = this.GetGameRoom(roomRef);
			int roundLimit = 0;
			gameRoom.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				CustomParams state = r.GetState<CustomParams>(AccessMode.ReadOnly);
				roundLimit = state.RoundLimit;
			});
			if (team1Score >= roundLimit || team2Score >= roundLimit)
			{
				throw new GameRoomManagerException(string.Format("Can't start room with score {0}-{1}, round limit {2}. Score should be less than round limit", team1Score, team2Score, roundLimit));
			}
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x00065DD8 File Offset: 0x000641D8
		private void CheckRoomGameMode(RoomReference roomRef)
		{
			IGameRoom gameRoom = this.GetGameRoom(roomRef);
			this.CheckRoomGameMode(gameRoom);
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x00065DF4 File Offset: 0x000641F4
		private void CheckRoomGameMode(IGameRoom room)
		{
			string gameMode = string.Empty;
			room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				MissionExtension extension = r.GetExtension<MissionExtension>();
				gameMode = extension.Mission.gameMode;
			});
			if (!ManualRoomService.AllowedGameModes.Contains(gameMode))
			{
				throw new GameRoomManagerException(string.Format("Room game mode isn't supported {0}", gameMode));
			}
		}

		// Token: 0x04000BF9 RID: 3065
		internal const string SuccessResponse = "Success";

		// Token: 0x04000BFA RID: 3066
		private static readonly string[] AllowedGameModes = new string[]
		{
			"ptb"
		};

		// Token: 0x04000BFB RID: 3067
		private readonly IGameRoomActivator m_roomActivator;

		// Token: 0x04000BFC RID: 3068
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000BFD RID: 3069
		private readonly IGameRoomOfferService m_gameRoomOfferService;

		// Token: 0x04000BFE RID: 3070
		private readonly IDALService m_dalService;

		// Token: 0x04000BFF RID: 3071
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000C00 RID: 3072
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04000C01 RID: 3073
		private readonly IQueryManager m_queryManager;

		// Token: 0x04000C02 RID: 3074
		private readonly IConfigProvider<ManualRoomConfig> m_configProvider;
	}
}
