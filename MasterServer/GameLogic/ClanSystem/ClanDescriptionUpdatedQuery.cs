using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000283 RID: 643
	[QueryAttributes(TagName = "clan_description_updated")]
	internal class ClanDescriptionUpdatedQuery : BaseQuery
	{
		// Token: 0x06000E05 RID: 3589 RVA: 0x00038934 File Offset: 0x00036D34
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			string value = queryParams[0] as string;
			request.SetAttribute("description", value);
		}
	}
}
