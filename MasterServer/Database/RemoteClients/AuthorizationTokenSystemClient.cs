using System;
using MasterServer.DAL;
using MasterServer.DAL.AuthorizationTokenSystem;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000041 RID: 65
	internal class AuthorizationTokenSystemClient : DALCacheProxy<IDALService>, IAuthorizationTokenSystemClient
	{
		// Token: 0x0600010A RID: 266 RVA: 0x000093E4 File Offset: 0x000077E4
		public void StoreToken(ulong userId, AuthorizationToken authorizationToken, TimeSpan tokenTtl)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<AuthorizationToken> options = new DALCacheProxy<IDALService>.SetOptionsScalar<AuthorizationToken>
			{
				cache_domain = cache_domains.user[userId].token,
				cache_expiration = tokenTtl,
				set_func = (() => new DALResult<AuthorizationToken>(authorizationToken, new DALStats()))
			};
			base.PureStore<AuthorizationToken>(options, "StoreToken");
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00009444 File Offset: 0x00007844
		public AuthorizationToken GetToken(ulong userId)
		{
			DALCacheProxy<IDALService>.Options<AuthorizationToken> options = new DALCacheProxy<IDALService>.Options<AuthorizationToken>();
			options.cache_domain = cache_domains.user[userId].token;
			options.get_data = (() => null);
			DALCacheProxy<IDALService>.Options<AuthorizationToken> options2 = options;
			return base.PureGetData<AuthorizationToken>(options2, "GetToken");
		}
	}
}
