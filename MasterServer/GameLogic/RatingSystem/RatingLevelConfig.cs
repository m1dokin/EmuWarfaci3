using System;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x02000414 RID: 1044
	public class RatingLevelConfig
	{
		// Token: 0x06001685 RID: 5765 RVA: 0x0005E57B File Offset: 0x0005C97B
		public RatingLevelConfig(uint level, uint pointsRequired, uint adjustment)
		{
			this.Level = level;
			this.PointsRequired = pointsRequired;
			this.Adjustment = adjustment;
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06001686 RID: 5766 RVA: 0x0005E598 File Offset: 0x0005C998
		// (set) Token: 0x06001687 RID: 5767 RVA: 0x0005E5A0 File Offset: 0x0005C9A0
		public uint Level { get; private set; }

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06001688 RID: 5768 RVA: 0x0005E5A9 File Offset: 0x0005C9A9
		// (set) Token: 0x06001689 RID: 5769 RVA: 0x0005E5B1 File Offset: 0x0005C9B1
		public uint PointsRequired { get; private set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x0600168A RID: 5770 RVA: 0x0005E5BA File Offset: 0x0005C9BA
		// (set) Token: 0x0600168B RID: 5771 RVA: 0x0005E5C2 File Offset: 0x0005C9C2
		public uint Adjustment { get; private set; }

		// Token: 0x0600168C RID: 5772 RVA: 0x0005E5CC File Offset: 0x0005C9CC
		public override string ToString()
		{
			return string.Format("Level_{0} PointsRequired={1}, Adjustment={2}", this.Level, this.PointsRequired, this.Adjustment);
		}
	}
}
