using System;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200041A RID: 1050
	[Service]
	internal class ApplyDynamicMultipliersCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016A7 RID: 5799 RVA: 0x0005EBBC File Offset: 0x0005CFBC
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			outputData.bonusExp = (uint)(0.5 + (double)(outputData.bonusExp * player.dynamicMultiplier.ExperienceMultiplier));
			outputData.bonusMoney = (uint)(0.5 + (double)(outputData.bonusMoney * player.dynamicMultiplier.MoneyMultiplier));
			outputData.bonusSponsorPoints = (uint)(0.5 + (double)(outputData.bonusSponsorPoints * player.dynamicMultiplier.SponsorPointsMultiplier));
			outputData.gainedExp = (uint)(0.5 + (double)(outputData.gainedExp * player.dynamicMultiplier.ExperienceMultiplier));
			outputData.gainedMoney = (uint)(0.5 + (double)(outputData.gainedMoney * player.dynamicMultiplier.MoneyMultiplier));
			outputData.gainedSponsorPoints = (uint)(0.5 + (double)(outputData.gainedSponsorPoints * player.dynamicMultiplier.SponsorPointsMultiplier));
			outputData.dynamicMultiplier = player.dynamicMultiplier;
		}
	}
}
