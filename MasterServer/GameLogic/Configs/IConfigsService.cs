using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x02000289 RID: 649
	[Contract]
	public interface IConfigsService
	{
		// Token: 0x06000E15 RID: 3605
		int GetHash();

		// Token: 0x06000E16 RID: 3606
		List<XmlElement> GetConfigs(XmlDocument doc);
	}
}
