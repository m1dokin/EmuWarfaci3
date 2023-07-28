using System;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x020000A6 RID: 166
	public class RatingSeasonReward
	{
		// Token: 0x06000294 RID: 660 RVA: 0x0000D0A8 File Offset: 0x0000B4A8
		public RatingSeasonReward(string seasonResultRewardName, string ratingAchievedRewardName)
		{
			this.SeasonResultRewardName = seasonResultRewardName;
			this.RatingAchievedRewardName = ratingAchievedRewardName;
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000295 RID: 661 RVA: 0x0000D0BE File Offset: 0x0000B4BE
		// (set) Token: 0x06000296 RID: 662 RVA: 0x0000D0C6 File Offset: 0x0000B4C6
		public string SeasonResultRewardName { get; private set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000297 RID: 663 RVA: 0x0000D0CF File Offset: 0x0000B4CF
		// (set) Token: 0x06000298 RID: 664 RVA: 0x0000D0D7 File Offset: 0x0000B4D7
		public string RatingAchievedRewardName { get; private set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000299 RID: 665 RVA: 0x0000D0E0 File Offset: 0x0000B4E0
		public bool HasSeasonResultReward
		{
			get
			{
				return !string.IsNullOrEmpty(this.SeasonResultRewardName);
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600029A RID: 666 RVA: 0x0000D0F0 File Offset: 0x0000B4F0
		public bool HasRatingAchievedReward
		{
			get
			{
				return !string.IsNullOrEmpty(this.RatingAchievedRewardName);
			}
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000D100 File Offset: 0x0000B500
		public override string ToString()
		{
			return string.Format("RatingAchievedReward='{0}', SeasonResultReward='{1}'", this.RatingAchievedRewardName, this.SeasonResultRewardName);
		}
	}
}
