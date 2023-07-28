using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DedicatedPoolServer.Model;
using HK2Net;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006C1 RID: 1729
	[Contract]
	public interface IServerInfo
	{
		// Token: 0x1400009B RID: 155
		// (add) Token: 0x0600242F RID: 9263
		// (remove) Token: 0x06002430 RID: 9264
		event EventHandler<ServerEntityEventArgs> ServerEntityEvent;

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06002431 RID: 9265
		bool IsGlobalLbsEnabled { get; }

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06002432 RID: 9266
		bool IsReconnectByNodeEnabled { get; }

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06002433 RID: 9267
		int SearchByNodePerformanceRange { get; }

		// Token: 0x06002434 RID: 9268
		bool IsLocalServer(string serverId);

		// Token: 0x06002435 RID: 9269
		bool IsTestDediclient(string serverId);

		// Token: 0x06002436 RID: 9270
		bool GetServer(string serverId, bool connectedOnly, out ServerEntity ent);

		// Token: 0x06002437 RID: 9271
		bool GetTestServer(out ServerEntity ent);

		// Token: 0x06002438 RID: 9272
		List<ServerEntity> GetBoundServers(bool connectedOnly);

		// Token: 0x06002439 RID: 9273
		Task<DedicatedInfo> RequestServerByServerId(DedicatedMode mode, string buildTypeInRoom, string serverId);

		// Token: 0x0600243A RID: 9274
		Task<DedicatedInfo> RequestServer(DedicatedMode mode, string buildTypeInRoom, string regionId);

		// Token: 0x0600243B RID: 9275
		bool ReleaseServer(string serverId, string sessionId);

		// Token: 0x0600243C RID: 9276
		bool ReleaseServer(string serverId, bool isForcedRelease = false);

		// Token: 0x0600243D RID: 9277
		bool ReleaseServer(ServerEntity server);

		// Token: 0x0600243E RID: 9278
		void OnServerBound(bool bindOk, string serverId);

		// Token: 0x0600243F RID: 9279
		void OnServerInfo(ServerEntity srv);

		// Token: 0x06002440 RID: 9280
		void DumpServers();

		// Token: 0x06002441 RID: 9281
		void DumpServer(string serverId);

		// Token: 0x06002442 RID: 9282
		void DebugStealServer(string serverId);
	}
}
