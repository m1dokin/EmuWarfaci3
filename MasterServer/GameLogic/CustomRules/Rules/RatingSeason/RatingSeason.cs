using System;
using MasterServer.DAL.RatingSystem;
using MasterServer.GameLogic.RatingSystem;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x02000096 RID: 150
	public class RatingSeason
	{
		// Token: 0x0600023D RID: 573 RVA: 0x0000C271 File Offset: 0x0000A671
		public RatingSeason(RatingSeasonInfo ratingSeasonInfo)
		{
			this.m_seasonActive = ratingSeasonInfo.IsActive;
			this.AnnouncementEndDate = ratingSeasonInfo.AnnouncementEndDate;
			this.GamesEndDate = ratingSeasonInfo.GamesEndDate;
			this.SeasonId = ratingSeasonInfo.SeasonId;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000C2AD File Offset: 0x0000A6AD
		private RatingSeason()
		{
			this.m_seasonActive = false;
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600023F RID: 575 RVA: 0x0000C2BC File Offset: 0x0000A6BC
		// (set) Token: 0x06000240 RID: 576 RVA: 0x0000C2C4 File Offset: 0x0000A6C4
		public string SeasonId { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000241 RID: 577 RVA: 0x0000C2CD File Offset: 0x0000A6CD
		// (set) Token: 0x06000242 RID: 578 RVA: 0x0000C2D5 File Offset: 0x0000A6D5
		public DateTime AnnouncementEndDate { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000243 RID: 579 RVA: 0x0000C2DE File Offset: 0x0000A6DE
		// (set) Token: 0x06000244 RID: 580 RVA: 0x0000C2E6 File Offset: 0x0000A6E6
		public DateTime GamesEndDate { get; private set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000245 RID: 581 RVA: 0x0000C2F0 File Offset: 0x0000A6F0
		public bool IsActive
		{
			get
			{
				bool flag = RatingSeasonRangeHelper.IsActive(this.AnnouncementEndDate, this.GamesEndDate);
				return this.m_seasonActive && flag;
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000C320 File Offset: 0x0000A720
		public override string ToString()
		{
			return string.Format("SeasonId='{0}', AnnouncementEnd(UTC)='{1}', GamesEnd(UTC)='{2}', Active='{3}'", new object[]
			{
				this.SeasonId,
				this.AnnouncementEndDate.ToString("yyyy-MM-ddTHH:mm"),
				this.GamesEndDate.ToString("yyyy-MM-ddTHH:mm"),
				this.m_seasonActive
			});
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000C380 File Offset: 0x0000A780
		public override int GetHashCode()
		{
			return this.AnnouncementEndDate.GetHashCode() ^ this.GamesEndDate.GetHashCode() ^ this.IsActive.GetHashCode();
		}

		// Token: 0x040000FE RID: 254
		public static readonly RatingSeason DisabledSeason = new RatingSeason();

		// Token: 0x040000FF RID: 255
		private readonly bool m_seasonActive;
	}
}
