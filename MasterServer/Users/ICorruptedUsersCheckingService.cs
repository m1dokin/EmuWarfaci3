using System;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x0200074E RID: 1870
	[Contract]
	internal interface ICorruptedUsersCheckingService
	{
		// Token: 0x0600269F RID: 9887
		void PerformCheck(TimeSpan untouchedForCheck);
	}
}
