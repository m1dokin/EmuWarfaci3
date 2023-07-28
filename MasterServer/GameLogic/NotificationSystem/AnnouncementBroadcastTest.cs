using System;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x0200058A RID: 1418
	[ConsoleCmdAttributes(CmdName = "announcement_broadcast_test", ArgsSize = 0, Help = "Broadcast announcement update to other masterservers")]
	internal class AnnouncementBroadcastTest : IConsoleCmd
	{
		// Token: 0x06001E78 RID: 7800 RVA: 0x0007BB60 File Offset: 0x00079F60
		public AnnouncementBroadcastTest(IOnlineClient onlineClient, IQueryManager queryManager)
		{
			this.m_onlineClient = onlineClient;
			this.m_queryManager = queryManager;
		}

		// Token: 0x06001E79 RID: 7801 RVA: 0x0007BB78 File Offset: 0x00079F78
		public void ExecuteCmd(string[] args)
		{
			Log.Info("Announcement broadcast test");
			this.m_queryManager.Request("master_server_bcast", this.m_onlineClient.TargetRoute, new object[]
			{
				"announcement",
				"0",
				"no_self_send"
			});
		}

		// Token: 0x04000ED6 RID: 3798
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04000ED7 RID: 3799
		private readonly IQueryManager m_queryManager;
	}
}
