using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Matchmaking.Queries;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200081F RID: 2079
	[QueryAttributes(TagName = "gameroom_open")]
	internal class OpenRoomQuery : BaseQuery
	{
		// Token: 0x06002AD1 RID: 10961 RVA: 0x000B8FD3 File Offset: 0x000B73D3
		public OpenRoomQuery(IGameRoomActivator roomActivator, IDALService dalService, IRankSystem rankSystem, IProfanityCheckService profanityCheckService, MasterServer.GameLogic.MissionSystem.IMissionSystem missionSystem)
		{
			this.m_roomActivator = roomActivator;
			this.m_dalService = dalService;
			this.m_rankSystem = rankSystem;
			this.m_profanityCheckService = profanityCheckService;
			this.m_missionSystem = missionSystem;
		}

		// Token: 0x06002AD2 RID: 10962 RVA: 0x000B9000 File Offset: 0x000B7400
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "OpenRoomQuery"))
			{
				UserInfo.User user_info;
				if (!base.GetClientInfo(fromJid, out user_info))
				{
					result = -3;
				}
				else
				{
					GameRoomType gameRoomType = (GameRoomType)int.Parse(request.GetAttribute("room_type"));
					int teamId = int.Parse(request.GetAttribute("team_id"));
					RoomPlayer.EStatus status = (RoomPlayer.EStatus)int.Parse(request.GetAttribute("status"));
					int classId = int.Parse(request.GetAttribute("class_id"));
					string groupId = request.GetAttribute("group_id");
					bool isPrivate = int.Parse(request.GetAttribute("private")) != 0;
					string roomName = request.GetAttribute("room_name");
					string mission_key = request.GetAttribute("mission");
					if (string.IsNullOrEmpty(mission_key))
					{
						Log.Warning<ulong>("'{0}' can't open game room with empty mission key.", user_info.ProfileID);
						result = -1;
					}
					else
					{
						SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(user_info.ProfileID);
						if (profileInfo.UserID != user_info.UserID)
						{
							Log.Warning<ulong>("Can't find ProfileInfo '{0}' for opening room.", user_info.ProfileID);
							result = -1;
						}
						else if (!this.m_rankSystem.CanJoinChannel(profileInfo.RankInfo.RankId))
						{
							Log.Warning<ulong, int>("Failed to open room by user '{0}' with incorrect rank {1}", profileInfo.UserID, profileInfo.RankInfo.RankId);
							result = 11;
						}
						else if (!QuickPlayQuery.IsGameTypeValidForCurrentChannel(gameRoomType))
						{
							Log.Warning<GameRoomType>("Room type '{0}' is invalid for current channel for room opening.", gameRoomType);
							result = -1;
						}
						else if (gameRoomType.IsPvpAutoStartMode())
						{
							Log.Warning<string>("Autostart room can't be opened with OpenRoomQuery, jid {0}", fromJid);
							result = -1;
						}
						else
						{
							ProfanityCheckResult profanityCheckResult = this.m_profanityCheckService.CheckRoomName(user_info.UserID, user_info.Nickname, roomName);
							if (profanityCheckResult != ProfanityCheckResult.Reserved)
							{
								if (profanityCheckResult != ProfanityCheckResult.Failed)
								{
									if (request.HasAttribute("mission_data"))
									{
										if (!Resources.DebugQueriesEnabled)
										{
											Log.Error<string>("Open room from mission data is disabled, jid {0}", fromJid);
											return -1;
										}
										string attribute = request.GetAttribute("mission_data");
										this.m_missionSystem.AddMission(mission_key, attribute);
									}
									GameRoomRetCode code = GameRoomRetCode.OK;
									IGameRoom gameRoom = this.m_roomActivator.OpenRoom(gameRoomType, delegate(IGameRoom r)
									{
										r.RoomName = roomName;
										r.Private = isPrivate;
										if (!string.IsNullOrEmpty(mission_key))
										{
											MissionExtension extension = r.GetExtension<MissionExtension>();
											code = extension.SetMission(mission_key);
											if (code == GameRoomRetCode.OK && r.IsPveMode() && !user_info.ProfileProgression.IsMissionTypeUnlocked(extension.MissionType.Name))
											{
												code = GameRoomRetCode.MISSION_RESTRICTED;
											}
											if (code == GameRoomRetCode.OK)
											{
												if (r.IsPvpMode())
												{
													CustomParamsExtension extension2 = r.GetExtension<CustomParamsExtension>();
													extension2.CheckAndSetRestrictions(request);
												}
												code = r.AddPlayer(user_info.ProfileID, groupId, teamId, classId, TimeSpan.Zero, status, GameRoomPlayerAddReason.RoomBrowser);
											}
										}
										if (code != GameRoomRetCode.OK)
										{
											Log.Warning<string>("Failed to create room with mission '{0}', discarding", mission_key);
											throw new DiscardRoomException();
										}
									});
									if (code != GameRoomRetCode.OK)
									{
										result = (int)code;
									}
									else
									{
										XmlElement newChild = gameRoom.FullStateSnapshot(RoomUpdate.Target.Client, response.OwnerDocument, user_info.ProfileID);
										response.AppendChild(newChild);
										result = 0;
									}
								}
								else
								{
									Log.Error<ulong>("Failed to open room by user {0}: room name invalid", user_info.ProfileID);
									result = 21;
								}
							}
							else
							{
								Log.Error<ulong>("Failed to open room by user {0}: room name reserved", user_info.ProfileID);
								result = 22;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x040016D4 RID: 5844
		private readonly IGameRoomActivator m_roomActivator;

		// Token: 0x040016D5 RID: 5845
		private readonly IDALService m_dalService;

		// Token: 0x040016D6 RID: 5846
		private readonly IRankSystem m_rankSystem;

		// Token: 0x040016D7 RID: 5847
		private readonly IProfanityCheckService m_profanityCheckService;

		// Token: 0x040016D8 RID: 5848
		private readonly MasterServer.GameLogic.MissionSystem.IMissionSystem m_missionSystem;
	}
}
