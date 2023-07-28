using System;

namespace MasterServer.Common
{
	// Token: 0x0200001C RID: 28
	public class EWMA
	{
		// Token: 0x0600006A RID: 106 RVA: 0x0000642D File Offset: 0x0000482D
		public EWMA(float approxRate, float timeWindow)
		{
			this.m_approxRate = approxRate;
			this.m_timeWindow = timeWindow;
			this.m_alpha = 2f / (this.m_approxRate * this.m_timeWindow + 1f);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00006464 File Offset: 0x00004864
		public float Approximate(float appr_value, float raw_value)
		{
			return this.m_alpha * raw_value + (1f - this.m_alpha) * appr_value;
		}

		// Token: 0x0400003F RID: 63
		private float m_approxRate;

		// Token: 0x04000040 RID: 64
		private float m_timeWindow;

		// Token: 0x04000041 RID: 65
		private float m_alpha;
	}
}
