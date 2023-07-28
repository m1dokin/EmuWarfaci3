using System;

namespace MasterServer.DAL
{
	// Token: 0x02000004 RID: 4
	public interface IAbuseReportSystem
	{
		// Token: 0x06000004 RID: 4
		DALResultVoid AddAbuseReport(ulong from_pid, ulong to_pid, string type);

		// Token: 0x06000005 RID: 5
		DALResultVoid AddReportToHistory(ulong from_pid, ulong to_pid, string type, uint source, string message);

		// Token: 0x06000006 RID: 6
		DALResultMulti<SAbuseReport> GetAbuseReports(ulong profile_id);

		// Token: 0x06000007 RID: 7
		DALResult<bool> ClearAbuseReportHistory(TimeSpan abuseReportLifetime, TimeSpan dbTimeout, int batchSize);

		// Token: 0x06000008 RID: 8
		DALResultMulti<SAbuseHistory> GetAbuseHistory();

		// Token: 0x06000009 RID: 9
		DALResultMulti<SAbuseHistory> GetAbuseHistoryToUser(ulong to_pid);

		// Token: 0x0600000A RID: 10
		DALResultMulti<SAbuseHistory> GetAbuseHistoryFromUser(ulong from_pid);

		// Token: 0x0600000B RID: 11
		DALResultMulti<SAbuseHistory> GetAbuseHistoryByDate(ulong start_date, ulong end_date, sbyte abuse_source, uint count);

		// Token: 0x0600000C RID: 12
		DALResult<uint> GetAbusesCount(ulong start_date, ulong end_date);

		// Token: 0x0600000D RID: 13
		DALResultMulti<SAbuseTopReport> GetTopAbusers(uint count);

		// Token: 0x0600000E RID: 14
		DALResultVoid GenerateAbuseHistory(uint count, uint dayInterval);
	}
}
