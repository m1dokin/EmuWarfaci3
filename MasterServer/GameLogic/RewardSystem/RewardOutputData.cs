using System;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.SkillSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020007B8 RID: 1976
	internal struct RewardOutputData
	{
		// Token: 0x04001565 RID: 5477
		public ulong profileId;

		// Token: 0x04001566 RID: 5478
		public ulong userId;

		// Token: 0x04001567 RID: 5479
		public uint gainedMoney;

		// Token: 0x04001568 RID: 5480
		public uint gainedExp;

		// Token: 0x04001569 RID: 5481
		public uint gainedSponsorPoints;

		// Token: 0x0400156A RID: 5482
		public uint gainedClanPoints;

		// Token: 0x0400156B RID: 5483
		public uint bonusMoney;

		// Token: 0x0400156C RID: 5484
		public uint bonusExp;

		// Token: 0x0400156D RID: 5485
		public uint bonusSponsorPoints;

		// Token: 0x0400156E RID: 5486
		public uint gainedMoneyBooster;

		// Token: 0x0400156F RID: 5487
		public uint gainedExpBooster;

		// Token: 0x04001570 RID: 5488
		public uint gainedSponsorPointsBooster;

		// Token: 0x04001571 RID: 5489
		public float percentMoneyBooster;

		// Token: 0x04001572 RID: 5490
		public float percentExpBooster;

		// Token: 0x04001573 RID: 5491
		public float percentSponsorPointsBooster;

		// Token: 0x04001574 RID: 5492
		public uint completedStages;

		// Token: 0x04001575 RID: 5493
		public bool isVip;

		// Token: 0x04001576 RID: 5494
		public bool isClanWar;

		// Token: 0x04001577 RID: 5495
		public int score;

		// Token: 0x04001578 RID: 5496
		public TimeSpan sessionTime;

		// Token: 0x04001579 RID: 5497
		public SRewardMultiplier dynamicMultiplier;

		// Token: 0x0400157A RID: 5498
		public SessionOutcome outcome;

		// Token: 0x0400157B RID: 5499
		public ProfileProgressionInfo progression;

		// Token: 0x0400157C RID: 5500
		public ProfileCrownReward crownReward;

		// Token: 0x0400157D RID: 5501
		public SponsorDataUpdate SponsorData;

		// Token: 0x0400157E RID: 5502
		public SkillType skillType;

		// Token: 0x0400157F RID: 5503
		public double skillDiff;

		// Token: 0x04001580 RID: 5504
		public int ratingReward;

		// Token: 0x04001581 RID: 5505
		public uint winStreakBonus;

		// Token: 0x04001582 RID: 5506
		public int ratingWinsToAdd;

		// Token: 0x04001583 RID: 5507
		public string sessionId;

		// Token: 0x04001584 RID: 5508
		public bool firstWin;
	}
}
