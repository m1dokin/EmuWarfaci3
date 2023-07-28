using System;
using System.Collections.Concurrent;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x0200029C RID: 668
	internal class SessionContracts
	{
		// Token: 0x040006A4 RID: 1700
		public readonly ConcurrentDictionary<ulong, SessionContracts.SessionInfo> Contracts = new ConcurrentDictionary<ulong, SessionContracts.SessionInfo>();

		// Token: 0x0200029D RID: 669
		public class SessionInfo
		{
			// Token: 0x06000E6C RID: 3692 RVA: 0x0003A1F8 File Offset: 0x000385F8
			public SessionInfo(SProfileItem item, ProfileContract contract)
			{
				this.ContractProfileItem = item;
				this.Contract = contract;
				this.Multiplier = default(SRewardMultiplier);
			}

			// Token: 0x040006A5 RID: 1701
			public SProfileItem ContractProfileItem;

			// Token: 0x040006A6 RID: 1702
			public ProfileContract Contract;

			// Token: 0x040006A7 RID: 1703
			public SRewardMultiplier Multiplier;
		}
	}
}
