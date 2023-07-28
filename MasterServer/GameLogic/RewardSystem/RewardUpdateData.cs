using System;
using MasterServer.DAL;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020007B2 RID: 1970
	internal class RewardUpdateData
	{
		// Token: 0x0400153C RID: 5436
		public string nickname;

		// Token: 0x0400153D RID: 5437
		public Guid mission;

		// Token: 0x0400153E RID: 5438
		public MissionStatus status;

		// Token: 0x0400153F RID: 5439
		public RewardOutputData rewards;
	}
}
