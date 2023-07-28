using System;
using System.Xml;
using HK2Net;

namespace MasterServer.GameLogic.AntiCheat
{
	// Token: 0x0200025F RID: 607
	[Contract]
	internal interface IAntiCheatService
	{
		// Token: 0x06000D57 RID: 3415
		void WriteConfig(XmlElement request);

		// Token: 0x06000D58 RID: 3416
		void ProcessSessionReport(XmlElement report);
	}
}
