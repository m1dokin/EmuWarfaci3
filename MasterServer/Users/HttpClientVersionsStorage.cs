using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Web;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;

namespace MasterServer.Users
{
	// Token: 0x02000753 RID: 1875
	internal abstract class HttpClientVersionsStorage<TVersion> : ServiceModule, IClientVersionsStorage<TVersion>
	{
		// Token: 0x060026B0 RID: 9904 RVA: 0x000A3E97 File Offset: 0x000A2297
		protected HttpClientVersionsStorage(IHttpClientBuilder httpClientBuilder, IHttpRequestFactory httpRequestFactory)
		{
			this.m_httpClientBuilder = httpClientBuilder;
			this.m_httpRequestFactory = httpRequestFactory;
		}

		// Token: 0x060026B1 RID: 9905 RVA: 0x000A3EB0 File Offset: 0x000A22B0
		public override void Init()
		{
			ConfigSection section = Resources.XMPPSettings.GetSection("xmpp");
			Uri[] hostUrls = (from s in section.Get("auth_url").Split(new char[]
			{
				','
			})
			select new Uri(s)).ToArray<Uri>();
			this.m_webClient = this.m_httpClientBuilder.Failover(hostUrls).Build();
		}

		// Token: 0x060026B2 RID: 9906 RVA: 0x000A3F27 File Offset: 0x000A2327
		public override void Stop()
		{
			if (this.m_webClient == null)
			{
				return;
			}
			this.m_webClient.Dispose();
			this.m_webClient = null;
		}

		// Token: 0x060026B3 RID: 9907
		protected abstract TVersion Deserialize(string @string);

		// Token: 0x060026B4 RID: 9908
		protected abstract string Serialize(TVersion version);

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x060026B5 RID: 9909
		protected abstract string RequestUrlPath { get; }

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x060026B6 RID: 9910
		protected abstract string VersionsDelimiter { get; }

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x060026B7 RID: 9911
		protected abstract string MasterVersion { get; }

		// Token: 0x060026B8 RID: 9912 RVA: 0x000A3F48 File Offset: 0x000A2348
		public bool IsVersionsSetUpToDate()
		{
			return HttpClientVersionsStorage<TVersion>.VersionsPack.Parse(this.GetVersionsRaw(), this.VersionsDelimiter).MasterVersion.Equals(this.MasterVersion, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x060026B9 RID: 9913 RVA: 0x000A3F7C File Offset: 0x000A237C
		public IEnumerable<TVersion> GetVersions()
		{
			return HttpClientVersionsStorage<TVersion>.VersionsPack.Parse(this.GetVersionsRaw(), this.VersionsDelimiter).Versions.Select(new Func<string, TVersion>(this.Deserialize));
		}

		// Token: 0x060026BA RID: 9914 RVA: 0x000A3FB4 File Offset: 0x000A23B4
		private string GetVersionsRaw()
		{
			IHttpRequest request = this.m_httpRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.ClientVersions).Method(RequestMethod.GET).UrlPath(this.RequestUrlPath).QueryParams(new object[]
			{
				"action",
				"get_versions"
			}).Build();
			XDocument xdocument;
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				string text = new StreamReader(httpResponse.ContentStream).ReadToEnd();
				try
				{
					xdocument = XDocument.Parse(text);
				}
				catch (XmlException innerException)
				{
					throw new XmlException(string.Format("Can't parse '{0}'", text), innerException);
				}
			}
			XElement root = xdocument.Root;
			if (root == null)
			{
				Log.Error("[ClientVersionsManagementService] Responce received from common_data.php was empty. Something went seriously wrong!");
				return string.Empty;
			}
			string value = root.Attribute("result").Value;
			return (string.Compare(value, "ok", StringComparison.InvariantCultureIgnoreCase) == 0) ? root.Value : string.Empty;
		}

