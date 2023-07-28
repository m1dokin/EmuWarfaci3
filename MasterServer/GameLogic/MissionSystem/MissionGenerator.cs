using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003BB RID: 955
	internal class MissionGenerator
	{
		// Token: 0x0600152C RID: 5420 RVA: 0x00057928 File Offset: 0x00055D28
		public MissionGenerator()
		{
			this.m_missionSystem = ServicesManager.GetService<IMissionSystem>();
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x00057960 File Offset: 0x00055D60
		public MissionGenerator.Mission Generate(MissionGenerator.GenerateParams prms)
		{
			MissionGraph graph = this.m_missionSystem.MissionGraphRepository.GetGraph(prms.LevelGraph);
			if (graph == null)
			{
				throw new Exception(string.Format("Invalid mission graph '{0}'", prms.LevelGraph));
			}
			MissionGenerator.Mission mission = new MissionGenerator.Mission();
			mission.Uid = Guid.NewGuid();
			mission.Name = prms.Name;
			mission.Setting = prms.Setting;
			mission.TimeOfDay = prms.TimeOfDay;
			mission.Difficulty = prms.Difficulty;
			mission.MissionType = prms.MissionType;
			mission.GameMode = prms.VictoryCondition;
			mission.LevelGraph = prms.LevelGraph;
			mission.ReleaseMission = prms.ReleaseMission;
			mission.Version = prms.Version;
			mission.UI_Info.DescriptionText = graph.UI_Info.DescriptionText;
			mission.UI_Info.DescriptionIcon = graph.UI_Info.DescriptionIcon;
			mission.UI_Info.GameModeText = graph.UI_Info.GameModeText;
			mission.UI_Info.GameModeIcon = graph.UI_Info.GameModeIcon;
			Log.Info<string>("Generating mission: graph '{0}'", prms.LevelGraph);
			prms.Dump();
			this.RandomizeMissionName(mission, prms, graph);
			this.RandomizeObjectives(mission, prms, graph);
			this.RandomizeSublevels(mission, prms, graph);
			this.CreateTeleports(mission);
			mission.AddMainObjective();
			return mission;
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x00057AB4 File Offset: 0x00055EB4
		private void RandomizeMissionName(MissionGenerator.Mission new_mission, MissionGenerator.GenerateParams prms, MissionGraph graph)
		{
			if (string.IsNullOrEmpty(graph.UI_Info.DisplayName))
			{
				return;
			}
			string[] array = graph.UI_Info.DisplayName.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 0)
			{
				int num = this.m_random.Next(array.Length);
				string text = array[num].Trim();
				if (text.StartsWith("@"))
				{
					new_mission.Name = text;
				}
			}
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x00057B2C File Offset: 0x00055F2C
		private void RandomizeObjectives(MissionGenerator.Mission new_mission, MissionGenerator.GenerateParams prms, MissionGraph graph)
		{
			List<SecondaryObjective> objectives = this.m_missionSystem.ObjectivesRepository.GetObjectives((SecondaryObjective x) => string.Compare(x.Difficulty, graph.Difficulty, true) == 0);
			int num = (prms.SecondaryObjectives == -1) ? graph.SecondaryObjectives : prms.SecondaryObjectives;
			while (objectives.Count > num)
			{
				objectives.RemoveAt(this.m_random.Next(objectives.Count));
			}
			foreach (SecondaryObjective so in objectives)
			{
				new_mission.AddSecondaryObjective(so);
			}
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x00057BFC File Offset: 0x00055FFC
		public void Validate(MissionGenerator.GenerateParams prms)
		{
			MissionGraph graph = this.m_missionSystem.MissionGraphRepository.GetGraph(prms.LevelGraph);
			if (graph == null)
			{
				throw new Exception(string.Format("Invalid mission graph '{0}'", prms.LevelGraph));
			}
			if (graph.SubMissionPatterns.Length == 0 && graph.GameMode == "pve")
			{
				throw new Exception(string.Format("No submission patterns for graph '{0}'", prms.LevelGraph));
			}
			MissionGenerator.Mission mission = new MissionGenerator.Mission();
			mission.Difficulty = prms.Difficulty;
			Log.Info<string>("Validating mission: graph '{0}'", prms.LevelGraph);
			prms.Dump();
			this.ValidatePatterns(mission, prms, graph, 0);
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x00057CA8 File Offset: 0x000560A8
		private void ValidatePatterns(MissionGenerator.Mission new_mission, MissionGenerator.GenerateParams prms, MissionGraph graph, int patternId)
		{
			if (patternId == graph.SubMissionPatterns.Length)
			{
				return;
			}
			MissionGraph.SubMissionPattern sm_pattern = graph.SubMissionPatterns[patternId];
			List<SubMissionConfig.ParameterSet> list = this.m_missionSystem.SubMissionConfigRepository.MatchSubMissionParams(prms.Setting, (SubMissionConfig.ParameterSet X) => (prms.RestrictSublevels.Count == 0 || prms.RestrictSublevels.Exists((MissionGenerator.GenerateParams.SubLevelRestriction R) => R.Name == X.SubMission.LevelName.ToLower())) && (string.IsNullOrEmpty(sm_pattern.Name) || string.Compare(sm_pattern.Name, X.SubMission.LevelName, true) == 0) && (string.IsNullOrEmpty(sm_pattern.MissionFlow) || string.Compare(sm_pattern.MissionFlow, X.MissionFlow, true) == 0) && X.Kind == sm_pattern.Kind && X.Difficulty == sm_pattern.Difficulty && X.SubMission.BaseLevel != null);
			if (list.Count == 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("No matches left for mission generation {0}\n", graph.Name);
				throw new Exception(stringBuilder.ToString());
			}
			foreach (SubMissionConfig.ParameterSet parameterSet in list)
			{
				this.ValidatePatterns(new_mission, prms, graph, patternId + 1);
			}
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x00057D90 File Offset: 0x00056190
		private void RandomizeSublevels(MissionGenerator.Mission new_mission, MissionGenerator.GenerateParams prms, MissionGraph graph)
		{
			MissionGraph.SubMissionPattern[] subMissionPatterns = graph.SubMissionPatterns;
			for (int i = 0; i < subMissionPatterns.Length; i++)
			{
				MissionGenerator.<RandomizeSublevels>c__AnonStorey5 <RandomizeSublevels>c__AnonStorey2 = new MissionGenerator.<RandomizeSublevels>c__AnonStorey5();
				<RandomizeSublevels>c__AnonStorey2.sm_pattern = subMissionPatterns[i];
				List<SubMissionConfig.ParameterSet> list = this.m_missionSystem.SubMissionConfigRepository.MatchSubMissionParams(prms.Setting, (SubMissionConfig.ParameterSet X) => (prms.RestrictSublevels.Count == 0 || prms.RestrictSublevels.Exists((MissionGenerator.GenerateParams.SubLevelRestriction R) => R.Name == X.SubMission.LevelName.ToLower() && (string.IsNullOrEmpty(R.Flow) || R.Flow == X.MissionFlow.ToLower()))) && (string.IsNullOrEmpty(<RandomizeSublevels>c__AnonStorey2.sm_pattern.Name) || string.Compare(<RandomizeSublevels>c__AnonStorey2.sm_pattern.Name, X.SubMission.LevelName, true) == 0) && (string.IsNullOrEmpty(<RandomizeSublevels>c__AnonStorey2.sm_pattern.MissionFlow) || string.Compare(<RandomizeSublevels>c__AnonStorey2.sm_pattern.MissionFlow, X.MissionFlow, true) == 0) && X.Kind == <RandomizeSublevels>c__AnonStorey2.sm_pattern.Kind && X.Difficulty == prms.Difficulty && X.SubMission.BaseLevel != null);
				SubMissionConfig.ParameterSet subLevel = null;
				if (list.Count == 1)
				{
					subLevel = list[0];
				}
				if (subLevel == null && list.Count == 0)
				{
					throw new Exception("Failed to pick unique sublevel");
				}
				if (subLevel == null)
				{
					SoftShufflePool subLevelPool = prms.ShufflePools.GetSubLevelPool(prms, list);
					SoftShufflePoolElement subLevelElement = subLevelPool.GetNextElement(prms.SoftShuffleGenerate);
					subLevel = list.Find((SubMissionConfig.ParameterSet E) => E.SubMission.LevelName == subLevelElement.Key);
					if (subLevel == null || subLevel.SubMission.LevelName != subLevelElement.Key)
					{
						throw new Exception("Failed to pick unique sublevel from pool");
					}
					if (subLevelElement.UsageCount % 2 == 0)
					{
						SubMissionConfig.ParameterSet parameterSet = list.Find((SubMissionConfig.ParameterSet E) => E.SubMission.LevelName == subLevelElement.Key && E.MissionFlow != subLevel.MissionFlow);
						if (parameterSet != null && parameterSet.SubMission.LevelName == subLevelElement.Key)
						{
							subLevel = parameterSet;
						}
					}
				}
				new_mission.SubLevels.Add(subLevel);
			}
			if (new_mission.SubLevels.Count == 0 && !string.IsNullOrEmpty(prms.Setting))
			{
				List<SubMissionConfig> list2 = this.m_missionSystem.SubMissionConfigRepository.MatchSubMission(prms.Setting, (SubMissionConfig X) => true);
				if (list2.Count != 0)
				{
					new_mission.BaseLevel = list2[0];
				}
			}
			if (graph.BaseMissionPattern != null && string.IsNullOrEmpty(prms.Setting))
			{
				List<SubMissionConfig.ParameterSet> list3 = this.m_missionSystem.SubMissionConfigRepository.MatchSubMissionParams(prms.Setting, (SubMissionConfig.ParameterSet X) => X.Kind == graph.BaseMissionPattern.Kind && X.SubMission.BaseLevel == null);
				if (list3.Count != 0)
				{
					new_mission.BaseLevelParams = list3[this.m_random.Next(list3.Count)];
					new_mission.BaseLevel = new_mission.BaseLevelParams.SubMission;
				}
			}
			if (new_mission.BaseLevel == null)
			{
				new_mission.BaseLevel = new_mission.SubLevels[0].SubMission.BaseLevel;
			}
			if (new_mission.BaseLevelParams == null && new_mission.BaseLevel.ParameterSets.Length != 0)
			{
				new_mission.BaseLevelParams = new_mission.BaseLevel.ParameterSets[this.m_random.Next(new_mission.BaseLevel.ParameterSets.Length)];
			}
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x000580F0 File Offset: 0x000564F0
		private void CreateTeleports(MissionGenerator.Mission new_mission)
		{
			for (int i = 0; i < new_mission.SubLevels.Count - 1; i++)
			{
				SubMissionConfig.ParameterSet parameterSet = new_mission.SubLevels[i];
				SubMissionConfig.ParameterSet parameterSet2 = new_mission.SubLevels[i + 1];
				new_mission.Teleports.Add(new MissionGenerator.Teleport(parameterSet.TeleportFinish, parameterSet2.TeleportStart));
			}
		}

		// Token: 0x040009EF RID: 2543
		public const int GENERATOR_VERSION = 0;

		// Token: 0x040009F0 RID: 2544
		private IMissionSystem m_missionSystem;

		// Token: 0x040009F1 RID: 2545
		private Random m_random = new Random((int)DateTime.Now.Ticks);

		// Token: 0x020003BC RID: 956
		public class Mission
		{
			// Token: 0x06001536 RID: 5430 RVA: 0x00058180 File Offset: 0x00056580
			public void AddMainObjective()
			{
				Objective objective = new Objective("primary");
				objective.AddValue("timelimit", this.GetTimeLimitFromSublevels().ToString());
				this.m_objectivesList.Insert(0, objective);
			}

			// Token: 0x06001537 RID: 5431 RVA: 0x000581C4 File Offset: 0x000565C4
			public void AddSecondaryObjective(SecondaryObjective so)
			{
				Objective objective = new Objective("secondary");
				objective.AddValue("id", so.Id.ToString());
				this.m_objectivesList.Add(objective);
			}

			// Token: 0x06001538 RID: 5432 RVA: 0x00058208 File Offset: 0x00056608
			private int GetTimeLimitFromSublevels()
			{
				int num = 0;
				foreach (SubMissionConfig.ParameterSet parameterSet in this.SubLevels)
				{
					num += parameterSet.TimeLimit;
				}
				return num;
			}

			// Token: 0x040009F3 RID: 2547
			public Guid Uid;

			// Token: 0x040009F4 RID: 2548
			public string Name;

			// Token: 0x040009F5 RID: 2549
			public string Setting;

			// Token: 0x040009F6 RID: 2550
			public SubMissionConfig BaseLevel;

			// Token: 0x040009F7 RID: 2551
			public SubMissionConfig.ParameterSet BaseLevelParams;

			// Token: 0x040009F8 RID: 2552
			public string TimeOfDay;

			// Token: 0x040009F9 RID: 2553
			public string Difficulty;

			// Token: 0x040009FA RID: 2554
			public string MissionType;

			// Token: 0x040009FB RID: 2555
			public string GameMode;

			// Token: 0x040009FC RID: 2556
			public string LevelGraph;

			// Token: 0x040009FD RID: 2557
			public bool ReleaseMission;

			// Token: 0x040009FE RID: 2558
			public int Version;

			// Token: 0x040009FF RID: 2559
			public List<SubMissionConfig.ParameterSet> SubLevels = new List<SubMissionConfig.ParameterSet>();

			// Token: 0x04000A00 RID: 2560
			public List<Objective> m_objectivesList = new List<Objective>();

			// Token: 0x04000A01 RID: 2561
			public MissionUIInfo UI_Info;

			// Token: 0x04000A02 RID: 2562
			public List<MissionGenerator.Teleport> Teleports = new List<MissionGenerator.Teleport>();
		}

		// Token: 0x020003BD RID: 957
		public class Teleport
		{
			// Token: 0x06001539 RID: 5433 RVA: 0x0005826C File Offset: 0x0005666C
			public Teleport(string start, string finish)
			{
				this.Start = start;
				this.Finish = finish;
			}

			// Token: 0x04000A03 RID: 2563
			public string Start;

			// Token: 0x04000A04 RID: 2564
			public string Finish;
		}

		// Token: 0x020003BE RID: 958
		public class GenerateParams
		{
			// Token: 0x0600153B RID: 5435 RVA: 0x0005829C File Offset: 0x0005669C
			public void Dump()
			{
				Log.Info<string>("    Difficulty: {0}", this.Difficulty);
				Log.Info<string>("    Type: {0}", this.MissionType);
				Log.Info<string>("    Setting: {0}", this.Setting);
				Log.Info<string>("    Graph: {0}", this.LevelGraph);
			}

			// Token: 0x04000A05 RID: 2565
			public string Name;

			// Token: 0x04000A06 RID: 2566
			public string LevelGraph;

			// Token: 0x04000A07 RID: 2567
			public string TimeOfDay;

			// Token: 0x04000A08 RID: 2568
			public string Difficulty;

			// Token: 0x04000A09 RID: 2569
			public string MissionType;

			// Token: 0x04000A0A RID: 2570
			public string Setting;

			// Token: 0x04000A0B RID: 2571
			public string VictoryCondition;

			// Token: 0x04000A0C RID: 2572
			public bool ReleaseMission;

			// Token: 0x04000A0D RID: 2573
			public readonly int Version;

			// Token: 0x04000A0E RID: 2574
			public int SecondaryObjectives = -1;

			// Token: 0x04000A0F RID: 2575
			public List<MissionGenerator.GenerateParams.SubLevelRestriction> RestrictSublevels = new List<MissionGenerator.GenerateParams.SubLevelRestriction>();

			// Token: 0x04000A10 RID: 2576
			public SoftShufflePools ShufflePools;

			// Token: 0x04000A11 RID: 2577
			public bool SoftShuffleGenerate;

			// Token: 0x020003BF RID: 959
			public struct SubLevelRestriction
			{
				// Token: 0x0600153C RID: 5436 RVA: 0x000582E9 File Offset: 0x000566E9
				public SubLevelRestriction(string name)
				{
					this.Name = name.ToLower();
					this.Flow = string.Empty;
				}

				// Token: 0x0600153D RID: 5437 RVA: 0x00058302 File Offset: 0x00056702
				public SubLevelRestriction(string name, string flow)
				{
					this.Name = name.ToLower();
					this.Flow = flow.ToLower();
				}

				// Token: 0x04000A12 RID: 2578
				public readonly string Name;

				// Token: 0x04000A13 RID: 2579
				public readonly string Flow;
			}
		}
	}
}
