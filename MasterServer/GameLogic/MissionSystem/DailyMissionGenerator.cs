using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003A1 RID: 929
	internal class DailyMissionGenerator : IDailyMissionGenerator
	{
		// Token: 0x06001498 RID: 5272 RVA: 0x00053780 File Offset: 0x00051B80
		public DailyMissionGenerator(Config cfg, IMissionSystem missionSystem, IDALService dalService)
		{
			this.m_config = cfg;
			this.m_missionSystem = missionSystem;
			this.SoftShufflePools = new SoftShufflePools(dalService);
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06001499 RID: 5273 RVA: 0x000537C6 File Offset: 0x00051BC6
		// (set) Token: 0x0600149A RID: 5274 RVA: 0x000537CE File Offset: 0x00051BCE
		public SoftShufflePools SoftShufflePools { get; private set; }

		// Token: 0x0600149B RID: 5275 RVA: 0x000537D8 File Offset: 0x00051BD8
		private DailyGenSettings GetSettings(ConfigSection cfg)
		{
			DailyGenSettings dailyGenSettings = new DailyGenSettings();
			dailyGenSettings.MissionType = cfg.Name.ToLower();
			cfg.Get("difficulty", out dailyGenSettings.Difficulty);
			if (Array.IndexOf<string>(DailyMissionGenerator.DIFFICULTIES, dailyGenSettings.Difficulty) == -1)
			{
				throw new Exception(string.Format("Invalid difficulty in mission generation config for {0} {1}", dailyGenSettings.MissionType, dailyGenSettings.Difficulty));
			}
			cfg.Get("generate_step", out dailyGenSettings.GenerateStep);
			if (dailyGenSettings.GenerateStep < 1)
			{
				throw new Exception(string.Format("Invalid generate_step in mission generation config for {0} {1}", dailyGenSettings.MissionType, dailyGenSettings.GenerateStep.ToString()));
			}
			cfg.Get("generate_count", out dailyGenSettings.GenerateCount);
			if (dailyGenSettings.GenerateCount < 1)
			{
				throw new Exception(string.Format("Invalid generate_count in mission generation config for {0} {1}", dailyGenSettings.MissionType, dailyGenSettings.GenerateCount.ToString()));
			}
			cfg.Get("expire_count", out dailyGenSettings.ExpireCount);
			if (dailyGenSettings.ExpireCount < 1)
			{
				throw new Exception(string.Format("Invalid expire_count in mission generation config for {0} {1}", dailyGenSettings.MissionType, dailyGenSettings.ExpireCount.ToString()));
			}
			cfg.Get("propagate_on_expire", out dailyGenSettings.PropagateOnExpire);
			cfg.Get("soft_shuffle_generate", out dailyGenSettings.SoftShuffleGenerate);
			dailyGenSettings.TimesOfDay = cfg.GetList("TimeOfDay");
			dailyGenSettings.Settings = new List<DailyGenSettings.SettingCfg>();
			foreach (ConfigSection configSection in cfg.GetSection("settings").GetSections("setting"))
			{
				dailyGenSettings.Settings.Add(new DailyGenSettings.SettingCfg(configSection.Get("name"), configSection.GetList("sublevels")));
			}
			dailyGenSettings.LevelGraphs = new List<string>();
			foreach (string text in cfg.GetList("LevelGraph"))
			{
				dailyGenSettings.LevelGraphs.Add(text.ToLower());
			}
			ConfigSection section = cfg.GetSection("SecondaryObjectives");
			if (section != null)
			{
				section.Register("min", 0, out dailyGenSettings.SecondaryObjMin);
				section.Register("max", 0, out dailyGenSettings.SecondaryObjMax);
			}
			return dailyGenSettings;
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x00053A6C File Offset: 0x00051E6C
		private DailyGenSettings GetTypeConfig(string type)
		{
			if (string.IsNullOrEmpty(type))
			{
				return null;
			}
			string tempType = type;
			ConfigSection section;
			do
			{
				section = this.m_config.GetSection(tempType);
				ConfigSection configSection = (from item in this.m_config.GetAllSections().Values
				select item[0]).SingleOrDefault((ConfigSection item) => item.Get("propagate_on_expire") == tempType);
				tempType = ((configSection != null) ? configSection.Name : string.Empty);
			}
			while (section != null && section.GetAllSections().Count == 0);
			if (section != null)
			{
				return this.GetSettings(section);
			}
			return null;
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x00053B30 File Offset: 0x00051F30
		public int GetTotalGenerationPeriod()
		{
			int num = 0;
			int num2 = 0;
			foreach (string type in this.m_config.GetAllSections().Keys)
			{
				DailyGenSettings typeConfig = this.GetTypeConfig(type);
				if (typeConfig != null)
				{
					num += typeConfig.GenerateCount;
					num2 = Math.Max(num2, typeConfig.GenerateStep * typeConfig.ExpireCount);
				}
			}
			int num3 = num * num2;
			if (num3 == 0)
			{
				Log.Warning("Total generation period is zero");
			}
			return num3;
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x00053BDC File Offset: 0x00051FDC
		private string PickUpLevelGraph(MissionGenerator.GenerateParams prms, string difficulty)
		{
			List<MissionGraph> graphs = this.m_missionSystem.MissionGraphRepository.GetGraphs();
			foreach (MissionGraph missionGraph in graphs)
			{
				if (!(missionGraph.SettingRestriction.ToLower() != prms.Setting.ToLower()))
				{
					if (missionGraph.SubMissionPatterns.Length == prms.RestrictSublevels.Count)
					{
						List<MissionGraph.SubMissionPattern> list = new List<MissionGraph.SubMissionPattern>(missionGraph.SubMissionPatterns);
						using (List<MissionGenerator.GenerateParams.SubLevelRestriction>.Enumerator enumerator2 = prms.RestrictSublevels.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								MissionGenerator.GenerateParams.SubLevelRestriction subLevel = enumerator2.Current;
								List<SubMissionConfig> list2 = this.m_missionSystem.SubMissionConfigRepository.MatchSubMission(prms.Setting.ToLower(), (SubMissionConfig X) => X.LevelName.ToLower() == subLevel.Name.ToLower());
								if (list2.Count != 1)
								{
									throw new Exception("Cannot pick up level graph");
								}
								foreach (MissionGraph.SubMissionPattern subMissionPattern in list)
								{
									if ((string.IsNullOrEmpty(subMissionPattern.Name) || subMissionPattern.Name.ToLower() == subLevel.Name.ToLower()) && (string.IsNullOrEmpty(subMissionPattern.MissionFlow) || subMissionPattern.MissionFlow.ToLower() == subLevel.Flow.ToLower()) && subMissionPattern.Difficulty.ToLower() == difficulty.ToLower() && subMissionPattern.Kind.ToLower() == list2[0].ParameterSets[0].Kind.ToLower())
									{
										list.Remove(subMissionPattern);
										break;
									}
								}
							}
						}
						if (list.Count == 0)
						{
							return missionGraph.Name;
						}
					}
				}
			}
			throw new Exception("Unexpected: cannot pick up level graph");
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x00053E70 File Offset: 0x00052270
		public int GetMissionCount(string type)
		{
			DailyGenSettings typeConfig = this.GetTypeConfig(type);
			return (typeConfig == null) ? 0 : (typeConfig.ExpireCount * typeConfig.GenerateCount);
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x00053EA0 File Offset: 0x000522A0
		public bool IsMissionExpired(MissionContext entry, int newGeneration)
		{
			if (entry.Version < 0)
			{
				return true;
			}
			DailyGenSettings typeConfig = this.GetTypeConfig(entry.missionType.Name);
			if (typeConfig == null)
			{
				return true;
			}
			int num = typeConfig.ExpireCount * typeConfig.GenerateStep;
			int num2 = (entry.Generation - 1) / typeConfig.GenerateStep * typeConfig.GenerateStep + 1;
			return num2 + num <= newGeneration;
		}

		// Token: 0x060014A1 RID: 5281 RVA: 0x00053F05 File Offset: 0x00052305
		public bool IsSoftShuffleGenerate(DailyGenSettings cfg)
		{
			return cfg != null && cfg.SoftShuffleGenerate != 0;
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x00053F1C File Offset: 0x0005231C
		public bool IsSoftShuffleGenerate(string type)
		{
			ConfigSection section = this.m_config.GetSection(type);
			if (section != null)
			{
				DailyGenSettings settings = this.GetSettings(section);
				return settings.SoftShuffleGenerate != 0;
			}
			return false;
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x00053F54 File Offset: 0x00052354
		public bool MissionSetValid(MissionSet currentSet)
		{
			IEnumerable<IGrouping<string, MissionContext>> enumerable = from m in currentSet.Missions.Values
			group m by m.missionType.Name;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (IGrouping<string, MissionContext> grouping in enumerable)
			{
				dictionary[grouping.Key] = grouping.Count<MissionContext>();
			}
			foreach (List<ConfigSection> list in this.m_config.GetAllSections().Values)
			{
				string name = list[0].Name;
				int missionCount = this.GetMissionCount(name);
				if (!dictionary.ContainsKey(name) || missionCount > dictionary[name])
				{
					return false;
				}
			}
			return enumerable.Count<IGrouping<string, MissionContext>>() == this.m_config.GetAllSections().Count;
		}

		// Token: 0x060014A4 RID: 5284 RVA: 0x00054098 File Offset: 0x00052498
		public bool ValidateMissionContext(MissionContext missionCtx)
		{
			if (string.IsNullOrEmpty(missionCtx.missionType.Name))
			{
				return false;
			}
			using (List<SubLevel>.Enumerator enumerator = missionCtx.subLevels.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SubLevel subLevel = enumerator.Current;
					List<SubMissionConfig.ParameterSet> list = this.m_missionSystem.SubMissionConfigRepository.MatchSubMissionParams(missionCtx.baseLevel.name, (SubMissionConfig.ParameterSet X) => X.MissionFlow == subLevel.flow && X.SubMission.LevelName == subLevel.name && X.Difficulty == missionCtx.difficulty);
					if (list.Count == 0)
					{
						Log.Warning("Failed to find sub level {0} for flow {1} and diff {2}. Drop mission {3}", new object[]
						{
							subLevel.name,
							subLevel.flow,
							missionCtx.difficulty,
							missionCtx.uid
						});
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x000541BC File Offset: 0x000525BC
		public MissionSet GenerateNewMissionSet(MissionSet currentSet)
		{
			MissionSet missionSet = new MissionSet
			{
				Generation = currentSet.Generation + 1
			};
			Log.Info<int>("Generating new mission set. Generation {0}", missionSet.Generation);
			foreach (KeyValuePair<Guid, MissionContext> keyValuePair in currentSet.Missions)
			{
				if (!this.IsMissionExpired(keyValuePair.Value, missionSet.Generation))
				{
					missionSet.Missions.Add(keyValuePair.Key, keyValuePair.Value);
				}
				else
				{
					Log.Info<Guid, int, MissionType>("Mission expired {0} {1} {2}", keyValuePair.Key, keyValuePair.Value.Generation, keyValuePair.Value.missionType);
					ConfigSection section = this.m_config.GetSection(keyValuePair.Value.missionType.Name);
					if (section != null)
					{
						string text = section.Get("propagate_on_expire");
						if (!string.IsNullOrEmpty(text))
						{
							MissionGenerator.Mission mission = this.ChangeMissionDifficulty(keyValuePair.Value, text);
							if (mission != null)
							{
								MissionContext missionContext = MissionParser.Parse(mission);
								missionContext.Generation = missionSet.Generation;
								Log.Info<string, string, string>("Propagated mission {0} {1} {2}", missionContext.uid, missionContext.name, missionContext.gameMode);
								missionSet.Missions.Add(new Guid(missionContext.uid), missionContext);
							}
						}
					}
				}
			}
			foreach (string missionType in this.m_config.GetAllSections().Keys)
			{
				List<MissionContext> list = this.GenerateFreshDailyMissions(currentSet, missionType);
				foreach (MissionContext missionContext2 in list)
				{
					missionContext2.Generation = missionSet.Generation;
					Log.Info<string, string, string>("New mission {0} {1} {2}", missionContext2.uid, missionContext2.name, missionContext2.gameMode);
					missionSet.Missions.Add(new Guid(missionContext2.uid), missionContext2);
				}
			}
			missionSet.Hash = this.GetMissionHash(missionSet.Missions.Values);
			return missionSet;
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x0005445C File Offset: 0x0005285C
		private string GetMissionHash(IEnumerable<MissionContext> missions)
		{
			int num = 0;
			foreach (MissionContext missionContext in missions)
			{
				num ^= missionContext.GetHashCode();
			}
			return num.ToString();
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x000544C4 File Offset: 0x000528C4
		private List<MissionContext> GenerateFreshDailyMissions(MissionSet current_set, string missionType)
		{
			List<MissionContext> list = new List<MissionContext>();
			MissionGenerator missionGenerator = this.m_missionSystem.MissionGenerator;
			ConfigSection section = this.m_config.GetSection(missionType);
			if (section == null || section.GetAllSections().Count == 0)
			{
				return list;
			}
			DailyGenSettings settings = this.GetSettings(section);
			if (current_set.Generation % settings.GenerateStep != 0)
			{
				return list;
			}
			MissionGenerator.GenerateParams generateParams = new MissionGenerator.GenerateParams();
			generateParams.ReleaseMission = true;
			generateParams.VictoryCondition = "pve";
			generateParams.Difficulty = settings.Difficulty;
			generateParams.MissionType = settings.MissionType;
			generateParams.ShufflePools = this.SoftShufflePools;
			generateParams.SoftShuffleGenerate = this.IsSoftShuffleGenerate(settings);
			int generateCount = settings.GenerateCount;
			for (int num = 0; num != generateCount; num++)
			{
				MissionGenerator.Mission mission = null;
				SoftShufflePool artSettingPool = this.SoftShufflePools.GetArtSettingPool(settings.Settings, generateParams.MissionType);
				SoftShufflePoolElement settingElement = artSettingPool.GetNextElement(settings.SoftShuffleGenerate != 0);
				if (settingElement == null)
				{
					Log.Warning<string>("Cannot retrieve next element from the soft shuffle pool '{0}'", artSettingPool.Key);
				}
				else
				{
					DailyGenSettings.SettingCfg settingCfg = settings.Settings.Find((DailyGenSettings.SettingCfg s) => s.Setting.ToLower() == settingElement.Key.ToLower());
					generateParams.Name = string.Format("@mission_{0}_{1}", missionType, num + 1);
					generateParams.Setting = settingCfg.Setting;
					generateParams.RestrictSublevels = settingCfg.SublevelRestrictions;
					List<MissionGraph> graphs = this.m_missionSystem.MissionGraphRepository.GetGraphs();
					SoftShufflePool levelGraphPool = this.SoftShufflePools.GetLevelGraphPool(generateParams, settings.LevelGraphs);
					int num2 = 0;
					while (num2 != DailyMissionGenerator.MAX_RETRIES && graphs.Count != 0)
					{
						SoftShufflePoolElement nextElement = levelGraphPool.GetNextElement(settings.SoftShuffleGenerate != 0);
						if (nextElement != null)
						{
							generateParams.LevelGraph = nextElement.Key;
							generateParams.TimeOfDay = settings.TimesOfDay[this.m_random.Next(settings.TimesOfDay.Count)];
							generateParams.SecondaryObjectives = ((settings.SecondaryObjMax != 0) ? this.m_random.Next(settings.SecondaryObjMin, settings.SecondaryObjMax + 1) : -1);
							try
							{
								mission = missionGenerator.Generate(generateParams);
								break;
							}
							catch (Exception ex)
							{
								Log.Warning("Failed to generate mission: " + ex.Message);
							}
						}
						else
						{
							Log.Warning<string>("Cannot retrieve next element from the soft shuffle pool '{0}'", levelGraphPool.Key);
						}
						num2++;
					}
					if (mission == null)
					{
						throw new Exception(string.Format("Failed to generate required mission set for difficulty '{0}'", settings.Difficulty));
					}
					MissionContext item = MissionParser.Parse(mission);
					list.Add(item);
				}
			}
			return list;
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x00054798 File Offset: 0x00052B98
		private MissionGenerator.Mission ChangeMissionDifficulty(MissionContext mission, string newMissionType)
		{
			if (mission.missionType.Name == newMissionType)
			{
				throw new Exception(string.Format("Trying to set type '{0}' for mission '{1}', but the mission already have that type", newMissionType, mission.name));
			}
			MissionGenerator missionGenerator = this.m_missionSystem.MissionGenerator;
			ConfigSection section = this.m_config.GetSection(newMissionType);
			if (section == null)
			{
				Log.Warning<string>("Failed to find section for new mission type {0}. Mission will be expired without changing its difficulty.", newMissionType.ToLower());
				return null;
			}
			DailyGenSettings typeConfig = this.GetTypeConfig(mission.missionType.Name);
			if (typeConfig == null)
			{
				Log.Warning<string>("Failed to find old mission type config {0}. Mission will be expired without changing its difficulty.", mission.missionType.Name.ToLower());
				return null;
			}
			MissionGenerator.GenerateParams generateParams = new MissionGenerator.GenerateParams();
			generateParams.ReleaseMission = true;
			generateParams.VictoryCondition = "pve";
			generateParams.MissionType = newMissionType;
			generateParams.Difficulty = section.Get("difficulty");
			generateParams.SoftShuffleGenerate = this.IsSoftShuffleGenerate(typeConfig);
			DailyGenSettings.SettingCfg settingCfg = typeConfig.Settings.Find((DailyGenSettings.SettingCfg s) => s.Setting.ToLower() == mission.baseLevel.name.ToLower());
			if (settingCfg == null)
			{
				Log.Warning<string>("Failed to find setting for baselevel {0}. Mission will be expired without changing its difficulty.", mission.baseLevel.name.ToLower());
				return null;
			}
			generateParams.Name = mission.name;
			generateParams.Setting = settingCfg.Setting;
			generateParams.RestrictSublevels.Clear();
			foreach (SubLevel subLevel in mission.subLevels)
			{
				generateParams.RestrictSublevels.Add(new MissionGenerator.GenerateParams.SubLevelRestriction(subLevel.name, subLevel.flow));
			}
			if (!string.IsNullOrEmpty(mission.levelGraph))
			{
				generateParams.LevelGraph = mission.levelGraph;
			}
			else
			{
				generateParams.LevelGraph = this.PickUpLevelGraph(generateParams, mission.difficulty);
			}
			for (int num = 0; num != DailyMissionGenerator.MAX_RETRIES; num++)
			{
				generateParams.TimeOfDay = mission.timeOfDay;
				generateParams.SecondaryObjectives = ((typeConfig.SecondaryObjMax != 0) ? this.m_random.Next(typeConfig.SecondaryObjMin, typeConfig.SecondaryObjMax + 1) : -1);
				try
				{
					return missionGenerator.Generate(generateParams);
				}
				catch (Exception ex)
				{
					Log.Warning("Failed to change mission difficulty: " + ex.Message);
				}
			}
			return null;
		}

		// Token: 0x060014A9 RID: 5289 RVA: 0x00054A58 File Offset: 0x00052E58
		public void DebugValidateMissionGraphs()
		{
			MissionGenerator missionGenerator = this.m_missionSystem.MissionGenerator;
			List<string> list = new List<string>();
			foreach (string text in DailyMissionGenerator.DIFFICULTIES)
			{
				Log.Info<string>("Validating mission graphs for difficulty: '{0}'", text);
				ConfigSection section = this.m_config.GetSection(text);
				if (section == null)
				{
					Log.Info<string>("No configuration for difficulty '{0}'", text);
				}
				else
				{
					DailyGenSettings settings = this.GetSettings(section);
					MissionGenerator.GenerateParams generateParams = new MissionGenerator.GenerateParams();
					generateParams.Difficulty = settings.Difficulty;
					generateParams.ShufflePools = this.SoftShufflePools;
					generateParams.SoftShuffleGenerate = this.IsSoftShuffleGenerate(generateParams.MissionType);
					int num = settings.ExpireCount * settings.GenerateCount;
					for (int num2 = 0; num2 != num; num2++)
					{
						DailyGenSettings.SettingCfg settingCfg = settings.Settings[num2 % settings.Settings.Count];
						List<MissionGraph> graphs = this.m_missionSystem.MissionGraphRepository.GetGraphs();
						generateParams.Setting = settingCfg.Setting;
						generateParams.RestrictSublevels = settingCfg.SublevelRestrictions;
						if (graphs.Count == 0)
						{
							list.Add(string.Format("FAILED: there are no mission graphs for setting {0}", settingCfg.Setting));
						}
						foreach (MissionGraph missionGraph in graphs)
						{
							generateParams.LevelGraph = missionGraph.Name;
							try
							{
								missionGenerator.Validate(generateParams);
							}
							catch (Exception ex)
							{
								list.Add(string.Format("FAILED: {0}", ex.Message));
							}
						}
					}
				}
			}
			if (list.Count == 0)
			{
				Log.Info("Validation DONE");
			}
			else
			{
				Log.Info("Validation FAILED");
				foreach (string p in list)
				{
					Log.Info<string>("    {0}", p);
				}
			}
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x00054C9C File Offset: 0x0005309C
		public void DebugEmulateRotation(int elementsNum, int shufflesNum)
		{
			SoftShufflePool softShufflePool = new SoftShufflePool("EmulateRotation");
			List<SoftShufflePoolElement> list = new List<SoftShufflePoolElement>();
			for (int i = 0; i < elementsNum; i++)
			{
				list.Add(new SoftShufflePoolElement(i.ToString()));
			}
			softShufflePool.SyncWithContent(list);
			Log.Info(string.Empty);
			Log.Info<int, int>("Emulated rotation of {0} elements for {1} shuffles:", elementsNum, shufflesNum);
			for (int j = 0; j < shufflesNum; j++)
			{
				string text = "Shuffle " + j.ToString() + ":\t";
				for (int k = 0; k < elementsNum; k++)
				{
					SoftShufflePoolElement nextElementEmulate = softShufflePool.GetNextElementEmulate();
					if (nextElementEmulate != null)
					{
						text = text + nextElementEmulate.Key + "\t";
					}
				}
				Log.Info<string>("{0}", text);
			}
			Log.Info(string.Empty);
		}

		// Token: 0x040009B6 RID: 2486
		private static readonly int MAX_RETRIES = 5;

		// Token: 0x040009B7 RID: 2487
		public static readonly string[] DIFFICULTIES = new string[]
		{
			"easy",
			"normal",
			"hard",
			"survival"
		};

		// Token: 0x040009B8 RID: 2488
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x040009B9 RID: 2489
		private readonly Config m_config;

		// Token: 0x040009BA RID: 2490
		private readonly Random m_random = new Random((int)DateTime.Now.Ticks);
	}
}
