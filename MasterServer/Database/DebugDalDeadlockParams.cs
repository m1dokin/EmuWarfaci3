using System;
using CommandLine;

namespace MasterServer.Database
{
	// Token: 0x020001D8 RID: 472
	internal class DebugDalDeadlockParams
	{
		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000900 RID: 2304 RVA: 0x000223D2 File Offset: 0x000207D2
		// (set) Token: 0x06000901 RID: 2305 RVA: 0x000223DA File Offset: 0x000207DA
		[Option('n', "name", Required = true, HelpText = "query name")]
		public string QueryName { get; set; }

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000902 RID: 2306 RVA: 0x000223E3 File Offset: 0x000207E3
		// (set) Token: 0x06000903 RID: 2307 RVA: 0x000223EB File Offset: 0x000207EB
		[Option('r', "retries", Required = true, HelpText = "retries count")]
		public int RetriesCount { get; set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000904 RID: 2308 RVA: 0x000223F4 File Offset: 0x000207F4
		// (set) Token: 0x06000905 RID: 2309 RVA: 0x000223FC File Offset: 0x000207FC
		[Option('c', "count", Required = false, DefaultValue = 2147483647, HelpText = "number of blocked requests")]
		public int Count { get; set; }
	}
}
