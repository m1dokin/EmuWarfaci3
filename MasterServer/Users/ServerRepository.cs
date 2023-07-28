using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;

namespace MasterServer.Users
{
	// Token: 0x02000759 RID: 1881
	[Service]
	[Singleton]
	internal class ServerRepository : ServiceModule, IServerRepository
	{
		// Token: 0x060026D6 RID: 9942 RVA: 0x000A4350 File Offset: 0x000A2750
		public ServerRepository(IUserStatusProxy userStatusProxy, IOnlineClient onlineClient)
		{
			this.m_userStatusProxy = userStatusProxy;
			this.m_onlineClient = onlineClient;
			this.m_onlineServers.ItemExpired += this.OnServerExpired;
		}

		// Token: 0x140000A8 RID: 168
		// (add) Token: 0x060026D7 RID: 9943 RVA: 0x000A43A8 File Offset: 0x000A27A8
		// (remove) Token: 0x060026D8 RID: 9944 RVA: 0x000A43E0 File Offset: 0x000A27E0
		public event OnServerBindHandler OnServerBind;

		// Token: 0x060026D9 RID: 9945 RVA: 0x000A4416 File Offset: 0x000A2816
		public override void Start()
		{
			base.Start();
			this.m_userStatusProxy.OnUserStatus += this.OnServerStatus;
			this.m_onlineClient.ConnectionStateChanged += this.OnConnectionStateChanged;
		}

		// Token: 0x060026DA RID: 9946 RVA: 0x000A444C File Offset: 0x000A284C
		public override void Stop()
		{
			this.OnServerBind = null;
			this.m_userStatusProxy.OnUserStatus -= this.OnServerStatus;
			this.m_onlineClient.ConnectionStateChanged -= this.OnConnectionStateChanged;
			this.m_onlineServers.Dispose();
			base.Stop();
		}

		// Token: 0x060026DB RID: 9947 RVA: 0x000A449F File Offset: 0x000A289F
		public bool IsOnline(string jid)
		{
			return this.m_onlineServers.ContainsKey(jid);
		}

		// Token: 0x060026DC RID: 9948 RVA: 0x000A44B0 File Offset: 0x000A28B0
		public string GetServerID(string online_id)
		{
			string result;
			if (!this.m_onlineServers.TryGetValue(online_id, out result))
			{
				this.m_deletedServers.TryGetValue(online_id, out result);
			}
			return result;
		}

		// Token: 0x060026DD RID: 9949 RVA: 0x000A44E0 File Offset: 0x000A28E0
		public void AddServer(string online_id, string server_id)
		{
			string text = this.m_onlineServers.Replace(online_id, server_id);
			if (string.IsNullOrEmpty(text))
			{
				this.NotifyOnServerBind(true, online_id, server_id);
			}
			else if (string.Compare(text, server_id) != 0)
			{
				Log.Warning<string, string, string>("Substituting server {0} id from {1} to {2}", online_id, text, server_id);
				this.NotifyOnServerBind(false, online_id, text);
				this.NotifyOnServerBind(true, online_id, server_id);
			}
		}

		// Token: 0x060026DE RID: 9950 RVA: 0x000A4540 File Offset: 0x000A2940
		public void RemoveServer(string onlineId)
		{
			string text;
			if (this.m_onlineServers.Pop(onlineId, out text))
			{
				this.m_deletedServers.Add(onlineId, text);
				this.NotifyOnServerBind(false, onlineId, text);
			}
		}

		// Token: 0x060026DF RID: 9951 RVA: 0x000A4578 File Offset: 0x000A2978
		public void RemoveAllServers()
		{
			List<string> servers = new List<string>();
			this.m_onlineServers.Enumerate(delegate(string _, string serverId)
			{
				servers.Add(serverId);
				return true;
			});
			servers.ForEach(delegate(string serverId)
			{
				this.RemoveServer(serverId);
			});
		}

		// Token: 0x060026E0 RID: 9952 RVA: 0x000A45CC File Offset: 0x000A29CC
		private void NotifyOnServerBind(bool isBound, string onlineId, string serverId)
		{
			Log.Info<string, string, string>("Server {0} [{1}] {2}", serverId, onlineId, (!isBound) ? "UNBOUND" : "BOUND");
			if (this.OnServerBind != null && !string.IsNullOrEmpty(serverId))
			{
				this.OnServerBind(isBound, onlineId, serverId);
			}
		}

		// Token: 0x060026E1 RID: 9953 RVA: 0x000A461E File Offset: 0x000A2A1E
		private void OnServerExpired(string online_id, string server_id)
		{
			Log.Info<string, string>("Server {0} [{1}] expired and will be removed from online list", server_id, online_id);
			this.NotifyOnServerBind(false, online_id, server_id);
		}

		// Token: 0x060026E2 RID: 9954 RVA: 0x000A4635 File Offset: 0x000A2A35
		private void OnServerStatus(UserStatus prev_status, UserStatus new_status, string onlineId)
		{
			if (new_status == UserStatus.Offline)
			{
				this.RemoveServer(onlineId);
			}
		}

		// Token: 0x060026E3 RID: 9955 RVA: 0x000A4644 File Offset: 0x000A2A44
		private void OnConnectionStateChanged(EConnectionState prev, EConnectionState current)
		{
			if (current == EConnectionState.Disconnected && prev != EConnectionState.Disconnected)
			{
				Log.Info("Lost connection - removing all servers");
				this.RemoveAllServers();
			}
		}

		// Token: 0x040013F9 RID: 5113
		private const int SERVER_PRESENCE_TIMEOUT_SEC = 300;

		// Token: 0x040013FA RID: 5114
		private const int DELETED_SERVER_TIMEOUT_SEC = 300;

		// Token: 0x040013FB RID: 5115
		private CacheDictionary<string, string> m_onlineServers = new CacheDictionary<string, string>(300);

		// Token: 0x040013FC RID: 5116
		private CacheDictionary<string, string> m_deletedServers = new CacheDictionary<string, string>(300);

		// Token: 0x040013FD RID: 5117
		private readonly IUserStatusProxy m_userStatusProxy;

		// Token: 0x040013FE RID: 5118
		private readonly IOnlineClient m_onlineClient;
	}
}
