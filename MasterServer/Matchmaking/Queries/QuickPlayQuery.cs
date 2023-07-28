using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason;
using MasterServer.GameLogic.MissionAccessLimitation;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.PersistentSettingsSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RatingSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Matchmaking.Data;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.Matchmaking.Queries
{
	// Token: 0x02000772 RID: 1906
	[QueryAttributes(TagName = "gameroom_quickplay")]
	internal class QuickPlayQuery : BaseQuery
	{
		// Token: 0x06002784 RID: 10116 RVA: 0x000A8660 File Offset: 0x000A6A60
		public QuickPlayQuery(IProfanityCheckService profanityCheckService, MasterServer.GameLogic.MissionSystem.IMissionSystem missionSystem, IDALService dalService, IRankSystem rankSystem, IMatchmakingSystem matchmakingSystem, IMissionAccessLimitationService limitationService, IUserProxyRepository userProxyRepository, IPersistentSettingsService persistentSettings, IRatingSeasonService ratingSeasonService, ISkillService skillService, IRatingGameBanService ratingGameBanService, IConfigProvider<GroupSizeConfig> groupSizeConfig)
		{
			this.m_profanityCheckService = profanityCheckService;
			this.m_missionSystem = missionSystem;
			this.m_dalService = dalService;
			this.m_rankSystem = rankSystem;
			this.m_matchmakingSystem = matchmakingSystem;
			this.m_limitationService = limitationService;
			this.m_userProxyRepository = userProxyRepository;
			this.m_persistentSettings = persistentSettings;
			this.m_ratingSeasonService = ratingSeasonService;
			this.m_skillService = skillService;
			this.m_ratingGameBanService = ratingGameBanService;
			this.m_groupSizeConfig = groupSizeConfig;
		}

		// Token: 0x06002785 RID: 10117 RVA: 0x000A86D0 File Offset: 0x000A6AD0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "QuickPlayQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("uid");
					MMSettings mmsettings = this.ParseMMSettings(request);
					List<ulong> list = this.ParseGroupList(request);
					list.Add(user.ProfileID);
					if ((from pid in list
					group pid by pid).Any((IGrouping<ulong, ulong> g) => g.Count<ulong>() > 1))
					{
						Log.Error<ulong, string>("Group list for quickplay from {0} contains duplicates '{1}'.", user.ProfileID, string.Join(",", (from pid in list
						select pid.ToString(CultureInfo.InvariantCulture)).ToArray<string>()));
						result = -1;
					}
					else if (!QuickPlayQuery.IsGameTypeValidForCurrentChannel(mmsettings.RoomType))
					{
						Log.Warning<GameRoomType>("Room type '{0}' is invalid for current channel for quickplay starting.", mmsettings.RoomType);
						result = -1;
					}
					else
					{
						int count = list.Count;
						if (!this.IsGroupSizeValidForRoomType(mmsettings.RoomType, count))
						{
							Log.Warning<GameRoomType, int>("Room type '{0}' doesn't support groups with {1} players.", mmsettings.RoomType, count);
							result = -1;
						}
						else
						{
							if (mmsettings.RoomType == GameRoomType.PvP_Rating)
							{
								list = (from p in list
								where !this.m_ratingGameBanService.IsPlayerBanned(p)
								select p).ToList<ulong>();
								if (!list.Any<ulong>())
								{
									return 28;
								}
							}
							ProfanityCheckResult profanityCheckResult = this.m_profanityCheckService.CheckRoomName(user.UserID, user.Nickname, mmsettings.RoomName);
							if (profanityCheckResult != ProfanityCheckResult.Reserved)
							{
								if (profanityCheckResult != ProfanityCheckResult.Failed)
								{
									if (!string.IsNullOrEmpty(mmsettings.MissionId) && this.m_missionSystem.IsMissionExpired(mmsettings.MissionId))
									{
										result = 1;
									}
									else
									{
										RatingSeason ratingSeason = this.m_ratingSeasonService.GetRatingSeason();
										if (mmsettings.RoomType.IsPvpRatingMode() && !ratingSeason.IsActive)
										{
											result = 27;
										}
										else
										{
											bool flag = user.ProfileProgression.IsMissionTypeUnlocked(mmsettings.MissionType);
											bool flag2 = this.m_limitationService.CanJoinMission(user.ProfileID, mmsettings.MissionType);
											if (mmsettings.RoomType.IsPveMode() && (!flag || !flag2))
											{
												result = 12;
											}
											else if (this.m_dalService.ProfileSystem.GetProfileInfo(user.ProfileID).UserID != user.UserID)
											{
												Log.Warning<ulong>("Can't find ProfileInfo '{0}' for quick play.", user.ProfileID);
												result = -1;
											}
											else
											{
												SProfileInfo[] source = (from profileId in list
												select new
												{
													profileId = profileId,
													profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId)
												} into <>__TranspIdent12
												where !this.m_rankSystem.CanJoinChannel(<>__TranspIdent12.profileInfo.RankInfo.RankId)
												select <>__TranspIdent12.profileInfo).ToArray<SProfileInfo>();
												if (source.Any<SProfileInfo>())
												{
													Log.Info<string, string>("Failed to quick play by users '{0}' with incorrect ranks {1}", string.Join(",", (from p in source
													select p.UserID.ToString(CultureInfo.InvariantCulture)).ToArray<string>()), string.Join(",", (from p in source
													select p.RankInfo.RankId.ToString(CultureInfo.InvariantCulture)).ToArray<string>()));
													result = 11;
												}
												else
												{
													MMEntityInfo mmentityInfo = this.CreateEntityInfo(user, attribute, mmsettings, list);
													if (mmentityInfo == null)
													{
														result = -1;
													}
													else
													{
														if (mmsettings.RoomType.IsPvpRatingMode())
														{
															foreach (ulong profileId2 in list)
															{
																this.m_ratingSeasonService.UpdateSeasonForPlayer(profileId2);
															}
														}
														this.m_matchmakingSystem.QueueEntity(mmentityInfo);
														result = 0;
													}
												}
											}
										}
									}
								}
								else
								{
									Log.Error<ulong>("Failed to open room for quickplay by user {0}: room name invalid", user.ProfileID);
									result = 21;
								}
							}
							else
							{
								Log.Error<ulong>("Failed to open room for quickplay by user {0}: room name reserved", user.ProfileID);
								result = 22;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06002786 RID: 10118 RVA: 0x000A8B50 File Offset: 0x000A6F50
		private MMEntityInfo CreateEntityInfo(UserInfo.User initiator, string mmEntityId, MMSettings mmSettings, IList<ulong> groupList)
		{
			IEnumerable<UserInfo.User> users = this.m_userProxyRepository.GetUserOrProxyByProfileId(groupList, true);
			if (groupList.Any((ulong pid) => !users.Any((UserInfo.User user) => user.ProfileID == pid)))
			{
				Log.Error<string>("Can't get user information for quickplay for all users '{0}'", string.Join(",", (from pid in groupList
				select pid.ToString(CultureInfo.InvariantCulture)).ToArray<string>()));
				return null;
			}
			List<MMPlayerInfo> players = (from user in this.m_userProxyRepository.GetUserOrProxyByProfileId(groupList, true)
			select new
			{
				user = user,
				profile = this.m_dalService.ProfileSystem.GetProfileInfo(user.ProfileID)
			} into <>__TranspIdent13
			select new MMPlayerInfo
			{
				User = <>__TranspIdent13.user,
				PersistentSettings = this.m_persistentSettings.GetProfileSettings(<>__TranspIdent13.user.ProfileID),
				PlayerCurrentClass = ProfileProgressionInfo.PlayerClassFromClassId(<>__TranspIdent13.profile.CurrentClass),
				Skill = this.m_skillService.GetSkill(<>__TranspIdent13.user.ProfileID, SkillTypeHelper.GetSkillTypeByRoomType(mmSettings.RoomType))
			}).ToList<MMPlayerInfo>();
			return new MMEntityInfo
			{
				Id = mmEntityId,
				Settings = mmSettings,
				Initiator = initiator,
				Players = players
			};
		}

		// Token: 0x06002787 RID: 10119 RVA: 0x000A8C3C File Offset: 0x000A703C
		private MMSettings ParseMMSettings(XmlElement request)
		{
			GameRoomType roomType = (GameRoomType)int.Parse(request.GetAttribute("room_type"));
			string attribute = request.GetAttribute("room_name");
			string attribute2 = request.GetAttribute("mission_id");
			string attribute3 = request.GetAttribute("game_mode");
			string missionType = (!request.HasAttribute("mission_type")) ? "normal" : request.GetAttribute("mission_type");
			int teamId = int.Parse(request.GetAttribute("team_id"));
			string attribute4 = request.GetAttribute("class_id");
			ulong num = ulong.Parse(request.GetAttribute("timestamp"));
			DateTime startTimeUtc = (num != 0UL) ? TimeUtils.UTCTimestampToUTCTime(num) : DateTime.UtcNow;
			if (!string.IsNullOrEmpty(attribute2))
			{
				MissionContext mission = this.m_missionSystem.GetMission(attribute2);
				if (mission != null)
				{
					missionType = mission.missionType.Name;
				}
			}
			return new MMSettings
			{
				RoomType = roomType,
				RoomName = attribute,
				MissionId = attribute2,
				GameMode = attribute3,
				MissionType = missionType,
				TeamId = teamId,
				ClassId = int.Parse(attribute4),
				StartTimeUtc = startTimeUtc
			};
		}

		// Token: 0x06002788 RID: 10120 RVA: 0x000A8D74 File Offset: 0x000A7174
		private List<ulong> ParseGroupList(XmlElement request)
		{
			List<ulong> list = new List<ulong>();
			IEnumerator enumerator = request.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						if (xmlElement.Name == "group")
						{
							IEnumerator enumerator2 = xmlElement.ChildNodes.GetEnumerator();
							try
							{
								while (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									XmlNode xmlNode2 = (XmlNode)obj2;
									if (xmlNode2.NodeType == XmlNodeType.Element)
									{
										XmlElement xmlElement2 = (XmlElement)xmlNode2;
										if (xmlElement2.Name == "player")
										{
											string attribute = xmlElement2.GetAttribute("profile_id");
											list.Add(ulong.Parse(attribute));
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
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			return list;
		}

		// Token: 0x06002789 RID: 10121 RVA: 0x000A8EA8 File Offset: 0x000A72A8
		public static bool IsGameTypeValidForCurrentChannel(GameRoomType gameType)
		{
			if (Resources.IsDevMode)
			{
				return true;
			}
			if (gameType.IsPveMode())
			{
				return Resources.ChannelTypes.IsPvE(Resources.Channel);
			}
			if (gameType.IsPvpMode())
			{
				return Resources.ChannelTypes.IsPvP(Resources.Channel);
			}
			Log.Warning<GameRoomType>("Can't check game type for current channel. Unknown game type '{0}' found.", gameType);
			return false;
		}

		// Token: 0x0600278A RID: 10122 RVA: 0x000A8EFC File Offset: 0x000A72FC
		private bool IsGroupSizeValidForRoomType(GameRoomType roomType, int groupSize)
		{
			GroupSizeConfig groupSizeConfig = this.m_groupSizeConfig.Get();
			int groupSize2 = groupSizeConfig.GetGroupSize(roomType);
			return groupSize <= groupSize2;
		}

		// Token: 0x04001480 RID: 5248
		private const string DefaultMissionType = "normal";

		// Token: 0x04001481 RID: 5249
		private readonly IProfanityCheckService m_profanityCheckService;

		// Token: 0x04001482 RID: 5250
		private readonly MasterServer.GameLogic.MissionSystem.IMissionSystem m_missionSystem;

		// Token: 0x04001483 RID: 5251
		private readonly IDALService m_dalService;

		// Token: 0x04001484 RID: 5252
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04001485 RID: 5253
		private readonly IMatchmakingSystem m_matchmakingSystem;

		// Token: 0x04001486 RID: 5254
		private readonly IMissionAccessLimitationService m_limitationService;

		// Token: 0x04001487 RID: 5255
		private readonly IUserProxyRepository m_userProxyRepository;

		// Token: 0x04001488 RID: 5256
		private readonly IPersistentSettingsService m_persistentSettings;

		// Token: 0x04001489 RID: 5257
		private readonly IRatingSeasonService m_ratingSeasonService;

		// Token: 0x0400148A RID: 5258
		private readonly ISkillService m_skillService;

		// Token: 0x0400148B RID: 5259
		private readonly IRatingGameBanService m_ratingGameBanService;

		// Token: 0x0400148C RID: 5260
		private readonly IConfigProvider<GroupSizeConfig> m_groupSizeConfig;
	}
}
