using System;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000C0 RID: 192
	internal class RatingGameBanConfig
	{
		// Token: 0x0600030E RID: 782 RVA: 0x0000E8CF File Offset: 0x0000CCCF
		internal RatingGameBanConfig(bool enabled, TimeSpan banTimeout)
		{
			this.BanEnabled = enabled;
			this.BanTimeout = banTimeout;
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0000E8E5 File Offset: 0x0000CCE5
		// (set) Token: 0x06000310 RID: 784 RVA: 0x0000E8ED File Offset: 0x0000CCED
		public bool BanEnabled { get; private set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0000E8F6 File Offset: 0x0000CCF6
		// (set) Token: 0x06000312 RID: 786 RVA: 0x0000E8FE File Offset: 0x0000CCFE
		public TimeSpan BanTimeout { get; private set; }
	}
}
