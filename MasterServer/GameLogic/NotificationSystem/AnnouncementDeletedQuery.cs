using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000585 RID: 1413
	[QueryAttributes(TagName = "announcement_deleted")]
	internal class AnnouncementDeletedQuery : BaseQuery
	{
		// Token: 0x06001E5A RID: 7770 RVA: 0x0007B324 File Offset: 0x00079724
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			ulong num = (ulong)queryParams[1];
			request.SetAttribute("bcast_receivers", value);
			request.SetAttribute("deleted_id", num.ToString());
		}
	}
}
