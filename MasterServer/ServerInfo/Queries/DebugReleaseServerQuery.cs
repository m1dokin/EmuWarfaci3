using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.ServerInfo.Queries
{
	// Token: 0x020006BF RID: 1727
	[QueryAttributes(TagName = "debug_release_server")]
	internal class DebugReleaseServerQuery : BaseQuery
	{
		// Token: 0x0600242B RID: 9259 RVA: 0x000973EC File Offset: 0x000957EC
		public DebugReleaseServerQuery(IServerInfo serverInfo)
		{
			this.m_serverInfo = serverInfo;
		}

		// Token: 0x0600242C RID: 9260 RVA: 0x000973FC File Offset: 0x000957FC
		public override int QueryGetResponse(string from, XmlElement request, XmlElement response)
		{
			if (!Resources.DebugQueriesEnabled)
			{
				return -1;
			}
			string attribute = request.GetAttribute("server_id");
			if (this.m_serverInfo.ReleaseServer(attribute, true))
			{
				response.SetAttribute("result", "ok");
				return 0;
			}
			response.SetAttribute("result", "failed");
			return -1;
		}

		// Token: 0x04001228 RID: 4648
		public const string QueryName = "debug_release_server";

		// Token: 0x04001229 RID: 4649
		private const string ServerIdAttributeName = "server_id";

		// Token: 0x0400122A RID: 4650
		private readonly IServerInfo m_serverInfo;
	}
}
