using System;
using System.Text.RegularExpressions;
using HK2Net;
using MasterServer.Core;
using Network.Http.Builders;

namespace MasterServer.Users
{
	// Token: 0x02000756 RID: 1878
	[Service]
	[Singleton]
	internal class HttpRegexClientVersionsStorage : HttpClientVersionsStorage<Regex>, IRegexClientVersionsStorage, IClientVersionsStorage<Regex>
	{
		// Token: 0x060026C5 RID: 9925 RVA: 0x000A4312 File Offset: 0x000A2712
		public HttpRegexClientVersionsStorage(IHttpClientBuilder httpClientBuilder, IHttpRequestFactory httpRequestFactory) : base(httpClientBuilder, httpRequestFactory)
		{
		}

		// Token: 0x060026C6 RID: 9926 RVA: 0x000A431C File Offset: 0x000A271C
		protected override Regex Deserialize(string @string)
		{
			return new Regex(@string);
		}

		// Token: 0x060026C7 RID: 9927 RVA: 0x000A4324 File Offset: 0x000A2724
		protected override string Serialize(Regex version)
		{
			return version.ToString().Replace("\\", "\\\\");
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x060026C8 RID: 9928 RVA: 0x000A433B File Offset: 0x000A273B
		protected override string RequestUrlPath
		{
			get
			{
				return "common_data.php";
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x060026C9 RID: 9929 RVA: 0x000A4342 File Offset: 0x000A2742
		protected override string VersionsDelimiter
		{
			get
			{
				return ";";
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x060026CA RID: 9930 RVA: 0x000A4349 File Offset: 0x000A2749
		protected override string MasterVersion
		{
			get
			{
				return Resources.MasterVersion;
			}
		}

		// Token: 0x040013F7 RID: 5111
		private const string REQUEST_URL_PATH = "common_data.php";

		// Token: 0x040013F8 RID: 5112
		private const string VERSIONS_DELIMITER = ";";
	}
}
