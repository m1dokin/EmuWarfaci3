using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.Core
{
	// Token: 0x02000131 RID: 305
	[QueryAttributes(TagName = "stop_service")]
	internal class StopServiceQuery : BaseQuery
	{
		// Token: 0x06000505 RID: 1285 RVA: 0x00015974 File Offset: 0x00013D74
		public StopServiceQuery(IApplicationService applicationService)
		{
			this.m_applicationService = applicationService;
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x00015984 File Offset: 0x00013D84
		public override int QueryGetResponse(string from, XmlElement request, XmlElement response)
		{
			string[] array = from.Split(new char[]
			{
				'@'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (!array[0].Equals("optool", StringComparison.OrdinalIgnoreCase))
			{
				return -3;
			}
			int num;
			this.m_applicationService.ScheduleShutdown((request == null || !int.TryParse(request.GetAttribute("timeout_sec"), out num)) ? TimeSpan.Zero : TimeSpan.FromSeconds((double)num));
			return 0;
		}

		// Token: 0x04000215 RID: 533
		private readonly IApplicationService m_applicationService;
	}
}
