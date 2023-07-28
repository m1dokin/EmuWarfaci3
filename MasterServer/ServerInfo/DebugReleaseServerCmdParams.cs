using System;
using CommandLine;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006B7 RID: 1719
	internal class DebugReleaseServerCmdParams
	{
		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06002410 RID: 9232 RVA: 0x00096F5A File Offset: 0x0009535A
		// (set) Token: 0x06002411 RID: 9233 RVA: 0x00096F62 File Offset: 0x00095362
		[Option('f', "force", HelpText = "Commands MS to issue release_server request even if LDS is not currently bound to this MS. For debug purposes only.")]
		public bool ForceRelease { get; set; }

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06002412 RID: 9234 RVA: 0x00096F6B File Offset: 0x0009536B
		// (set) Token: 0x06002413 RID: 9235 RVA: 0x00096F73 File Offset: 0x00095373
		[Option('s', "server_id", DefaultValue = "", Required = true, HelpText = "Online ID of LDS to be released. If used with -f key, this LDS might not be bound to the current MS.")]
		public string ServerId { get; set; }
	}
}
