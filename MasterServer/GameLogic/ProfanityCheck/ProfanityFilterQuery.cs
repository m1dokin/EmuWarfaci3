using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Platform.ProfanityCheck;

namespace MasterServer.GameLogic.ProfanityCheck
{
	// Token: 0x020003FD RID: 1021
	[QueryAttributes(TagName = "masterserver_profanity_filter")]
	internal class ProfanityFilterQuery : BaseQuery
	{
		// Token: 0x06001616 RID: 5654 RVA: 0x0005D180 File Offset: 0x0005B580
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			request.SetAttribute("source", value);
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x0005D1A4 File Offset: 0x0005B5A4
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			bool flag = response.GetAttribute("profane") == "1";
			string attribute = response.GetAttribute("result");
			return new ProfanityFilterResult
			{
				Result = ((!flag) ? ProfanityCheckResult.Succeeded : ProfanityCheckResult.Failed),
				Filtered = attribute
			};
		}

		// Token: 0x04000AAF RID: 2735
		public const string QueryName = "masterserver_profanity_filter";
	}
}
