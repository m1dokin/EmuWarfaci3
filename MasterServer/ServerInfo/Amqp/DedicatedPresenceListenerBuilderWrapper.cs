using System;
using HK2Net;
using Network.Amqp;
using Network.Builders;

namespace MasterServer.ServerInfo.Amqp
{
	// Token: 0x020006B4 RID: 1716
	[Service]
	internal class DedicatedPresenceListenerBuilderWrapper : ListenerBuilderWrapper<DedicatedPresenceRequest, IDedicatedPresenceController>
	{
		// Token: 0x06002406 RID: 9222 RVA: 0x00096EFB File Offset: 0x000952FB
		public DedicatedPresenceListenerBuilderWrapper(IServerBuilder<DedicatedPresenceRequest> serverBuilder, IDedicatedPresenceController controller) : base(serverBuilder, controller)
		{
		}
	}
}
