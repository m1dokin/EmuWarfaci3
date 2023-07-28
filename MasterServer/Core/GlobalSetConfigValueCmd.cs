using System;
using MasterServer.Common;
using MasterServer.CryOnlineNET;

namespace MasterServer.Core
{
	// Token: 0x0200010B RID: 267
	[ConsoleCmdAttributes(CmdName = "set_global", ArgsSize = 3)]
	internal class GlobalSetConfigValueCmd : IConsoleCmd
	{
		// Token: 0x06000453 RID: 1107 RVA: 0x00012BAF File Offset: 0x00010FAF
		public GlobalSetConfigValueCmd(IOnlineClient onlineClient, IQueryManager queryManager)
		{
			this.m_onlineClient = onlineClient;
			this.m_queryManager = queryManager;
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00012BC8 File Offset: 0x00010FC8
		public void ExecuteCmd(string[] args)
		{
			if (args.Length == 3)
			{
				args[0] = "set";
				string text = Utils.CreateFormatedString(args);
				ConsoleCmdManager.ExecuteCmd(text);
				this.m_queryManager.Request("master_server_bcast", this.m_onlineClient.TargetRoute, new object[]
				{
					"set_global",
					text,
					"no_self_send"
				});
			}
		}

		// Token: 0x040001CA RID: 458
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x040001CB RID: 459
		private readonly IQueryManager m_queryManager;
	}
}
