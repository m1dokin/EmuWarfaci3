using System;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x0200009A RID: 154
	public class RatingWinStreakConfig
	{
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000253 RID: 595 RVA: 0x0000C446 File Offset: 0x0000A846
		// (set) Token: 0x06000254 RID: 596 RVA: 0x0000C44E File Offset: 0x0000A84E
		public bool Enabled { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000255 RID: 597 RVA: 0x0000C457 File Offset: 0x0000A857
		// (set) Token: 0x06000256 RID: 598 RVA: 0x0000C45F File Offset: 0x0000A85F
		public uint BonusAmount { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000257 RID: 599 RVA: 0x0000C468 File Offset: 0x0000A868
		// (set) Token: 0x06000258 RID: 600 RVA: 0x0000C470 File Offset: 0x0000A870
		public uint StartFromStreak { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000259 RID: 601 RVA: 0x0000C479 File Offset: 0x0000A879
		// (set) Token: 0x0600025A RID: 602 RVA: 0x0000C481 File Offset: 0x0000A881
		public uint ApplyBelowRating { get; set; }

		// Token: 0x0600025B RID: 603 RVA: 0x0000C48C File Offset: 0x0000A88C
		public override string ToString()
		{
			return string.Format("Enabled='{0}', BonusAmount='{1}', StartFromStreak='{2}', ApplyBelowRating='{3}'", new object[]
			{
				this.Enabled,
				this.BonusAmount,
				this.StartFromStreak,
				this.ApplyBelowRating
			});
		}
	}
}
