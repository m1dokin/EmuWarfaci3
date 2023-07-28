using System;
using MasterServer.Database;

namespace MasterServer.Core
{
	// Token: 0x020007AF RID: 1967
	[ConsoleCmdAttributes(CmdName = "debug_pool_leak", ArgsSize = 1, Help = "Leak number of pool connections")]
	internal class DebugPoolLeakCmd : IConsoleCmd
	{
		// Token: 0x06002889 RID: 10377 RVA: 0x000AE560 File Offset: 0x000AC960
		public void ExecuteCmd(string[] args)
		{
			int num = int.Parse(args[1]);
			for (int i = 0; i < num; i++)
			{
				ConnectionPool.Entity entity = Resources.MasterConnectionPool.CreateConnection();
				Log.Info<int>("Open connection {0}", entity.ID);
			}
		}
	}
}
