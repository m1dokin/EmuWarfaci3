using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000354 RID: 852
	[Contract]
	internal interface IItemsExpiration
	{
		// Token: 0x0600131E RID: 4894
		void ExpireItemsByDate(ProfileProxy profile);

		// Token: 0x0600131F RID: 4895
		bool ExpireItem(ulong user_id, ulong profileId, ulong itemId);

		// Token: 0x06001320 RID: 4896
		void UnequipItem(ulong profileId, ulong itemId);

		// Token: 0x06001321 RID: 4897
		void ExpireItems(ClassPresenceData args);

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x06001322 RID: 4898
		// (remove) Token: 0x06001323 RID: 4899
		event ItemExpiredDelegate OnItemExpired;

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06001324 RID: 4900
		// (remove) Token: 0x06001325 RID: 4901
		event Action<UserInfo.User, string, IList<SProfileItem>> OnGotBrokenPermanentItems;
	}
}
