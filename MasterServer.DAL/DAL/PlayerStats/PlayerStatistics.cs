using System;
using System.Collections.Generic;
using MasterServer.DAL.Utils;
using OLAPHypervisor;

namespace MasterServer.DAL.PlayerStats
{
	// Token: 0x0200003B RID: 59
	[Serializable]
	public class PlayerStatistics
	{
		// Token: 0x0400008A RID: 138
		public DBVersion Version;

		// Token: 0x0400008B RID: 139
		public List<Measure> Measures;
	}
}
