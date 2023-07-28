using System;
using HK2Net;
using MasterServer.Core.WebRequest;
using MasterServer.Telemetry.Metrics;
using Network;
using Network.Amqp;
using Network.Builders;
using Network.Monitoring;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x02000068 RID: 104
	[Service]
	[Singleton]
	internal class GiServerBuilderWrapper : ServerBuilderWrapper<GiCommandRequest, GiCommandResponse, IGiController>
	{
		// Token: 0x0600018D RID: 397 RVA: 0x0000A42C File Offset: 0x0000882C
		public GiServerBuilderWrapper(IGiRpcMetricsTracker tracker, IServerBuilder<GiCommandRequest, GiCommandResponse> serverBuilder, IGiController controller) : base(serverBuilder, controller)
		{
			base.InternalBuilder = base.InternalBuilder.WithTracers(new IRequestTracer[]
			{
				new GiRequestsHandlerMetricTracer(tracker),
				new LogRequestTracer()
			}).WithTransport(TransportType.Amqp).WithService(new GiRequestsHandlerService());
		}
	}
}
