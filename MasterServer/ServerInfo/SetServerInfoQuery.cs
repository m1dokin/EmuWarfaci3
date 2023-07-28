using System;
using System.Xml;
using DedicatedPoolServer.Model;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.ServerInfo
{
	// Token: 0x02000831 RID: 2097
	[QueryAttributes(TagName = "setserver")]
	internal class SetServerInfoQuery : BaseQuery
	{
		// Token: 0x06002B6A RID: 11114 RVA: 0x000BBCF7 File Offset: 0x000BA0F7
		public SetServerInfoQuery(IServerInfo serverInfo, IOnlineClient onlineClient, IClientVersionsManagementService clientVersionsManagementService)
		{
			this.m_serverInfo = serverInfo;
			this.m_onlineClient = onlineClient;
			this.m_clientVersionsManagementService = clientVersionsManagementService;
		}

		// Token: 0x06002B6B RID: 11115 RVA: 0x000BBD14 File Offset: 0x000BA114
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetServerInfoQuery"))
			{
				ClientVersion clientVersion;
				ClientVersion.TryParse(request.GetAttribute("version"), out clientVersion);
				if (clientVersion != SetServerInfoQuery.m_masterVersion && (!Resources.DebugQueriesEnabled || !this.m_clientVersionsManagementService.Validate(clientVersion)))
				{
					response.SetAttribute("status", "version_mismatch");
					Log.Warning<string, ClientVersion>("SetServer request from '{0}' was declined due to the version mismatch ({1})", fromJid, clientVersion);
					result = 0;
				}
				else if (!request.HasAttribute("server") || string.IsNullOrEmpty(request.GetAttribute("server")))
				{
					result = 0;
				}
				else
				{
					ServerEntity serverEntity;
					this.ParseServerInfo(request, fromJid, out serverEntity);
					base.ServerRepository.AddServer(fromJid, serverEntity.ServerID);
					this.m_serverInfo.OnServerInfo(serverEntity);
					if (!this.m_serverInfo.IsGlobalLbsEnabled && this.m_serverInfo.IsReconnectByNodeEnabled)
					{
						response.SetAttribute("master_node", this.m_onlineClient.Server);
					}
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06002B6C RID: 11116 RVA: 0x000BBE40 File Offset: 0x000BA240
		private void ParseServerInfo(XmlElement el, string jid, out ServerEntity ent)
		{
			float val = (float)int.Parse(el.GetAttribute("cpu_usage"));
			float num = (float)int.Parse(el.GetAttribute("memory_usage"));
			float val2 = Math.Max(0f, num - 900f);
			ent = new ServerEntity
			{
				ServerID = el.GetAttribute("server"),
				OnlineID = jid,
				Hostname = el.GetAttribute("host"),
				Port = int.Parse(el.GetAttribute("port")),
				Node = el.GetAttribute("node"),
				Mode = ((!(el.GetAttribute("mode") == "pure_pvp")) ? DedicatedMode.PVP_PVE : DedicatedMode.PurePVP),
				SessionID = el.GetAttribute("session_id"),
				Mission = el.GetAttribute("mission_key"),
				Status = (EGameServerStatus)int.Parse(el.GetAttribute("status")),
				BuildType = el.GetAttribute("build_type"),
				MasterServerId = Resources.XmppResource,
				PerformanceIndex = Math.Max(val, val2) * 100f + Math.Min(val, val2) + num / 1000f
			};
		}

		// Token: 0x04001724 RID: 5924
		public const string QueryName = "setserver";

		// Token: 0x04001725 RID: 5925
		private static readonly ClientVersion m_masterVersion = ClientVersion.Parse(Resources.MasterVersion);

		// Token: 0x04001726 RID: 5926
		private readonly IServerInfo m_serverInfo;

		// Token: 0x04001727 RID: 5927
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04001728 RID: 5928
		private readonly IClientVersionsManagementService m_clientVersionsManagementService;
	}
}
