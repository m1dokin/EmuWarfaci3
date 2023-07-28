using System;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005CD RID: 1485
	public interface IStatsFilter
	{
		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06001FBB RID: 8123
		EStatsFilter FilterId { get; }

		// Token: 0x06001FBC RID: 8124
		bool Compare(IStatsFilter filter);
	}
}
