using System;
using CommandLine;

namespace MasterServer.GameLogic.VoucherSystem.ConsoleCommands
{
	// Token: 0x02000444 RID: 1092
	internal class CleanUpVoucherParams
	{
		// Token: 0x17000228 RID: 552
		// (get) Token: 0x0600174C RID: 5964 RVA: 0x00060D12 File Offset: 0x0005F112
		// (set) Token: 0x0600174D RID: 5965 RVA: 0x00060D1A File Offset: 0x0005F11A
		[Option('u', "user_id", Required = true, HelpText = "user, whom voucher would be removed")]
		public ulong UserId { get; set; }
	}
}
