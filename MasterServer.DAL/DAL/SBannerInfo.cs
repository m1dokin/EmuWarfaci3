using System;

namespace MasterServer.DAL
{
	// Token: 0x02000087 RID: 135
	[Serializable]
	public struct SBannerInfo
	{
		// Token: 0x06000191 RID: 401 RVA: 0x00005029 File Offset: 0x00003429
		public SBannerInfo(uint badge, uint mark, uint stripe)
		{
			this.Badge = badge;
			this.Mark = mark;
			this.Stripe = stripe;
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00005040 File Offset: 0x00003440
		public override bool Equals(object obj)
		{
			return obj != null && obj is SBannerInfo && this.Equals((SBannerInfo)obj);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00005061 File Offset: 0x00003461
		public bool Equals(SBannerInfo b)
		{
			return b.Badge == this.Badge && b.Mark == this.Mark && b.Stripe == this.Stripe;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00005099 File Offset: 0x00003499
		public override int GetHashCode()
		{
			return (int)(this.Badge ^ this.Mark ^ this.Stripe);
		}

		// Token: 0x0400015F RID: 351
		public uint Badge;

		// Token: 0x04000160 RID: 352
		public uint Mark;

		// Token: 0x04000161 RID: 353
		public uint Stripe;
	}
}
