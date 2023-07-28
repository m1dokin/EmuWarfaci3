using System;
using MasterServer.Core;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000789 RID: 1929
	internal class SubLevel
	{
		// Token: 0x060027F5 RID: 10229 RVA: 0x000ABC3D File Offset: 0x000AA03D
		public SubLevel(IRewardPool rewardPool)
		{
			this.pool = rewardPool;
			this.crownRewardPool = new CrownRewardPool();
		}

		// Token: 0x060027F6 RID: 10230 RVA: 0x000ABC57 File Offset: 0x000AA057
		public void Dump()
		{
			Log.Info<string>("Sub level: {0}", this.name);
			Log.Info<uint>("Id: {0}", this.id);
			this.pool.Dump();
		}

		// Token: 0x040014DA RID: 5338
		public uint id;

		// Token: 0x040014DB RID: 5339
		public string name;

		// Token: 0x040014DC RID: 5340
		public string flow;

		// Token: 0x040014DD RID: 5341
		public IRewardPool pool;

		// Token: 0x040014DE RID: 5342
		public CrownRewardPool crownRewardPool;
	}
}
