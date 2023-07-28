using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000821 RID: 2081
	[QueryAttributes(TagName = "gameroom_sync")]
	internal class SyncRoomQuery : BaseQuery
	{
		// Token: 0x06002AD6 RID: 10966 RVA: 0x000B978C File Offset: 0x000B7B8C
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			IEnumerable<string> enumerable = (IEnumerable<string>)queryParams[0];
			XmlElement node = (XmlElement)queryParams[1];
			string text = string.Empty;
			foreach (string str in enumerable)
			{
				if (text.Length != 0)
				{
					text += ',';
				}
				text += str;
			}
			request.SetAttribute("bcast_receivers", text);
			XmlNode newChild = request.OwnerDocument.ImportNode(node, true);
			request.AppendChild(newChild);
		}
	}
}
