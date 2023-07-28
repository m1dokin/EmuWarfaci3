using System;
using System.Collections.Generic;
using HK2Net;
using Network.Amqp;
using Network.Interfaces;

namespace MasterServer.ServerInfo.Amqp
{
	// Token: 0x020006B5 RID: 1717
	[Service]
	[Singleton]
	internal class DedicatedPresenceListenerBuilder : BusListenerBuilder<DedicatedPresenceRequest>
	{
		// Token: 0x06002407 RID: 9223 RVA: 0x00096F05 File Offset: 0x00095305
		public DedicatedPresenceListenerBuilder(IEnumerable<IBus> busCollection, ITypeNameSerializer typeNameSerializer) : base(busCollection, typeNameSerializer)
		{
		}
	}
}
