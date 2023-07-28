using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.Users;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000236 RID: 566
	[Contract]
	internal interface IItemService
	{
		// Token: 0x06000C1A RID: 3098
		RepairItemResponse RepairItem(UserInfo.User user, ulong profileItemId);

		// Token: 0x06000C1B RID: 3099
		RepairItemResponse RepairItem(UserInfo.User user, ulong profileItemId, ILogGroup logGroup);

		// Token: 0x06000C1C RID: 3100
		RepairItemResponse RepairItem(UserInfo.User user, ulong profileItemId, ulong repairCost);

		// Token: 0x06000C1D RID: 3101
		RepairEquipmentOperationResult RepairMultipleItems(UserInfo.User user, IEnumerable<SProfileItem> expiredProfileItems);

		// Token: 0x06000C1E RID: 3102
		RepairEquipmentOperationResult RepairMultipleItems(UserInfo.User user, IEnumerable<ulong> profileItems);

		// Token: 0x06000C1F RID: 3103
		void ExtendItem(UserInfo.User user, int supplierId, ulong offerId, long offerHash, ulong profileItemId);

		// Token: 0x06000C20 RID: 3104
		GiveItemResponse GivePermanentItem(ulong userId, string itemName, LogGroup.ProduceType produceType, ILogGroup logGroup, string reason = "-");

		// Token: 0x06000C21 RID: 3105
		GiveItemResponse GiveExpirableItem(ulong userId, string itemName, TimeSpan expirationTime, LogGroup.ProduceType produceType, ILogGroup logGroup, string reason = "-");

		// Token: 0x06000C22 RID: 3106
		GiveItemResponse GiveRegularItem(ulong userId, string itemName, LogGroup.ProduceType produceType, IPurchaseListener purchaseListener, ILogGroup logGroup, string reason = "-", bool ignoreLimit = false);

		// Token: 0x06000C23 RID: 3107
		GiveItemResponse GiveConsumableItem(ulong userId, string itemName, ushort amount, LogGroup.ProduceType produceType, ILogGroup logGroup, ushort maxAmount = 0, string reason = "-");

		// Token: 0x06000C24 RID: 3108
		GiveItemResponse GiveCoin(ulong userId, ushort amount, LogGroup.ProduceType produceType, ILogGroup logGroup, ushort maxAmount = 0, string reason = "-");

		// Token: 0x06000C25 RID: 3109
		ConsumeItemResponse ConsumeItem(UserInfo.User user, string serverJid, string sessionId, uint checkpoint, ulong profileItemId, ushort quantity);

		// Token: 0x06000C26 RID: 3110
		void DeleteItem(UserInfo.User user, ulong profileItemId);

		// Token: 0x06000C27 RID: 3111
		void DeleteItem(UserInfo.User user, ulong profileItemId, Action<SProfileItem> action);

		// Token: 0x06000C28 RID: 3112
		OfferItem GetOfferItemByName(string itemName);

		// Token: 0x06000C29 RID: 3113
		bool CanGiveItem(string itemName, OfferType itemType);

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x06000C2A RID: 3114
		// (remove) Token: 0x06000C2B RID: 3115
		event Action<GiveItemResponse> ItemGiven;
	}
}
