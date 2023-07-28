using System;
using CommandLine;

namespace MasterServer.Core.Commands
{
	// Token: 0x02000031 RID: 49
	internal class HelpCmdParams
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x0000790B File Offset: 0x00005D0B
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x00007913 File Offset: 0x00005D13
		[Option('c', "contains", HelpText = "Filter commands by containing this string.")]
		public string Contains { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x0000791C File Offset: 0x00005D1C
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x00007924 File Offset: 0x00005D24
		[Option('f', "full", HelpText = "Gets help about commands parameters.", DefaultValue = false)]
		public bool Full { get; set; }
	}
}
