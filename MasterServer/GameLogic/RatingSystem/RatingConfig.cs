using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x02000413 RID: 1043
	public class RatingConfig
	{
		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06001677 RID: 5751 RVA: 0x0005E403 File Offset: 0x0005C803
		// (set) Token: 0x06001678 RID: 5752 RVA: 0x0005E40B File Offset: 0x0005C80B
		public uint Step { get; set; }

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06001679 RID: 5753 RVA: 0x0005E414 File Offset: 0x0005C814
		// (set) Token: 0x0600167A RID: 5754 RVA: 0x0005E41C File Offset: 0x0005C81C
		public uint LeavePenalty { get; set; }

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x0600167B RID: 5755 RVA: 0x0005E425 File Offset: 0x0005C825
		// (set) Token: 0x0600167C RID: 5756 RVA: 0x0005E42D File Offset: 0x0005C82D
		public uint MaxRatingPoints { get; set; }

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x0600167D RID: 5757 RVA: 0x0005E436 File Offset: 0x0005C836
		// (set) Token: 0x0600167E RID: 5758 RVA: 0x0005E43E File Offset: 0x0005C83E
		public uint MaxRatingLevel { get; set; }

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x0600167F RID: 5759 RVA: 0x0005E447 File Offset: 0x0005C847
		// (set) Token: 0x06001680 RID: 5760 RVA: 0x0005E44F File Offset: 0x0005C84F
		public uint TopRatingCapacity { get; set; }

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06001681 RID: 5761 RVA: 0x0005E458 File Offset: 0x0005C858
		// (set) Token: 0x06001682 RID: 5762 RVA: 0x0005E460 File Offset: 0x0005C860
		public IEnumerable<RatingLevelConfig> RatingLevelConfigs { get; set; }

		// Token: 0x06001683 RID: 5763 RVA: 0x0005E46C File Offset: 0x0005C86C
		public RatingLevelConfig GetRatingLevelConfigByPoints(uint ratingPoints)
		{
			return this.RatingLevelConfigs.LastOrDefault((RatingLevelConfig r) => ratingPoints >= r.PointsRequired);
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x0005E4A0 File Offset: 0x0005C8A0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Step={0}", this.Step));
			stringBuilder.AppendLine(string.Format("LeavePenalty={0}", this.LeavePenalty));
			stringBuilder.AppendLine(string.Format("TopRatingCapacity={0}", this.TopRatingCapacity));
			foreach (RatingLevelConfig ratingLevelConfig in this.RatingLevelConfigs)
			{
				stringBuilder.AppendLine(ratingLevelConfig.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
