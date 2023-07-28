using System;
using MasterServer.Core;
using MasterServer.DAL.AuthorizationTokenSystem;
using NLog;

namespace MasterServer.Users.AuthorizationTokens.Commands
{
	// Token: 0x020006DF RID: 1759
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_create_authorization_token", Help = "Create authorization token for user specified by user_id")]
	internal class DebugCreateAuthorizationTokenCmd : ConsoleCommand<DebugCreateAuthorizationTokenCmdParams>
	{
		// Token: 0x060024F1 RID: 9457 RVA: 0x0009A6A4 File Offset: 0x00098AA4
		public DebugCreateAuthorizationTokenCmd(IAuthorizationTokenService authorizationTokenService)
		{
			this.m_authorizationTokenService = authorizationTokenService;
		}

		// Token: 0x060024F2 RID: 9458 RVA: 0x0009A6B4 File Offset: 0x00098AB4
		protected override void Execute(DebugCreateAuthorizationTokenCmdParams param)
		{
			try
			{
				AuthorizationToken argument = this.m_authorizationTokenService.RequestNewToken(param.UserId);
				DebugCreateAuthorizationTokenCmd.Logger.Info<AuthorizationToken, ulong>("[debug_create_token] Token '{0}' created for user '{1}'", argument, param.UserId);
			}
			catch (Exception value)
			{
				DebugCreateAuthorizationTokenCmd.Logger.Error<Exception>(value);
			}
		}

		// Token: 0x040012AA RID: 4778
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		// Token: 0x040012AB RID: 4779
		private readonly IAuthorizationTokenService m_authorizationTokenService;
	}
}
