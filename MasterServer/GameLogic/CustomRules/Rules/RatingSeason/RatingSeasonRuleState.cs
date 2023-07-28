using System;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x020000A0 RID: 160
	public class RatingSeasonRuleState : CustomRuleState
	{
		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0000C9B4 File Offset: 0x0000ADB4
		// (set) Token: 0x06000273 RID: 627 RVA: 0x0000C9BC File Offset: 0x0000ADBC
		public uint MaxRatingLevelAchieved { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000274 RID: 628 RVA: 0x0000C9C5 File Offset: 0x0000ADC5
		// (set) Token: 0x06000275 RID: 629 RVA: 0x0000C9CD File Offset: 0x0000ADCD
		public string SeasonId { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000276 RID: 630 RVA: 0x0000C9D6 File Offset: 0x0000ADD6
		// (set) Token: 0x06000277 RID: 631 RVA: 0x0000C9DE File Offset: 0x0000ADDE
		public string SeasonResultRewardName { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000278 RID: 632 RVA: 0x0000C9E7 File Offset: 0x0000ADE7
		public bool HasSeasonReward
		{
			get
			{
				return !string.IsNullOrEmpty(this.SeasonResultRewardName);
			}
		}

		// Token: 0x0400010D RID: 269
		public const byte TYPE_ID = 4;

		// Token: 0x0400010E RID: 270
		public const int DATA_VERSION = 0;
	}
}
