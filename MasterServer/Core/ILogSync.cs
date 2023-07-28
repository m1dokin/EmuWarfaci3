using System;

namespace MasterServer.Core
{
	// Token: 0x02000142 RID: 322
	public interface ILogSync : IDisposable
	{
		// Token: 0x06000598 RID: 1432
		void WriteToLog(int group, string data);

		// Token: 0x06000599 RID: 1433
		void EndGroup(int group);
	}
}
