using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;

namespace MasterServer.Database
{
	// Token: 0x020001E0 RID: 480
	[ConsoleCmdAttributes(CmdName = "get_memcached_servers", ArgsSize = 0)]
	internal class GetMemcachedServersCmd : IConsoleCmd
	{
		// Token: 0x06000944 RID: 2372 RVA: 0x00022C85 File Offset: 0x00021085
		public GetMemcachedServersCmd(IMemcachedService memcachedService)
		{
			this.m_memcachedService = memcachedService;
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x00022C94 File Offset: 0x00021094
		public void ExecuteCmd(string[] args)
		{
			List<MemcachedServer> serverList = this.m_memcachedService.GetServerList();
			StringBuilder stringBuilder = new StringBuilder(10);
			foreach (MemcachedServer memcachedServer in serverList)
			{
				stringBuilder.Append(string.Concat(new object[]
				{
					memcachedServer.m_address,
					":",
					memcachedServer.m_port,
					" ",
					memcachedServer.m_active,
					'\n'
				}));
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x04000548 RID: 1352
		private readonly IMemcachedService m_memcachedService;
	}
}
