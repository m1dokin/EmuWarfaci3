using System;
using HK2Net;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006C3 RID: 1731
	[Contract]
	public interface IMemoryUsageCollector
	{
		// Token: 0x06002446 RID: 9286
		string GetMemoryUsageInfo(object o);
	}
}
