using System;
using System.Threading.Tasks;
using DedicatedPoolServer.Model;
using HK2Net;

namespace MasterServer.DedicatedService
{
	// Token: 0x0200004B RID: 75
	[Contract]
	public interface IDedicatedService
	{
		// Token: 0x0600012F RID: 303
		Task<DedicatedInfo> LockDedicatedAsync(DedicatedFilter filter);

		// Token: 0x06000130 RID: 304
		Task UnlockDedicatedAsync(string dedicatedId, string masterId);
	}
}
