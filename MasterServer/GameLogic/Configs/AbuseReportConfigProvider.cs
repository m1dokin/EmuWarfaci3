using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x02000285 RID: 645
	[Service]
	[Singleton]
	internal class AbuseReportConfigProvider : IConfigProvider
	{
		// Token: 0x06000E09 RID: 3593 RVA: 0x000389A0 File Offset: 0x00036DA0
		public int GetHash()
		{
			return Resources.AbuseManagerConfig.GetHashCode();
		}

		// Token: 0x06000E0A RID: 3594 RVA: 0x000389AC File Offset: 0x00036DAC
		public IEnumerable<XmlElement> GetConfig(XmlDocument doc)
		{
			List<XmlElement> list = new List<XmlElement>();
			XmlNode xmlNode = Resources.AbuseManagerConfig.ToXmlNode(doc);
			list.Add((XmlElement)xmlNode);
			return list;
		}
	}
}
