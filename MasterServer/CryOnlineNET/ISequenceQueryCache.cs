using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001B4 RID: 436
	[Contract]
	internal interface ISequenceQueryCache
	{
		// Token: 0x06000821 RID: 2081
		int SaveData(List<XmlElement> data, string token, CacheType type);

		// Token: 0x06000822 RID: 2082
		int GetData(int tokenId, string token, out List<XmlElement> data);

		// Token: 0x06000823 RID: 2083
		void FreeData(int tokenId, string token);
	}
}
