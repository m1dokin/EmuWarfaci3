using System;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x02000064 RID: 100
	internal class ManualRoomConfig
	{
		// Token: 0x0600017C RID: 380 RVA: 0x0000A29F File Offset: 0x0000869F
		public ManualRoomConfig(TimeSpan timeout)
		{
			this.Timeout = timeout;
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600017D RID: 381 RVA: 0x0000A2AE File Offset: 0x000086AE
		// (set) Token: 0x0600017E RID: 382 RVA: 0x0000A2B6 File Offset: 0x000086B6
		public TimeSpan Timeout { get; private set; }
	}
}
