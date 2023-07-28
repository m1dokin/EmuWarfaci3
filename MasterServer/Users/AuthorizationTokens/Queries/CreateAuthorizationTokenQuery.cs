using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.DAL.AuthorizationTokenSystem;

namespace MasterServer.Users.AuthorizationTokens.Queries
{
	// Token: 0x020006E1 RID: 1761
	[QueryAttributes(TagName = "create_authorization_token")]
	internal class CreateAuthorizationTokenQuery : BaseQuery
	{
		// Token: 0x060024F7 RID: 9463 RVA: 0x0009A735 File Offset: 0x00098B35
		public CreateAuthorizationTokenQuery(IAuthorizationTokenService authorizationTokenService)
		{
			this.m_authorizationTokenService = authorizationTokenService;
		}

		// Token: 0x060024F8 RID: 9464 RVA: 0x0009A744 File Offset: 0x00098B44
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			AuthorizationToken authorizationToken = this.m_authorizationTokenService.RequestNewToken(user.UserID);
			response.SetAttribute("token", authorizationToken.ToString());
			return 0;
		}

		// Token: 0x040012AD RID: 4781
		private const string QueryName = "create_authorization_token";

		// Token: 0x040012AE RID: 4782
		private const string TokenAttributeName = "token";

		// Token: 0x040012AF RID: 4783
		private readonly IAuthorizationTokenService m_authorizationTokenService;
	}
}
