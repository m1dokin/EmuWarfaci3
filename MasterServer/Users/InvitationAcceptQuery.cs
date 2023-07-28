using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.Users
{
	// Token: 0x020007D9 RID: 2009
	[QueryAttributes(TagName = "invitation_accept")]
	internal class InvitationAcceptQuery : BaseQuery
	{
		// Token: 0x06002912 RID: 10514 RVA: 0x000B2088 File Offset: 0x000B0488
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "InvitationAcceptQuery"))
			{
				string attribute = request.GetAttribute("ticket");
				EInvitationStatus einvitationStatus = (EInvitationStatus)int.Parse(request.GetAttribute("result"));
				IUserInvitation service = ServicesManager.GetService<IUserInvitation>();
				EInvitationStatus einvitationStatus2 = service.InvitationResponse(attribute, einvitationStatus);
				result = (int)((einvitationStatus2 != einvitationStatus) ? einvitationStatus2 : EInvitationStatus.Accepted);
			}
			return result;
		}
	}
}
