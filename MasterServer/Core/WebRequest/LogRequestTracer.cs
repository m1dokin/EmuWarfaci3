using System;
using Network.Interfaces;
using Network.Monitoring;

namespace MasterServer.Core.WebRequest
{
	// Token: 0x02000166 RID: 358
	internal class LogRequestTracer : IRequestTracer
	{
		// Token: 0x06000669 RID: 1641 RVA: 0x0001A27A File Offset: 0x0001867A
		public void RequestStarted(IRemoteRequest request)
		{
			LogRequestTracer.Write("RequestStarted - {0}", new object[]
			{
				request
			});
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0001A290 File Offset: 0x00018690
		public void RequestCompleted(IRemoteRequest request, IRemoteResponse response)
		{
			LogRequestTracer.Write("RequestCompleted - {0} :: {1}", new object[]
			{
				request,
				response
			});
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0001A2AA File Offset: 0x000186AA
		public void RequestError(IRemoteRequest request, Exception ex)
		{
			LogRequestTracer.Write("RequestError - {0} :: {1}", new object[]
			{
				request,
				ex
			});
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x0001A2C4 File Offset: 0x000186C4
		public void FilterRequestIn(string filterName, IRemoteRequest request)
		{
			LogRequestTracer.Write("FilterRequestIn - {0} :: {1}", new object[]
			{
				filterName,
				request
			});
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x0001A2DE File Offset: 0x000186DE
		public void FilterResponseOut(string filterName, IRemoteRequest request, IRemoteResponse response)
		{
			LogRequestTracer.Write("FilterResponseOut - {0} :: {1}", new object[]
			{
				filterName,
				request
			});
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x0001A2F8 File Offset: 0x000186F8
		public void FilterResponseOutError(string filterName, IRemoteRequest request, Exception ex)
		{
			LogRequestTracer.Write("FilterResponseOutError - {0} :: {1} :: {2}", new object[]
			{
				filterName,
				request,
				ex
			});
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0001A316 File Offset: 0x00018716
		public void RequestProcessingStarted(IRemoteRequest request)
		{
			LogRequestTracer.Write("RequestProcessingStarted - {0}", new object[]
			{
				request
			});
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0001A32C File Offset: 0x0001872C
		public void RequestProcessingCompleted(IRemoteRequest request, IRemoteResponse response)
		{
			LogRequestTracer.Write("RequestProcessingCompleted - {0} :: {1}", new object[]
			{
				request,
				response
			});
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0001A346 File Offset: 0x00018746
		public void RequestProcessingError(IRemoteRequest request, Exception ex)
		{
			LogRequestTracer.Write("RequestProcessingError - {0} :: {1}", new object[]
			{
				request,
				ex
			});
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0001A360 File Offset: 0x00018760
		private static void Write(string fmt, params object[] args)
		{
			Log.Verbose(Log.Group.Network, fmt, args);
		}
	}
}
