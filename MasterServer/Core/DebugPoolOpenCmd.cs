using System;
using System.Collections.Generic;
using MasterServer.Database;

namespace MasterServer.Core
{
	// Token: 0x020007AE RID: 1966
	[ConsoleCmdAttributes(CmdName = "debug_pool_open", ArgsSize = 1, Help = "Open number of pool connections")]
	internal class DebugPoolOpenCmd : IConsoleCmd
	{
		// Token: 0x06002886 RID: 10374 RVA: 0x000AE4D4 File Offset: 0x000AC8D4
		public void ExecuteCmd(string[] args)
		{
			int num = int.Parse(args[1]);
			List<ConnectionPool.Entity> list = new List<ConnectionPool.Entity>();
			for (int i = 0; i < num; i++)
			{
				ConnectionPool.Entity entity = Resources.MasterConnectionPool.CreateConnection();
				list.Add(entity);
				Log.Info<int>("Open connection {0}", entity.ID);
			}
			list.ForEach(delegate(ConnectionPool.Entity e)
			{
				e.Dispose();
			});
			list.Clear();
		}
	}
}
