using System;
using Network;
using Newtonsoft.Json;

namespace MasterServer.DedicatedService
{
	// Token: 0x0200004A RID: 74
	[Domain("dps.control.unlock")]
	public class DedicatedUnlockRequest
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600012B RID: 299 RVA: 0x000098B1 File Offset: 0x00007CB1
		// (set) Token: 0x0600012C RID: 300 RVA: 0x000098B9 File Offset: 0x00007CB9
		[JsonProperty]
		public string DedicatedId { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600012D RID: 301 RVA: 0x000098C2 File Offset: 0x00007CC2
		// (set) Token: 0x0600012E RID: 302 RVA: 0x000098CA File Offset: 0x00007CCA
		[JsonProperty]
		public string MasterId { get; set; }
	}
}
