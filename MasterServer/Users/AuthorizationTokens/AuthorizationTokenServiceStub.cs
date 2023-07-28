using System;
using HK2Net;
using MasterServer.DAL.AuthorizationTokenSystem;

namespace MasterServer.Users.AuthorizationTokens
{
	// Token: 0x020006DE RID: 1758
	[Service]
	[Singleton]
	public class AuthorizationTokenServiceStub : IAuthorizationTokenService
	{
		// Token: 0x17000396 RID: 918
		// (get) Token: 0x060024EE RID: 9454 RVA: 0x0009A68F File Offset: 0x00098A8F
		public TimeSpan TokenTtl
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x060024EF RID: 9455 RVA: 0x0009A696 File Offset: 0x00098A96
		public AuthorizationToken RequestNewToken(ulong userId)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060024F0 RID: 9456 RVA: 0x0009A69D File Offset: 0x00098A9D
		public bool ValidateToken(ulong userId, string authorizationTokenStr)
		{
			throw new NotImplementedException();
		}
	}
}
