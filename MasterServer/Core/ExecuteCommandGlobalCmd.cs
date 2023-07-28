using System;
using MasterServer.Common;
using MasterServer.CryOnlineNET;

namespace MasterServer.Core
{
	// Token: 0x0200010A RID: 266
	[ConsoleCmdAttributes(CmdName = "execute_command_global")]
	internal class ExecuteCommandGlobalCmd : IConsoleCmd
	{
		// Token: 0x06000451 RID: 1105 RVA: 0x00012B29 File Offset: 0x00010F29
		public ExecuteCommandGlobalCmd(IOnlineClient onlineClient, IQueryManager queryManager)
		{
			this.m_onlineClient = onlineClient;
			this.m_queryManager = queryManager;
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00012B40 File Offset: 0x00010F40
		public void ExecuteCmd(string[] args)
		{
			if (args.Length > 1)
			{
				string[] array = new string[args.Length - 1];
				Array.Copy(args, 1, array, 0, array.Length);
				string text = Utils.CreateFormatedString(array);
				ConsoleCmdManager.ExecuteCmd(text);
				this.m_queryManager.Request("master_server_bcast", this.m_onlineClient.TargetRoute, new object[]
				{
					"global_command",
					text,
					"no_self_send"
				});
			}
		}

		// Token: 0x040001C8 RID: 456
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x040001C9 RID: 457
		private readonly IQueryManager m_queryManager;
	}
}
