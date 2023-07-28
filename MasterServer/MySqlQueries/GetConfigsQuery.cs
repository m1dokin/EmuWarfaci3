using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.Configs;

namespace MasterServer.MySqlQueries
{
	// Token: 0x02000287 RID: 647
	[QueryAttributes(TagName = "get_configs")]
	internal class GetConfigsQuery : PagedQueryStatic
	{
		// Token: 0x06000E0F RID: 3599 RVA: 0x00038AC3 File Offset: 0x00036EC3
		public GetConfigsQuery(IConfigsService configsService)
		{
			this.m_configsService = configsService;
		}

		// Token: 0x06000E10 RID: 3600 RVA: 0x00038AD2 File Offset: 0x00036ED2
		protected override int GetMaxBatch()
		{
			return 250;
		}

		// Token: 0x06000E11 RID: 3601 RVA: 0x00038ADC File Offset: 0x00036EDC
		protected override string GetDataHash()
		{
			return this.m_configsService.GetHash().ToString();
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x00038B02 File Offset: 0x00036F02
		protected override List<XmlElement> GetData(XmlDocument doc)
		{
			return this.m_configsService.GetConfigs(doc);
		}

		// Token: 0x04000674 RID: 1652
		private const int CONFIGS_MAX_BATCH = 250;

		// Token: 0x04000675 RID: 1653
		private readonly IConfigsService m_configsService;
	}
}
