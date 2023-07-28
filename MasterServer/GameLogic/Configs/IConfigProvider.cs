using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x02000288 RID: 648
	[Contract]
	public interface IConfigProvider
	{
		// Token: 0x06000E13 RID: 3603
		int GetHash();

		// Token: 0x06000E14 RID: 3604
		IEnumerable<XmlElement> GetConfig(XmlDocument doc);
	}
}
