using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000337 RID: 823
	[Contract]
	internal interface IShopBuyMultipleOfferValidation
	{
		// Token: 0x0600128C RID: 4748
		IEnumerable<ulong> Validate(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds);

		// Token: 0x0600128D RID: 4749
		void Confirm(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds);
	}
}
