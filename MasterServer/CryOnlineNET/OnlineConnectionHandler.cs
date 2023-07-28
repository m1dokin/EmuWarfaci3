using System;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000185 RID: 389
	internal class OnlineConnectionHandler : IOnlineConnectionListener
	{
		// Token: 0x06000714 RID: 1812 RVA: 0x0001B5A0 File Offset: 0x000199A0
		public OnlineConnectionHandler(ConnectionParams connParams, IOnlineConnectionHandlerCallback clb)
		{
			this.m_connectionParams = connParams;
			this.m_callback = clb;
			this.m_online = CryOnline.CryOnlineGetInstance();
			this.SetConnectionParams();
			this.m_connection = this.m_online.CreateConnection(Resources.XmppOnlineDomain);
			this.m_online.RegisterConnectionListener(this, Resources.XmppOnlineDomain);
			this.m_statsListener = new OnlineConnectionHandler.OnlineQueryStatsListener(this.m_callback);
			this.m_connection.RegisterQueryStatsListener(this.m_statsListener);
			this.m_logListener = new OnlineConnectionHandler.OnlineLogLestener(this.m_callback);
			this.m_online.RegisterLog(this.m_logListener);
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0001B63E File Offset: 0x00019A3E
		public void Disconnect()
		{
			if (this.m_connection != null)
			{
				this.m_connection = null;
				this.m_online.ReleaseConnection(Resources.XmppOnlineDomain);
				this.m_online.UnregisterConnectionListener(this);
				this.m_online.UnregisterLog();
			}
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x0001B67C File Offset: 0x00019A7C
		public void BlockingConnect()
		{
			try
			{
				this.m_connection.Connect(EConnectionMode.eCM_Blocking);
			}
			finally
			{
				this.Disconnect();
				this.m_online.Tick();
			}
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x0001B6C0 File Offset: 0x00019AC0
		private void SetConnectionParams()
		{
			IOnline online = CryOnline.CryOnlineGetInstance();
			IOnlineConfiguration configuration = online.GetConfiguration(Resources.XmppOnlineDomain);
			configuration.SetServer(this.m_connectionParams.Host);
			configuration.SetServerPort(this.m_connectionParams.Port);
			configuration.SetOnlineId(this.m_connectionParams.OnlineID);
			configuration.SetResource(Resources.XmppResource);
			configuration.SetPassword(this.m_connectionParams.Password);
			configuration.SetHost(this.m_connectionParams.XmppHost);
			configuration.SetTLSPolicy((!this.m_connectionParams.TLS) ? EOnlineTLSPolicy.eOnlineTLS_Disabled : EOnlineTLSPolicy.eOnlineTLS_Required);
			configuration.SetFSProxy(this.m_connectionParams.Proxy);
			configuration.SetFSProxyPort(this.m_connectionParams.ProxyPort);
			configuration.SetDefaultCompression(ECompressType.eCS_SmartCompress);
			configuration.SetThreadMode(0);
			if (this.m_connectionParams.TcpReceiveBufSize > 0)
			{
				configuration.SetTcpReceiveBufferSize(this.m_connectionParams.TcpReceiveBufSize);
			}
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0001B7AE File Offset: 0x00019BAE
		public override void OnConnectionAvailable(IOnlineConnection connection)
		{
			Log.Info<string, int>("XMPP connected to {0}:{1}", this.m_connectionParams.Host, this.m_connectionParams.Port);
			this.m_connected = true;
			this.m_callback.OnConnectionStateChanged(EConnectionState.Connected);
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x0001B7E3 File Offset: 0x00019BE3
		public override void OnConnectionLost(IOnlineConnection connection, EOnlineError reason, string errorDesc)
		{
			if (this.m_connected)
			{
				Log.Info<EOnlineError, string>("OnlineClient connection lost : {0} {1}", reason, errorDesc);
			}
			this.m_connected = false;
			this.m_callback.OnConnectionStateChanged(EConnectionState.Disconnected);
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0001B80F File Offset: 0x00019C0F
		public override void OnConnectionTick(IOnlineConnection connection)
		{
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x0001B811 File Offset: 0x00019C11
		public override void OnPresence(IOnlineConnection connection, string online_id, EOnlinePresence presence)
		{
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x0001B814 File Offset: 0x00019C14
		public override void OnUserStatus(IOnlineConnection connection, string online_id, string prev_status, string new_status)
		{
			try
			{
				this.m_callback.OnUserStatus(online_id, prev_status, new_status);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x0400043D RID: 1085
		private readonly ConnectionParams m_connectionParams;

		// Token: 0x0400043E RID: 1086
		private readonly IOnlineConnectionHandlerCallback m_callback;

		// Token: 0x0400043F RID: 1087
		private readonly IOnline m_online;

		// Token: 0x04000440 RID: 1088
		private IOnlineConnection m_connection;

		// Token: 0x04000441 RID: 1089
		private bool m_connected;

		// Token: 0x04000442 RID: 1090
		private readonly OnlineConnectionHandler.OnlineQueryStatsListener m_statsListener;

		// Token: 0x04000443 RID: 1091
		private readonly OnlineConnectionHandler.OnlineLogLestener m_logListener;

		// Token: 0x02000186 RID: 390
		private class OnlineQueryStatsListener : IOnlineQueryStatsListener
		{
			// Token: 0x0600071D RID: 1821 RVA: 0x0001B854 File Offset: 0x00019C54
			public OnlineQueryStatsListener(IOnlineConnectionHandlerCallback clb)
			{
				this.callback = clb;
			}

			// Token: 0x0600071E RID: 1822 RVA: 0x0001B863 File Offset: 0x00019C63
			public override void OnQueryStats(SQueryStats stats)
			{
				this.callback.OnQueryStats(stats.OnlineQueryStats());
			}

			// Token: 0x04000444 RID: 1092
			private readonly IOnlineConnectionHandlerCallback callback;
		}

		// Token: 0x02000187 RID: 391
		private class OnlineLogLestener : IOnlineLog
		{
			// Token: 0x0600071F RID: 1823 RVA: 0x0001B876 File Offset: 0x00019C76
			public OnlineLogLestener(IOnlineConnectionHandlerCallback clb)
			{
				this.callback = clb;
			}

			// Token: 0x06000720 RID: 1824 RVA: 0x0001B885 File Offset: 0x00019C85
			public override void OnLogMessage(EOnlineLogLevel level, string msg)
			{
				this.callback.OnLogMessage(level, msg);
			}

			// Token: 0x04000445 RID: 1093
			private readonly IOnlineConnectionHandlerCallback callback;
		}
	}
}
