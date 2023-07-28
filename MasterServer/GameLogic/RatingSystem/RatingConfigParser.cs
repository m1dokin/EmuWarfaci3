using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000CA RID: 202
	public class RatingConfigParser
	{
		// Token: 0x06000343 RID: 835 RVA: 0x0000F0CC File Offset: 0x0000D4CC
		public RatingConfig Parse(Config ratingConfigData)
		{
			uint step = this.ParseConfigAttribute(ratingConfigData, "step");
			uint num = this.ParseConfigAttribute(ratingConfigData, "top_rating_capacity");
			uint leavePenalty = this.ParseConfigAttribute(ratingConfigData, "leave_penalty");
			List<RatingConfigParser.RatingLevelAttributes> list = this.ParseRatingLevelConfigAttributes(ratingConfigData.GetSections("rating"));
			List<RatingLevelConfig> list2 = new List<RatingLevelConfig>();
			for (int i = 0; i < list.Count; i++)
			{
				RatingConfigParser.RatingLevelAttributes ratingLevelAttributes = list[i];
				list2.Add(new RatingLevelConfig((uint)i, ratingLevelAttributes.PointsRequired, ratingLevelAttributes.Adjustment));
			}
			uint maxRatingPoints = list2.Max((RatingLevelConfig x) => x.PointsRequired) + num;
			uint maxRatingLevel = list2.Max((RatingLevelConfig x) => x.Level);
			return new RatingConfig
			{
				Step = step,
				LeavePenalty = leavePenalty,
				TopRatingCapacity = num,
				RatingLevelConfigs = list2,
				MaxRatingPoints = maxRatingPoints,
				MaxRatingLevel = maxRatingLevel
			};
		}

		// Token: 0x06000344 RID: 836 RVA: 0x0000F1E8 File Offset: 0x0000D5E8
		private uint ParseConfigAttribute(Config ratingConfigData, string attributeName)
		{
			uint result;
			ratingConfigData.Get(attributeName, out result);
			return result;
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0000F200 File Offset: 0x0000D600
		private List<RatingConfigParser.RatingLevelAttributes> ParseRatingLevelConfigAttributes(IEnumerable<ConfigSection> ratingLevelsConfigData)
		{
			List<RatingConfigParser.RatingLevelAttributes> list = new List<RatingConfigParser.RatingLevelAttributes>();
			foreach (ConfigSection configSection in ratingLevelsConfigData)
			{
				uint maxLevelPoints;
				configSection.Get("points_required", out maxLevelPoints);
				uint adjustment;
				configSection.Get("adjustment", out adjustment);
				list.Add(new RatingConfigParser.RatingLevelAttributes(maxLevelPoints, adjustment));
			}
			list.Sort((RatingConfigParser.RatingLevelAttributes a, RatingConfigParser.RatingLevelAttributes b) => (a.PointsRequired < b.PointsRequired) ? -1 : 1);
			return list;
		}

		// Token: 0x020000CB RID: 203
		private class RatingLevelAttributes
		{
			// Token: 0x06000349 RID: 841 RVA: 0x0000F2CA File Offset: 0x0000D6CA
			public RatingLevelAttributes(uint maxLevelPoints, uint adjustment)
			{
				this.PointsRequired = maxLevelPoints;
				this.Adjustment = adjustment;
			}

			// Token: 0x17000076 RID: 118
			// (get) Token: 0x0600034A RID: 842 RVA: 0x0000F2E0 File Offset: 0x0000D6E0
			// (set) Token: 0x0600034B RID: 843 RVA: 0x0000F2E8 File Offset: 0x0000D6E8
			public uint PointsRequired { get; private set; }

			// Token: 0x17000077 RID: 119
			// (get) Token: 0x0600034C RID: 844 RVA: 0x0000F2F1 File Offset: 0x0000D6F1
			// (set) Token: 0x0600034D RID: 845 RVA: 0x0000F2F9 File Offset: 0x0000D6F9
			public uint Adjustment { get; private set; }
		}
	}
}
