using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003CB RID: 971
	internal class SubMissionConfigRepository
	{
		// Token: 0x06001559 RID: 5465 RVA: 0x0005972A File Offset: 0x00057B2A
		public SubMissionConfigRepository()
		{
			this.m_levelsDirectory = Path.Combine(Resources.GetResourcesDirectory(), "levels");
			this.LoadLevelConfigs(this.m_levelsDirectory);
			this.ResolveBaseLevels();
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x00059764 File Offset: 0x00057B64
		public List<SubMissionConfig> MatchSubMission(Predicate<SubMissionConfig> pred)
		{
			List<SubMissionConfig> list = new List<SubMissionConfig>();
			foreach (string setting in this.m_submissionsBySetting.Keys)
			{
				list.AddRange(this.MatchSubMission(setting, pred));
			}
			return list;
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x000597D4 File Offset: 0x00057BD4
		public List<SubMissionConfig> MatchSubMission(string setting, Predicate<SubMissionConfig> pred)
		{
			if (string.IsNullOrEmpty(setting))
			{
				return this.MatchSubMission(pred);
			}
			List<SubMissionConfig> list = new List<SubMissionConfig>();
			List<SubMissionConfig> list2;
			if (!this.m_submissionsBySetting.TryGetValue(setting, out list2))
			{
				return list;
			}
			foreach (SubMissionConfig subMissionConfig in list2)
			{
				if (pred(subMissionConfig))
				{
					list.Add(subMissionConfig);
				}
			}
			return list;
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x00059868 File Offset: 0x00057C68
		public List<SubMissionConfig.ParameterSet> MatchSubMissionParams(Predicate<SubMissionConfig.ParameterSet> pred)
		{
			List<SubMissionConfig.ParameterSet> list = new List<SubMissionConfig.ParameterSet>();
			foreach (string setting in this.m_submissionsBySetting.Keys)
			{
				list.AddRange(this.MatchSubMissionParams(setting, pred));
			}
			return list;
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x000598D8 File Offset: 0x00057CD8
		public List<SubMissionConfig.ParameterSet> MatchSubMissionParams(string setting, Predicate<SubMissionConfig.ParameterSet> pred)
		{
			setting = setting.ToLower();
			if (string.IsNullOrEmpty(setting))
			{
				return this.MatchSubMissionParams(pred);
			}
			List<SubMissionConfig.ParameterSet> list = new List<SubMissionConfig.ParameterSet>();
			List<SubMissionConfig> list2;
			if (!this.m_submissionsBySetting.TryGetValue(setting, out list2))
			{
				return list;
			}
			foreach (SubMissionConfig subMissionConfig in list2)
			{
				foreach (SubMissionConfig.ParameterSet parameterSet in subMissionConfig.ParameterSets)
				{
					if (pred(parameterSet))
					{
						list.Add(parameterSet);
					}
				}
			}
			return list;
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x0005999C File Offset: 0x00057D9C
		private void LoadLevelConfigs(string dir)
		{
			string[] directories = Directory.GetDirectories(dir);
			if (directories.Length != 0)
			{
				foreach (string dir2 in directories)
				{
					this.LoadLevelConfigs(dir2);
				}
				return;
			}
			this.LoadLevelConfig(dir);
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x000599E4 File Offset: 0x00057DE4
		private void LoadLevelConfig(string dir)
		{
			SubMissionConfig subMissionConfig = new SubMissionConfig();
			subMissionConfig.LevelName = Path.GetFileNameWithoutExtension(dir);
			subMissionConfig.ParameterSets = new SubMissionConfig.ParameterSet[0];
			string config_file = subMissionConfig.LevelName.ToLower() + ".xml";
			List<string> list = new List<string>(Directory.GetFiles(dir, "*.xml"));
			config_file = list.Find((string X) => X.ToLower().EndsWith(config_file));
			if (!File.Exists(config_file))
			{
				return;
			}
			try
			{
				this.ParseLevelDescription(subMissionConfig, config_file);
				string text = list.Find((string X) => X.EndsWith(SubMissionConfigRepository.MISSION_CONFIG_FILE_NAME));
				if (!string.IsNullOrEmpty(text))
				{
					this.ParseSubLevelConfig(subMissionConfig, text);
				}
				List<SubMissionConfig> list2;
				if (!this.m_submissionsBySetting.TryGetValue(subMissionConfig.Setting, out list2))
				{
					list2 = new List<SubMissionConfig>();
					this.m_submissionsBySetting.Add(subMissionConfig.Setting, list2);
				}
				list2.Add(subMissionConfig);
			}
			catch (Exception e)
			{
				Log.Warning<string>("Error while loading mission config '{0}'", subMissionConfig.LevelName);
				Log.Warning(e);
			}
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x00059B1C File Offset: 0x00057F1C
		private void ParseLevelDescription(SubMissionConfig sm_config, string file)
		{
			sm_config.Setting = string.Empty;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(file);
			XmlNodeList elementsByTagName = xmlDocument.DocumentElement.GetElementsByTagName("Setting");
			XmlElement xmlElement = elementsByTagName[0] as XmlElement;
			if (xmlElement == null)
			{
				throw new ApplicationException(string.Format("Can't get settings for '{0}'", sm_config.LevelName));
			}
			sm_config.Setting = xmlElement.GetAttribute("name").ToLower();
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x00059B94 File Offset: 0x00057F94
		private void ParseSubLevelConfig(SubMissionConfig sm_config, string file)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(file);
			List<SubMissionConfig.ParameterSet> list = new List<SubMissionConfig.ParameterSet>();
			IEnumerator enumerator = xmlDocument.DocumentElement.GetElementsByTagName("ParameterSet").GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					XmlElement xmlElement = xmlNode as XmlElement;
					if (xmlElement != null)
					{
						SubMissionConfig.ParameterSet parameterSet = new SubMissionConfig.ParameterSet();
						parameterSet.SubMission = sm_config;
						parameterSet.Difficulty = xmlElement.GetAttribute("difficulty");
						parameterSet.DifficultyCfg = xmlElement.GetAttribute("difficulty_cfg");
						parameterSet.Kind = xmlElement.GetAttribute("kind");
						parameterSet.MissionType = xmlElement.GetAttribute("mission_type");
						parameterSet.MissionFlow = xmlElement.GetAttribute("mission_flow");
						parameterSet.TimeLimit = int.Parse(xmlElement.GetAttribute("time_limit"));
						parameterSet.Score = this.TryParseOrDefault(xmlElement.GetAttribute("score"));
						parameterSet.TeleportStart = xmlElement.GetAttribute("start_teleport");
						parameterSet.TeleportFinish = xmlElement.GetAttribute("finish_teleport");
						parameterSet.ScorePool = int.Parse(xmlElement.GetAttribute("score_pool"));
						parameterSet.WinPool = int.Parse(xmlElement.GetAttribute("win_pool"));
						parameterSet.LosePool = int.Parse(xmlElement.GetAttribute("lose_pool"));
						if (xmlElement.HasAttribute("draw_pool"))
						{
							parameterSet.DrawPool = int.Parse(xmlElement.GetAttribute("draw_pool"));
						}
						else
						{
							parameterSet.DrawPool = parameterSet.LosePool;
						}
						XmlNodeList elementsByTagName = xmlElement.GetElementsByTagName("CrownRewardsThresholds");
						if (elementsByTagName.Count > 0)
						{
							parameterSet.CrownThresholds = new Dictionary<string, SubMissionConfig.ParameterSet.Threshold>();
							IEnumerator enumerator2 = elementsByTagName[0].ChildNodes.GetEnumerator();
							try
							{
								while (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									XmlElement xmlElement2 = (XmlElement)obj2;
									SubMissionConfig.ParameterSet.Threshold value;
									value.Bronze = uint.Parse(xmlElement2.GetAttribute("bronze"));
									value.Silver = uint.Parse(xmlElement2.GetAttribute("silver"));
									value.Gold = uint.Parse(xmlElement2.GetAttribute("gold"));
									parameterSet.CrownThresholds.Add(xmlElement2.Name, value);
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
						else
						{
							Log.Info<string, string, string>("No crown rewards thresholds for sublevel {0}, parameter set '{1}_{2}'", sm_config.LevelName, parameterSet.Difficulty, parameterSet.Kind);
						}
						XmlNodeList elementsByTagName2 = xmlElement.GetElementsByTagName("RewardPools");
						if (elementsByTagName2.Count > 0)
						{
							parameterSet.PoolRewards = new Dictionary<string, string>();
							IEnumerator enumerator3 = elementsByTagName2[0].ChildNodes.GetEnumerator();
							try
							{
								while (enumerator3.MoveNext())
								{
									object obj3 = enumerator3.Current;
									XmlElement xmlElement3 = (XmlElement)obj3;
									parameterSet.PoolRewards.Add(xmlElement3.GetAttribute("name"), xmlElement3.GetAttribute("value"));
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
						list.Add(parameterSet);
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
			sm_config.ParameterSets = list.ToArray();
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x00059F50 File Offset: 0x00058350
		private void ResolveBaseLevels()
		{
			foreach (KeyValuePair<string, List<SubMissionConfig>> keyValuePair in this.m_submissionsBySetting)
			{
				SubMissionConfig subMissionConfig = keyValuePair.Value.Find((SubMissionConfig X) => X.LevelName.EndsWith("_base"));
				if (subMissionConfig != null)
				{
					foreach (SubMissionConfig subMissionConfig2 in keyValuePair.Value)
					{
						if (!object.ReferenceEquals(subMissionConfig2, subMissionConfig))
						{
							subMissionConfig2.BaseLevel = subMissionConfig;
						}
					}
				}
			}
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x0005A038 File Offset: 0x00058438
		private int TryParseOrDefault(string value)
		{
			int num;
			return (!int.TryParse(value, out num)) ? 0 : num;
		}

		// Token: 0x04000A44 RID: 2628
		private static readonly string MISSION_CONFIG_FILE_NAME = "submissionconfig.xml";

		// Token: 0x04000A45 RID: 2629
		private string m_levelsDirectory;

		// Token: 0x04000A46 RID: 2630
		private Dictionary<string, List<SubMissionConfig>> m_submissionsBySetting = new Dictionary<string, List<SubMissionConfig>>();
	}
}
