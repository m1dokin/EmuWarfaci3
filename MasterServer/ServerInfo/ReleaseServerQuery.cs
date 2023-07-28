using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006CF RID: 1743
	[QueryAttributes(TagName = "release_server")]
	internal class ReleaseServerQuery : BaseQuery
	{
		// Token: 0x060024B2 RID: 9394 RVA: 0x00099C2C File Offset: 0x0009802C
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			string value = (string)args[0];
			request.SetAttribute("server_jid", value);
		}

		// Token: 0x060024B3 RID: 9395 RVA: 0x00099C4E File Offset: 0x0009804E
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			return 0;
		}

		// Token: 0x04001289 RID: 4745
		public const string ServerJidAttributeName = "server_jid";
	}
}
