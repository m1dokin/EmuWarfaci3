using System;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000DD RID: 221
	internal class Rating
	{
		// Token: 0x06000392 RID: 914 RVA: 0x0000FF5D File Offset: 0x0000E35D
		public Rating(uint level, uint points, uint winStreak, string seasonId)
		{
			this.Level = level;
			this.Points = points;
			this.WinStreak = winStreak;
			this.SeasonId = seasonId;
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000393 RID: 915 RVA: 0x0000FF82 File Offset: 0x0000E382
		// (set) Token: 0x06000394 RID: 916 RVA: 0x0000FF8A File Offset: 0x0000E38A
		public uint Level { get; private set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000395 RID: 917 RVA: 0x0000FF93 File Offset: 0x0000E393
		// (set) Token: 0x06000396 RID: 918 RVA: 0x0000FF9B File Offset: 0x0000E39B
		public uint Points { get; private set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000397 RID: 919 RVA: 0x0000FFA4 File Offset: 0x0000E3A4
		// (set) Token: 0x06000398 RID: 920 RVA: 0x0000FFAC File Offset: 0x0000E3AC
		public uint WinStreak { get; private set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000399 RID: 921 RVA: 0x0000FFB5 File Offset: 0x0000E3B5
		// (set) Token: 0x0600039A RID: 922 RVA: 0x0000FFBD File Offset: 0x0000E3BD
		public string SeasonId { get; private set; }

		// Token: 0x0600039B RID: 923 RVA: 0x0000FFC8 File Offset: 0x0000E3C8
		public override string ToString()
		{
			return string.Format("rating level = {0}, rating points = {1} (season id = {2}, win streak = {3})", new object[]
			{
				this.Level,
				this.Points,
				this.SeasonId,
				this.WinStreak
			});
		}
	}
}
