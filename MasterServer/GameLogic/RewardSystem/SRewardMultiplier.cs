using System;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005A5 RID: 1445
	public struct SRewardMultiplier
	{
		// Token: 0x06001F0B RID: 7947 RVA: 0x0007E294 File Offset: 0x0007C694
		public static SRewardMultiplier operator *(SRewardMultiplier multiplierA, SRewardMultiplier multiplierB)
		{
			if (multiplierA.IsEmpty() || multiplierB.IsEmpty())
			{
				if (multiplierA.IsEmpty())
				{
					return (!multiplierB.IsEmpty()) ? multiplierB : SRewardMultiplier.Empty;
				}
				if (multiplierB.IsEmpty())
				{
					return (!multiplierA.IsEmpty()) ? multiplierA : SRewardMultiplier.Empty;
				}
			}
			return new SRewardMultiplier
			{
				MoneyMultiplier = multiplierA.MoneyMultiplier * multiplierB.MoneyMultiplier,
				ExperienceMultiplier = multiplierA.ExperienceMultiplier * multiplierB.ExperienceMultiplier,
				SponsorPointsMultiplier = multiplierA.SponsorPointsMultiplier * multiplierB.SponsorPointsMultiplier,
				CrownMultiplier = multiplierA.CrownMultiplier * multiplierB.CrownMultiplier,
				Description = string.Join(" ", new string[]
				{
					multiplierA.Description ?? string.Empty,
					multiplierB.Description ?? string.Empty
				}),
				ProviderID = string.Join(" | ", new string[]
				{
					multiplierA.ProviderID ?? string.Empty,
					multiplierB.ProviderID ?? string.Empty
				})
			};
		}

		// Token: 0x06001F0C RID: 7948 RVA: 0x0007E3E8 File Offset: 0x0007C7E8
		public bool IsEmpty()
		{
			return this.MoneyMultiplier == 1f && this.ExperienceMultiplier == 1f && this.SponsorPointsMultiplier == 1f && this.CrownMultiplier == 1f;
		}

		// Token: 0x06001F0D RID: 7949 RVA: 0x0007E438 File Offset: 0x0007C838
		public override string ToString()
		{
			return string.Format("money_multiplier='{0}' experience_multiplier='{1}' sponsor_points_multiplier='{2}' crown_multiplier='{3}' description='{4}' provider ID='{5}'", new object[]
			{
				this.MoneyMultiplier,
				this.ExperienceMultiplier,
				this.SponsorPointsMultiplier,
				this.CrownMultiplier,
				this.Description,
				this.ProviderID
			});
		}

		// Token: 0x04000F20 RID: 3872
		public float MoneyMultiplier;

		// Token: 0x04000F21 RID: 3873
		public float ExperienceMultiplier;

		// Token: 0x04000F22 RID: 3874
		public float SponsorPointsMultiplier;

		// Token: 0x04000F23 RID: 3875
		public float CrownMultiplier;

		// Token: 0x04000F24 RID: 3876
		public string Description;

		// Token: 0x04000F25 RID: 3877
		public string ProviderID;

		// Token: 0x04000F26 RID: 3878
		public static readonly SRewardMultiplier Empty = new SRewardMultiplier
		{
			MoneyMultiplier = 1f,
			ExperienceMultiplier = 1f,
			SponsorPointsMultiplier = 1f,
			CrownMultiplier = 1f,
			Description = string.Empty,
			ProviderID = string.Empty
		};
	}
}
