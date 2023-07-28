using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.Users
{
	// Token: 0x020007DB RID: 2011
	[QueryAttributes(TagName = "invitation_result")]
	internal class InvitationResultQuery : BaseQuery
	{
		// Token: 0x06002916 RID: 10518 RVA: 0x000B2220 File Offset: 0x000B0620
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			EInvitationStatus einvitationStatus = (EInvitationStatus)queryParams[0];
			string value = queryParams[1].ToString();
			string value2 = (string)queryParams[2];
			string name = "result";
			int num = (int)einvitationStatus;
			request.SetAttribute(name, num.ToString());
			request.SetAttribute("user", value);
			request.SetAttribute("is_follow", value2);
			if (queryParams.Length > 3)
			{
				request.SetAttribute("user_id", ((ulong)queryParams[3]).ToString());
			}
		}

		// Token: 0x040015E8 RID: 5608
		public const string QueryName = "invitation_result";
	}
}
