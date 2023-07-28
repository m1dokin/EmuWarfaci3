using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006B9 RID: 1721
	[QueryAttributes(TagName = "bind_server_info")]
	internal class BindServerInfoQuery : BaseQuery
	{
		// Token: 0x06002418 RID: 9240 RVA: 0x00097028 File Offset: 0x00095428
		public BindServerInfoQuery(IOnlineClient onlineClient, IServerInfo serverInfo)
		{
			this.m_serverInfo = serverInfo;
		}

		// Token: 0x06002419 RID: 9241 RVA: 0x00097038 File Offset: 0x00095438
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			request.SetAttribute("server", value);
		}

		// Token: 0x0600241A RID: 9242 RVA: 0x0009705C File Offset: 0x0009545C
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			bool bindOk = response.GetAttribute("bind_result") == "success";
			string attribute = response.GetAttribute("server");
			this.m_serverInfo.OnServerBound(bindOk, attribute);
			return null;
		}

		// Token: 0x0400121A RID: 4634
		private readonly IServerInfo m_serverInfo;
	}
}
