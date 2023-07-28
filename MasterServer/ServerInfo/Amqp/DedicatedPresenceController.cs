using System;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Users;
using Network.Interfaces;
using Util.Common;

namespace MasterServer.ServerInfo.Amqp
{
	// Token: 0x020006B3 RID: 1715
	[Service]
	[Singleton]
	internal class DedicatedPresenceController : IDedicatedPresenceController, IRemoteService<DedicatedPresenceRequest>, IDisposable
	{
		// Token: 0x06002403 RID: 9219 RVA: 0x00096DE6 File Offset: 0x000951E6
		public DedicatedPresenceController(IServerRepository serverRepository, IServerInfo serverInfo)
		{
			this.m_serverRepository = serverRepository;
			this.m_serverInfo = serverInfo;
		}

		// Token: 0x06002404 RID: 9220 RVA: 0x00096DFC File Offset: 0x000951FC
		public Task MakeRequest(DedicatedPresenceRequest request)
		{
			if (!request.IsOnline)
			{
				this.m_serverRepository.RemoveServer(Utils.MakeJid("dedicated", request.DedicatedId).ToString());
			}
			else
			{
				ServerEntity serverEntity = new ServerEntity
				{
					ServerID = request.DedicatedId,
					OnlineID = request.LdsJid,
					BuildType = request.BuildType,
					Hostname = request.BoxId,
					Port = request.Port,
					Node = request.Node,
					Mode = request.Mode,
					SessionID = request.SessionId,
					Mission = request.MissionKey,
					Status = request.Status,
					MasterServerId = Resources.XmppResource,
					PerformanceIndex = (float)request.PerformanceIndex
				};
				this.m_serverRepository.AddServer(serverEntity.OnlineID, request.DedicatedId);
				this.m_serverInfo.OnServerInfo(serverEntity);
			}
			return TaskHelpers.Completed();
		}

		// Token: 0x06002405 RID: 9221 RVA: 0x00096EF9 File Offset: 0x000952F9
		public void Dispose()
		{
		}

		// Token: 0x04001211 RID: 4625
		private const string DedicatedClientId = "dedicated";

		// Token: 0x04001212 RID: 4626
		private readonly IServerRepository m_serverRepository;

		// Token: 0x04001213 RID: 4627
		private readonly IServerInfo m_serverInfo;
	}
}
