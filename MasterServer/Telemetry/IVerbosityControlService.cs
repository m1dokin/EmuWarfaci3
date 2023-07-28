using System;
using HK2Net;

namespace MasterServer.Telemetry
{
	// Token: 0x02000736 RID: 1846
	[Contract]
	public interface IVerbosityControlService
	{
		// Token: 0x06002630 RID: 9776
		VerbosityLevel GetVerbosityLevel(string gameMode, string difficulty, string mapName);
	}
}
