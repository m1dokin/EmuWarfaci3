using System;
using System.Collections.Generic;

namespace MasterServer.XMPP
{
	// Token: 0x02000811 RID: 2065
	public class ServerLoadStats
	{
		// Token: 0x06002A4C RID: 10828 RVA: 0x000B67F3 File Offset: 0x000B4BF3
		public ServerLoadStats() : this(0, 0f, new List<Tuple<string, float>>(), new List<Tuple<string, int>>())
		{
		}

		// Token: 0x06002A4D RID: 10829 RVA: 0x000B680B File Offset: 0x000B4C0B
		public ServerLoadStats(int online, float load) : this(online, load, new List<Tuple<string, float>>(), new List<Tuple<string, int>>())
		{
		}

		// Token: 0x06002A4E RID: 10830 RVA: 0x000B681F File Offset: 0x000B4C1F
		public ServerLoadStats(int online, float load, IEnumerable<Tuple<string, float>> loadStats, IEnumerable<Tuple<string, int>> onlineStats)
		{
			this.Online = online;
			this.Load = load;
			this.LoadStats = loadStats;
			this.OnlineStats = onlineStats;
		}

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06002A4F RID: 10831 RVA: 0x000B6844 File Offset: 0x000B4C44
		// (set) Token: 0x06002A50 RID: 10832 RVA: 0x000B684C File Offset: 0x000B4C4C
		public int Online { get; private set; }

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06002A51 RID: 10833 RVA: 0x000B6855 File Offset: 0x000B4C55
		// (set) Token: 0x06002A52 RID: 10834 RVA: 0x000B685D File Offset: 0x000B4C5D
		public float Load { get; private set; }

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06002A53 RID: 10835 RVA: 0x000B6866 File Offset: 0x000B4C66
		// (set) Token: 0x06002A54 RID: 10836 RVA: 0x000B686E File Offset: 0x000B4C6E
		public IEnumerable<Tuple<string, int>> OnlineStats { get; private set; }

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06002A55 RID: 10837 RVA: 0x000B6877 File Offset: 0x000B4C77
		// (set) Token: 0x06002A56 RID: 10838 RVA: 0x000B687F File Offset: 0x000B4C7F
		public IEnumerable<Tuple<string, float>> LoadStats { get; private set; }
	}
}
