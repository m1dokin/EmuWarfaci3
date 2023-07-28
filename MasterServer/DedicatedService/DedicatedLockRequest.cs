using System;
using DedicatedPoolServer.Model;
using Network;

namespace MasterServer.DedicatedService
{
	// Token: 0x02000049 RID: 73
	[Domain("dps.control.lock")]
	public class DedicatedLockRequest
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00009898 File Offset: 0x00007C98
		// (set) Token: 0x06000129 RID: 297 RVA: 0x000098A0 File Offset: 0x00007CA0
		public DedicatedFilter Filter { get; set; }
	}
}
