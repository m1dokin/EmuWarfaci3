using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000338 RID: 824
	[Contract]
	internal interface IMultipleOfferValidationProvider : IDisposable
	{
		// Token: 0x0600128E RID: 4750
		void Initialize();

		// Token: 0x0600128F RID: 4751
		IEnumerable<ulong> Validate(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds);

		// Token: 0x06001290 RID: 4752
		void Confirm(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds);
	}
}
