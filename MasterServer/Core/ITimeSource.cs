using System;
using HK2Net;

namespace MasterServer.Core
{
	// Token: 0x02000164 RID: 356
	[Contract]
	public interface ITimeSource
	{
		// Token: 0x06000663 RID: 1635
		DateTime Now();

		// Token: 0x06000664 RID: 1636
		DateTime UtcNow();
	}
}
