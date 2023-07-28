using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.PersistentSettingsSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.SkillSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020007BA RID: 1978
	[Service]
	[Singleton]
	internal class RewardCalculator : ServiceModule, IRewardCalculator
	{
		// Token: 0x06002894 RID: 10388 RVA: 0x000AE653 File Offset: 0x000ACA53
		public RewardCalculator(IDALService dalService, IMissionSystem missionSystem, IPersistentSettingsService persistentSettings, IRewardCalculationChainFactory chainFactory)
		{
			this.m_dalService = dalService;
			this.m_missionSystem = missionSystem;
			this.m_persistentSettings = persistentSettings;
			this.m_calculationChainFactory = chainFactory;
		}

		// Token: 0x06002895 RID: 10389 RVA: 0x000AE678 File Offset: 0x000ACA78
		public IEnumerable<RewardOutputData> Calculate(RewardInputData inputData, string sessionId)
		{
			RewardCalculator.<Calculate>c__AnonStorey0 <Calculate>c__AnonStorey = new RewardCalculator.<Calculate>c__AnonStorey0();
			<Calculate>c__AnonStorey.inputData = inputData;
			<Calculate>c__AnonStorey.sessionId = sessionId;
			List<RewardOutputData> list = new List<RewardOutputData>();
			<Calculate>c__AnonStorey.ctx = this.m_missionSystem.GetMission(<Calculate>c__AnonStorey.inputData.missionId);
			if (<Calculate>c__AnonStorey.ctx == null)
			{
				Log.Warning("[RewardCalculator.Calculate] Unexpected : GetMission return null");
				return list;
			}
			List<IRewardCalculatorElement> arr = this.m_calculationChainFactory.CreateRewardCalculationChain(<Calculate>c__AnonStorey.inputData.roomType, <Calculate>c__AnonStorey.ctx.missionType);
			foreach (KeyValuePair<byte, RewardInputData.Team> keyValuePair in <Calculate>c__AnonStorey.inputData.teams)
			{
				using (List<RewardInputData.Team.Player>.Enumerator enumerator2 = keyValuePair.Value.playerScores.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						RewardCalculator.<Calculate>c__AnonStorey1 <Calculate>c__AnonStorey2 = new RewardCalculator.<Calculate>c__AnonStorey1();
						<Calculate>c__AnonStorey2.pl = enumerator2.Current;
						RewardCalculator.<Calculate>c__AnonStorey2 <Calculate>c__AnonStorey3 = new RewardCalculator.<Calculate>c__AnonStorey2();
						<Calculate>c__AnonStorey3.<>f__ref$0 = <Calculate>c__AnonStorey;
						<Calculate>c__AnonStorey3.<>f__ref$1 = <Calculate>c__AnonStorey2;
						<Calculate>c__AnonStorey3.reward = 0U;
						SessionOutcome outcome = RewardCalculatorHelper.GetOutcome(<Calculate>c__AnonStorey.ctx, <Calculate>c__AnonStorey.inputData.winnerTeamId, (int)keyValuePair.Key);
						PersistentSettings profileSettings = this.m_persistentSettings.GetProfileSettings(<Calculate>c__AnonStorey2.pl.profileId);
						string value = profileSettings.GetValue("sponsor.id");
						uint sponsorId;
						uint.TryParse(value, out sponsorId);
						RewardCalculator.<Calculate>c__AnonStorey2 <Calculate>c__AnonStorey4 = <Calculate>c__AnonStorey3;
						RewardOutputData data = default(RewardOutputData);
						data.profileId = <Calculate>c__AnonStorey2.pl.profileId;
						data.progression = new ProfileProgressionInfo(<Calculate>c__AnonStorey2.pl.profileId, this.m_dalService);
						data.SponsorData.sponsorId = sponsorId;
						data.outcome = outcome;
						data.isVip = <Calculate>c__AnonStorey2.pl.isVip;
						data.isClanWar = <Calculate>c__AnonStorey.inputData.IsClanWar;
						data.score = <Calculate>c__AnonStorey2.pl.score;
						data.crownReward = new ProfileCrownReward();
						data.sessionTime = <Calculate>c__AnonStorey2.pl.sessionTime;
						data.skillType = SkillType.None;
						data.sessionId = <Calculate>c__AnonStorey.sessionId;
						<Calculate>c__AnonStorey4.data = data;
						arr.SafeForEach(delegate(IRewardCalculatorElement c)
						{
							c.Calculate(<Calculate>c__AnonStorey3.<>f__ref$0.ctx, <Calculate>c__AnonStorey3.<>f__ref$0.inputData, <Calculate>c__AnonStorey3.<>f__ref$1.pl, <Calculate>c__AnonStorey3.<>f__ref$0.sessionId, ref <Calculate>c__AnonStorey3.reward, ref <Calculate>c__AnonStorey3.data);
						});
						list.Add(<Calculate>c__AnonStorey3.data);
					}
				}
			}
			return list;
		}

		// Token: 0x04001585 RID: 5509
		private readonly IDALService m_dalService;

		// Token: 0x04001586 RID: 5510
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04001587 RID: 5511
		private readonly IPersistentSettingsService m_persistentSettings;

		// Token: 0x04001588 RID: 5512
		private readonly IRewardCalculationChainFactory m_calculationChainFactory;
	}
}
