using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Web;
using MasterServer.GameLogic.GameInterface;
using Network.Http;
using Network.Http.Builders;
using Network.Interfaces;

namespace MasterServer.Users
{
	// Token: 0x02000740 RID: 1856
	[Service]
	[Singleton]
	internal class AuthService : ServiceModule, IAuthService, INameReservationService, ITagStorage
	{
		// Token: 0x06002641 RID: 9793 RVA: 0x000A1EFE File Offset: 0x000A02FE
		public AuthService(IHttpClientBuilder webClientBuilder, IHttpRequestFactory webRequestFactory)
		{
			this.m_webClientBuilder = webClientBuilder;
			this.m_webRequestFactory = webRequestFactory;
		}

		// Token: 0x06002642 RID: 9794 RVA: 0x000A1F14 File Offset: 0x000A0314
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

		// Token: 0x06002643 RID: 9795 RVA: 0x000A1F8B File Offset: 0x000A038B
		public override void Stop()
		{
			if (this.m_webClient != null)
			{
				this.m_webClient.Dispose();
				this.m_webClient = null;
			}
		}

		// Token: 0x06002644 RID: 9796 RVA: 0x000A1FAC File Offset: 0x000A03AC
		public NameReservationResult ReserveName(string name, NameReservationGroup group, ulong user_id)
		{
			NameReservationResult result;
			try
			{
				IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).Method(RequestMethod.POST).UrlPath("name_reservation.php").QueryParams(new object[]
				{
					"action",
					"reserve",
					"name",
					name,
					"group",
					(int)group,
					"realm",
					Resources.RealmId,
					"owner",
					user_id
				}).Build();
				using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
				{
					AuthService.RawAuthResponse rawAuthResponse = this.ToAuthResponse(httpResponse);
					if (rawAuthResponse.Ok)
					{
						result = NameReservationResult.OK;
					}
					else if (string.Compare(rawAuthResponse.ErrorDescription, "already_exist", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						result = NameReservationResult.ALREADY_EXIST;
					}
					else
					{
						result = NameReservationResult.SERVICE_ERROR;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Failed to reserve name: " + ex.Message);
				result = NameReservationResult.SERVICE_ERROR;
			}
			return result;
		}

		// Token: 0x06002645 RID: 9797 RVA: 0x000A20D4 File Offset: 0x000A04D4
		public NameReservationResult CancelNameReservation(string name, NameReservationGroup group, ulong user_id)
		{
			NameReservationResult result;
			try
			{
				IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).Method(RequestMethod.POST).UrlPath("name_reservation.php").QueryParams(new object[]
				{
					"action",
					"cancel_reservation",
					"name",
					name,
					"group",
					(int)group,
					"realm",
					(user_id == 0UL) ? 0 : Resources.RealmId,
					"owner",
					user_id
				}).Build();
				using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
				{
					AuthService.RawAuthResponse rawAuthResponse = this.ToAuthResponse(httpResponse);
					if (rawAuthResponse.Ok)
					{
						result = NameReservationResult.OK;
					}
					else if (string.Compare(rawAuthResponse.ErrorDescription, "not_exist", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						result = NameReservationResult.NOT_EXIST;
					}
					else
					{
						result = NameReservationResult.SERVICE_ERROR;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Failed to cancel name reservation: " + ex.Message);
				result = NameReservationResult.SERVICE_ERROR;
			}
			return result;
		}

		// Token: 0x06002646 RID: 9798 RVA: 0x000A220C File Offset: 0x000A060C
		public NameReservationResult GetUserIdByReservedNickname(string name, NameReservationGroup group, out ulong userId)
		{
			userId = 0UL;
			NameReservationResult result;
			try
			{
				IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).Method(RequestMethod.POST).UrlPath("name_reservation.php").QueryParams(new object[]
				{
					"action",
					"get_userId_reservation",
					"name",
					name,
					"group",
					(int)group
				}).Build();
				using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
				{
					AuthService.RawAuthResponse rawAuthResponse = this.ToAuthResponse(httpResponse);
					if (rawAuthResponse.Ok)
					{
						userId = AuthService.ParseUserId(rawAuthResponse.Result, "get_userId_reservation", "user_id");
						result = NameReservationResult.OK;
					}
					else
					{
						result = ((!rawAuthResponse.ErrorDescription.Equals("not_exist", StringComparison.InvariantCultureIgnoreCase)) ? NameReservationResult.SERVICE_ERROR : NameReservationResult.NOT_EXIST);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Failed to get user id by reserved  name: " + ex.Message);
				result = NameReservationResult.SERVICE_ERROR;
			}
			return result;
		}

		// Token: 0x06002647 RID: 9799 RVA: 0x000A2328 File Offset: 0x000A0728
		public List<SUserAccessLevel> GetAccessLevel(ulong user_id)
		{
			try
			{
				IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).UrlPath("warfaceauth.php").QueryParams(new object[]
				{
					"action",
					"get_user_privileges",
					"user_id",
					user_id
				}).Build();
				using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
				{
					AuthService.RawAuthResponse rawAuthResponse = this.ToAuthResponse(httpResponse);
					return this.ParseUserAccess(rawAuthResponse.Result);
				}
			}
			catch (Exception ex)
			{
				Log.Error<ulong, string>("Failed to get access level for user {0}: {1}", user_id, ex.Message);
			}
			return new List<SUserAccessLevel>();
		}

		// Token: 0x06002648 RID: 9800 RVA: 0x000A23F4 File Offset: 0x000A07F4
		public List<SUserAccessLevel> GetAccessLevel()
		{
			try
			{
				IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).UrlPath("warfaceauth.php").QueryParams(new object[]
				{
					"action",
					"get_privileges"
				}).Build();
				using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
				{
					AuthService.RawAuthResponse rawAuthResponse = this.ToAuthResponse(httpResponse);
					return this.ParseUserAccess(rawAuthResponse.Result);
				}
			}
			catch (Exception ex)
			{
				Log.Error<string>("Failed to list of access level: {0}", ex.Message);
			}
			return new List<SUserAccessLevel>();
		}

		// Token: 0x06002649 RID: 9801 RVA: 0x000A24AC File Offset: 0x000A08AC
		public bool SetAccessLevel(SUserAccessLevel userAccessLevel)
		{
			try
			{
				if (!Resources.DebugQueriesEnabled && userAccessLevel.accessLevel.HasFlag(AccessLevel.Debug))
				{
					throw new ApplicationException("Trying to set Debug Access without DebugQueriesEnabled");
				}
				IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).Method(RequestMethod.POST).UrlPath("warfaceauth.php").QueryParams(new object[]
				{
					"action",
					"set_user_privileges",
					"id",
					userAccessLevel.db_id,
					"user_id",
					userAccessLevel.user_id,
					"access_level",
					(uint)userAccessLevel.accessLevel,
					"ip_mask",
					userAccessLevel.ip_mask
				}).Build();
				using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
				{
					AuthService.RawAuthResponse rawAuthResponse = this.ToAuthResponse(httpResponse);
					return rawAuthResponse.Ok;
				}
			}
			catch (Exception ex)
			{
				Log.Error<ulong, string>("Failed to set access level for user {0}: {1}", userAccessLevel.user_id, ex.Message);
			}
			return false;
		}

		// Token: 0x0600264A RID: 9802 RVA: 0x000A2614 File Offset: 0x000A0A14
		public bool RemoveAccessLevel(ulong id, ulong user_id)
		{
			try
			{
				IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).Method(RequestMethod.POST).UrlPath("warfaceauth.php").QueryParams(new object[]
				{
					"action",
					"remove_user_privileges",
					"id",
					id,
					"user_id",
					user_id
				}).Build();
				IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request);
				httpResponse.Dispose();
			}
			catch (Exception ex)
			{
				Log.Error<ulong, string>("Failed to remove access level with id {0}: {1}", id, ex.Message);
				return false;
			}
			return true;
		}

