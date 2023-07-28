using System;
using CommandLine;

namespace MasterServer.Core.Logging.Commands
{
	// Token: 0x02000106 RID: 262
	internal class SetLogLevelCmdParams
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x00012798 File Offset: 0x00010B98
		// (set) Token: 0x06000446 RID: 1094 RVA: 0x000127A0 File Offset: 0x00010BA0
		[Option('l', "level", Required = true, HelpText = "Log level. Possible values are Fatal, Error, Warn, Info, Debug, Trace.")]
		public string Level { get; set; }
	}
}
