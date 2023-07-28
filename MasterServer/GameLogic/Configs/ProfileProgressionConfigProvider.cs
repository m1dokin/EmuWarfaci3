using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x0200028D RID: 653
	[Service]
	[Singleton]
	internal class ProfileProgressionConfigProvider : IConfigProvider
	{
		// Token: 0x06000E22 RID: 3618 RVA: 0x00038CA0 File Offset: 0x000370A0
		public IEnumerable<XmlElement> GetConfig(XmlDocument doc)
		{
			List<XmlElement> list = new List<XmlElement>();
			XmlNode xmlNode = Resources.ProfileProgressionConfig.ToXmlNode(doc);
			list.Add((XmlElement)xmlNode);
			return list;
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x00038CCC File Offset: 0x000370CC
		public int GetHash()
		{
			return Resources.ProfileProgressionConfig.GetHashCode();
		}
	}
}
