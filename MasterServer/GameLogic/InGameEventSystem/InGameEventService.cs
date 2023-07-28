using System;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.InGameEventSystem.Exceptions;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.InGameEventSystem
{
	// Token: 0x0200030D RID: 781
	[Service]
	[Singleton]
	internal class InGameEventService : ServiceModule, IInGameEventsService
	{
		// Token: 0x060011F9 RID: 4601 RVA: 0x00047010 File Offset: 0x00045410
		public InGameEventService(ISessionStorage sessionStorage, IGameRoomManager gameRoomManager, IDALService dalService, ILogService logService)
		{
			this.m_sessionStorage = sessionStorage;
			this.m_gameRoomManager = gameRoomManager;
			this.m_dalService = dalService;
			this.m_logService = logService;
		}

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x060011FA RID: 4602 RVA: 0x00047038 File Offset: 0x00045438
		// (remove) Token: 0x060011FB RID: 4603 RVA: 0x00047070 File Offset: 0x00045470
		public event OnInGameRewardDelegate OnInGameReward;

		// Token: 0x060011FC RID: 4604 RVA: 0x000470A8 File Offset: 0x000454A8
		public void FireInGameEvent(string serverJid, string eventName, string sessionId, ulong profileId, InGameEventData data)
		{
			if (!this.m_sessionStorage.ValidateSession(serverJid, sessionId))
			{
				throw new InGameEventServiceException(string.Format("Ignoring in-game event '{0}' from server '{1}' which has incorrect session id '{2}' for profile '{3}'", new object[]
				{
					eventName,
					serverJid,
					sessionId,
					profileId
				}));
			}
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profileId);
			if (roomByPlayer == null)
			{
				throw new InGameEventServiceException(string.Format("Ignoring in-game event '{0}' from server '{1}': profile '{2}' doesn't play in session '{3}'", new object[]
				{
					eventName,
					serverJid,
					profileId,
					sessionId
				}));
			}
			string roomSessionId = string.Empty;
			roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				roomSessionId = r.SessionID;
			});
			if (roomSessionId != sessionId)
			{
				throw new InGameEventServiceException(string.Format("Ignoring in-game event '{0}' from server '{1}': profile '{2}' doesn't play in session '{3}'", new object[]
				{
					eventName,
					serverJid,
					profileId,
					sessionId
				}));
			}
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			if (profileInfo.Id == 0UL)
			{
				throw new InGameEventServiceException(string.Format("Ignoring in-game event '{0}' from server '{1}' for nonexistent profile '{2}' in session '{3}'", new object[]
				{
					eventName,
					serverJid,
					profileId,
					sessionId
				}));
			}
			if (eventName == "in_game_reward")
			{
				this.m_logService.Event.InGameEventReward(eventName, sessionId, profileId, data["reward_set"]);
				if (this.OnInGameReward != null)
				{
					this.OnInGameReward(sessionId, data["mission_type"], profileInfo, data["reward_set"]);
				}
			}
			else
			{
				Log.Warning<string>("Event '{0}' isn't supported yet", eventName);
			}
		}

		// Token: 0x04000808 RID: 2056
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000809 RID: 2057
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x0400080A RID: 2058
		private readonly IDALService m_dalService;

		// Token: 0x0400080B RID: 2059
		private readonly ILogService m_logService;
	}
}
