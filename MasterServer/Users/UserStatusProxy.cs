using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;

namespace MasterServer.Users
{
	// Token: 0x02000808 RID: 2056
	[Service]
	[Singleton]
	internal class UserStatusProxy : ServiceModule, IUserStatusProxy
	{
		// Token: 0x06002A25 RID: 10789 RVA: 0x000B5E76 File Offset: 0x000B4276
		public UserStatusProxy(IOnlineClient onlineClientService, IQoSQueue qosQueue)
		{
			this.m_onlineClientService = onlineClientService;
			this.m_qosQueue = qosQueue;
		}

		// Token: 0x140000B5 RID: 181
		// (add) Token: 0x06002A26 RID: 10790 RVA: 0x000B5E8C File Offset: 0x000B428C
		// (remove) Token: 0x06002A27 RID: 10791 RVA: 0x000B5EC4 File Offset: 0x000B42C4
		public event OnUserStatusHandler OnUserStatus;

		// Token: 0x06002A28 RID: 10792 RVA: 0x000B5EFA File Offset: 0x000B42FA
		public override void Start()
		{
			base.Start();
			this.m_onlineClientService.UserStatusChanged += this.OnUserStatusChanged;
		}

		// Token: 0x06002A29 RID: 10793 RVA: 0x000B5F19 File Offset: 0x000B4319
		public override void Stop()
		{
			this.OnUserStatus = null;
			base.Stop();
		}

		// Token: 0x06002A2A RID: 10794 RVA: 0x000B5F28 File Offset: 0x000B4328
		private void OnUserStatusChanged(string onlineId, UserStatus prev, UserStatus now)
		{
			UserStatusProxy.StatusInfo status = new UserStatusProxy.StatusInfo(onlineId, prev, now);
			TShapingInfo shaping_info = new TShapingInfo
			{
				query_name = "_status_",
				query_class = null,
				from_jid = onlineId
			};
			this.m_qosQueue.QueueWorkItem(shaping_info, delegate(object _)
			{
				this.HandleUserStatus(status);
			});
		}

		// Token: 0x06002A2B RID: 10795 RVA: 0x000B5F8C File Offset: 0x000B438C
		private void HandleUserStatus(UserStatusProxy.StatusInfo pi)
		{
			Log.Verbose("User {0} is {1}", new object[]
			{
				pi.onlineId,
				pi.new_status
			});
			if (this.OnUserStatus != null)
			{
				this.OnUserStatus(pi.prev_status, pi.new_status, pi.onlineId);
			}
		}

		// Token: 0x04001678 RID: 5752
		private readonly IOnlineClient m_onlineClientService;

		// Token: 0x04001679 RID: 5753
		private readonly IQoSQueue m_qosQueue;

		// Token: 0x02000809 RID: 2057
		private struct StatusInfo
		{
			// Token: 0x06002A2C RID: 10796 RVA: 0x000B5FED File Offset: 0x000B43ED
			public StatusInfo(string oid, UserStatus ps, UserStatus ns)
			{
				this.onlineId = oid;
				this.prev_status = ps;
				this.new_status = ns;
			}

			// Token: 0x0400167A RID: 5754
			public readonly string onlineId;

			// Token: 0x0400167B RID: 5755
			public readonly UserStatus prev_status;

			// Token: 0x0400167C RID: 5756
			public readonly UserStatus new_status;
		}
	}
}
