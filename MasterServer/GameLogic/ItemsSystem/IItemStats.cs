using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.ContractSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200036C RID: 876
	[Contract]
	internal interface IItemStats
	{
		// Token: 0x06001386 RID: 4998
		Dictionary<ulong, StackableItemStats> GetStackableItemStats();

		// Token: 0x06001387 RID: 4999
		StackableItemStats GetStackableItemStats(ulong itemId);

		// Token: 0x06001388 RID: 5000
		bool IsVipItem(ulong item_id);

		// Token: 0x06001389 RID: 5001
		bool IsBoosterItem(ulong item_id);

		// Token: 0x0600138A RID: 5002
		RandomBoxDesc GetRandomBoxDesc(ulong item_id);

		// Token: 0x0600138B RID: 5003
		BundleDesc GetBundleDesc(ulong itemId);

		// Token: 0x0600138C RID: 5004
		BoosterDesc GetBoosterDesc(ulong item_id);

		// Token: 0x0600138D RID: 5005
		Dictionary<uint, List<ContractDesc>> GetContractsDesc();

		// Token: 0x0600138E RID: 5006
		IList<RandomBoxDesc> GetRandomBoxesDesc();

		// Token: 0x0600138F RID: 5007
		IList<BundleDesc> GetBundlesDesc();

		// Token: 0x06001390 RID: 5008
		ContractDesc GetContractByName(string name);

		// Token: 0x06001391 RID: 5009
		TaggedItemDesc GetTaggedItemDesc(ulong itemId);

		// Token: 0x06001392 RID: 5010
		bool IsItemAvailableForUser(string itemName, UserInfo.User user);

		// Token: 0x06001393 RID: 5011
		bool IsItemAvailableForUser(ulong itemId, UserInfo.User user);

		// Token: 0x06001394 RID: 5012
		SItem GetAccessItemByMisisonType(string missionType);

		// Token: 0x06001395 RID: 5013
		MetaGameDesc GetMetaGameDesc(ulong itemId);

		// Token: 0x06001396 RID: 5014
		IEnumerable<MetaGameDesc> GetMetaGameDescs();

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x06001397 RID: 5015
		// (remove) Token: 0x06001398 RID: 5016
		event Action ItemStatsUpdated;
	}
}
