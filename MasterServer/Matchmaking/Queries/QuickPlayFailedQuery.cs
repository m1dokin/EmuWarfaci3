using System;
using System.Globalization;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking.Queries
{
	// Token: 0x02000677 RID: 1655
	[QueryAttributes(TagName = "gameroom_quickplay_failed")]
	internal class QuickPlayFailedQuery : BaseQuery
	{
		// Token: 0x060022F6 RID: 8950 RVA: 0x00092350 File Offset: 0x00090750
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			MMResultEntity mmresultEntity = (MMResultEntity)queryParams[1];
			request.SetAttribute("bcast_receivers", value);
			request.SetAttribute("uid", mmresultEntity.EntityId.ToString(CultureInfo.InvariantCulture));
		}

		// Token: 0x0400118D RID: 4493
		public const string QueryName = "gameroom_quickplay_failed";
	}
}
