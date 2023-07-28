using System;
using DedicatedPoolServer.Model;
using Network.Interfaces;

namespace MasterServer.DedicatedService
{
	// Token: 0x02000048 RID: 72
	public class DedicatedControlResponse : IRemoteResponse, IRemoteMessage, IDisposable
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000122 RID: 290 RVA: 0x0000986D File Offset: 0x00007C6D
		// (set) Token: 0x06000123 RID: 291 RVA: 0x00009875 File Offset: 0x00007C75
		public DedicatedInfo Info { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000124 RID: 292 RVA: 0x0000987E File Offset: 0x00007C7E
		public Uri Url
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00009881 File Offset: 0x00007C81
		public void Dispose()
		{
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00009883 File Offset: 0x00007C83
		public override string ToString()
		{
			return this.Info.Dump();
		}
	}
}
