using System;
using System.Net;
using System.Text;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Core.Web;
using MasterServer.Platform.Payment.Exceptions;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;

namespace MasterServer.Platform.Payment
{
	// Token: 0x0200068F RID: 1679
	[Service]
	[Singleton]
	internal class HttpWalletEmulatorService : ServiceModule, IWalletService
	{
		// Token: 0x06002339 RID: 9017 RVA: 0x000950DD File Offset: 0x000934DD
		public HttpWalletEmulatorService(IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory)
		{
			this.m_webClientBuilder = webClientBuilder;
			this.m_webRequestFactory = webRequestFactory;
		}

		// Token: 0x0600233A RID: 9018 RVA: 0x000950F4 File Offset: 0x000934F4
		public override void Init()
		{
			base.Init();
			string uriString = Resources.ModuleSettings.GetSection("Payment").Get("url");
			Uri[] hostUrls = new Uri[]
			{
				new Uri(uriString)
			};
			this.m_webClient = this.m_webClientBuilder.Failover(hostUrls).Build();
		}

		// Token: 0x0600233B RID: 9019 RVA: 0x00095148 File Offset: 0x00093548
		public override void Stop()
		{
			if (this.m_webClient != null)
			{
				this.m_webClient.Dispose();
				this.m_webClient = null;
			}
		}

		// Token: 0x0600233C RID: 9020 RVA: 0x00095167 File Offset: 0x00093567
		public ulong GetMoney(ulong userId)
		{
			return this.GetMoneyFromStorage(userId);
		}

		// Token: 0x0600233D RID: 9021 RVA: 0x00095170 File Offset: 0x00093570
		public void SetMoney(ulong userId, ulong amount)
		{
			try
			{
				this.SetMoneyToStorage(userId, amount);
			}
			catch (AggregateException inner)
			{
				throw new PaymentServiceException(inner);
			}
		}

		// Token: 0x0600233E RID: 9022 RVA: 0x000951A4 File Offset: 0x000935A4
		public ulong AddMoney(ulong userId, ulong amount)
		{
			ulong result;
			try
			{
				ulong moneyFromStorage = this.GetMoneyFromStorage(userId);
				result = this.SetMoneyToStorage(userId, moneyFromStorage + amount);
			}
			catch (AggregateException inner)
			{
				throw new PaymentServiceException(inner);
			}
			return result;
		}

		// Token: 0x0600233F RID: 9023 RVA: 0x000951E4 File Offset: 0x000935E4
		public PaymentResult SpendMoney(ulong userId, ulong amount)
		{
			ulong moneyFromStorage = this.GetMoneyFromStorage(userId);
			if (amount > moneyFromStorage)
			{
				return PaymentResult.NotEnoughMoney;
			}
			this.SetMoneyToStorage(userId, moneyFromStorage - amount);
			return PaymentResult.Ok;
		}

		// Token: 0x06002340 RID: 9024 RVA: 0x00095210 File Offset: 0x00093610
		public void SetRequestFailedState(bool state)
		{
			try
			{
				HttpWalletEmulatorService.Settings requestSettings = this.GetRequestSettings();
				requestSettings.Fail = state;
				this.SetRequestSettings(requestSettings);
			}
			catch (Exception inner)
			{
				throw new PaymentServiceException(string.Format("Can't set request failed state to {0}", state), inner);
			}
		}

		// Token: 0x06002341 RID: 9025 RVA: 0x00095260 File Offset: 0x00093660
		public bool GetRequestFailedState()
		{
			bool fail;
			try
			{
				HttpWalletEmulatorService.Settings requestSettings = this.GetRequestSettings();
				fail = requestSettings.Fail;
			}
			catch (Exception inner)
			{
				throw new PaymentServiceException(string.Format("Can't get request failed state", new object[0]), inner);
			}
			return fail;
		}

