using System;

namespace MasterServer.Core
{
	// Token: 0x02000832 RID: 2098
	public class FunctionProfiler : IDisposable
	{
		// Token: 0x06002B6E RID: 11118 RVA: 0x000BBF8B File Offset: 0x000BA38B
		public FunctionProfiler(Profiler.EModule module, string funcName)
		{
			this.m_startTime = DateTime.UtcNow;
			this.m_funcName = funcName;
			this.m_module = module;
		}

		// Token: 0x06002B6F RID: 11119 RVA: 0x000BBFAC File Offset: 0x000BA3AC
		public void Dispose()
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow - this.m_startTime;
			Profiler.FunctionReport(this.m_module, this.m_funcName, timeSpan.TotalMilliseconds);
		}

		// Token: 0x04001729 RID: 5929
		private DateTime m_startTime;

		// Token: 0x0400172A RID: 5930
		private Profiler.EModule m_module;

		// Token: 0x0400172B RID: 5931
		private string m_funcName;
	}
}
