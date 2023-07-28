using System;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000418 RID: 1048
	[Service]
	internal class ApplyBoostersCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016A3 RID: 5795 RVA: 0x0005E9A8 File Offset: 0x0005CDA8
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			outputData.bonusExp += (uint)(0.5 + (double)(outputData.bonusExp * player.xp_boost));
			outputData.bonusMoney += (uint)(0.5 + (double)(outputData.bonusMoney * player.gm_boost));
			outputData.bonusSponsorPoints += (uint)(0.5 + (double)(outputData.bonusSponsorPoints * player.vp_boost));
			outputData.gainedExpBooster = (uint)(0.5 + (double)(outputData.gainedExp * player.xp_boost));
			outputData.gainedMoneyBooster = (uint)(0.5 + (double)(outputData.gainedMoney * player.gm_boost));
			outputData.gainedSponsorPointsBooster = (uint)(0.5 + (double)(outputData.gainedSponsorPoints * player.vp_boost));
			outputData.gainedExp += outputData.gainedExpBooster;
			outputData.gainedMoney += outputData.gainedMoneyBooster;
			outputData.gainedSponsorPoints += outputData.gainedSponsorPointsBooster;
			outputData.percentExpBooster = player.xp_boost;
			outputData.percentMoneyBooster = player.gm_boost;
			outputData.percentSponsorPointsBooster = player.vp_boost;
		}
	}
}
