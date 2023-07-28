using System;
using System.Collections.Generic;
using DedicatedPoolServer.Model;
using MasterServer.Core;
using MasterServer.ServerInfo;
using MasterServer.Users;

namespace MasterServer.Telemetry
{
	// Token: 0x020007C7 RID: 1991
	internal class ServerTrackerObserver
	{
		// Token: 0x060028C7 RID: 10439 RVA: 0x000B0BD0 File Offset: 0x000AEFD0
		public ServerTrackerObserver(ITelemetryService srv, IServerRepository serverRepository)
		{
			this.m_service = srv;
			this.m_serverTrackers = new Dictionary<string, ServerTracker>();
			serverRepository.OnServerBind += this.OnServerPresence;
			IServerInfo service = ServicesManager.GetService<IServerInfo>();
			service.ServerEntityEvent += this.OnServerInfo;
		}

		// Token: 0x060028C8 RID: 10440 RVA: 0x000B0C20 File Offset: 0x000AF020
		private void OnServerPresence(bool bound, string sOnlineId, string server_id)
		{
			using (new FunctionProfiler(Profiler.EModule.TELEMETRY, "ServerTrackerObserver.OnServerPresence"))
			{
				object serverTrackers = this.m_serverTrackers;
				lock (serverTrackers)
				{
					if (bound && !this.m_serverTrackers.ContainsKey(sOnlineId))
					{
						ServerTracker value = new ServerTracker(this.m_service, sOnlineId);
						this.m_serverTrackers[sOnlineId] = value;
					}
					else if (!bound && this.m_serverTrackers.ContainsKey(sOnlineId))
					{
						this.m_serverTrackers[sOnlineId].Status = EGameServerStatus.None;
						this.m_serverTrackers.Remove(sOnlineId);
					}
				}
			}
		}

		// Token: 0x060028C9 RID: 10441 RVA: 0x000B0CF4 File Offset: 0x000AF0F4
		private void OnServerInfo(object sender, ServerEntityEventArgs e)
		{
			using (new FunctionProfiler(Profiler.EModule.TELEMETRY, "ServerTrackerObserver.OnServerInfo"))
			{
				if (e.State == ServerEntityState.SERVER_CHANGED && e.State == ServerEntityState.SERVER_BOUND)
				{
					object serverTrackers = this.m_serverTrackers;
					lock (serverTrackers)
					{
						ServerTracker serverTracker;
						if (!this.m_serverTrackers.TryGetValue(e.Entity.OnlineID, out serverTracker))
						{
							Log.Warning<string>("Server {0} is not present", e.ServerId);
						}
						else
						{
							serverTracker.Status = e.Entity.Status;
						}
					}
				}
			}
		}

		// Token: 0x040015AD RID: 5549
		private ITelemetryService m_service;

		// Token: 0x040015AE RID: 5550
		private Dictionary<string, ServerTracker> m_serverTrackers;
	}
}
