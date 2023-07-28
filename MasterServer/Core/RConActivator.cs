using System;
using System.Collections;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using CookComputing.XmlRpc;

namespace MasterServer.Core
{
	// Token: 0x02000149 RID: 329
	internal class RConActivator : MarshalByRefObject
	{
		// Token: 0x060005BA RID: 1466 RVA: 0x00017018 File Offset: 0x00015418
		public void Start(IPAddress bindAddr, int port, RConCallback callback)
		{
			IDictionary dictionary = new Hashtable();
			dictionary["name"] = "MasterServerHttpChannel";
			dictionary["port"] = port;
			if (!bindAddr.Equals(IPAddress.Any))
			{
				dictionary["bindTo"] = bindAddr.ToString();
			}
			this.m_channel = new HttpChannel(dictionary, null, new XmlRpcServerFormatterSinkProvider());
			ChannelServices.RegisterChannel(this.m_channel, false);
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
			}
			this.m_listener = new RConListener(callback);
			RemotingServices.Marshal(this.m_listener, "rcon", typeof(IRConListener));
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x000170C8 File Offset: 0x000154C8
		public void Stop()
		{
			ChannelServices.UnregisterChannel(this.m_channel);
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x000170D5 File Offset: 0x000154D5
		public override object InitializeLifetimeService()
		{
			return null;
		}

		// Token: 0x040003BB RID: 955
		private HttpChannel m_channel;

		// Token: 0x040003BC RID: 956
		private RConListener m_listener;
	}
}
