using System;

namespace MasterServer.Core
{
	// Token: 0x02000143 RID: 323
	internal class CNullSync : ILogSync, IDisposable
	{
		// Token: 0x0600059B RID: 1435 RVA: 0x00016B85 File Offset: 0x00014F85
		public void WriteToLog(int group, string data)
		{
			Log.Verbose(Log.Group.LogServer, "[LogService] {0}: {1}", new object[]
			{
				group,
				data
			});
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x00016BA6 File Offset: 0x00014FA6
		public void EndGroup(int group)
		{
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x00016BA8 File Offset: 0x00014FA8
		public void Dispose()
		{
		}
	}
}
