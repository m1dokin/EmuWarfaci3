using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.ServerInfo;
using MasterServer.Users;

namespace MasterServer.DebugQueries
{
	// Token: 0x0200021C RID: 540
	[DebugQuery]
	[QueryAttributes(TagName = "debug_session_open")]
	internal class DebugSessionOpen : BaseQuery
	{
		// Token: 0x06000BB7 RID: 2999 RVA: 0x0002C574 File Offset: 0x0002A974
		public DebugSessionOpen(ISessionStorage sessionStorage, IGameRoomManager gameRoomManager, IMissionSystem missionSystem, IGameRoomActivator roomActivator)
		{
			this.m_sessionStorage = sessionStorage;
			this.m_gameRoomManager = gameRoomManager;
			this.m_missionSystem = missionSystem;
			this.m_gameRoomActivator = roomActivator;
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x0002C59C File Offset: 0x0002A99C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "DebugSessionOpen"))
			{
				UserInfo.User userInfo;
				if (!base.GetClientInfo(fromJid, out userInfo))
				{
					result = -3;
				}
				else
				{
					string serverName = request.GetAttribute("serverName");
					string sessionId = request.GetAttribute("sessionName");
					string attribute = request.GetAttribute("mode");
					base.ServerRepository.AddServer(userInfo.OnlineID, serverName);
					bool flag = Resources.ChannelTypes.IsPvE(Resources.Channel);
					List<MissionContextBase> matchmakingMissions = this.m_missionSystem.GetMatchmakingMissions();
					MissionContextBase mission = (!flag) ? this.GetPvpMission(matchmakingMissions, attribute) : this.GetPveMission(matchmakingMissions);
					IGameRoom gameRoom;
					if ((gameRoom = this.m_gameRoomManager.GetRoomByPlayer(userInfo.ProfileID)) == null)
					{
						gameRoom = this.m_gameRoomActivator.OpenRoom((!flag) ? GameRoomType.PvP_Public : GameRoomType.PvE_Private, delegate(IGameRoom r)
						{
							r.RoomName = "TestRoom";
							MissionExtension extension = r.GetExtension<MissionExtension>();
							extension.SetMission(mission.uid);
							CustomParams state = r.GetState<CustomParams>(AccessMode.ReadWrite);
							state.Autobalance = false;
							SessionState state2 = r.GetState<SessionState>(AccessMode.ReadWrite);
							state2.SessionStartTime = DateTime.UtcNow;
							r.AddPlayer(userInfo.ProfileID, 0, 1, RoomPlayer.EStatus.Ready, GameRoomPlayerAddReason.RoomBrowser);
							RoomPlayer player = r.GetPlayer(userInfo.ProfileID);
							player.RoomStatus = RoomPlayer.EStatus.Ready;
						});
					}
					IGameRoom gameRoom2 = gameRoom;
					gameRoom2.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						ServerExtension extension = r.GetExtension<ServerExtension>();
						extension.BindServer(new ServerEntity
						{
							SessionID = sessionId,
							ServerID = serverName
						});
					});
					if (!this.m_sessionStorage.ValidateSession(userInfo.OnlineID, sessionId))
					{
						result = -1;
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x0002C70C File Offset: 0x0002AB0C
		private MissionContextBase GetPveMission(List<MissionContextBase> missions)
		{
			return missions.Find((MissionContextBase x) => x.missionType.IsTraining());
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x0002C740 File Offset: 0x0002AB40
		private MissionContextBase GetPvpMission(List<MissionContextBase> missions, string mode)
		{
			MissionContextBase result;
			if (string.IsNullOrEmpty(mode))
			{
				result = missions.Find((MissionContextBase x) => x.channels.Contains(Resources.Channel));
			}
			else
			{
				result = missions.Find((MissionContextBase x) => x.channels.Contains(Resources.Channel) && x.gameMode == mode);
			}
			return result;
		}

		// Token: 0x04000573 RID: 1395
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000574 RID: 1396
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000575 RID: 1397
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000576 RID: 1398
		private readonly IGameRoomActivator m_gameRoomActivator;
	}
}
