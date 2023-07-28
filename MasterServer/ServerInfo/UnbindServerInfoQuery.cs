using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006CE RID: 1742
	[QueryAttributes(TagName = "unbind_server_info")]
	internal class UnbindServerInfoQuery : BaseQuery
	{
		// Token: 0x060024B0 RID: 9392 RVA: 0x00099BCC File Offset: 0x00097FCC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "UnbindServerInfoQuery"))
			{
				string attribute = request.GetAttribute("server");
				base.ServerRepository.RemoveServer(fromJid);
				result = 0;
			}
			return result;
		}
	}
}
