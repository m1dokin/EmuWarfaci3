using System;
using HK2Net;
using MasterServer.GameLogic.RatingSystem;

namespace MasterServer.GameLogic.SkillSystem.Providers
{
	// Token: 0x02000433 RID: 1075
	[Service]
	[Singleton]
	internal class RatingSkillProvider : ISkillProvider
	{
		// Token: 0x060016FB RID: 5883 RVA: 0x0005FDC7 File Offset: 0x0005E1C7
		public RatingSkillProvider(IRatingSeasonService ratingSeasonService, IRatingService ratingService)
		{
			this.m_ratingSeasonService = ratingSeasonService;
			this.m_ratingService = ratingService;
			this.SkillType = SkillType.Rating;
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x060016FC RID: 5884 RVA: 0x0005FDE4 File Offset: 0x0005E1E4
		// (set) Token: 0x060016FD RID: 5885 RVA: 0x0005FDEC File Offset: 0x0005E1EC
		public SkillType SkillType { get; private set; }

		// Token: 0x060016FE RID: 5886 RVA: 0x0005FDF8 File Offset: 0x0005E1F8
		public Skill GetSkill(ulong profileId)
		{
			Rating playerRating = this.m_ratingSeasonService.GetPlayerRating(profileId);
			return new Skill(this.SkillType, playerRating.Level, 0.0);
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x0005FE2E File Offset: 0x0005E22E
		public double GetMaxSkillValue()
		{
			return this.m_ratingService.MaxRatingLevel;
		}

		// Token: 0x04000B16 RID: 2838
		private readonly IRatingSeasonService m_ratingSeasonService;

		// Token: 0x04000B17 RID: 2839
		private readonly IRatingService m_ratingService;
	}
}