		// Token: 0x060026BB RID: 9915 RVA: 0x000A40CC File Offset: 0x000A24CC
		public void StoreVersions(IEnumerable<TVersion> versions)
		{
			HttpClientVersionsStorage<TVersion>.VersionsPack versionsPack = new HttpClientVersionsStorage<TVersion>.VersionsPack(this.MasterVersion, versions.Select(new Func<TVersion, string>(this.Serialize)));
			string str = versionsPack.ToString(this.VersionsDelimiter);
			string data = "versions=" + HttpUtility.UrlEncode(str);
			IHttpRequest request = this.m_httpRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.ClientVersions).Method(RequestMethod.POST).UrlPath(this.RequestUrlPath).QueryParams(new object[]
			{
				"action",
				"store_versions"
			}).Content("application/x-www-form-urlencoded", data, Encoding.ASCII).Build();
			XDocument xdocument;
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				xdocument = XDocument.Parse(new StreamReader(httpResponse.ContentStream).ReadToEnd());
			}
			XElement root = xdocument.Root;
			if (root == null)
			{
				Log.Error("[ClientVersionsManagementService] Response received from common_data.php was empty. Something went seriously wrong!");
				return;
			}
			string value = root.Attribute("result").Value;
			if (string.Compare(value, "ok", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				Log.Info("[ClientVersionsManagementService] Supported client versions were successfully synced.");
			}
			else
			{
				Log.Warning<string>("[ClientVersionsManagementService] Failed to store supported client versions: '{0}'", root.Attribute("description").Value);
			}
		}

		// Token: 0x040013F0 RID: 5104
		private readonly IHttpClientBuilder m_httpClientBuilder;

		// Token: 0x040013F1 RID: 5105
		private readonly IHttpRequestFactory m_httpRequestFactory;

		// Token: 0x040013F2 RID: 5106
		private IRemoteService<IHttpRequest, IHttpResponse> m_webClient;

		// Token: 0x02000754 RID: 1876
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct VersionsPack
		{
			// Token: 0x060026BD RID: 9917 RVA: 0x000A4234 File Offset: 0x000A2634
			public VersionsPack(string masterVersion, IEnumerable<string> versions)
			{
				this = default(HttpClientVersionsStorage<TVersion>.VersionsPack);
				this.MasterVersion = masterVersion;
				this.Versions = versions.ToList<string>();
			}

			// Token: 0x170003A5 RID: 933
			// (get) Token: 0x060026BE RID: 9918 RVA: 0x000A4250 File Offset: 0x000A2650
			// (set) Token: 0x060026BF RID: 9919 RVA: 0x000A4258 File Offset: 0x000A2658
			public string MasterVersion { get; private set; }

			// Token: 0x170003A6 RID: 934
			// (get) Token: 0x060026C0 RID: 9920 RVA: 0x000A4261 File Offset: 0x000A2661
			// (set) Token: 0x060026C1 RID: 9921 RVA: 0x000A4269 File Offset: 0x000A2669
			public IEnumerable<string> Versions { get; private set; }

			// Token: 0x060026C2 RID: 9922 RVA: 0x000A4274 File Offset: 0x000A2674
			public static HttpClientVersionsStorage<TVersion>.VersionsPack Parse(string source, string versionsDelimiter)
			{
				string[] array = source.Split(new string[]
				{
					"||"
				}, 2, StringSplitOptions.RemoveEmptyEntries);
				string masterVersion = (array.Length <= 0) ? string.Empty : array[0];
				IEnumerable<string> versions = (array.Length <= 1) ? Enumerable.Empty<string>() : array[1].Split(new string[]
				{
					versionsDelimiter
				}, StringSplitOptions.RemoveEmptyEntries);
				return new HttpClientVersionsStorage<TVersion>.VersionsPack(masterVersion, versions);
			}

			// Token: 0x060026C3 RID: 9923 RVA: 0x000A42DD File Offset: 0x000A26DD
			public override string ToString()
			{
				return this.ToString(";");
			}

			// Token: 0x060026C4 RID: 9924 RVA: 0x000A42EA File Offset: 0x000A26EA
			public string ToString(string versionsDelimiter)
			{
				return string.Format("{0}{1}{2}", this.MasterVersion, "||", string.Join(versionsDelimiter, this.Versions.ToArray<string>()));
			}

			// Token: 0x040013F4 RID: 5108
			private const string HEADER_DELIMITER = "||";
		}
	}
}
