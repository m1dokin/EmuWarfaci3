using System;
using CommandLine;

namespace MasterServer.Users.AuthorizationTokens.Commands
{
	// Token: 0x020006E0 RID: 1760
	public class DebugCreateAuthorizationTokenCmdParams
	{
		// Token: 0x17000397 RID: 919
		// (get) Token: 0x060024F5 RID: 9461 RVA: 0x0009A724 File Offset: 0x00098B24
		// (set) Token: 0x060024F6 RID: 9462 RVA: 0x0009A72C File Offset: 0x00098B2C
		[Option('u', "user_id", HelpText = "User Id", Required = true)]
		public ulong UserId { get; set; }
	}
}
