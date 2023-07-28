using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Web;
using MasterServer.Database;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003AA RID: 938
	[Service]
	[Singleton]
	internal class RealmsMissionGeneration : ServiceModule, IRealmsMissionGeneration
	{
		// Token: 0x060014BF RID: 5311 RVA: 0x00055059 File Offset: 0x00053459
		public RealmsMissionGeneration(IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory)
		{
			this.m_webClientBuilder = webClientBuilder;
			this.m_webRequestFactory = webRequestFactory;
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x00055070 File Offset: 0x00053470
		public override void Init()
		{
			ConfigSection section = Resources.XMPPSettings.GetSection("xmpp");
			Uri[] hostUrls = (from s in section.Get("auth_url").Split(new char[]
			{
				','
			})
			select new Uri(s)).ToArray<Uri>();
			this.m_webClient = this.m_webClientBuilder.Failover(hostUrls).Build();
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x000550E7 File Offset: 0x000534E7
		public override void Stop()
		{
			if (this.m_webClient != null)
			{
				this.m_webClient.Dispose();
				this.m_webClient = null;
			}
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x060014C2 RID: 5314 RVA: 0x00055106 File Offset: 0x00053506
		// (set) Token: 0x060014C3 RID: 5315 RVA: 0x0005510E File Offset: 0x0005350E
		public bool Enabled { get; private set; }

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x060014C4 RID: 5316 RVA: 0x00055117 File Offset: 0x00053517
		public bool GenerationRole
		{
			get
			{
				if (this.Enabled)
				{
					return Resources.RealmDBUpdaterPermission;
				}
				return Resources.RealmDBUpdaterPermission || Resources.DBUpdaterPermission;
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x060014C5 RID: 5317 RVA: 0x0005513C File Offset: 0x0005353C
		public bool RealmSyncRole
		{
			get
			{
				return this.Enabled && Resources.DBUpdaterPermission && !Resources.RealmDBUpdaterPermission;
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x060014C6 RID: 5318 RVA: 0x00055160 File Offset: 0x00053560
		public bool SlaveRole
		{
			get
			{
				return !Resources.RealmDBUpdaterPermission && !Resources.DBUpdaterPermission;
			}
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x00055178 File Offset: 0x00053578
		public RealmMissionLoadResult LoadRealmMissions(bool forceSync)
		{
			MissionSet missionSet = new MissionSet();
			if (!this.Enabled)
			{
				return new RealmMissionLoadResult(LoadResult.EMPTY);
			}
			IHttpRequest request;
			if (forceSync)
			{
				request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Missions).UrlPath("common_data.php").QueryParams(new object[]
				{
					"action",
					"get_missions"
				}).Build();
			}
			else
			{
				IDBUpdateService service = ServicesManager.GetService<IDBUpdateService>();
				IDALService service2 = ServicesManager.GetService<IDALService>();
				string dataGroupHash = service.GetDataGroupHash(MissionGenerationService.DATA_HASH_GROUP, true);
				int generation = service2.MissionSystem.GetGeneration();
				request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Missions).UrlPath("common_data.php").QueryParams(new object[]
				{
					"action",
					"get_missions",
					"hash",
					dataGroupHash,
					"generation",
					generation
				}).Build();
			}
			string xml;
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				xml = new StreamReader(httpResponse.ContentStream).ReadToEnd();
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			XmlElement documentElement = xmlDocument.DocumentElement;
			string attribute = documentElement.GetAttribute("result");
			if (string.Compare(attribute, "empty", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				Log.Warning("Can't get missions from common mission generation service.");
				return new RealmMissionLoadResult(LoadResult.EMPTY);
			}
			if (string.Compare(attribute, "equal", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return new RealmMissionLoadResult(LoadResult.EQUAL);
			}
			missionSet.Generation = int.Parse(documentElement.Attributes["generation"].Value);
			missionSet.Hash = documentElement.Attributes["hash"].Value;
			byte[] array = null;
			try
			{
				array = Convert.FromBase64String(documentElement.InnerXml.Trim());
			}
			catch (FormatException)
			{
			}
			if (Utils.IsGZipHeader(array))
			{
				byte[] bytes;
				using (GZipStream gzipStream = new GZipStream(new MemoryStream(array), CompressionMode.Decompress))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						byte[] array2 = new byte[1024];
						int count;
						while ((count = gzipStream.Read(array2, 0, array2.Length)) != 0)
						{
							memoryStream.Write(array2, 0, count);
						}
						bytes = memoryStream.ToArray();
					}
				}
				xml = string.Format("<missions>{0}</missions>", Encoding.UTF8.GetString(bytes));
				xmlDocument.LoadXml(xml);
				documentElement = xmlDocument.DocumentElement;
			}
			IEnumerator enumerator = documentElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = obj as XmlElement;
					if (xmlElement != null && xmlElement.Name == "mission")
					{
						MissionContext missionContext = MissionParser.Parse(xmlElement.OuterXml);
						missionSet.Missions.Add(new Guid(missionContext.uid), missionContext);
					}
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
			return new RealmMissionLoadResult(LoadResult.OK, missionSet);
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x000554E4 File Offset: 0x000538E4
		public bool SaveRealmMissions(MissionSet realm_missions)
		{
			if (!this.Enabled)
			{
				return true;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MissionContext missionContext in realm_missions.Missions.Values)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(missionContext.data);
				xmlDocument.DocumentElement.SetAttribute("generation", missionContext.Generation.ToString());
				stringBuilder.AppendLine(xmlDocument.OuterXml);
			}
			string text = stringBuilder.ToString();
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			byte[] inArray;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					gzipStream.Write(bytes, 0, bytes.Length);
				}
				inArray = memoryStream.ToArray();
			}
			text = "missions=" + HttpUtility.UrlEncode(Convert.ToBase64String(inArray));
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Missions).Method(RequestMethod.POST).UrlPath("common_data.php").QueryParams(new object[]
			{
				"action",
				"store_missions",
				"generation",
				realm_missions.Generation,
				"hash",
				realm_missions.Hash
			}).Content("application/x-www-form-urlencoded", text, Encoding.ASCII).Build();
			XmlDocument xmlDocument2 = new XmlDocument();
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				xmlDocument2.Load(httpResponse.ContentStream);
			}
			string attribute = xmlDocument2.DocumentElement.GetAttribute("result");
			if (string.Compare(attribute, "ok", StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				Log.Warning("Can't store missions to common mission generation service.");
				return false;
			}
			return true;
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x00055718 File Offset: 0x00053B18
		public void SetRealmGeneration(Config cfg)
		{
			this.Enabled = Resources.UseRealmMissions;
			if (cfg.HasValue("use_realm_missions"))
			{
				int num;
				cfg.Get("use_realm_missions", out num);
				this.Enabled = (this.Enabled && num == 1);
			}
			Log.Info<string>("Realm mission generation is {0}", (!this.Enabled) ? "disabled" : "enabled");
		}

		// Token: 0x040009C8 RID: 2504
		private const string m_requestUrlPath = "common_data.php";

		// Token: 0x040009C9 RID: 2505
		private readonly IHttpClientBuilder m_webClientBuilder;

		// Token: 0x040009CA RID: 2506
		private readonly IHttpRequestFactory m_webRequestFactory;

		// Token: 0x040009CB RID: 2507
		private IRemoteService<IHttpRequest, IHttpResponse> m_webClient;
	}
}
