using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Configuration;

namespace MasterServer.GameLogic.MissionSystem.MissionGeneration
{
	// Token: 0x020003A6 RID: 934
	[Service]
	[Singleton]
	public class MissionGenerationConfigReader : IMissionGenerationConfigReader
	{
		// Token: 0x060014B4 RID: 5300 RVA: 0x00055018 File Offset: 0x00053418
		public MissionGenerationConfigReader(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x00055027 File Offset: 0x00053427
		public Config Read()
		{
			return this.m_configurationService.GetConfig(MsConfigInfo.MissionGenerationConfiguration);
		}

		// Token: 0x040009C1 RID: 2497
		private readonly IConfigurationService m_configurationService;
	}
}
