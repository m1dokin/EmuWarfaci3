using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Platform.ProfanityCheck;

namespace MasterServer.GameLogic.ProfanityCheck
{
	// Token: 0x020003FB RID: 1019
	[QueryAttributes(TagName = "masterserver_profanity_check")]
	internal class ProfanityCheckQuery : BaseQuery
	{
		// Token: 0x06001612 RID: 5650 RVA: 0x0005D0F4 File Offset: 0x0005B4F4
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			request.SetAttribute("source", value);
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x0005D118 File Offset: 0x0005B518
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			string attribute = response.GetAttribute("profane");
			if (attribute != null)
			{
				if (attribute == "1")
				{
					return ProfanityCheckResult.Failed;
				}
				if (attribute == "2")
				{
					return ProfanityCheckResult.Reserved;
				}
			}
			return ProfanityCheckResult.Succeeded;
		}

		// Token: 0x04000AAC RID: 2732
		public const string QueryName = "masterserver_profanity_check";
	}
}
