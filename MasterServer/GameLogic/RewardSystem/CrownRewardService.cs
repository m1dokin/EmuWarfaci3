using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.PerformanceSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A3 RID: 1443
	[Service]
	[Singleton]
	internal class CrownRewardService : ServiceModule, ICrownRewardService
	{
		// Token: 0x06001F04 RID: 7940 RVA: 0x0007DCD0 File Offset: 0x0007C0D0
		public CrownRewardService(MasterServer.GameLogic.MissionSystem.IMissionSystem missionSystem, ISessionStorage sessionStorage)
		{
			this.m_missionCrownRewards = new Dictionary<string, CrownRewardInfo>();
			ConfigSection section = Resources.ModuleSettings.GetSection("StarSystem");
			this.m_isEnabled = (int.Parse(section.Get("enabled")) > 0);
			section.OnConfigChanged += this.StartSystemConfigOnOnConfigChanged;
			this.LoadRewardsFromConfig();
			ConfigSection section2 = Resources.Rewards.GetSection("CrownRewards");
			foreach (ConfigSection configSection in section2.GetSections("Reward"))
			{
				configSection.OnConfigChanged += delegate(ConfigEventArgs args)
				{
					this.LoadRewardsFromConfig();
				};
			}
			this.m_missionSystem = missionSystem;
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x06001F05 RID: 7941 RVA: 0x0007DDAC File Offset: 0x0007C1AC
		public ProfileCrownReward CalculateCrownReward(ulong profileId, RewardInputData inputData, string sessionId)
		{
			ProfileCrownReward profileCrownReward = new ProfileCrownReward();
			MissionContext mission = this.m_missionSystem.GetMission(inputData.missionId);
			if (mission == null)
			{
				Log.Warning<string>("[CrownRewardService] Unexpected : GetMission return null, for mission '{0}'", inputData.missionId);
				return profileCrownReward;
			}
			int playerTeamId = (int)inputData.teams.Keys.FirstOrDefault<byte>();
			MissionStatus missionStatus = (RewardCalculatorHelper.GetOutcome(mission, inputData.winnerTeamId, playerTeamId) != SessionOutcome.Won) ? MissionStatus.Failed : MissionStatus.Finished;
			if (!this.m_isEnabled || !inputData.IsPvE || missionStatus != MissionStatus.Finished)
			{
				return profileCrownReward;
			}
			CrownRewardPool crownRewardPool = new CrownRewardPool();
			crownRewardPool.CalculateTreshold(mission);
			CrownRewardInfo crownRewardInfo;
			if (!this.m_missionCrownRewards.TryGetValue(mission.missionType.Name, out crownRewardInfo))
			{
				return profileCrownReward;
			}
			ProfilePerformanceSessionStorage data = this.m_sessionStorage.GetData<ProfilePerformanceSessionStorage>(sessionId, ESessionData.ProfilePerformanceInfo);
			if (data == null)
			{
				throw new NullReferenceException(string.Format("Cant find profile performance info for session {0}", sessionId));
			}
			ProfilePerformanceInfo profilePerformanceInfo;
			if (!data.ProfilePerformanceInfos.TryGetValue(profileId, out profilePerformanceInfo))
			{
				throw new KeyNotFoundException(string.Format("Profile info for {0} can't be found", profileId));
			}
			ProfilePerformanceInfo.MissionPerformance missionPerformance;
			if (!profilePerformanceInfo.MissionPerformances.TryGetValue(inputData.missionId, out missionPerformance) || missionPerformance.Status != MissionStatus.Finished)
			{
				missionPerformance = new ProfilePerformanceInfo.MissionPerformance();
			}
			foreach (KeyValuePair<uint, uint> keyValuePair in inputData.playersPerformances)
			{
				if (Enum.IsDefined(typeof(CrownRewardThreshold.PerformanceCategory), (int)keyValuePair.Key) && keyValuePair.Value != 0U)
				{
					CrownRewardThreshold.PerformanceCategory key = (CrownRewardThreshold.PerformanceCategory)keyValuePair.Key;
					IEnumerator enumerator2 = Enum.GetValues(typeof(League)).GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							object obj = enumerator2.Current;
							League league = (League)obj;
							ProfilePerformanceInfo.StatPerformance statPerformance;
							if (!missionPerformance.StatPerformances.TryGetValue(keyValuePair.Key, out statPerformance))
							{
								statPerformance = new ProfilePerformanceInfo.StatPerformance();
							}
							LeagueThresholdBasic leagueThresholdBasic;
							if (crownRewardPool.TryGetThreshold(key, out leagueThresholdBasic) && leagueThresholdBasic.IsThresholdPassed(league, statPerformance.ProfilePerformance, keyValuePair.Value))
							{
								profileCrownReward.Add(new ProfileStatCrownReward(keyValuePair.Key, (ulong)crownRewardInfo[league], (ulong)keyValuePair.Value));
							}
							else if (this.m_missionCrownRewards.ContainsKey(mission.missionType.Name))
							{
								profileCrownReward.Add(new ProfileStatCrownReward(keyValuePair.Key, 0UL, (ulong)keyValuePair.Value));
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
			return profileCrownReward;
		}

		// Token: 0x06001F06 RID: 7942 RVA: 0x0007E084 File Offset: 0x0007C484
		private void LoadRewardsFromConfig()
		{
			Dictionary<string, CrownRewardInfo> dictionary = new Dictionary<string, CrownRewardInfo>();
			ConfigSection section = Resources.Rewards.GetSection("CrownRewards");
			foreach (ConfigSection configSection in section.GetSections("Reward"))
			{
				CrownRewardInfo value = new CrownRewardInfo();
				string key = configSection.Get("type");
				dictionary.Add(key, value);
				dictionary[key][League.BRONZE] = uint.Parse(configSection.Get("bronze"));
				dictionary[key][League.SILVER] = uint.Parse(configSection.Get("silver"));
				dictionary[key][League.GOLD] = uint.Parse(configSection.Get("gold"));
			}
			Interlocked.Exchange<Dictionary<string, CrownRewardInfo>>(ref this.m_missionCrownRewards, dictionary);
		}

		// Token: 0x06001F07 RID: 7943 RVA: 0x0007E17C File Offset: 0x0007C57C
		private void StartSystemConfigOnOnConfigChanged(ConfigEventArgs args)
		{
			if (string.Compare(args.Name, "enabled", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.m_isEnabled = (args.iValue > 0);
			}
		}

		// Token: 0x04000F1C RID: 3868
		private readonly MasterServer.GameLogic.MissionSystem.IMissionSystem m_missionSystem;

		// Token: 0x04000F1D RID: 3869
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000F1E RID: 3870
		private Dictionary<string, CrownRewardInfo> m_missionCrownRewards;

		// Token: 0x04000F1F RID: 3871
		private bool m_isEnabled;
	}
}
