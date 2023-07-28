using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameLogic.Configs;

namespace MasterServer.Users.Regions
{
	// Token: 0x020006E4 RID: 1764
	[Service]
	[Singleton]
	internal class RegionsConfigProvider : IConfigProvider
	{
		// Token: 0x06002506 RID: 9478 RVA: 0x0009A8B6 File Offset: 0x00098CB6
		public RegionsConfigProvider(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x06002507 RID: 9479 RVA: 0x0009A8C8 File Offset: 0x00098CC8
		public IEnumerable<XmlElement> GetConfig(XmlDocument doc)
		{
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			yield return (XmlElement)config.GetSection("Regions").ToXmlNode(doc);
			yield break;
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x0009A8F4 File Offset: 0x00098CF4
		public int GetHash()
		{
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			return config.GetSection("Regions").GetHashCode();
		}

		// Token: 0x040012B2 RID: 4786
		private readonly IConfigurationService m_configurationService;
	}
}
