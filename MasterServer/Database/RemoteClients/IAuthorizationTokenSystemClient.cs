using System;
using MasterServer.DAL.AuthorizationTokenSystem;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000040 RID: 64
	public interface IAuthorizationTokenSystemClient
	{
		// Token: 0x06000107 RID: 263
		void StoreToken(ulong userId, AuthorizationToken authorizationToken, TimeSpan tokenTtl);

		// Token: 0x06000108 RID: 264
		AuthorizationToken GetToken(ulong userId);
	}
}
