using System;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x0200078E RID: 1934
	public struct DependencyInfo
	{
		// Token: 0x0600280B RID: 10251 RVA: 0x000ABF51 File Offset: 0x000AA351
		public bool IsValid()
		{
			return this.min >= 0U && this.min <= this.full && this.full > 0U;
		}

		// Token: 0x040014F4 RID: 5364
		public uint min;

		// Token: 0x040014F5 RID: 5365
		public uint full;
	}
}
