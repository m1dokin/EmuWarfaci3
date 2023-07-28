using System;

namespace MasterServer.Core
{
	// Token: 0x020007B0 RID: 1968
	[ConsoleCmdAttributes(CmdName = "simulate_disconect", ArgsSize = 0)]
	internal class SimulateDisconectCmd : IConsoleCmd
	{
		// Token: 0x0600288B RID: 10379 RVA: 0x000AE5AC File Offset: 0x000AC9AC
		public void ExecuteCmd(string[] args)
		{
			IOnline online = CryOnline.CryOnlineGetInstance();
			if (online != null)
			{
				online.ReleaseConnection(Resources.XmppOnlineDomain);
			}
		}
	}
}
