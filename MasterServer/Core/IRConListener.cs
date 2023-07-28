using System;
using CookComputing.XmlRpc;

namespace MasterServer.Core
{
	// Token: 0x0200014B RID: 331
	internal interface IRConListener
	{
		// Token: 0x060005C2 RID: 1474
		[XmlRpcMethod("execute")]
		string ExecConsoleCmd(string cmd);
	}
}