		// Token: 0x0600264B RID: 9803 RVA: 0x000A26CC File Offset: 0x000A0ACC
		public string GetPersistentUserTags(ulong user_id)
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).UrlPath("warfaceauth.php").QueryParams(new object[]
			{
				"action",
				"get_user_tags",
				"user_id",
				user_id
			}).Build();
			string attribute;
			using (IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request))
			{
				AuthService.RawAuthResponse rawAuthResponse = this.ToAuthResponse(httpResponse);
				attribute = rawAuthResponse.Result.GetAttribute("tags");
			}
			return attribute;
		}

		// Token: 0x0600264C RID: 9804 RVA: 0x000A2770 File Offset: 0x000A0B70
		public void SetPersistentUserTags(ulong user_id, string tags)
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).Method(RequestMethod.POST).UrlPath("warfaceauth.php").QueryParams(new object[]
			{
				"action",
				"set_user_tags",
				"user_id",
				user_id,
				"tags",
				tags
			}).Build();
			IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request);
			httpResponse.Dispose();
		}

		// Token: 0x0600264D RID: 9805 RVA: 0x000A27F0 File Offset: 0x000A0BF0
		public void RemovePersistentUserTags(ulong user_id)
		{
			IHttpRequest request = this.m_webRequestFactory.NewRequest().Domain<RequestDomain>(RequestDomain.Auth).Method(RequestMethod.POST).UrlPath("warfaceauth.php").QueryParams(new object[]
			{
				"action",
				"remove_user_tags",
				"user_id",
				user_id
			}).Build();
			IHttpResponse httpResponse = this.m_webClient.MakeRequestBlocking(request);
			httpResponse.Dispose();
		}

		// Token: 0x0600264E RID: 9806 RVA: 0x000A2864 File Offset: 0x000A0C64
		private AuthService.RawAuthResponse ToAuthResponse(IHttpResponse resp)
		{
			AuthService.RawAuthResponse result;
			using (StreamReader streamReader = new StreamReader(resp.ContentStream))
			{
				result = AuthService.RawAuthResponse.Parse(streamReader.ReadToEnd());
			}
			return result;
		}

		// Token: 0x0600264F RID: 9807 RVA: 0x000A28AC File Offset: 0x000A0CAC
		private List<SUserAccessLevel> ParseUserAccess(XmlElement elem)
		{
			List<SUserAccessLevel> list = new List<SUserAccessLevel>();
			IEnumerator enumerator = elem.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = (XmlElement)obj;
					ulong dbID = ulong.Parse(xmlElement.GetAttribute("id"));
					ulong userID = ulong.Parse(xmlElement.GetAttribute("user_id"));
					AccessLevel accessLevel = (AccessLevel)Enum.Parse(typeof(AccessLevel), xmlElement.GetAttribute("access_level"), true);
					string attribute = xmlElement.GetAttribute("ip_mask");
					SUserAccessLevel item = new SUserAccessLevel(dbID, userID, accessLevel, attribute);
					if (Resources.DebugQueriesEnabled || !item.accessLevel.HasFlag(AccessLevel.Debug))
					{
						list.Add(item);
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
			return list;
		}

		// Token: 0x06002650 RID: 9808 RVA: 0x000A29AC File Offset: 0x000A0DAC
		private static ulong ParseUserId(XmlNode elem, string nodeName, string attributeName = "user_id")
		{
			XmlNode xmlNode;
			if (elem.Name.Equals(nodeName) && elem.Attributes != null)
			{
				xmlNode = elem.Attributes[attributeName];
			}
			else
			{
				xmlNode = elem.SelectSingleNode(string.Format(".//{0}/@{1}", nodeName, attributeName));
			}
			ulong result;
			if (xmlNode != null && ulong.TryParse(xmlNode.Value, out result))
			{
				return result;
			}
			return 0UL;
		}

		// Token: 0x040013B9 RID: 5049
		private readonly IHttpClientBuilder m_webClientBuilder;

		// Token: 0x040013BA RID: 5050
		private readonly IHttpRequestFactory m_webRequestFactory;

		// Token: 0x040013BB RID: 5051
		private IRemoteService<IHttpRequest, IHttpResponse> m_webClient;

		// Token: 0x040013BC RID: 5052
		private const string m_authUrlPath = "warfaceauth.php";

		// Token: 0x040013BD RID: 5053
		private const string m_nameReservationUrlPath = "name_reservation.php";

		// Token: 0x02000741 RID: 1857
		private class RawAuthResponse
		{
			// Token: 0x06002653 RID: 9811 RVA: 0x000A2A28 File Offset: 0x000A0E28
			public static AuthService.RawAuthResponse Parse(string response)
			{
				AuthService.RawAuthResponse result;
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(response);
					XmlElement documentElement = xmlDocument.DocumentElement;
					bool flag = string.Compare(documentElement.GetAttribute("result"), "ok", StringComparison.InvariantCultureIgnoreCase) == 0;
					string errorDescription = (!flag) ? documentElement.GetAttribute("description") : string.Empty;
					result = new AuthService.RawAuthResponse
					{
						Ok = flag,
						ErrorDescription = errorDescription,
						Result = documentElement
					};
				}
				catch
				{
					Log.Warning("Auth response parsing error:\n" + response);
					throw;
				}
				return result;
			}

			// Token: 0x040013BF RID: 5055
			public bool Ok;

			// Token: 0x040013C0 RID: 5056
			public string ErrorDescription;

			// Token: 0x040013C1 RID: 5057
			public XmlElement Result;
		}
	}
}
