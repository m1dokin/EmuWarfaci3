using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using Network.Amqp;
using Network.Interfaces;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002D8 RID: 728
	[Service]
	[Singleton]
	internal class GiJsonBusServerBuilder : BusServerBuilder<GiCommandRequest, GiCommandResponse>
	{
		// Token: 0x06000F9D RID: 3997 RVA: 0x0003F040 File Offset: 0x0003D440
		public GiJsonBusServerBuilder(IEnumerable<IBus> busCollection, ITypeNameSerializer typeNameSerializer) : base(busCollection, typeNameSerializer)
		{
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x0003F04A File Offset: 0x0003D44A
		public override IServer Build()
		{
			return Resources.GIPort.IsValid() ? base.Build() : new GiJsonBusServerBuilder.DummyServer();
		}

		// Token: 0x020002D9 RID: 729
		private class DummyServer : IServer
		{
			// Token: 0x06000FA0 RID: 4000 RVA: 0x0003F073 File Offset: 0x0003D473
			public void Stop()
			{
			}
		}
	}
}
