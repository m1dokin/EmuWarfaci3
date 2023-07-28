using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.PunishmentSystem
{
	// Token: 0x0200058F RID: 1423
	[QueryAttributes(TagName = "remote_screenshot")]
	internal class RemoteScreenshotQuery : BaseQuery
	{
		// Token: 0x06001EA6 RID: 7846 RVA: 0x0007C790 File Offset: 0x0007AB90
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			request.SetAttribute("profile_id", queryParams[0].ToString());
			request.SetAttribute("frontBuffer", (!(bool)queryParams[1]) ? "0" : "1");
			request.SetAttribute("count", queryParams[2].ToString());
			request.SetAttribute("scaleW", queryParams[3].ToString());
			request.SetAttribute("scaleH", queryParams[4].ToString());
			request.SetAttribute("screenshot_id", queryParams[5].ToString());
			request.SetAttribute("initiator", queryParams[6].ToString());
		}
	}
}
