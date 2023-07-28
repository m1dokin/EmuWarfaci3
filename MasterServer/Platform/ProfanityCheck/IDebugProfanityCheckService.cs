using System;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006A3 RID: 1699
	[Contract]
	[BootstrapExplicit]
	internal interface IDebugProfanityCheckService
	{
		// Token: 0x060023AC RID: 9132
		void SetRequestFailedState(bool state);

		// Token: 0x060023AD RID: 9133
		bool GetRequestFailedState();

		// Token: 0x060023AE RID: 9134
		void SetRequestTimeout(TimeSpan timeout);

		// Token: 0x060023AF RID: 9135
		TimeSpan GetRequestTimeout();
	}
}
