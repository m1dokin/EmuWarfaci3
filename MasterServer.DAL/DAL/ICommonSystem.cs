using System;

namespace MasterServer.DAL
{
	// Token: 0x0200009B RID: 155
	public interface ICommonSystem
	{
		// Token: 0x060001DC RID: 476
		DALResultVoid UpdateServer(string msId, SServerEntity entity);

		// Token: 0x060001DD RID: 477
		DALResultMulti<SServerEntity> GetFreeServers(string msId);

		// Token: 0x060001DE RID: 478
		DALResultMulti<SVersionStamp> GetDataVersionStamps();

		// Token: 0x060001DF RID: 479
		DALResultVoid SetDataVersionStamp(string group, string hash);

		// Token: 0x060001E0 RID: 480
		DALResult<string> GetTotalDataVersionStamp();

		// Token: 0x060001E1 RID: 481
		DALResult<bool> TryLockUpdaterPermission(string onlineId);

		// Token: 0x060001E2 RID: 482
		DALResultVoid UnlockUpdaterPermission(string onlineId);

		// Token: 0x060001E3 RID: 483
		DALResultVoid ResetUpdaterPermission();

		// Token: 0x060001E4 RID: 484
		DALResultVoid DebugExecuteNoQuery(string sql);

		// Token: 0x060001E5 RID: 485
		DALResultVoid DebugDropProcedure(string procedureName);
	}
}
