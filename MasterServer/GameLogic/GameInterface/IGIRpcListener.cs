using System;
using CookComputing.XmlRpc;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002EC RID: 748
	internal interface IGIRpcListener
	{
		// Token: 0x0600116E RID: 4462
		[XmlRpcMethod("execute")]
		string Execute(string cmd);
	}
}
