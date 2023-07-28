using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoom
{
	// Token: 0x020004F1 RID: 1265
	[QueryAttributes(TagName = "on_surrender_requested")]
	internal class SurrenderQuery : BaseQuery
	{
		// Token: 0x06001B2C RID: 6956 RVA: 0x0006EE9C File Offset: 0x0006D29C
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = Convert.ToString(queryParams[0]);
			request.SetAttribute("team_id", value);
		}

		// Token: 0x04000D00 RID: 3328
		public const string Name = "on_surrender_requested";
	}
}
