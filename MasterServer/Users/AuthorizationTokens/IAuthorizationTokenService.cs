using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;
using MasterServer.DAL.AuthorizationTokenSystem;

namespace MasterServer.Users.AuthorizationTokens
{
	// Token: 0x0200075C RID: 1884
	[Contract]
	[BootstrapExplicit]
	public interface IAuthorizationTokenService
	{
		// Token: 0x060026EA RID: 9962
		AuthorizationToken RequestNewToken(ulong userId);

		// Token: 0x060026EB RID: 9963
		bool ValidateToken(ulong userId, string authorizationTokenStr);
	}
}
