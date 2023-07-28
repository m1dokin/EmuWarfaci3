using System;
using DedicatedPoolServer.Model;
using MasterServer.Core;

namespace MasterServer.Telemetry
{
	// Token: 0x020007C5 RID: 1989
	internal class ServerTracker
	{
		// Token: 0x060028C3 RID: 10435 RVA: 0x000B0A64 File Offset: 0x000AEE64
		public ServerTracker(ITelemetryService service, string onlineId)
		{
			this.Service = service;
			this.OnlineID = onlineId;
			this.m_status = EGameServerStatus.None;
			this.m_lastTick = DateTime.Now.Ticks / 10000L;
		}

		// Token: 0x060028C4 RID: 10436 RVA: 0x000B0AA6 File Offset: 0x000AEEA6
		private static ServerTracker.ESrvState MapState(EGameServerStatus st)
		{
			if (st == EGameServerStatus.None || st == EGameServerStatus.Failed)
			{
				return ServerTracker.ESrvState.Offline;
			}
			if (st == EGameServerStatus.Playing || st == EGameServerStatus.Ready || st == EGameServerStatus.PostGame || st == EGameServerStatus.Waiting)
			{
				return ServerTracker.ESrvState.Busy;
			}
			return ServerTracker.ESrvState.Idle;
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x060028C5 RID: 10437 RVA: 0x000B0AD6 File Offset: 0x000AEED6
		// (set) Token: 0x060028C6 RID: 10438 RVA: 0x000B0AE0 File Offset: 0x000AEEE0
		public EGameServerStatus Status
		{
			get
			{
				return this.m_status;
			}
			set
			{
				ServerTracker.ESrvState esrvState = ServerTracker.MapState(this.m_status);
				ServerTracker.ESrvState esrvState2 = ServerTracker.MapState(value);
				this.m_status = value;
				if (esrvState == esrvState2)
				{
					return;
				}
				DateTime now = DateTime.Now;
				string text = now.ToString("yyyy-MM-dd");
				long num = now.Ticks / 10000L;
				if (this.Service.CheckMode(TelemetryMode.Realm))
				{
					string text2 = null;
					if (esrvState == ServerTracker.ESrvState.Busy)
					{
						text2 = "srv_play_time";
					}
					else if (esrvState == ServerTracker.ESrvState.Idle)
					{
						text2 = "srv_idle_time";
					}
					if (text2 != null)
					{
						this.Service.AddMeasure((num - this.m_lastTick) / 100L, new object[]
						{
							"stat",
							text2,
							"server",
							Resources.ServerName,
							"host",
							Resources.Hostname,
							"date",
							text
						});
					}
				}
				this.m_lastTick = num;
			}
		}

		// Token: 0x040015A5 RID: 5541
		public readonly ITelemetryService Service;

		// Token: 0x040015A6 RID: 5542
		public readonly string OnlineID;

		// Token: 0x040015A7 RID: 5543
		private long m_lastTick;

		// Token: 0x040015A8 RID: 5544
		private EGameServerStatus m_status;

		// Token: 0x020007C6 RID: 1990
		private enum ESrvState
		{
			// Token: 0x040015AA RID: 5546
			Offline,
			// Token: 0x040015AB RID: 5547
			Idle,
			// Token: 0x040015AC RID: 5548
			Busy
		}
	}
}
