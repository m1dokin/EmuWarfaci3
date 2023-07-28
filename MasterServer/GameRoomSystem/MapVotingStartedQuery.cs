using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200049E RID: 1182
	[QueryAttributes(TagName = "map_voting_started")]
	internal class MapVotingStartedQuery : BaseQuery
	{
		// Token: 0x06001931 RID: 6449 RVA: 0x00066C38 File Offset: 0x00065038
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			XmlDocument ownerDocument = request.OwnerDocument;
			string value = args[0].ToString();
			IEnumerable<string> enumerable = args[1] as IEnumerable<string>;
			int num = (int)args[2];
			request.SetAttribute("bcast_receivers", value);
			request.SetAttribute("voting_time", num.ToString(CultureInfo.InvariantCulture));
			XmlElement xmlElement = ownerDocument.CreateElement("missions");
			foreach (string value2 in enumerable)
			{
				XmlElement xmlElement2 = ownerDocument.CreateElement("mission");
				xmlElement2.SetAttribute("uid", value2);
				xmlElement.AppendChild(xmlElement2);
			}
			request.AppendChild(xmlElement);
		}

		// Token: 0x04000C0B RID: 3083
		public const string QueryName = "map_voting_started";
	}
}
