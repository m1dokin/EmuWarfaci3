using System;
using System.Collections.Generic;

namespace MasterServer.Core
{
	// Token: 0x02000146 RID: 326
	public interface IProcessingQueue<T> : IDisposable
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060005A3 RID: 1443
		// (set) Token: 0x060005A4 RID: 1444
		int QueueLimit { get; set; }

		// Token: 0x060005A5 RID: 1445
		void Start();

		// Token: 0x060005A6 RID: 1446
		void Add(T item);

		// Token: 0x060005A7 RID: 1447
		void Add(IEnumerable<T> item);

		// Token: 0x060005A8 RID: 1448
		void Process(T item);

		// Token: 0x060005A9 RID: 1449
		void Stop();
	}
}
