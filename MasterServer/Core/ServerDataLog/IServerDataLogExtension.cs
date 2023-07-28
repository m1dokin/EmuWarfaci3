using System;
using HK2Net;

namespace MasterServer.Core.ServerDataLog
{
	// Token: 0x02000128 RID: 296
	[Contract]
	internal interface IServerDataLogExtension : IDisposable
	{
		// Token: 0x060004DC RID: 1244
		void Start();

		// Token: 0x060004DD RID: 1245
		void Enable(bool enable);
	}
}
