using System;
using System.Text;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Web;
using MasterServer.Platform.ProfanityCheck.Exceptions;
using MasterServer.Telemetry.Metrics;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006A4 RID: 1700
	[Service]
	[Singleton]
	internal class HttpEmulatorProfanityCheckService : HttpProfanityCheckService, IDebugProfanityCheckService
	{
		// Token: 0x060023B0 RID: 9136 RVA: 0x00096461 File Offset: 0x00094861
		public HttpEmulatorProfanityCheckService(IProfanityMetricsTracker profanityMetricsTracker, IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory) : base(profanityMetricsTracker, webClientBuilder, webRequestFactory)
		{
		}

		// Token: 0x060023B1 RID: 9137 RVA: 0x0009646C File Offset: 0x0009486C
		public void SetRequestFailedState(bool state)
		{
			try
			{
				HttpEmulatorProfanityCheckService.Settings requestSettings = this.GetRequestSettings();
				requestSettings.Fail = state;
				this.SetRequestSettings(requestSettings);
			}
			catch (Exception inner)
			{
				throw new ProfanityServiceException(string.Format("Can't set request failed state to {0}", state), inner);
			}
		}

		// Token: 0x060023B2 RID: 9138 RVA: 0x000964BC File Offset: 0x000948BC
		public bool GetRequestFailedState()
		{
			bool fail;
			try
			{
				HttpEmulatorProfanityCheckService.Settings requestSettings = this.GetRequestSettings();
				fail = requestSettings.Fail;
			}
			catch (Exception inner)
			{
				throw new ProfanityServiceException(string.Format("Can't get request failed state", new object[0]), inner);
			}
			return fail;
		}

		// Token: 0x060023B3 RID: 9139 RVA: 0x00096508 File Offset: 0x00094908
		public void SetRequestTimeout(TimeSpan timeout)
		{
			try
			{
				HttpEmulatorProfanityCheckService.Settings requestSettings = this.GetRequestSettings();
				requestSettings.Timeout = timeout;
				this.SetRequestSettings(requestSettings);
			}
			catch (Exception inner)
			{
				throw new ProfanityServiceException(string.Format("Can't set request timeout to {0}", timeout), inner);
			}
		}

		// Token: 0x060023B4 RID: 9140 RVA: 0x00096558 File Offset: 0x00094958
		public TimeSpan GetRequestTimeout()
		{
			TimeSpan timeout;
			try
			{
				HttpEmulatorProfanityCheckService.Settings requestSettings = this.GetRequestSettings();
				timeout = requestSettings.Timeout;
			}
			catch (Exception inner)
			{
				throw new ProfanityServiceException(string.Format("Can't get request timeout", new object[0]), inner);
			}
			return timeout;
		}

		// Token: 0x060023B5 RID: 9141 RVA: 0x000965A4 File Offset: 0x000949A4
		private HttpEmulatorProfanityCheckService.Settings GetRequestSettings()
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Profanity).Method(RequestMethod.GET).UrlPath("request/settings").Build();
			HttpEmulatorProfanityCheckService.Settings result;
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
				result = new HttpEmulatorProfanityCheckService.Settings(fail, timeout);
			}
			return result;
		}

		// Token: 0x060023B6 RID: 9142 RVA: 0x00096694 File Offset: 0x00094A94
		private void SetRequestSettings(HttpEmulatorProfanityCheckService.Settings settings)
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
						Log.Info<HttpEmulatorProfanityCheckService.Settings>("Request settings has been set: {0}", settings);
						return;
					}
				}
				throw new ProfanityServiceException(string.Format("Can't set request settings to <failed_state={0} timeout={1}>: {2}.", settings.Fail, settings.Timeout, fullContent));
			}
		}

		// Token: 0x020006A5 RID: 1701
		private class Settings
		{
			// Token: 0x060023B7 RID: 9143 RVA: 0x000967E8 File Offset: 0x00094BE8
			public Settings(bool fail, TimeSpan timeout)
			{
				this.Fail = fail;
				this.Timeout = timeout;
			}

			// Token: 0x060023B8 RID: 9144 RVA: 0x000967FE File Offset: 0x00094BFE
			public override string ToString()
			{
				return string.Format("Settings: Fail={0} Timeout={1}", this.Fail, this.Timeout);
			}

			// Token: 0x17000371 RID: 881
			// (get) Token: 0x060023B9 RID: 9145 RVA: 0x00096820 File Offset: 0x00094C20
			// (set) Token: 0x060023BA RID: 9146 RVA: 0x00096828 File Offset: 0x00094C28
			public bool Fail { get; set; }

			// Token: 0x17000372 RID: 882
			// (get) Token: 0x060023BB RID: 9147 RVA: 0x00096831 File Offset: 0x00094C31
			// (set) Token: 0x060023BC RID: 9148 RVA: 0x00096839 File Offset: 0x00094C39
			public TimeSpan Timeout { get; set; }
		}
	}
}
