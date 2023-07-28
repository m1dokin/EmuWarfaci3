using System;
using CommandLine;

namespace MasterServer.GameLogic.VoucherSystem.ConsoleCommands
{
	// Token: 0x02000442 RID: 1090
	internal class ChangeVoucherCurrentIndexParams
	{
		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06001747 RID: 5959 RVA: 0x00060CD7 File Offset: 0x0005F0D7
		// (set) Token: 0x06001748 RID: 5960 RVA: 0x00060CDF File Offset: 0x0005F0DF
		[Option('i', "index", Required = true, HelpText = "Index value")]
		public ulong IndexValue { get; set; }
	}
}
