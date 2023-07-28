using System;
using HK2Net;

namespace MasterServer.Core
{
	// Token: 0x0200013C RID: 316
	[Contract]
	internal interface ILogService
	{
		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06000523 RID: 1315
		// (remove) Token: 0x06000524 RID: 1316
		event Action<string> OnEvent;

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000525 RID: 1317
		ILogGroup Event { get; }

		// Token: 0x06000526 RID: 1318
		ILogGroup CreateGroup();

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000527 RID: 1319
		long EventCount { get; }
	}
}
