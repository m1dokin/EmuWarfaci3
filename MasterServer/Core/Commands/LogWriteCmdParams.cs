using System;
using CommandLine;

namespace MasterServer.Core.Commands
{
	// Token: 0x02000033 RID: 51
	internal class LogWriteCmdParams
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x0000794A File Offset: 0x00005D4A
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x00007952 File Offset: 0x00005D52
		[Option('t', "text", Required = true, HelpText = "Text to write in console")]
		public string Text { get; set; }
	}
}
