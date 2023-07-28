using System;
using System.Collections;
using System.Linq;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000794 RID: 1940
	[QueryAttributes(TagName = "set_rewards_info")]
	internal class SetRewardsInfoQuery : BaseQuery
	{
		// Token: 0x0600283D RID: 10301 RVA: 0x000ACEBA File Offset: 0x000AB2BA
		public SetRewardsInfoQuery(ISessionStorage sessionStorage, IGameRoomManager roomManager, IRewardService rewardService, IMissionSystem missionSystem)
		{
			this.m_sessionStorage = sessionStorage;
			this.m_roomManager = roomManager;
			this.m_rewardService = rewardService;
			this.m_missionSystem = missionSystem;
		}

		// Token: 0x0600283E RID: 10302 RVA: 0x000ACEE0 File Offset: 0x000AB2E0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetRewardsInfoQuery"))
			{
				string attribute = request.GetAttribute("session_id");
				if (!this.ValidateSession(fromJid, attribute))
				{
					Log.Warning<string, string>("Ignoring rewards info from server {0} which has incorrect session id {1}", fromJid, attribute);
					result = -1;
				}
				else
				{
					RewardInputData rewardInputData = this.ParseRewardInputs(attribute, request);
					MissionContext mission = this.m_missionSystem.GetMission(rewardInputData.missionId);
					if (mission == null)
					{
						Log.Warning("Query tutorial_result : Unexpected : GetMission return null");
						result = -1;
					}
					else
					{
						this.m_rewardService.GiveRewards(attribute, mission, rewardInputData);
						this.m_sessionStorage.RemoveData(attribute, ESessionData.ProfilePerformanceInfo);
						this.m_sessionStorage.RemoveData(attribute, ESessionData.PlayTime);
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x0600283F RID: 10303 RVA: 0x000ACFA8 File Offset: 0x000AB3A8
		private RewardInputData ParseRewardInputs(string sessionId, XmlElement request)
		{
			RewardInputData rewardInputData = new RewardInputData();
			rewardInputData.missionId = request.GetAttribute("mission_id");
			rewardInputData.difficulty = byte.Parse(request.GetAttribute("difficulty"));
			rewardInputData.roomType = this.ParseRoomType(request.GetAttribute("room_type"));
			rewardInputData.incompleteSession = (byte.Parse(request.GetAttribute("incomplete_session")) != 0);
			rewardInputData.sessionTime = float.Parse(request.GetAttribute("session_time"));
			rewardInputData.sessionKillCount = uint.Parse(request.GetAttribute("session_kill_count"));
			rewardInputData.passedSubLevelsCount = uint.Parse(request.GetAttribute("passed_sublevels_count"));
			rewardInputData.passedCheckpointsCount = uint.Parse(request.GetAttribute("passed_checkpoints_count"));
			rewardInputData.winnerTeamId = int.Parse(request.GetAttribute("winning_team_id"));
			rewardInputData.maxSessionScore = int.Parse(request.GetAttribute("max_session_score"));
			rewardInputData.secondaryObjectivesCompleted = byte.Parse(request.GetAttribute("secondary_objectives_completed"));
			MissionContext mission = this.m_missionSystem.GetMission(rewardInputData.missionId);
			if (mission != null)
			{
				byte b = (byte)mission.objectives.Count((MissionObjective objective) => objective.type == "secondary");
				if (rewardInputData.secondaryObjectivesCompleted > b)
				{
					Log.Warning<byte, string, byte>("Completed {0} secondary objectives, but mission {1} only have {2}", rewardInputData.secondaryObjectivesCompleted, rewardInputData.missionId, b);
					rewardInputData.secondaryObjectivesCompleted = Math.Min(rewardInputData.secondaryObjectivesCompleted, b);
				}
			}
			SessionBoosters data = this.m_sessionStorage.GetData<SessionBoosters>(sessionId, ESessionData.Boosters);
			if (data == null)
			{
				Log.Warning<string>("Can't find boosters info for session {0}", sessionId);
			}
			SessionRewardMultiplier data2 = this.m_sessionStorage.GetData<SessionRewardMultiplier>(sessionId, ESessionData.RewardMultiplier);
			if (data2 == null)
			{
				Log.Warning<string>("Can't find reward multiplier info for session {0}", sessionId);
			}
			IEnumerator enumerator = request.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = xmlNode as XmlElement;
						if (xmlElement.Name == "team")
						{
							byte b2 = byte.Parse(xmlElement.GetAttribute("id"));
							if (!rewardInputData.teams.ContainsKey(b2))
							{
								rewardInputData.teams.Add(b2, new RewardInputData.Team());
							}
							IEnumerator enumerator2 = xmlElement.ChildNodes.GetEnumerator();
							try
							{
								while (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									XmlNode xmlNode2 = (XmlNode)obj2;
									if (xmlNode2.NodeType == XmlNodeType.Element)
									{
										XmlElement xmlElement2 = xmlNode2 as XmlElement;
										if (xmlElement2.Name == "profile")
										{
											RewardInputData.Team.Player player = new RewardInputData.Team.Player();
											player.score = int.Parse(xmlElement2.GetAttribute("score"));
											player.profileId = ulong.Parse(xmlElement2.GetAttribute("profile_id"));
											player.teamId = b2;
											player.inSessionFromStart = (byte.Parse(xmlElement2.GetAttribute("in_session_from_start")) != 0);
											player.sessionTime = TimeSpan.FromSeconds((double)float.Parse(xmlElement2.GetAttribute("player_session_time")));
											player.firstCheckpoint = uint.Parse(xmlElement2.GetAttribute("first_checkpoint"));
											player.lastCheckpoint = uint.Parse(xmlElement2.GetAttribute("last_checkpoint"));
											player.groupId = xmlElement2.GetAttribute("group_id");
											if (data != null)
											{
												SessionBoosters.ProfileBoosters profileBoosters;
												if (data.Boosters.TryGetValue(player.profileId, out profileBoosters))
												{
													player.isVip = profileBoosters.IsVip;
													player.xp_boost = profileBoosters.Boosters[BoosterType.XPBooster];
													player.vp_boost = profileBoosters.Boosters[BoosterType.VPBooster];
													player.gm_boost = profileBoosters.Boosters[BoosterType.GMBooster];
												}
												else
												{
													Log.Warning<ulong>("Can't find boosters info for profile {0}", player.profileId);
												}
											}
											if (data2 != null)
											{
												SessionRewardMultiplier.ProfileRewardMultiplier profileRewardMultiplier;
												if (data2.Multiplier.TryGetValue(player.profileId, out profileRewardMultiplier))
												{
													player.dynamicMultiplier = profileRewardMultiplier.Multiplier;
												}
												else
												{
													Log.Warning<ulong>("Can't find dynamic reward multiplier info for profile {0}", player.profileId);
												}
											}
											rewardInputData.teams[b2].playerScores.Add(player);
										}
									}
								}
							}
							finally
							{
								IDisposable disposable;
								if ((disposable = (enumerator2 as IDisposable)) != null)
								{
									disposable.Dispose();
								}
							}
						}
						else if (xmlElement.Name == "players_performance")
						{
							IEnumerator enumerator3 = xmlElement.ChildNodes.GetEnumerator();
							try
							{
								while (enumerator3.MoveNext())
								{
									object obj3 = enumerator3.Current;
									XmlElement xmlElement3 = (XmlElement)obj3;
									rewardInputData.playersPerformances.Add(uint.Parse(xmlElement3.GetAttribute("id")), uint.Parse(xmlElement3.GetAttribute("value")));
								}
							}
							finally
							{
								IDisposable disposable2;
								if ((disposable2 = (enumerator3 as IDisposable)) != null)
								{
									disposable2.Dispose();
								}
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable3;
				if ((disposable3 = (enumerator as IDisposable)) != null)
				{
					disposable3.Dispose();
				}
			}
			this.m_sessionStorage.RemoveData(sessionId, ESessionData.Boosters);
			this.m_sessionStorage.RemoveData(sessionId, ESessionData.RewardMultiplier);
			foreach (RewardInputData.Team team in rewardInputData.teams.Values)
			{
				team.playerScores.Sort((RewardInputData.Team.Player p1, RewardInputData.Team.Player p2) => p1.score.CompareTo(p2.score));
				int count = team.playerScores.Count;
				for (int i = 0; i < count; i++)
				{
					team.playerScores[i].position = count - i - 1;
				}
			}
			CustomParams data3 = this.m_sessionStorage.GetData<CustomParams>(sessionId, ESessionData.Restrictions);
			if (data3 != null)
			{
				rewardInputData.maxRoundLimit = data3.RoundLimit;
			}
			this.m_sessionStorage.RemoveData(sessionId, ESessionData.Restrictions);
			return rewardInputData;
		}

		// Token: 0x06002840 RID: 10304 RVA: 0x000AD614 File Offset: 0x000ABA14
		private GameRoomType ParseRoomType(string roomTypeString)
		{
			return Utils.ParseEnum<GameRoomType>(roomTypeString);
		}

		// Token: 0x06002841 RID: 10305 RVA: 0x000AD62C File Offset: 0x000ABA2C
		private bool ValidateSession(string jid, string sessionId)
		{
			if (!this.m_sessionStorage.ValidateSession(jid, sessionId))
			{
				return false;
			}
			string server_id;
			if (!base.GetServerID(jid, out server_id))
			{
				return false;
			}
			IGameRoom roomByServer = this.m_roomManager.GetRoomByServer(server_id);
			if (roomByServer != null)
			{
				try
				{
					roomByServer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						SessionExtension extension = r.GetExtension<SessionExtension>();
						extension.RewardsReceived(sessionId);
					});
				}
				catch (RoomClosedException)
				{
				}
			}
			return true;
		}

		// Token: 0x04001519 RID: 5401
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x0400151A RID: 5402
		private readonly IGameRoomManager m_roomManager;

		// Token: 0x0400151B RID: 5403
		private readonly IRewardService m_rewardService;

		// Token: 0x0400151C RID: 5404
		private readonly IMissionSystem m_missionSystem;
	}
}
