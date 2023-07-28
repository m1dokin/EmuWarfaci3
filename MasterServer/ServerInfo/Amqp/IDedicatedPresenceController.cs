using System;
using HK2Net;
using Network.Interfaces;

namespace MasterServer.ServerInfo.Amqp
{
	// Token: 0x020006B2 RID: 1714
	[Contract]
	internal interface IDedicatedPresenceController : IRemoteService<DedicatedPresenceRequest>, IDisposable
	{
	}
}
