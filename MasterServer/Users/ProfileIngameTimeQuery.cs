using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.Users
{
	// Token: 0x020007F6 RID: 2038
	[QueryAttributes(TagName = "player_ingame_time")]
	internal class ProfileIngameTimeQuery : BaseQuery
	{
		// Token: 0x060029E4 RID: 10724 RVA: 0x000B49C8 File Offset: 0x000B2DC8
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			Dictionary<ulong, int> dictionary = queryParams[0] as Dictionary<ulong, int>;
			foreach (KeyValuePair<ulong, int> keyValuePair in dictionary)
			{
				XmlElement xmlElement = request.OwnerDocument.CreateElement("player");
				xmlElement.SetAttribute("pid", keyValuePair.Key.ToString());
				xmlElement.SetAttribute("ingame_time", keyValuePair.Value.ToString());
				request.AppendChild(xmlElement);
			}
		}
	}
}
