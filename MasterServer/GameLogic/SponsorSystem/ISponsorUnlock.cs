using System;
using HK2Net;
using MasterServer.Users;

namespace MasterServer.GameLogic.SponsorSystem
{
	// Token: 0x020007BD RID: 1981
	[Contract]
	internal interface ISponsorUnlock
	{
		// Token: 0x140000AD RID: 173
		// (add) Token: 0x0600289B RID: 10395
		// (remove) Token: 0x0600289C RID: 10396
		event Func<UnlockItemInfo, bool> ItemUnlocked;

		// Token: 0x0600289D RID: 10397
		void ValidateProgression(ProfileProxy profile);

		// Token: 0x0600289E RID: 10398
		bool IsItemPresentInSponsors(string itemName);
	}
}
