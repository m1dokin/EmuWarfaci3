using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020007B3 RID: 1971
	internal struct SponsorDataUpdate
	{
		// Token: 0x04001540 RID: 5440
		public uint sponsorId;

		// Token: 0x04001541 RID: 5441
		public ulong totalSponsorPoints;

		// Token: 0x04001542 RID: 5442
		public ulong nextUnlockItemId;

		// Token: 0x04001543 RID: 5443
		public List<SponsorDataUpdate.ItemIDs> unlockedItems;

		// Token: 0x020007B4 RID: 1972
		public struct ItemIDs
		{
			// Token: 0x0600288D RID: 10381 RVA: 0x000AE5D8 File Offset: 0x000AC9D8
			public ItemIDs(ulong _inventoryId, ulong _profileItemId)
			{
				this.inventoryId = _inventoryId;
				this.profileItemId = _profileItemId;
			}

			// Token: 0x04001544 RID: 5444
			public ulong inventoryId;

			// Token: 0x04001545 RID: 5445
			public ulong profileItemId;
		}
	}
}
