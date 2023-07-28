using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using HK2Net;
using HK2Net.Attributes.Bootstrap;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Web;
using MasterServer.GameLogic.VoucherSystem.Serializers;
using MasterServer.Telemetry.Metrics;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;
using Util.Common;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x0200047C RID: 1148
	[Service]
	[Singleton]
	[BootstrapSpecific("trunk")]
	[BootstrapSpecific("west_emul")]
	internal class RestVoucherCommunicationClient : VoucherCommunicationClient
	{
		// Token: 0x06001833 RID: 6195 RVA: 0x00063F74 File Offset: 0x00062374
		public RestVoucherCommunicationClient(IVoucherMetricsTracker metricsTracker, IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory) : base(metricsTracker)
		{
			this.m_webClientBuilder = webClientBuilder;
			this.m_webRequestFactory = webRequestFactory;
			ConfigSection section = Resources.ModuleSettings.GetSection("Voucher");
			string uriString = section.GetSection("externalvoucher").Get("url");
			Uri[] hostUrls = new Uri[]
			{
				new Uri(uriString)
			};
			this.m_webClient = this.m_webClientBuilder.Failover(hostUrls).Build();
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x00063FE3 File Offset: 0x000623E3
		public override void Dispose()
		{
			if (this.m_webClient != null)
			{
				this.m_webClient.Dispose();
				this.m_webClient = null;
			}
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x00064004 File Offset: 0x00062404
		protected override Task<IEnumerable<Voucher>> GetVouchersAsyncImpl(ulong pageIndex, int pageSize)
		{
			Task<IEnumerable<Voucher>> result;
			try
			{
				IEnumerable<Voucher> vouchersImpl = this.GetVouchersImpl(pageIndex, pageSize);
				result = Task.FromResult<IEnumerable<Voucher>>(vouchersImpl);
			}
			catch (Exception exception)
			{
				result = TaskHelpers.Failed<IEnumerable<Voucher>>(exception);
			}
			return result;
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x00064044 File Offset: 0x00062444
		protected override Task<IEnumerable<Voucher>> GetNewVouchersAsyncImpl(ulong userId)
		{
			Task<IEnumerable<Voucher>> result;
			try
			{
				IEnumerable<Voucher> vouchersForUserByStatus = this.GetVouchersForUserByStatus(userId, "new");
				result = Task.FromResult<IEnumerable<Voucher>>(vouchersForUserByStatus);
			}
			catch (Exception exception)
			{
				result = TaskHelpers.Failed<IEnumerable<Voucher>>(exception);
			}
			return result;
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x00064088 File Offset: 0x00062488
		protected override IEnumerable<Voucher> GetAllVouchersImp(ulong userId)
		{
			return this.GetVouchersForUserByStatus(userId, "all");
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x00064098 File Offset: 0x00062498
		protected override void UpdateVoucherImpl(Voucher voucher)
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Voucher).Method(RequestMethod.PUT).UrlPath(string.Format("/dlc/{0}/", voucher.Id)).Build();
			using (this.m_webClient.MakeRequestBlocking(request))
			{
			}
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x0006410C File Offset: 0x0006250C
		private IEnumerable<Voucher> GetVouchersForUserByStatus(ulong userId, string status)
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Voucher).Method(RequestMethod.GET).UrlPath(string.Format("/dlc/user/{0}/status/{1}/", userId, status)).Build();
			List<Voucher> result;
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				result = this.ParseVouchers(httpResponse);
			}
			return result;
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x00064188 File Offset: 0x00062588
		private IEnumerable<Voucher> GetVouchersImpl(ulong pageIndex, int pageSize)
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Voucher).Method(RequestMethod.GET).UrlPath(string.Format("/cti/{0}/{1}", pageIndex, pageSize)).Build();
			List<Voucher> result;
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				result = this.ParseVouchers(httpResponse);
			}
			return result;
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x00064208 File Offset: 0x00062608
		private List<Voucher> ParseVouchers(IHttpResponse resp)
		{
			List<Voucher> list = new List<Voucher>();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(new BufferedStream(resp.ContentStream));
			VoucherXmlSerializer voucherXmlSerializer = new VoucherXmlSerializer();
			IEnumerator enumerator = xmlDocument.DocumentElement.GetElementsByTagName("voucher").GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement element = (XmlElement)obj;
					Voucher item = voucherXmlSerializer.Deserialize(element);
					list.Add(item);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return list;
		}

		// Token: 0x04000BA3 RID: 2979
		private readonly IHttpClientBuilder m_webClientBuilder;

		// Token: 0x04000BA4 RID: 2980
		private readonly IHttpRequestFactory m_webRequestFactory;

		// Token: 0x04000BA5 RID: 2981
		private IRemoteService<IHttpRequest, IHttpResponse> m_webClient;
	}
}
