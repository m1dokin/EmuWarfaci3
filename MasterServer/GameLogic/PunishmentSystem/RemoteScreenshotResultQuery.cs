using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.PunishmentSystem
{
	// Token: 0x02000590 RID: 1424
	[QueryAttributes(TagName = "remote_screenshot_result")]
	internal class RemoteScreenshotResultQuery : BaseQuery
	{
		// Token: 0x06001EA8 RID: 7848 RVA: 0x0007C840 File Offset: 0x0007AC40
		public override void SendRequest(string online_id, XmlElement request, params object[] args)
		{
			string value = args[0].ToString();
			string value2 = args[1].ToString();
			request.SetAttribute("screenshot_id", value);
			request.SetAttribute("path", value2);
		}

		// Token: 0x06001EA9 RID: 7849 RVA: 0x0007C878 File Offset: 0x0007AC78
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "RemoteScreenshotResultQuery"))
			{
				long screenshotKey = long.Parse(request.GetAttribute("screenshot_id"));
				string attribute = request.GetAttribute("path");
				IPunishmentService service = ServicesManager.GetService<IPunishmentService>();
				service.OnScreenShotResult(screenshotKey, string.Format("{0}:{1}", fromJid, attribute));
				result = 0;
			}
			return result;
		}
	}
}
