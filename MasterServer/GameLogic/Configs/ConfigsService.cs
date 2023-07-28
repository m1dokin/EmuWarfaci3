using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using HK2Net;
using MasterServer.Core.Services;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x0200028B RID: 651
	[Service]
	[Singleton]
	internal class ConfigsService : ServiceModule, IConfigsService, IDebugConfigService
	{
		// Token: 0x06000E18 RID: 3608 RVA: 0x00038B10 File Offset: 0x00036F10
		public ConfigsService(IEnumerable<IConfigProvider> configProviders)
		{
			this.m_configs = configProviders;
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x00038B1F File Offset: 0x00036F1F
		public int GetHash()
		{
			return this.m_configs.Aggregate(0, (int current, IConfigProvider configProvider) => current ^ configProvider.GetHash());
		}

		// Token: 0x06000E1A RID: 3610 RVA: 0x00038B4C File Offset: 0x00036F4C
		public List<XmlElement> GetConfigs(XmlDocument doc)
		{
			return (from configProvider in this.m_configs
			select configProvider.GetConfig(doc) into config
			where config != null
			select config).SelectMany((IEnumerable<XmlElement> configs) => configs).ToList<XmlElement>();
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x00038BC6 File Offset: 0x00036FC6
		public IEnumerable<IConfigProvider> GetConfigProviders()
		{
			return this.m_configs;
		}

		// Token: 0x04000676 RID: 1654
		private readonly IEnumerable<IConfigProvider> m_configs;
	}
}
