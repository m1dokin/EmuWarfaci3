using System;
using System.Collections.Generic;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Users;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x0200017C RID: 380
	[Service]
	[Singleton]
	internal class OnlineClient : ServiceModule, IOnlineClient, IOnlineConnectionHandlerCallback
	{
		// Token: 0x060006E4 RID: 1764 RVA: 0x0001AC2C File Offset: 0x0001902C
		public OnlineClient(IQueryManager queryManager)
		{
			this.m_queryManager = (QueryManager)queryManager;
			this.m_connection_prms = this.LoadConnectionPrms(new OnlineClient.XmppNode
			{
				Host = string.Empty,
				Port = 0
			});
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060006E5 RID: 1765 RVA: 0x0001AC86 File Offset: 0x00019086
		public string XmppHost
		{
			get
			{
				return this.m_connection_prms.XmppHost;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x0001AC93 File Offset: 0x00019093
		public string OnlineID
		{
			get
			{
				return this.m_connection_prms.OnlineID;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060006E7 RID: 1767 RVA: 0x0001ACA0 File Offset: 0x000190A0
		public string TargetRoute
		{
			get
			{
				return "k01." + CryOnline.CryOnlineGetInstance().GetConfiguration(Resources.XmppOnlineDomain).GetHost();
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x0001ACC0 File Offset: 0x000190C0
		public string Server
		{
			get
			{
				return CryOnline.CryOnlineGetInstance().GetConfiguration(Resources.XmppOnlineDomain).GetServer();
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060006E9 RID: 1769 RVA: 0x0001ACD6 File Offset: 0x000190D6
		public int ServerPort
		{
			get
			{
				return CryOnline.CryOnlineGetInstance().GetConfiguration(Resources.XmppOnlineDomain).GetServerPort();
			}
		}

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x060006EA RID: 1770 RVA: 0x0001ACEC File Offset: 0x000190EC
		// (remove) Token: 0x060006EB RID: 1771 RVA: 0x0001AD24 File Offset: 0x00019124
		public event ConnectionStateDeleg ConnectionStateChanged;

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x060006EC RID: 1772 RVA: 0x0001AD5C File Offset: 0x0001915C
		// (remove) Token: 0x060006ED RID: 1773 RVA: 0x0001AD94 File Offset: 0x00019194
		public event UserStatusDeleg UserStatusChanged;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x060006EE RID: 1774 RVA: 0x0001ADCC File Offset: 0x000191CC
		// (remove) Token: 0x060006EF RID: 1775 RVA: 0x0001AE04 File Offset: 0x00019204
		public event UserPingDeleg UserPing;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x060006F0 RID: 1776 RVA: 0x0001AE3C File Offset: 0x0001923C
		// (remove) Token: 0x060006F1 RID: 1777 RVA: 0x0001AE74 File Offset: 0x00019274
		public event OnlineQueryStatsDeleg OnlineQueryStats;

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060006F2 RID: 1778 RVA: 0x0001AEAA File Offset: 0x000192AA
		// (set) Token: 0x060006F3 RID: 1779 RVA: 0x0001AEB4 File Offset: 0x000192B4
		public EConnectionState ConnectionState
		{
			get
			{
				return this.m_connectionState;
			}
			set
			{
				if (this.m_connectionState != value)
				{
					EConnectionState connectionState = this.m_connectionState;
					this.m_connectionState = value;
					if (this.ConnectionStateChanged != null)
					{
						this.ConnectionStateChanged(connectionState, this.m_connectionState);
					}
				}
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060006F4 RID: 1780 RVA: 0x0001AEF8 File Offset: 0x000192F8
		public List<SOnlineServer> OnlineServers
		{
			get
			{
				return this.m_online_servers;
			}
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0001AF00 File Offset: 0x00019300
		public void SetOnlineServers(List<SOnlineServer> servers)
		{
			Interlocked.Exchange<List<SOnlineServer>>(ref this.m_online_servers, servers);
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x0001AF0F File Offset: 0x0001930F
		public override void Init()
		{
			ServicesManager.OnExecutionPhaseChanged += this.OnExecutionPhaseChanged;
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0001AF22 File Offset: 0x00019322
		private void OnExecutionPhaseChanged(ExecutionPhase execution_phase)
		{
			if (execution_phase == ExecutionPhase.Started)
			{
				this.InitConnection();
			}
			else if (execution_phase == ExecutionPhase.Stopping)
			{
				this.ShutdownConnection();
				this.ConnectionStateChanged = null;
				this.OnlineQueryStats = null;
			}
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0001AF54 File Offset: 0x00019354
		private void InitConnection()
		{
			CryOnline.CryOnlineInit();
			this.m_nodes = this.LoadXmppNodes();
			Random random = new Random(DateTime.UtcNow.Ticks.GetHashCode());
			this.m_current_node = random.Next(this.m_nodes.Count);
			this.m_queryManager.RegisterBinders();
			Console.Title = string.Format("MasterServer v{0} {1}", Resources.MasterVersion, Resources.Jid);
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0001AFD0 File Offset: 0x000193D0
		private List<OnlineClient.XmppNode> LoadXmppNodes()
		{
			List<OnlineClient.XmppNode> list = new List<OnlineClient.XmppNode>();
			ConfigSection section = Resources.XMPPSettings.GetSection("xmpp");
			foreach (ConfigSection configSection in section.GetSections("node"))
			{
				if (!string.IsNullOrEmpty(configSection.Get("server")))
				{
					list.Add(new OnlineClient.XmppNode
					{
						Host = configSection.Get("server"),
						Port = int.Parse(configSection.Get("port"))
					});
				}
			}
			Utils.Shuffle<OnlineClient.XmppNode>(list);
			return list;
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x0001B09C File Offset: 0x0001949C
		private ConnectionParams LoadConnectionPrms(OnlineClient.XmppNode node)
		{
			ConnectionParams connectionParams = new ConnectionParams();
			ConfigSection section = Resources.XMPPSettings.GetSection("xmpp");
			connectionParams.Host = node.Host;
			connectionParams.Port = node.Port;
			connectionParams.XmppHost = ((!Resources.BootstrapMode) ? section.Get("host") : string.Format("{0}.{1}", Resources.BootstrapName, section.Get("host")));
			connectionParams.OnlineID = section.Get("jid");
			connectionParams.Password = section.Get("password");
			connectionParams.TLS = bool.Parse(section.Get("tls"));
			connectionParams.Proxy = section.Get("proxy");
			connectionParams.ProxyPort = int.Parse(section.Get("proxy_port"));
			int.TryParse(section.Get("tcp_recv_buf_size"), out connectionParams.TcpReceiveBufSize);
			return connectionParams;
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0001B18A File Offset: 0x0001958A
		private void ShutdownConnection()
		{
			if (!this.m_isStarted)
			{
				return;
			}
			this.m_isClosing = true;
			if (this.ConnectionState != EConnectionState.Disconnected)
			{
				this.ConnectionState = EConnectionState.Disconnecting;
			}
			this.m_connectionHandler.Disconnect();
			this.m_queryManager.UnregisterBinders();
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0001B1C8 File Offset: 0x000195C8
		public void ServiceConnection()
		{
			this.m_isStarted = true;
			while (!this.m_isClosing)
			{
				this.ConnectionState = EConnectionState.Connecting;
				this.m_connection_prms = this.LoadConnectionPrms(this.m_nodes[this.m_current_node]);
				this.m_connectionHandler = new OnlineConnectionHandler(this.m_connection_prms, this);
				Log.Verbose(Log.Group.Network, "Starting to listen socket", new object[0]);
				this.m_connectionHandler.BlockingConnect();
				Log.Verbose(Log.Group.Network, "Ended to listen socket", new object[0]);
				if (this.ConnectionState != EConnectionState.Disconnected)
				{
					this.ConnectionState = EConnectionState.Disconnecting;
				}
				this.m_connectionHandler.Disconnect();
				this.ConnectionState = EConnectionState.Disconnected;
				if (!this.m_isClosing)
				{
					Thread.Sleep(5000);
					Log.Info("Trying to reconnect to jabber...");
					this.m_current_node = (this.m_current_node + 1) % this.m_nodes.Count;
				}
			}
			this.m_isStarted = false;
			this.m_isClosing = false;
			CryOnline.CryOnlineShutdown();
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0001B2C8 File Offset: 0x000196C8
		public void OnConnectionStateChanged(EConnectionState current)
		{
			Log.Verbose(Log.Group.Network, "OnlineClient:OnConnectionStateChanged({0})", new object[]
			{
				current
			});
			this.ConnectionState = current;
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0001B2F0 File Offset: 0x000196F0
		public void OnUserStatus(string online_id, string prev_status, string new_status)
		{
			if (this.UserStatusChanged != null)
			{
				UserStatus prev = (UserStatus)int.Parse(prev_status);
				UserStatus now = (UserStatus)int.Parse(new_status);
				this.UserStatusChanged(online_id, prev, now);
			}
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x0001B324 File Offset: 0x00019724
		public void OnQueryStats(OnlineQueryStats stats)
		{
			if (this.OnlineQueryStats != null)
			{
				this.OnlineQueryStats(stats);
			}
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x0001B33D File Offset: 0x0001973D
		public void OnLogMessage(EOnlineLogLevel level, string msg)
		{
			if (level == EOnlineLogLevel.eOnlineLog_Warning)
			{
				Log.Warning<string>("<CryOnline>: {0}", msg);
			}
			else if (level == EOnlineLogLevel.eOnlineLog_Error)
			{
				Log.Error<string>("<CryOnline>: {0}", msg);
			}
			else
			{
				Log.Info<string>("<CryOnline>: {0}", msg);
			}
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0001B378 File Offset: 0x00019778
		public ECompressType GetDefaultCompression()
		{
			return CryOnline.CryOnlineGetInstance().GetConfiguration(Resources.XmppOnlineDomain).GetDefaultCompression();
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x0001B38E File Offset: 0x0001978E
		public void SetDefaultCompression(ECompressType compr)
		{
			CryOnline.CryOnlineGetInstance().GetConfiguration(Resources.XmppOnlineDomain).SetDefaultCompression(compr);
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x0001B3A5 File Offset: 0x000197A5
		public int GetSendDelay()
		{
			return CryOnline.CryOnlineGetInstance().GetConfiguration(Resources.XmppOnlineDomain).GetSendDelay();
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x0001B3BB File Offset: 0x000197BB
		public void SetSendDelay(int delay)
		{
			CryOnline.CryOnlineGetInstance().GetConfiguration(Resources.XmppOnlineDomain).SetSendDelay(delay);
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0001B3D2 File Offset: 0x000197D2
		public string GetJid(string user, string resource)
		{
			return new Jid(user, this.XmppHost, resource).ToString();
		}

		// Token: 0x0400041E RID: 1054
		private const int RECONNECT_TIMEOUT_MSEC = 5000;

		// Token: 0x0400041F RID: 1055
		private ConnectionParams m_connection_prms;

		// Token: 0x04000420 RID: 1056
		private List<OnlineClient.XmppNode> m_nodes;

		// Token: 0x04000421 RID: 1057
		private int m_current_node;

		// Token: 0x04000422 RID: 1058
		private bool m_isStarted;

		// Token: 0x04000423 RID: 1059
		private bool m_isClosing;

		// Token: 0x04000424 RID: 1060
		private OnlineConnectionHandler m_connectionHandler;

		// Token: 0x04000425 RID: 1061
		private readonly QueryManager m_queryManager;

		// Token: 0x0400042A RID: 1066
		private EConnectionState m_connectionState = EConnectionState.Disconnected;

		// Token: 0x0400042B RID: 1067
		private List<SOnlineServer> m_online_servers = new List<SOnlineServer>();

		// Token: 0x0200017D RID: 381
		public struct XmppNode
		{
			// Token: 0x0400042C RID: 1068
			public string Host;

			// Token: 0x0400042D RID: 1069
			public int Port;
		}
	}
}
