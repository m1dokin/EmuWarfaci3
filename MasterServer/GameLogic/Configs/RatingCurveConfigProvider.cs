using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x0200005B RID: 91
	[Service]
	[Singleton]
	internal class RatingCurveConfigProvider : IConfigProvider
	{
		// Token: 0x06000165 RID: 357 RVA: 0x00009EEC File Offset: 0x000082EC
		public int GetHash()
		{
			return Resources.RatingCurveConfig.GetHashCode();
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00009EF8 File Offset: 0x000082F8
		public IEnumerable<XmlElement> GetConfig(XmlDocument doc)
		{
			List<XmlElement> list = new List<XmlElement>();
			XmlNode xmlNode = Resources.RatingCurveConfig.ToXmlNode(doc);
			list.Add((XmlElement)xmlNode);
			return list;
		}
	}
}
