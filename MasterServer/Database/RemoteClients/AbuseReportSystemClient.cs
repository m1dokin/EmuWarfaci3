using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001EE RID: 494
	internal class AbuseReportSystemClient : DALCacheProxy<IDALService>, IAbuseReportSystemClient
	{
		// Token: 0x06000999 RID: 2457 RVA: 0x00023E90 File Offset: 0x00022290
		internal void Reset(IAbuseReportSystem abuseSystem)
		{
			this.m_abuseSystem = abuseSystem;
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x00023E9C File Offset: 0x0002229C
		public void AddAbuseReport(ulong from_pid, ulong to_pid, string type)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_abuseSystem.AddAbuseReport(from_pid, to_pid, type))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00023EF4 File Offset: 0x000222F4
		public void AddReportToHistory(ulong from_pid, ulong to_pid, string type, uint source, string message)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_abuseSystem.AddReportToHistory(from_pid, to_pid, type, source, message))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x00023F5C File Offset: 0x0002235C
		public IEnumerable<SAbuseReport> GetAbuseReports(ulong profile_id)
		{
			DALCacheProxy<IDALService>.Options<SAbuseReport> options = new DALCacheProxy<IDALService>.Options<SAbuseReport>
			{
				get_data_stream = (() => this.m_abuseSystem.GetAbuseReports(profile_id))
			};
			return base.GetDataStream<SAbuseReport>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x00023FA4 File Offset: 0x000223A4
		public bool ClearAbuseReportHistory(TimeSpan abuseReportLifetime, TimeSpan dbTimeout, int batchSize)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				set_func = (() => this.m_abuseSystem.ClearAbuseReportHistory(abuseReportLifetime, dbTimeout, batchSize))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x00023FFC File Offset: 0x000223FC
		public IEnumerable<SAbuseHistory> GetAbuseHistory()
		{
			DALCacheProxy<IDALService>.Options<SAbuseHistory> options = new DALCacheProxy<IDALService>.Options<SAbuseHistory>
			{
				get_data_stream = (() => this.m_abuseSystem.GetAbuseHistory())
			};
			return base.GetDataStream<SAbuseHistory>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00024030 File Offset: 0x00022430
		public IEnumerable<SAbuseHistory> GetAbuseHistoryToUser(ulong to_pid)
		{
			DALCacheProxy<IDALService>.Options<SAbuseHistory> options = new DALCacheProxy<IDALService>.Options<SAbuseHistory>
			{
				get_data_stream = (() => this.m_abuseSystem.GetAbuseHistoryToUser(to_pid))
			};
			return base.GetDataStream<SAbuseHistory>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00024078 File Offset: 0x00022478
		public IEnumerable<SAbuseHistory> GetAbuseHistoryFromUser(ulong from_pid)
		{
			DALCacheProxy<IDALService>.Options<SAbuseHistory> options = new DALCacheProxy<IDALService>.Options<SAbuseHistory>
			{
				get_data_stream = (() => this.m_abuseSystem.GetAbuseHistoryFromUser(from_pid))
			};
			return base.GetDataStream<SAbuseHistory>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x000240C0 File Offset: 0x000224C0
		public IEnumerable<SAbuseHistory> GetAbuseHistoryByDate(ulong start_date, ulong end_date, sbyte report_source, uint count)
		{
			DALCacheProxy<IDALService>.Options<SAbuseHistory> options = new DALCacheProxy<IDALService>.Options<SAbuseHistory>
			{
				get_data_stream = (() => this.m_abuseSystem.GetAbuseHistoryByDate(start_date, end_date, report_source, count))
			};
			return base.GetDataStream<SAbuseHistory>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x00024120 File Offset: 0x00022520
		public uint GetAbusesCount(ulong start_date, ulong end_date)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<uint> options = new DALCacheProxy<IDALService>.SetOptionsScalar<uint>
			{
				set_func = (() => this.m_abuseSystem.GetAbusesCount(start_date, end_date))
			};
			return base.SetAndClearScalar<uint>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00024170 File Offset: 0x00022570
		public IEnumerable<SAbuseTopReport> GetTopAbusers(uint count)
		{
			DALCacheProxy<IDALService>.Options<SAbuseTopReport> options = new DALCacheProxy<IDALService>.Options<SAbuseTopReport>
			{
				get_data_stream = (() => this.m_abuseSystem.GetTopAbusers(count))
			};
			return base.GetDataStream<SAbuseTopReport>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x000241B8 File Offset: 0x000225B8
		public void GenererateAbuseHistory(uint count, uint dayInterval)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_abuseSystem.GenerateAbuseHistory(count, dayInterval))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0400054B RID: 1355
		private IAbuseReportSystem m_abuseSystem;
	}
}
