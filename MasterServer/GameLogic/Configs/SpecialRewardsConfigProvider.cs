using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x0200028E RID: 654
	[Service]
	[Singleton]
	internal class SpecialRewardsConfigProvider : IConfigProvider
	{
		// Token: 0x06000E25 RID: 3621 RVA: 0x00038CE0 File Offset: 0x000370E0
		public int GetHash()
		{
			return Resources.SpecialRewardSettings.GetHashCode();
		}

		// Token: 0x06000E26 RID: 3622 RVA: 0x00038CEC File Offset: 0x000370EC
		public IEnumerable<XmlElement> GetConfig(XmlDocument doc)
		{
			List<XmlElement> list = new List<XmlElement>();
			XmlNode xmlNode = Resources.SpecialRewardSettings.ToXmlNode(doc);
			list.Add((XmlElement)xmlNode);
			return list;
		}
	}
}
