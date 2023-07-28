using System;

namespace MasterServer.DAL.AuthorizationTokenSystem
{
	// Token: 0x02000099 RID: 153
	[Serializable]
	public class AuthorizationToken : IEquatable<AuthorizationToken>
	{
		// Token: 0x060001CF RID: 463 RVA: 0x0000588D File Offset: 0x00003C8D
		public AuthorizationToken()
		{
			this.m_token = Guid.NewGuid();
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x000058A0 File Offset: 0x00003CA0
		private AuthorizationToken(Guid guid)
		{
			this.m_token = guid;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x000058AF File Offset: 0x00003CAF
		public static AuthorizationToken Parse(string src)
		{
			return new AuthorizationToken(Guid.Parse(src));
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x000058BC File Offset: 0x00003CBC
		public bool Equals(AuthorizationToken other)
		{
			return this.m_token == other.m_token;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x000058D0 File Offset: 0x00003CD0
		public override string ToString()
		{
			return this.m_token.ToString();
		}

		// Token: 0x04000187 RID: 391
		private readonly Guid m_token;
	}
}