		// Token: 0x06002342 RID: 9026 RVA: 0x000952AC File Offset: 0x000936AC
		public void SetRequestTimeout(TimeSpan timeout)
		{
			try
			{
				HttpWalletEmulatorService.Settings requestSettings = this.GetRequestSettings();
				requestSettings.Timeout = timeout;
				this.SetRequestSettings(requestSettings);
			}
			catch (Exception inner)
			{
				throw new PaymentServiceException(string.Format("Can't set request timeout to {0}", timeout), inner);
			}
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x000952FC File Offset: 0x000936FC
		public TimeSpan GetRequestTimeout()
		{
			TimeSpan timeout;
			try
			{
				HttpWalletEmulatorService.Settings requestSettings = this.GetRequestSettings();
				timeout = requestSettings.Timeout;
			}
			catch (Exception inner)
			{
				throw new PaymentServiceException(string.Format("Can't get request timeout", new object[0]), inner);
			}
			return timeout;
		}

		// Token: 0x06002344 RID: 9028 RVA: 0x00095348 File Offset: 0x00093748
		private ulong SetMoneyToStorage(ulong userId, ulong amount)
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Wallet).Method(RequestMethod.PUT).UrlPath(string.Format("/{0}/{1}", userId, amount)).Build();
			ulong result;
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				string fullContent = httpResponse.GetFullContent();
				if (httpResponse.StatusCode != HttpStatusCode.OK)
				{
					throw new PaymentServiceInternalErrorException(string.Format("Can't set money for {0}: result {1}", userId, httpResponse.StatusCode));
				}
				result = ulong.Parse(fullContent);
			}
			return result;
		}

		// Token: 0x06002345 RID: 9029 RVA: 0x000953FC File Offset: 0x000937FC
		private ulong GetMoneyFromStorage(ulong userId)
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Wallet).Method(RequestMethod.GET).UrlPath(string.Format("/{0}", userId)).Build();
			ulong result;
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				string fullContent = httpResponse.GetFullContent();
				if (httpResponse.StatusCode != HttpStatusCode.OK)
				{
					throw new PaymentServiceInternalErrorException(string.Format("Can't get money for {0}: result {1}", userId, httpResponse.StatusCode));
				}
				result = ulong.Parse(fullContent);
			}
			return result;
		}

		// Token: 0x06002346 RID: 9030 RVA: 0x000954AC File Offset: 0x000938AC
		private HttpWalletEmulatorService.Settings GetRequestSettings()
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Profanity).Method(RequestMethod.GET).UrlPath("request/settings").Build();
			HttpWalletEmulatorService.Settings result;
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				string fullContent = httpResponse.GetFullContent();
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.LoadXml(fullContent);
				}
				catch (Exception innerException)
				{
					throw new XmlException(string.Format("Can't parse response: {0}", fullContent), innerException);
				}
				XmlElement documentElement = xmlDocument.DocumentElement;
				XmlElement xmlElement = documentElement["settings"];
				bool fail = uint.Parse(xmlElement.GetAttribute("fail")) > 0U;
				TimeSpan timeout = TimeSpan.FromMilliseconds(uint.Parse(xmlElement.GetAttribute("timeout_ms")));
				result = new HttpWalletEmulatorService.Settings(fail, timeout);
			}
			return result;
		}

		// Token: 0x06002347 RID: 9031 RVA: 0x0009559C File Offset: 0x0009399C
		private void SetRequestSettings(HttpWalletEmulatorService.Settings settings)
		{
			string data = string.Format("<request><settings fail=\"{0}\" timeout_ms=\"{1}\" /></request>", (!settings.Fail) ? 0 : 1, (int)settings.Timeout.TotalMilliseconds);
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Profanity).Method(RequestMethod.PUT).UrlPath("request/settings").Content("text/xml", data, Encoding.ASCII).Build();
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				string fullContent = httpResponse.GetFullContent();
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.LoadXml(fullContent);
				}
				catch (Exception innerException)
				{
					throw new XmlException(string.Format("Can't parse response: {0}", fullContent), innerException);
				}
				XmlElement documentElement = xmlDocument.DocumentElement;
				string attribute = documentElement.GetAttribute("result");
				if (attribute != null)
				{
					if (attribute == "ok")
					{
						Log.Info<HttpWalletEmulatorService.Settings>("Request settings has been set: {0}", settings);
						return;
					}
				}
				throw new PaymentServiceException(string.Format("Can't set request settings to <failed_state={0} timeout={1}>: {2}.", settings.Fail, settings.Timeout, fullContent));
			}
		}

		// Token: 0x040011DA RID: 4570
		private readonly IHttpClientBuilder m_webClientBuilder;

		// Token: 0x040011DB RID: 4571
		private readonly IHttpRequestFactory m_webRequestFactory;

		// Token: 0x040011DC RID: 4572
		private IRemoteService<IHttpRequest, IHttpResponse> m_webClient;

		// Token: 0x02000690 RID: 1680
		private class Settings
		{
			// Token: 0x06002348 RID: 9032 RVA: 0x000956F0 File Offset: 0x00093AF0
			public Settings(bool fail, TimeSpan timeout)
			{
				this.Fail = fail;
				this.Timeout = timeout;
			}

			// Token: 0x06002349 RID: 9033 RVA: 0x00095706 File Offset: 0x00093B06
			public override string ToString()
			{
				return string.Format("Settings: Fail={0} Timeout={1}", this.Fail, this.Timeout);
			}

			// Token: 0x1700036F RID: 879
			// (get) Token: 0x0600234A RID: 9034 RVA: 0x00095728 File Offset: 0x00093B28
			// (set) Token: 0x0600234B RID: 9035 RVA: 0x00095730 File Offset: 0x00093B30
			public bool Fail { get; set; }

			// Token: 0x17000370 RID: 880
			// (get) Token: 0x0600234C RID: 9036 RVA: 0x00095739 File Offset: 0x00093B39
			// (set) Token: 0x0600234D RID: 9037 RVA: 0x00095741 File Offset: 0x00093B41
			public TimeSpan Timeout { get; set; }
		}
	}
}
