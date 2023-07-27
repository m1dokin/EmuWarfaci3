using System;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000002 RID: 2
	internal class AbuseReportSystem : IAbuseReportSystem
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public AbuseReportSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002080 File Offset: 0x00000280
		public DALResultVoid AddAbuseReport(ulong from, ulong to, string type)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL AddAbuseReport(?from_pid, ?to_pid, ?type)", new object[]
			{
				"?from_pid",
				from,
				"?to_pid",
				to,
				"?type",
				type
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020E4 File Offset: 0x000002E4
		public DALResultMulti<SAbuseReport> GetAbuseReports(ulong profile_id)
		{
			CacheProxy.Options<SAbuseReport> options = new CacheProxy.Options<SAbuseReport>
			{
				db_serializer = this.m_abuseReportSerializer
			};
			options.query("CALL GetAbuseReports(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.GetStream<SAbuseReport>(options);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002138 File Offset: 0x00000338
		public DALResult<bool> ClearAbuseReportHistory(TimeSpan abuseReportLifetime, TimeSpan dbTimeout, int batchSize)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				db_cmd_timeout = (int)dbTimeout.TotalSeconds
			};
			setOptions.query("SELECT ClearAbuseReportHistory(?report_lifetime_min, ?batch_size)", new object[]
			{
				"?report_lifetime_min",
				(int)abuseReportLifetime.TotalMinutes,
				"?batch_size",
				batchSize
			});
			bool val = int.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString()) > 0;
			return new DALResult<bool>(val, setOptions.stats);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000021C0 File Offset: 0x000003C0
		public DALResultVoid AddReportToHistory(ulong from_pid, ulong to_pid, string type, uint source, string message)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL AddAbuseReportToHistory(?from_pid, ?to_pid, ?type, ?source, ?message)", new object[]
			{
				"?from_pid",
				from_pid,
				"?to_pid",
				to_pid,
				"?type",
				type,
				"?source",
				source,
				"?message",
				message
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002244 File Offset: 0x00000444
		public DALResultMulti<SAbuseHistory> GetAbuseHistory()
		{
			CacheProxy.Options<SAbuseHistory> options = new CacheProxy.Options<SAbuseHistory>
			{
				db_serializer = this.m_abuseHistorySerializer
			};
			options.query("CALL GetAbuseReports()", new object[0]);
			return this.m_dal.CacheProxy.GetStream<SAbuseHistory>(options);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002288 File Offset: 0x00000488
		public DALResultMulti<SAbuseHistory> GetAbuseHistoryToUser(ulong to_pid)
		{
			CacheProxy.Options<SAbuseHistory> options = new CacheProxy.Options<SAbuseHistory>
			{
				db_serializer = this.m_abuseHistorySerializer
			};
			options.query("CALL GetAbuseReportHistoryToUser(?to_pid)", new object[]
			{
				"?to_pid",
				to_pid
			});
			return this.m_dal.CacheProxy.GetStream<SAbuseHistory>(options);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000022DC File Offset: 0x000004DC
		public DALResultMulti<SAbuseHistory> GetAbuseHistoryFromUser(ulong from_pid)
		{
			CacheProxy.Options<SAbuseHistory> options = new CacheProxy.Options<SAbuseHistory>
			{
				db_serializer = this.m_abuseHistorySerializer
			};
			options.query("CALL GetAbuseReportHistoryFromUser(?from_pid)", new object[]
			{
				"?from_pid",
				from_pid
			});
			return this.m_dal.CacheProxy.GetStream<SAbuseHistory>(options);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002330 File Offset: 0x00000530
		public DALResultMulti<SAbuseHistory> GetAbuseHistoryByDate(ulong start_date, ulong end_date, sbyte abuse_source, uint count)
		{
			CacheProxy.Options<SAbuseHistory> options = new CacheProxy.Options<SAbuseHistory>
			{
				db_serializer = this.m_abuseHistorySerializer
			};
			options.query("CALL GetAbuseReportHistoryByDate(?start_date, ?end_date, ?abuse_source, ?records_count)", new object[]
			{
				"?start_date",
				start_date,
				"?end_date",
				end_date,
				"?abuse_source",
				abuse_source,
				"?records_count",
				count
			});
			return this.m_dal.CacheProxy.GetStream<SAbuseHistory>(options);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000023B8 File Offset: 0x000005B8
		public DALResult<uint> GetAbusesCount(ulong start_date, ulong end_date)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT GetAbusesCount(?start_date, ?end_date)", new object[]
			{
				"?start_date",
				start_date,
				"?end_date",
				end_date
			});
			uint val = Convert.ToUInt32(this.m_dal.CacheProxy.SetScalar(setOptions));
			return new DALResult<uint>(val, setOptions.stats);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002424 File Offset: 0x00000624
		public DALResultMulti<SAbuseTopReport> GetTopAbusers(uint count)
		{
			CacheProxy.Options<SAbuseTopReport> options = new CacheProxy.Options<SAbuseTopReport>
			{
				db_serializer = this.m_abuseTopSerializer
			};
			options.query("CALL GetTopAbusers(?count)", new object[]
			{
				"?count",
				count
			});
			return this.m_dal.CacheProxy.GetStream<SAbuseTopReport>(options);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002478 File Offset: 0x00000678
		public DALResultVoid GenerateAbuseHistory(uint count, uint dayInterval)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL GenerateAbuseHistory(?count, ?dayInterval)", new object[]
			{
				"?count",
				count,
				"?dayInterval",
				dayInterval
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000001 RID: 1
		private DAL m_dal;

		// Token: 0x04000002 RID: 2
		private AbuseReportSerializer m_abuseReportSerializer = new AbuseReportSerializer();

		// Token: 0x04000003 RID: 3
		private AbuseHistorySerializer m_abuseHistorySerializer = new AbuseHistorySerializer();

		// Token: 0x04000004 RID: 4
		private AbuseTopReportSerializer m_abuseTopSerializer = new AbuseTopReportSerializer();
	}
}
