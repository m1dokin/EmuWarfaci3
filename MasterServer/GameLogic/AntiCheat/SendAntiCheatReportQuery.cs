using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.AntiCheat
{
	// Token: 0x02000261 RID: 609
	[QueryAttributes(TagName = "send_anticheat_report", QoSClass = "anticheat_report")]
	internal class SendAntiCheatReportQuery : BaseQuery
	{
		// Token: 0x06000D5E RID: 3422 RVA: 0x00035350 File Offset: 0x00033750
		public SendAntiCheatReportQuery(IAntiCheatService antiCheatService)
		{
			this.m_antiCheatService = antiCheatService;
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x00035360 File Offset: 0x00033760
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			string text;
			if (!base.GetServerID(fromJid, out text))
			{
				return -1;
			}
			this.m_antiCheatService.ProcessSessionReport(request);
			return 0;
		}

		// Token: 0x04000627 RID: 1575
		private readonly IAntiCheatService m_antiCheatService;
	}
}
