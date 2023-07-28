using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.Core
{
	// Token: 0x0200002A RID: 42
	[QueryAttributes(TagName = "autorotate")]
	internal class AutorotateQuery : BaseQuery
	{
		// Token: 0x06000098 RID: 152 RVA: 0x00006E14 File Offset: 0x00005214
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			request.SetAttribute("bcast_receivers", value);
		}
	}
}
