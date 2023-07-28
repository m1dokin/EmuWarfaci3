using System;
using CommandLine;

namespace MasterServer.GameLogic.VoucherSystem.ConsoleCommands
{
	// Token: 0x02000447 RID: 1095
	internal class SynchronizeCorruptedVoucherParams
	{
		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06001754 RID: 5972 RVA: 0x00060DD8 File Offset: 0x0005F1D8
		// (set) Token: 0x06001755 RID: 5973 RVA: 0x00060DE0 File Offset: 0x0005F1E0
		[Option('i', "index", Required = true, HelpText = "Index for synchronization")]
		public ulong IndexValue { get; set; }
	}
}
