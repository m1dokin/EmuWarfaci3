using System;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005CE RID: 1486
	public class SessionOutcomeFilter : IStatsFilter
	{
		// Token: 0x06001FBD RID: 8125 RVA: 0x0008172F File Offset: 0x0007FB2F
		public SessionOutcomeFilter(SessionOutcome outcome)
		{
			this.sessionOutcome = outcome;
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06001FBE RID: 8126 RVA: 0x0008173E File Offset: 0x0007FB3E
		public EStatsFilter FilterId
		{
			get
			{
				return EStatsFilter.SESSION_OUTCOME;
			}
		}

		// Token: 0x06001FBF RID: 8127 RVA: 0x00081741 File Offset: 0x0007FB41
		public bool Compare(IStatsFilter filter)
		{
			return ((SessionOutcomeFilter)filter).sessionOutcome == this.sessionOutcome;
		}

		// Token: 0x04000F81 RID: 3969
		private SessionOutcome sessionOutcome;
	}
}
