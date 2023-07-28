using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000798 RID: 1944
	[QueryAttributes(TagName = "mission_update")]
	internal class MissionUpdateQuery : BaseQuery
	{
		// Token: 0x0600284D RID: 10317 RVA: 0x000AD970 File Offset: 0x000ABD70
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			XmlElement node = (XmlElement)queryParams[0];
			XmlNode newChild = request.OwnerDocument.ImportNode(node, true);
			request.AppendChild(newChild);
		}
	}
}
