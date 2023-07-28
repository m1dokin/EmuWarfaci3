using System;
using MasterServer.Core.Web;
using MasterServer.Telemetry.Metrics;
using Network.Http;
using Network.Interfaces;
using Network.Metadata;
using Network.Monitoring;

namespace MasterServer.Core.WebRequest
{
	// Token: 0x02000167 RID: 359
	internal class MetricRequestTracer : IRequestTracer
	{
		// Token: 0x06000673 RID: 1651 RVA: 0x0001A36E File Offset: 0x0001876E
		public MetricRequestTracer(IRequestMetricsTracker metricsTracker)
		{
			this.m_metricsTracker = metricsTracker;
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x0001A37D File Offset: 0x0001877D
		public void RequestStarted(IRemoteRequest request)
		{
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x0001A37F File Offset: 0x0001877F
		public void RequestCompleted(IRemoteRequest request, IRemoteResponse response)
		{
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x0001A381 File Offset: 0x00018781
		public void RequestError(IRemoteRequest request, Exception ex)
		{
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x0001A383 File Offset: 0x00018783
		public void FilterRequestIn(string filterName, IRemoteRequest request)
		{
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x0001A385 File Offset: 0x00018785
		public void FilterResponseOut(string filterName, IRemoteRequest request, IRemoteResponse response)
		{
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x0001A387 File Offset: 0x00018787
		public void FilterResponseOutError(string filterName, IRemoteRequest request, Exception ex)
		{
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x0001A389 File Offset: 0x00018789
		public void RequestProcessingStarted(IRemoteRequest request)
		{
			Log.Info<IRemoteRequest>("[Http request started: {0}]", request);
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x0001A398 File Offset: 0x00018798
		public void RequestProcessingCompleted(IRemoteRequest request, IRemoteResponse response)
		{
			RequestDomain domain = request.Metadata.Domain<RequestDomain>();
			UriBuilder uriBuilder = new UriBuilder(request.Url);
			IHttpResponse httpResponse = response as IHttpResponse;
			if (httpResponse != null)
			{
				HttpStatusCodes.Category category = HttpStatusCodes.GetCategory(httpResponse.StatusCode);
				if (category == HttpStatusCodes.Category.ServerError || category == HttpStatusCodes.Category.ClientError)
				{
					Log.Error<IRemoteRequest, TimeSpan>("[Http request failed: {0} in {1}]", request, request.Metadata.InnerRequestTime());
					this.m_metricsTracker.ReportHttpRequestFailed(domain, uriBuilder.Host, uriBuilder.Path, httpResponse.StatusCode);
				}
				else
				{
					Log.Info<IRemoteResponse, TimeSpan>("[Http request completed: {0} in {1}]", response, request.Metadata.InnerRequestTime());
					this.m_metricsTracker.ReportHttpRequestCompleted(domain, uriBuilder.Host, uriBuilder.Path);
				}
				this.m_metricsTracker.ReportHttpRequestTime(domain, uriBuilder.Host, uriBuilder.Path, request.Metadata.InnerRequestTime());
			}
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x0001A478 File Offset: 0x00018878
		public void RequestProcessingError(IRemoteRequest request, Exception ex)
		{
			RequestDomain domain = request.Metadata.Domain<RequestDomain>();
			UriBuilder uriBuilder = new UriBuilder(request.Url);
			Log.Error<IRemoteRequest, TimeSpan>("[Http request error: {0} in {1}]", request, request.Metadata.InnerRequestTime());
			this.m_metricsTracker.ReportHttpRequestCrashed(domain, uriBuilder.Host, uriBuilder.Path);
		}

		// Token: 0x040003F2 RID: 1010
		private readonly IRequestMetricsTracker m_metricsTracker;
	}
}
