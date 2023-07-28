using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003AE RID: 942
	internal class SoftShufflePools
	{
		// Token: 0x060014E5 RID: 5349 RVA: 0x00055EF8 File Offset: 0x000542F8
		public SoftShufflePools(IDALService dalService)
		{
			this.m_dalService = dalService;
			IEnumerable<SoftShufflePoolData> softShufflePools = this.m_dalService.MissionSystem.GetSoftShufflePools();
			foreach (SoftShufflePoolData softShufflePoolData in softShufflePools)
			{
				this.m_softShufflePools[softShufflePoolData.m_key] = new SoftShufflePool(softShufflePoolData);
			}
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x00055F88 File Offset: 0x00054388
		private string GetArtSettingPoolKey(string missionType)
		{
			string text = string.Format("as:{0}", missionType.ToLower());
			return text.Replace('\\', '/');
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x00055FB4 File Offset: 0x000543B4
		private string GetLevelGraphPoolKey(string setting, string type)
		{
			string text = string.Format("lg:{0}:{1}", setting.ToLower(), type.ToLower());
			return text.Replace('\\', '/');
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x00055FE4 File Offset: 0x000543E4
		private string GetSubLevelPoolKey(string setting, string type)
		{
			string text = string.Format("sl:{0}:{1}", setting.ToLower(), type.ToLower());
			return text.Replace('\\', '/');
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x00056014 File Offset: 0x00054414
		public SoftShufflePool GetArtSettingPool(List<DailyGenSettings.SettingCfg> settings, string missionType)
		{
			List<SoftShufflePoolElement> content = new List<SoftShufflePoolElement>(from s in settings
			select new SoftShufflePoolElement(s.Setting));
			string artSettingPoolKey = this.GetArtSettingPoolKey(missionType);
			if (!this.m_softShufflePools.ContainsKey(artSettingPoolKey))
			{
				this.m_softShufflePools[artSettingPoolKey] = new SoftShufflePool(artSettingPoolKey);
			}
			SoftShufflePool softShufflePool = this.m_softShufflePools[artSettingPoolKey];
			softShufflePool.SyncWithContent(content);
			return softShufflePool;
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x0005608C File Offset: 0x0005448C
		public SoftShufflePool GetLevelGraphPool(MissionGenerator.GenerateParams prms, List<string> levelGraphs)
		{
			IMissionSystem service = ServicesManager.GetService<IMissionSystem>();
			List<MissionGraph> graphs = service.MissionGraphRepository.GetGraphs();
			List<SoftShufflePoolElement> list = new List<SoftShufflePoolElement>();
			using (List<MissionGraph>.Enumerator enumerator = graphs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MissionGraph mg = enumerator.Current;
					if (mg.GameMode.ToLower() == prms.VictoryCondition.ToLower() && mg.Difficulty.ToLower() == prms.Difficulty.ToLower() && mg.SettingRestriction.ToLower() == prms.Setting.ToLower() && !string.IsNullOrEmpty(levelGraphs.Find((string N) => N.ToLower() == mg.Name.ToLower())))
					{
						list.Add(new SoftShufflePoolElement(mg.Name));
					}
				}
			}
			string levelGraphPoolKey = this.GetLevelGraphPoolKey(prms.Setting, prms.MissionType);
			if (!this.m_softShufflePools.ContainsKey(levelGraphPoolKey))
			{
				this.m_softShufflePools[levelGraphPoolKey] = new SoftShufflePool(levelGraphPoolKey);
			}
			SoftShufflePool softShufflePool = this.m_softShufflePools[levelGraphPoolKey];
			softShufflePool.SyncWithContent(list);
			return softShufflePool;
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x000561FC File Offset: 0x000545FC
		public SoftShufflePool GetSubLevelPool(MissionGenerator.GenerateParams prms, List<SubMissionConfig.ParameterSet> parameterSets)
		{
			List<SoftShufflePoolElement> list = new List<SoftShufflePoolElement>();
			foreach (SubMissionConfig.ParameterSet parameterSet in parameterSets)
			{
				string levelName = parameterSet.SubMission.LevelName;
				SoftShufflePoolElement softShufflePoolElement = list.Find((SoftShufflePoolElement E) => E.Key == levelName);
				if (softShufflePoolElement == null || softShufflePoolElement.Key != levelName)
				{
					list.Add(new SoftShufflePoolElement(levelName));
				}
			}
			string subLevelPoolKey = this.GetSubLevelPoolKey(prms.Setting, prms.MissionType);
			if (!this.m_softShufflePools.ContainsKey(subLevelPoolKey))
			{
				this.m_softShufflePools[subLevelPoolKey] = new SoftShufflePool(subLevelPoolKey);
			}
			SoftShufflePool softShufflePool = this.m_softShufflePools[subLevelPoolKey];
			softShufflePool.SyncWithContent(list);
			return softShufflePool;
		}

		// Token: 0x040009D1 RID: 2513
		private readonly IDALService m_dalService;

		// Token: 0x040009D2 RID: 2514
		private Dictionary<string, SoftShufflePool> m_softShufflePools = new Dictionary<string, SoftShufflePool>();
	}
}
