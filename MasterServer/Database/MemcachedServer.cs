using System;

namespace MasterServer.Database
{
	// Token: 0x020001DB RID: 475
	public struct MemcachedServer
	{
		// Token: 0x06000920 RID: 2336 RVA: 0x00022577 File Offset: 0x00020977
		public MemcachedServer(string addr, int port, bool active)
		{
			this.m_address = addr;
			this.m_port = port;
			this.m_active = active;
		}

		// Token: 0x04000540 RID: 1344
		public string m_address;

		// Token: 0x04000541 RID: 1345
		public int m_port;

		// Token: 0x04000542 RID: 1346
		public bool m_active;
	}
}
