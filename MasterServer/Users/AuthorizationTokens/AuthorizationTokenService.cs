using System;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Configuration;
using MasterServer.DAL.AuthorizationTokenSystem;
using MasterServer.Database;
using NLog;

namespace MasterServer.Users.AuthorizationTokens
{
	// Token: 0x0200075D RID: 1885
	[Service]
	[Singleton]
	internal class AuthorizationTokenService : ServiceModule, IAuthorizationTokenService
	{
		// Token: 0x060026EC RID: 9964 RVA: 0x000A475E File Offset: 0x000A2B5E
		public AuthorizationTokenService(IDALService dalService, IConfigurationService configurationService)
		{
			this.m_dalService = dalService;
			this.m_configurationService = configurationService;
		}

		// Token: 0x060026ED RID: 9965 RVA: 0x000A4774 File Offset: 0x000A2B74
		public override void Init()
		{
			this.m_configSection = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration).GetSection("AuthorizationTokenRepository");
			this.m_configSection.TryGet("token_ttl_sec", out this.m_tokenTtl, AuthorizationTokenService.DefaultTokenTtl);
			this.m_configSection.OnConfigChanged += this.OnOnConfigChanged;
		}

		// Token: 0x060026EE RID: 9966 RVA: 0x000A47D4 File Offset: 0x000A2BD4
		public override void Stop()
		{
			this.m_configSection.OnConfigChanged -= this.OnOnConfigChanged;
		}

		// Token: 0x060026EF RID: 9967 RVA: 0x000A47F0 File Offset: 0x000A2BF0
		public AuthorizationToken RequestNewToken(ulong userId)
		{
			AuthorizationToken authorizationToken = new AuthorizationToken();
			AuthorizationTokenService.Logger.Debug<AuthorizationToken, ulong>("[Token service] New token '{0}' generated for user '{1}'.", authorizationToken, userId);
			this.m_dalService.AuthorizationTokenSystem.StoreToken(userId, authorizationToken, this.m_tokenTtl);
			return authorizationToken;
		}

		// Token: 0x060026F0 RID: 9968 RVA: 0x000A4830 File Offset: 0x000A2C30
		public bool ValidateToken(ulong userId, string authorizationTokenStr)
		{
			AuthorizationToken other;
			try
			{
				other = AuthorizationToken.Parse(authorizationTokenStr);
			}
			catch (FormatException)
			{
				return false;
			}
			AuthorizationToken token = this.m_dalService.AuthorizationTokenSystem.GetToken(userId);
			AuthorizationTokenService.Logger.Debug<ulong, string>("[Token service] User '{0}' requested token. Found '{1}'.", userId, authorizationTokenStr);
			return token != null && token.Equals(other);
		}

		// Token: 0x060026F1 RID: 9969 RVA: 0x000A4898 File Offset: 0x000A2C98
		private void OnOnConfigChanged(ConfigEventArgs args)
		{
			if (args.Name.Equals("token_ttl_sec", StringComparison.OrdinalIgnoreCase))
			{
				this.m_tokenTtl = args.TimeSpanValue;
			}
		}

		// Token: 0x04001403 RID: 5123
		public const string AuthorizationTokenReposytorySectionName = "AuthorizationTokenRepository";

		// Token: 0x04001404 RID: 5124
		public const string TokenTtlSectionName = "token_ttl_sec";

		// Token: 0x04001405 RID: 5125
		private static readonly TimeSpan DefaultTokenTtl = TimeSpan.FromSeconds(10.0);

		// Token: 0x04001406 RID: 5126
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		// Token: 0x04001407 RID: 5127
		private readonly IDALService m_dalService;

		// Token: 0x04001408 RID: 5128
		private readonly IConfigurationService m_configurationService;

		// Token: 0x04001409 RID: 5129
		private TimeSpan m_tokenTtl;

		// Token: 0x0400140A RID: 5130
		private ConfigSection m_configSection;
	}
}
