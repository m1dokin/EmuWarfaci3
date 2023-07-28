using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001ED RID: 493
	internal interface IAbuseReportSystemClient
	{
		// Token: 0x0600098D RID: 2445
		void AddAbuseReport(ulong from_pid, ulong to_pid, string type);

		// Token: 0x0600098E RID: 2446
		void AddReportToHistory(ulong from_pid, ulong to_pid, string type, uint source, string message);

		// Token: 0x0600098F RID: 2447
		IEnumerable<SAbuseReport> GetAbuseReports(ulong profile_id);

		// Token: 0x06000990 RID: 2448
		bool ClearAbuseReportHistory(TimeSpan abuseReportLifetime, TimeSpan dbTimeout, int batchSize);

		// Token: 0x06000991 RID: 2449
		IEnumerable<SAbuseHistory> GetAbuseHistory();

		// Token: 0x06000992 RID: 2450
		IEnumerable<SAbuseHistory> GetAbuseHistoryToUser(ulong to_pid);

		// Token: 0x06000993 RID: 2451
		IEnumerable<SAbuseHistory> GetAbuseHistoryFromUser(ulong from_pid);

		// Token: 0x06000994 RID: 2452
		IEnumerable<SAbuseHistory> GetAbuseHistoryByDate(ulong start_date, ulong end_date, sbyte report_source, uint count);

		// Token: 0x06000995 RID: 2453
		uint GetAbusesCount(ulong start_date, ulong end_date);

		// Token: 0x06000996 RID: 2454
		IEnumerable<SAbuseTopReport> GetTopAbusers(uint count);

		// Token: 0x06000997 RID: 2455
		void GenererateAbuseHistory(uint count, uint dayInterval);
	}
}
