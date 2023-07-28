using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200049F RID: 1183
	[QueryAttributes(TagName = "map_voting_state")]
	internal class MapVotingStateQuery : BaseQuery
	{
		// Token: 0x06001933 RID: 6451 RVA: 0x00066D14 File Offset: 0x00065114
		public override void SendRequest(string online_id, XmlElement request, params object[] args)
		{
			XmlDocument ownerDocument = request.OwnerDocument;
			string value = args[0].ToString();
			Dictionary<string, int> dictionary = args[1] as Dictionary<string, int>;
			request.SetAttribute("bcast_receivers", value);
			foreach (KeyValuePair<string, int> keyValuePair in dictionary)
			{
				XmlElement xmlElement = ownerDocument.CreateElement("mission");
				xmlElement.SetAttribute("uid", keyValuePair.Key);
				xmlElement.SetAttribute("votes_num", keyValuePair.Value.ToString(CultureInfo.InvariantCulture));
				request.AppendChild(xmlElement);
			}
		}

		// Token: 0x04000C0C RID: 3084
		internal const string QueryName = "map_voting_state";
	}
}
