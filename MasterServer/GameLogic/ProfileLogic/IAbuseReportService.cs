using System;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200053F RID: 1343
	[Contract]
	internal interface IAbuseReportService
	{
		// Token: 0x06001D10 RID: 7440
		Task<EAbuseReportingResult> ProcessReport(UserInfo.User from, ulong to_uid, ulong to_pid, string to_nickname, string type, string comment);

		// Token: 0x06001D11 RID: 7441
		void StoreReportToDB(ulong from_pid, ulong to_pid, string type, EAbuseReportSource source, params string[] param);

		// Token: 0x06001D12 RID: 7442
		TimeSpan GetTotalOnlineTime(ulong profileId);

		// Token: 0x06001D13 RID: 7443
		TimeSpan GetTotalOnlineTime(ulong profileId, bool forceCacheReset);

		// Token: 0x06001D14 RID: 7444
		bool CleanupHistory(TimeSpan abuseReportLifetime, TimeSpan dbTimeout, int batchSize);

		// Token: 0x06001D15 RID: 7445
		void DebugCleanupHistory();
	}
}
