using System;
using System.Xml;
using HK2Net;

namespace MasterServer.Core
{
	// Token: 0x02000155 RID: 341
	[Contract]
	public interface IOnlineVariables
	{
		// Token: 0x060005F9 RID: 1529
		string Get(string key, OnlineVariableDestination dest);

		// Token: 0x060005FA RID: 1530
		void WriteVariables(XmlElement rootNode, OnlineVariableDestination destination);

		// Token: 0x060005FB RID: 1531
		void Dump();
	}
}
