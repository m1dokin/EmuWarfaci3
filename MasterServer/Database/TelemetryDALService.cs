using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.Database.RemoteClients;

namespace MasterServer.Database
{
	// Token: 0x02000047 RID: 71
	[Service]
	[Singleton]
	internal class TelemetryDALService : BaseDALService, ITelemetryDALService, IBaseDALService
	{
		// Token: 0x0600011E RID: 286 RVA: 0x00009830 File Offset: 0x00007C30
		public TelemetryDALService(IDAL dal, IDALCookieProvider cookieProvider) : base(dal, cookieProvider)
		{
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00009845 File Offset: 0x00007C45
		public ITelemetrySystemClient TelemetrySystem
		{
			get
			{
				return this.m_telemetrySystem;
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000984D File Offset: 0x00007C4D
		protected override void ResetClients()
		{
			this.m_telemetrySystem.Reset(base.DAL.TelemetrySystem);
		}

		// Token: 0x0400008C RID: 140
		private readonly TelemetrySystemClient m_telemetrySystem = new TelemetrySystemClient();
	}
}
