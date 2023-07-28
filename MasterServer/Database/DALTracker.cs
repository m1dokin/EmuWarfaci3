using System;
using MasterServer.Core.Diagnostics.Profiler;
using MasterServer.DAL;

namespace MasterServer.Database
{
	// Token: 0x020001C0 RID: 448
	internal class DALTracker : IDisposable
	{
		// Token: 0x0600085C RID: 2140 RVA: 0x00020AD1 File Offset: 0x0001EED1
		public DALTracker(BaseDALService dal, string proc)
		{
			this.dal = dal;
			this.stats = new DALProxyStats();
			this.stats.Procedure = proc;
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x00020AF7 File Offset: 0x0001EEF7
		public DALTracker.CTimeCache TimeCache(CacheOp op)
		{
			return new DALTracker.CTimeCache(this, op);
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x00020B00 File Offset: 0x0001EF00
		public DALTracker.CTimeDAL TimeDAL()
		{
			return new DALTracker.CTimeDAL(this);
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x00020B08 File Offset: 0x0001EF08
		public void Dispose()
		{
			this.dal.SendDALStats(this.stats);
		}

		// Token: 0x040004F1 RID: 1265
		private BaseDALService dal;

		// Token: 0x040004F2 RID: 1266
		private DALProxyStats stats;

		// Token: 0x020001C1 RID: 449
		public class CTimeCache : IDisposable
		{
			// Token: 0x06000860 RID: 2144 RVA: 0x00020B1B File Offset: 0x0001EF1B
			public CTimeCache(DALTracker tr, CacheOp op)
			{
				this.tr = tr;
				this.op = op;
				this.watch = new TimeExecution();
			}

			// Token: 0x06000861 RID: 2145 RVA: 0x00020B3C File Offset: 0x0001EF3C
			public void CacheHit(CacheLevel lvl, bool hit)
			{
				if (lvl == CacheLevel.L1)
				{
					if (hit)
					{
						this.tr.stats.L1CacheHits++;
					}
					else
					{
						this.tr.stats.L1CacheMisses++;
					}
				}
				else if (hit)
				{
					this.tr.stats.L2CacheHits++;
				}
				else
				{
					this.tr.stats.L2CacheMisses++;
				}
			}

			// Token: 0x06000862 RID: 2146 RVA: 0x00020BCA File Offset: 0x0001EFCA
			public void CacheClear(CacheLevel lvl)
			{
				if (lvl == CacheLevel.L1)
				{
					this.tr.stats.L1CacheClear++;
				}
				else
				{
					this.tr.stats.L2CacheClear++;
				}
			}

			// Token: 0x06000863 RID: 2147 RVA: 0x00020C07 File Offset: 0x0001F007
			public void Dispose()
			{
				this.tr.stats.CacheTime += this.watch.Stop();
			}

			// Token: 0x040004F3 RID: 1267
			private DALTracker tr;

			// Token: 0x040004F4 RID: 1268
			private CacheOp op;

			// Token: 0x040004F5 RID: 1269
			private TimeExecution watch;
		}

		// Token: 0x020001C2 RID: 450
		public class CTimeDAL : IDisposable
		{
			// Token: 0x06000864 RID: 2148 RVA: 0x00020C2F File Offset: 0x0001F02F
			public CTimeDAL(DALTracker tr)
			{
				this.tr = tr;
				this.watch = new TimeExecution();
			}

			// Token: 0x06000865 RID: 2149 RVA: 0x00020C4C File Offset: 0x0001F04C
			public void AddStats(DALStats stats)
			{
				this.tr.stats.DBQueries += stats.DBQueries;
				this.tr.stats.ConnectionAllocTime += stats.ConnectionAllocTime;
				this.tr.stats.DBTime += stats.DBTime;
				this.tr.stats.DBDeadlocks += stats.DBDeadlocks;
				this.tr.stats.DBDeadlocksTotal += stats.DBDeadlocksTotal;
			}

			// Token: 0x06000866 RID: 2150 RVA: 0x00020CF2 File Offset: 0x0001F0F2
			public void Dispose()
			{
				this.tr.stats.DALTime += this.watch.Stop();
			}

			// Token: 0x040004F6 RID: 1270
			private DALTracker tr;

			// Token: 0x040004F7 RID: 1271
			private TimeExecution watch;
		}
	}
}
