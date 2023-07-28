using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000766 RID: 1894
	[Contract]
	public interface IGameRoomServer
	{
		// Token: 0x06002748 RID: 10056
		bool RequestServer(ulong roomId, string session_id, string serverId);

		// Token: 0x06002749 RID: 10057
		void ReleaseServer(ulong roomId, string serverId);

		// Token: 0x0600274A RID: 10058
		void FailedServer(string serverId);

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x0600274B RID: 10059
		// (set) Token: 0x0600274C RID: 10060
		bool CleanupStaleServers { get; set; }

		// Token: 0x0600274D RID: 10061
		void OnMissionLoad(string server_id, MissionLoadResult result, string session_id);
	}
}
