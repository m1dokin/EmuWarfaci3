using System;
using CommandLine;

namespace MasterServer.GameLogic.VoucherSystem.ConsoleCommands
{
	// Token: 0x02000445 RID: 1093
	internal class DumpVouchersParams
	{
		// Token: 0x17000229 RID: 553
		// (get) Token: 0x0600174F RID: 5967 RVA: 0x00060D2B File Offset: 0x0005F12B
		// (set) Token: 0x06001750 RID: 5968 RVA: 0x00060D33 File Offset: 0x0005F133
		[Option('u', "user_id", Required = true, HelpText = "User id")]
		public ulong UserId { get; set; }
	}
}
