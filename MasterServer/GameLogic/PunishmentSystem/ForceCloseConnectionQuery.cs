using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.PunishmentSystem
{
	// Token: 0x0200040C RID: 1036
	[QueryAttributes(TagName = "force_close_connection")]
	internal class ForceCloseConnectionQuery : BaseQuery
	{
		// Token: 0x06001665 RID: 5733 RVA: 0x0005E2A4 File Offset: 0x0005C6A4
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			request.SetAttribute("nickname", value);
		}
	}
}
