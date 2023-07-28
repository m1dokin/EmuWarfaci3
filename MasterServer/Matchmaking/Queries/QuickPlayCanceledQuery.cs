using System;
using System.Globalization;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking.Queries
{
	// Token: 0x02000517 RID: 1303
	[QueryAttributes(TagName = "gameroom_quickplay_canceled")]
	internal class QuickPlayCanceledQuery : BaseQuery
	{
		// Token: 0x06001C53 RID: 7251 RVA: 0x00071D78 File Offset: 0x00070178
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			MMEntityInfo mmentityInfo = (MMEntityInfo)queryParams[1];
			request.SetAttribute("bcast_receivers", value);
			request.SetAttribute("uid", mmentityInfo.Id.ToString(CultureInfo.InvariantCulture));
		}

		// Token: 0x04000D8B RID: 3467
		public const string QueryName = "gameroom_quickplay_canceled";
	}
}
