using System;
using System.Collections.Generic;
using System.Threading;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000191 RID: 401
	internal class QueryDelay
	{
		// Token: 0x0600075D RID: 1885 RVA: 0x0001C25C File Offset: 0x0001A65C
		public void Delay(string tag)
		{
			int millisecondsTimeout = 0;
			if (this.m_delays.TryGetValue(tag, out millisecondsTimeout))
			{
				Thread.Sleep(millisecondsTimeout);
			}
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x0001C284 File Offset: 0x0001A684
		public void Dump()
		{
			foreach (KeyValuePair<string, int> keyValuePair in this.m_delays)
			{
				Log.Info<string, int>("Query {0} delay is {1} msec", keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x0001C2F4 File Offset: 0x0001A6F4
		public void SetDelay(string query_tag, int delay)
		{
			if (delay > 0)
			{
				this.m_delays[query_tag] = delay;
			}
			else
			{
				this.m_delays.Remove(query_tag);
			}
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x0001C31C File Offset: 0x0001A71C
		public void ResetAllDelays()
		{
			this.m_delays.Clear();
		}

		// Token: 0x04000474 RID: 1140
		private Dictionary<string, int> m_delays = new Dictionary<string, int>();
	}
}
