using System;
using System.Collections;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000583 RID: 1411
	[QueryAttributes(TagName = "confirm_notification")]
	internal class ConfirmNotificationQuery : BaseQuery
	{
		// Token: 0x06001E56 RID: 7766 RVA: 0x0007B1D4 File Offset: 0x000795D4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			INotificationService service = ServicesManager.GetService<INotificationService>();
			IEnumerator enumerator = request.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					ulong notificationId = ulong.Parse(xmlNode.Attributes["id"].Value.ToString());
					service.Confirm(user.ProfileID, notificationId, xmlNode.SelectSingleNode("confirmation"));
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return 0;
		}
	}
}
