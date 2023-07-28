using System;
using MasterServer.Core;

namespace MasterServer.Database
{
	// Token: 0x020001DF RID: 479
	[ConsoleCmdAttributes(CmdName = "flush_memcached", ArgsSize = 0)]
	internal class FlushMemcachedCmd : IConsoleCmd
	{
		// Token: 0x06000942 RID: 2370 RVA: 0x00022C5F File Offset: 0x0002105F
		public FlushMemcachedCmd(IMemcachedService memcachedService)
		{
			this.m_memcachedService = memcachedService;
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x00022C6E File Offset: 0x0002106E
		public void ExecuteCmd(string[] args)
		{
			this.m_memcachedService.Clear();
			Log.Info("Memcached data flushed");
		}

		// Token: 0x04000547 RID: 1351
		private readonly IMemcachedService m_memcachedService;
	}
}
