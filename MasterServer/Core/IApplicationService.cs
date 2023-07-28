using System;
using HK2Net;

namespace MasterServer.Core
{
	// Token: 0x02000025 RID: 37
	[Contract]
	internal interface IApplicationService
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600007F RID: 127
		// (remove) Token: 0x06000080 RID: 128
		event Action OnShutdownScheduled;

		// Token: 0x06000081 RID: 129
		void Run();

		// Token: 0x06000082 RID: 130
		void ScheduleShutdown(TimeSpan timeout);

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000083 RID: 131
		bool IsShutdownScheduled { get; }
	}
}
