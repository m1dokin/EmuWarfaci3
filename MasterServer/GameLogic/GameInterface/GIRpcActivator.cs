using System;
using System.Collections;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using CookComputing.XmlRpc;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002EA RID: 746
	internal class GIRpcActivator : MarshalByRefObject
	{
		// Token: 0x06001168 RID: 4456 RVA: 0x00044FF4 File Offset: 0x000433F4
		public void Start(IPAddress bindAddr, int port, GIRpcCallback callback)
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
			this.m_listener = new GIRpcListener(callback);
			RemotingServices.Marshal(this.m_listener, "gi", typeof(IGIRpcListener));
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x000450A4 File Offset: 0x000434A4
		public void Stop()
		{
			ChannelServices.UnregisterChannel(this.m_channel);
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x000450B1 File Offset: 0x000434B1
		public override object InitializeLifetimeService()
		{
			return null;
		}

		// Token: 0x040007AC RID: 1964
		private HttpChannel m_channel;

		// Token: 0x040007AD RID: 1965
		private GIRpcListener m_listener;
	}
}
