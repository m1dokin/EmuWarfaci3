using System;
using MasterServer.Core.Diagnostics.Profiler;
using MasterServer.DAL;

namespace MasterServer.Database
{
	// Token: 0x02000828 RID: 2088
	internal class TimeDB : IDisposable
	{
		// Token: 0x06002B06 RID: 11014 RVA: 0x000B9DD8 File Offset: 0x000B81D8
		public TimeDB(MySqlAccessor acc) : this(acc, new TimeExecution())
		{
		}

		// Token: 0x06002B07 RID: 11015 RVA: 0x000B9DE6 File Offset: 0x000B81E6
		public TimeDB(MySqlAccessor acc, TimeExecution w)
		{
			this.stats = acc.DALStats;
			this.watch = w;
		}

		// Token: 0x06002B08 RID: 11016 RVA: 0x000B9E04 File Offset: 0x000B8204
		public void Dispose()
		{
			if (this.stats == null)
			{
				return;
			}
			this.stats.DBTime += this.watch.Stop();
			this.stats.DBQueries++;
		}

		// Token: 0x040016FC RID: 5884
		private DALStats stats;

		// Token: 0x040016FD RID: 5885
		private TimeExecution watch;
	}
}
