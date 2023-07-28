using System;
using System.Globalization;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking.Queries
{
	// Token: 0x02000597 RID: 1431
	[QueryAttributes(TagName = "gameroom_quickplay_succeeded")]
	internal class QuickPlaySucceededQuery : BaseQuery
	{
		// Token: 0x06001ED9 RID: 7897 RVA: 0x0007D278 File Offset: 0x0007B678
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			MMResultEntity mmresultEntity = (MMResultEntity)queryParams[1];
			request.SetAttribute("bcast_receivers", value);
			request.SetAttribute("uid", mmresultEntity.EntityId.ToString(CultureInfo.InvariantCulture));
		}

		// Token: 0x04000F06 RID: 3846
		public const string QueryName = "gameroom_quickplay_succeeded";
	}
}
