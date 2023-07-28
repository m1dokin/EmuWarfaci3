using System;
using HK2Net;
using MasterServer.Database.RemoteClients;

namespace MasterServer.Database
{
	// Token: 0x02000046 RID: 70
	[Contract]
	internal interface ITelemetryDALService : IBaseDALService
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600011D RID: 285
		ITelemetrySystemClient TelemetrySystem { get; }
	}
}
