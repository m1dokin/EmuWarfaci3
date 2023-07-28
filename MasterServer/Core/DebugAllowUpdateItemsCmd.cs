using System;

namespace MasterServer.Core
{
	// Token: 0x020007A4 RID: 1956
	[ConsoleCmdAttributes(CmdName = "debug_update_items_allow", ArgsSize = 1)]
	internal class DebugAllowUpdateItemsCmd : IConsoleCmd
	{
		// Token: 0x06002873 RID: 10355 RVA: 0x000AE11C File Offset: 0x000AC51C
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 1)
			{
				Log.Info<bool>("debug_update_items_allow = {0}", Resources.DebugUpdateItemsAllow);
				return;
			}
			Resources.DebugUpdateItemsAllow = (uint.Parse(args[1]) == 1U);
		}
	}
}
