using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F7 RID: 503
	internal interface ICommonSystemClient
	{
		// Token: 0x06000A01 RID: 2561
		void UpdateServer(string msId, SServerEntity entity);

		// Token: 0x06000A02 RID: 2562
		void DebugExecuteNoQuery(string sql);

		// Token: 0x06000A03 RID: 2563
		void DebugDropProcedure(string procedureName);

		// Token: 0x06000A04 RID: 2564
		IEnumerable<SServerEntity> GetFreeServers(string msId);

		// Token: 0x06000A05 RID: 2565
		IEnumerable<SVersionStamp> GetDataVersionStamps();

		// Token: 0x06000A06 RID: 2566
		void SetDataVersionStamp(string group, string hash);

		// Token: 0x06000A07 RID: 2567
		string GetTotalDataVersionStamp();

		// Token: 0x06000A08 RID: 2568
		bool TryLockUpdaterPermission(string onlineId);

		// Token: 0x06000A09 RID: 2569
		void UnlockUpdaterPermission(string onlineId);

		// Token: 0x06000A0A RID: 2570
		void ResetUpdaterPermission();
	}
}
