using System;
using MasterServer.Core;
using MasterServer.Telemetry.Metrics;
using Network.Interfaces;
using Network.Metadata;
using Network.Monitoring;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002DA RID: 730
	internal class GiRequestsHandlerMetricTracer : IRequestTracer
	{
		// Token: 0x06000FA1 RID: 4001 RVA: 0x0003F075 File Offset: 0x0003D475
		public GiRequestsHandlerMetricTracer(IGiRpcMetricsTracker metricsTracker)
		{
			this.m_metricsTracker = metricsTracker;
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x0003F084 File Offset: 0x0003D484
		public void RequestStarted(IRemoteRequest request)
		{
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x0003F086 File Offset: 0x0003D486
		public void RequestCompleted(IRemoteRequest request, IRemoteResponse response)
		{
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x0003F088 File Offset: 0x0003D488
		public void RequestError(IRemoteRequest request, Exception ex)
		{
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x0003F08A File Offset: 0x0003D48A
		public void FilterRequestIn(string filterName, IRemoteRequest request)
		{
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0003F08C File Offset: 0x0003D48C
		public void FilterResponseOut(string filterName, IRemoteRequest request, IRemoteResponse response)
		{
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0003F08E File Offset: 0x0003D48E
		public void FilterResponseOutError(string filterName, IRemoteRequest request, Exception ex)
		{
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0003F090 File Offset: 0x0003D490
		public void RequestProcessingStarted(IRemoteRequest request)
		{
			string text;
			if (!GiRequestsHandlerMetricTracer.TryGetRequestDomain(request, out text))
			{
				return;
			}
			Log.Info<string>("[Gi request started: {0}]", text);
			this.m_metricsTracker.ReportConsumed(text);
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x0003F0C4 File Offset: 0x0003D4C4
		public void RequestProcessingCompleted(IRemoteRequest request, IRemoteResponse response)
		{
			string text;
			if (!GiRequestsHandlerMetricTracer.TryGetRequestDomain(request, out text))
			{
				return;
			}
			Log.Info<string>("[Gi request completed: {0}]", text);
			this.m_metricsTracker.ReportProcessingTime(text, request.Metadata.InnerRequestTime());
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0003F104 File Offset: 0x0003D504
		public void RequestProcessingError(IRemoteRequest request, Exception ex)
		{
			string text;
			if (!GiRequestsHandlerMetricTracer.TryGetRequestDomain(request, out text))
			{
				return;
			}
			Log.Error<string>("[Gi request error: {0}]", text);
			this.m_metricsTracker.ReportFailed(text);
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x0003F138 File Offset: 0x0003D538
		private static bool TryGetRequestDomain(IRemoteRequest request, out string domain)
		{
			domain = string.Empty;
			GiCommandRequest giCommandRequest = request as GiCommandRequest;
			if (giCommandRequest == null)
			{
				Log.Warning("Parameter 'request' is not an instance of GiCommandRequest type.");
				return false;
			}
			domain = giCommandRequest.Domain;
			return true;
		}

		// Token: 0x0400074F RID: 1871
		private readonly IGiRpcMetricsTracker m_metricsTracker;
	}
}
