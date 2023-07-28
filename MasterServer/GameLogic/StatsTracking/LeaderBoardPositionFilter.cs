using System;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005CF RID: 1487
	public class LeaderBoardPositionFilter : IStatsFilter
	{
		// Token: 0x06001FC0 RID: 8128 RVA: 0x00081756 File Offset: 0x0007FB56
		public LeaderBoardPositionFilter(uint position)
		{
			this.lbPosition = position;
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x06001FC1 RID: 8129 RVA: 0x00081765 File Offset: 0x0007FB65
		public EStatsFilter FilterId
		{
			get
			{
				return EStatsFilter.LEADERBOADR_POSITION;
			}
		}

		// Token: 0x06001FC2 RID: 8130 RVA: 0x00081768 File Offset: 0x0007FB68
		public bool Compare(IStatsFilter filter)
		{
			return ((LeaderBoardPositionFilter)filter).lbPosition >= this.lbPosition;
		}

		// Token: 0x04000F82 RID: 3970
		private uint lbPosition;
	}
}
