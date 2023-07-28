using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.ElectronicCatalog;
using MasterServer.Users;

namespace MasterServer.GameLogic.VoucherSystem
{
	// Token: 0x02000484 RID: 1156
	[Contract]
	internal interface IVoucherService
	{
		// Token: 0x06001856 RID: 6230
		Task<IEnumerable<GiveItemResponse>> ProccessVoucher(UserInfo.User user);
	}
}
