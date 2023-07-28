using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006C0 RID: 1728
	[DebugQuery]
	[QueryAttributes(TagName = "get_test_server")]
	internal class GetTestServerQuery : BaseQuery
	{
		// Token: 0x0600242E RID: 9262 RVA: 0x00097460 File Offset: 0x00095860
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetTestSertverQuery"))
			{
				IServerInfo service = ServicesManager.GetService<IServerInfo>();
				ServerEntity serverEntity;
				if (!service.GetTestServer(out serverEntity))
				{
					result = -1;
				}
				else
				{
					response.SetAttribute("srv_host_name", serverEntity.Hostname);
					response.SetAttribute("srv_mission", serverEntity.Mission);
					response.SetAttribute("srv_online_id", serverEntity.OnlineID);
					response.SetAttribute("srv_port", serverEntity.Port.ToString());
					response.SetAttribute("srv_server_id", serverEntity.ServerID);
					response.SetAttribute("srv_build_type", serverEntity.BuildType);
					result = 0;
				}
			}
			return result;
		}
	}
}
