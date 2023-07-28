using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.RewardSystem;
using Util.Common;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000788 RID: 1928
	internal class MissionParser
	{
		// Token: 0x060027EF RID: 10223 RVA: 0x000AB34C File Offset: 0x000A974C
		public static MissionContext Parse(string xml)
		{
			MissionParser missionParser = new MissionParser();
			return missionParser.ParseMissionData(xml);
		}

		// Token: 0x060027F0 RID: 10224 RVA: 0x000AB368 File Offset: 0x000A9768
		public static MissionContext Parse(MissionGenerator.Mission m)
		{
			MissionSerializer missionSerializer = new MissionSerializer();
			MissionParser missionParser = new MissionParser();
			MissionContext missionContext = missionParser.ParseMissionData(missionSerializer.Serialize(m));
			missionContext.levelGraph = m.LevelGraph;
			return missionContext;
		}

		// Token: 0x060027F1 RID: 10225 RVA: 0x000AB39C File Offset: 0x000A979C
		public static MissionContext Parse(SMission m)
		{
			string data = m.Data;
			MissionParser missionParser = new MissionParser();
			MissionContext missionContext = missionParser.ParseMissionData(data);
			missionContext.Generation = m.Generation;
			return missionContext;
		}

		// Token: 0x060027F2 RID: 10226 RVA: 0x000AB3D0 File Offset: 0x000A97D0
		public MissionContext ParseMissionData(string xml)
		{
			MissionContext missionContext = new MissionContext();
			missionContext.data = xml;
			bool flag = false;
			try
			{
				using (XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(xml)))
				{
					while (xmlTextReader.Read())
					{
						if (xmlTextReader.NodeType == XmlNodeType.Element)
						{
							if (xmlTextReader.Name.ToLower() == "description")
							{
								missionContext.UIInfo.DescriptionText = xmlTextReader.GetAttribute("text");
								missionContext.UIInfo.DescriptionIcon = xmlTextReader.GetAttribute("icon");
							}
							else if (xmlTextReader.Name.ToLower() == "gamemode")
							{
								missionContext.UIInfo.GameModeText = xmlTextReader.GetAttribute("text");
								missionContext.UIInfo.GameModeIcon = xmlTextReader.GetAttribute("icon");
							}
							else if (xmlTextReader.Name.ToLower() == "basemap")
							{
								missionContext.baseLevel.name = xmlTextReader.GetAttribute("name");
							}
							else if (xmlTextReader.Name.ToLower() == "mission")
							{
								missionContext.uid = xmlTextReader.GetAttribute("uid");
								missionContext.name = xmlTextReader.GetAttribute("name");
								missionContext.setting = xmlTextReader.GetAttribute("setting");
								if (xmlTextReader.GetAttribute("difficulty") != null)
								{
									missionContext.difficulty = xmlTextReader.GetAttribute("difficulty").ToLower();
								}
								else
								{
									missionContext.difficulty = "normal";
								}
								missionContext.missionType = new MissionType(xmlTextReader.GetAttribute("mission_type"));
								IRewardPool rewardPool = RewardPoolFactory.CreateRewardPool(missionContext.missionType);
								missionContext.baseLevel = new SubLevel(rewardPool);
								missionContext.gameMode = xmlTextReader.GetAttribute("game_mode").ToLower();
								string attribute = xmlTextReader.GetAttribute("level_graph");
								if (!string.IsNullOrEmpty(attribute))
								{
									missionContext.levelGraph = attribute.ToLower();
								}
								IGameModesSystem service = ServicesManager.GetService<IGameModesSystem>();
								GameModeSetting gameModeSetting = service.GetGameModeSetting(missionContext);
								if (gameModeSetting == null)
								{
									throw new MissionParseException(string.Format("Unknown game mode '{0}', discarding\n {1}", missionContext.gameMode, missionContext.data));
								}
								if (!gameModeSetting.GetSetting(ERoomSetting.NO_TEAMS_MODE, out missionContext.noTeamsMode))
								{
									throw new MissionParseException(string.Format("Can't retrieve 'no_teams_mode' setting for game mode '{0}'\n {1}", missionContext.gameMode, missionContext.data));
								}
								Guid guid;
								if (!Utils.TryParseGuid(missionContext.uid, out guid))
								{
									Log.Warning(string.Format("Mission guid {0} isn't valid, for mission {1} {2}", missionContext.uid, missionContext.gameMode, missionContext.name));
								}
								missionContext.releaseMission = (xmlTextReader.GetAttribute("release_mission") == "1");
								missionContext.clanWarMission = (xmlTextReader.GetAttribute("clan_war_mission") == "1");
								missionContext.onlyClanWarMission = (xmlTextReader.GetAttribute("only_clan_war_mission") == "1");
								missionContext.ratingMission = (xmlTextReader.GetAttribute("rating_game_mission") == "1");
								if (missionContext.IsPveMode() && missionContext.IsAvailableForRatingGame())
								{
									throw new MissionParseException(string.Format("Pve mission {0} couldn't be set up for rating games", missionContext.name));
								}
								string attribute2 = xmlTextReader.GetAttribute("channels");
								missionContext.channels = new HashSet<Resources.ChannelType>(Resources.ChannelTypes.LobbyChannels);
								if (!string.IsNullOrEmpty(attribute2) && missionContext.releaseMission)
								{
									missionContext.channels.Clear();
									foreach (string v in attribute2.ToLower().Replace(" ", string.Empty).Split(new char[]
									{
										','
									}))
									{
										missionContext.channels.Add(ReflectionUtils.EnumParse<Resources.ChannelType>(v));
									}
								}
								missionContext.timeOfDay = xmlTextReader.GetAttribute("time_of_day");
								missionContext.tutorialMission = 0;
								if (xmlTextReader.GetAttribute("tutorial_mission") != null && !int.TryParse(xmlTextReader.GetAttribute("tutorial_mission"), out missionContext.tutorialMission))
								{
									Log.Warning<string, string>("Tutorial mission {0} have wrong id {1} should be 1..8", missionContext.name, xmlTextReader.GetAttribute("tutorial_mission"));
								}
								if (xmlTextReader.GetAttribute("generation") != null)
								{
									int.TryParse(xmlTextReader.GetAttribute("generation"), out missionContext.Generation);
								}
								if (xmlTextReader.GetAttribute("version") != null)
								{
									int.TryParse(xmlTextReader.GetAttribute("version"), out missionContext.Version);
								}
							}
							else if (xmlTextReader.Name.ToLower() == "sublevels")
							{
								IRewardPool rewardPool2 = RewardPoolFactory.CreateRewardPool(missionContext.missionType);
								SubLevel subLevel = new SubLevel(rewardPool2);
								subLevel.name = missionContext.baseLevel.name;
								if (this.ParseSubLevel(subLevel, xmlTextReader))
								{
									missionContext.baseLevel = subLevel;
									flag = true;
								}
							}
							else if (xmlTextReader.Name.ToLower() == "objective")
							{
								MissionObjective item = default(MissionObjective);
								string attribute3 = xmlTextReader.GetAttribute("id");
								if (attribute3 != null)
								{
									item.id = int.Parse(attribute3);
								}
								item.type = xmlTextReader.GetAttribute("type");
								missionContext.objectives.Add(item);
							}
							else if (xmlTextReader.Name.ToLower() == "sublevel")
							{
								IRewardPool rewardPool3 = RewardPoolFactory.CreateRewardPool(missionContext.missionType);
								SubLevel subLevel2 = new SubLevel(rewardPool3);
								subLevel2.id = uint.Parse(xmlTextReader.GetAttribute("id"));
								subLevel2.name = xmlTextReader.GetAttribute("name");
								subLevel2.flow = xmlTextReader.GetAttribute("mission_flow");
								this.ParseSubLevel(subLevel2, xmlTextReader);
								missionContext.subLevels.Add(subLevel2);
							}
							else if (string.Compare(xmlTextReader.Name, "CrownRewardsThresholds", StringComparison.CurrentCultureIgnoreCase) == 0)
							{
								SubLevel subLevel3 = missionContext.subLevels[missionContext.subLevels.Count - 1];
								subLevel3.crownRewardPool.TryParse(xmlTextReader);
								if (!subLevel3.crownRewardPool.IsValid())
								{
									Log.Warning<string>("Crown thresholds isn't valid for {0}", subLevel3.name);
								}
							}
							else if (string.Compare(xmlTextReader.Name, "RewardPools", StringComparison.CurrentCultureIgnoreCase) == 0)
							{
								SubLevel subLevel4 = missionContext.subLevels[missionContext.subLevels.Count - 1];
								subLevel4.pool.TryParse(xmlTextReader);
							}
							else if (xmlTextReader.Name.ToLower() == "timedependency")
							{
								this.ParseDependencyInfo(ref missionContext.timeDependencyInfo, xmlTextReader, "min_time", "full_time", missionContext.name, "Time");
							}
							else if (xmlTextReader.Name.ToLower() == "killdependency")
							{
								this.ParseDependencyInfo(ref missionContext.killDependencyInfo, xmlTextReader, "min_kills", "full_kills", missionContext.name, "Kill");
							}
						}
					}
				}
			}
			catch (XmlException ex)
			{
				throw new MissionParseException(ex.ToString());
			}
			if (!missionContext.IsPveMode() && !flag)
			{
				missionContext.baseLevel.pool.ToDefault();
			}
			return missionContext;
		}

		// Token: 0x060027F3 RID: 10227 RVA: 0x000ABB3C File Offset: 0x000A9F3C
		private bool ParseSubLevel(SubLevel level, XmlTextReader reader)
		{
			if (!level.pool.TryParse(reader))
			{
				level.pool.ToDefault();
				return false;
			}
			return true;
		}

		// Token: 0x060027F4 RID: 10228 RVA: 0x000ABB60 File Offset: 0x000A9F60
		private void ParseDependencyInfo(ref DependencyInfo info, XmlTextReader reader, string minAttr, string fullAttr, string ctxName, string warningSuffix)
		{
			if (!uint.TryParse(reader.GetAttribute(minAttr), out info.min) || !uint.TryParse(reader.GetAttribute(fullAttr), out info.full))
			{
				Log.Warning<string, string>("{0}Dependency - load error: ContextName={1}", warningSuffix, ctxName);
				info.min = 0U;
				info.full = 0U;
			}
			else
			{
				bool flag = info.min == 0U || info.full == 0U;
				bool flag2 = info.min > info.full;
				if (flag || flag2)
				{
					Log.Warning("{0}Dependency - invalid value found: ContextName={1},{2}={3},{4}={5}", new object[]
					{
						warningSuffix,
						ctxName,
						minAttr,
						info.min,
						fullAttr,
						info.full
					});
					info.min = 0U;
					info.full = 0U;
				}
			}
		}
	}
}
