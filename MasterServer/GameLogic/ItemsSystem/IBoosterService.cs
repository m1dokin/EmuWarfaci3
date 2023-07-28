using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000314 RID: 788
	[Contract]
	public interface IBoosterService
	{
		// Token: 0x06001207 RID: 4615
		Dictionary<BoosterType, float> GetBoosters(ulong profile_id);

		// Token: 0x06001208 RID: 4616
		bool HasVipItem(ulong profile_id);
	}
}
