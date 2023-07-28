using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;

namespace MasterServer.Database
{
	// Token: 0x0200003A RID: 58
	public class DALConfig
	{
		// Token: 0x060000E3 RID: 227 RVA: 0x000081CF File Offset: 0x000065CF
		public DALConfig()
		{
			this.Cookie = string.Empty;
			this.QueryRetry = 0;
			this.QueryDeadlockEmulated = false;
			this.m_queryDeadlocks = new Dictionary<string, DALConfig.DeadlockCounter>();
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x000081FB File Offset: 0x000065FB
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x00008203 File Offset: 0x00006603
		public string Cookie { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x0000820C File Offset: 0x0000660C
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x00008214 File Offset: 0x00006614
		public int QueryRetry { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x0000821D File Offset: 0x0000661D
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00008225 File Offset: 0x00006625
		public bool QueryDeadlockEmulated { get; private set; }

		// Token: 0x060000EA RID: 234 RVA: 0x00008230 File Offset: 0x00006630
		public void DebugDeadlockSet(string query, int amount, int retries)
		{
			object queryDeadlocks = this.m_queryDeadlocks;
			lock (queryDeadlocks)
			{
				if (retries > 0)
				{
					this.m_queryDeadlocks[query] = new DALConfig.DeadlockCounter
					{
						TotalRetries = retries,
						CurrentRetries = retries,
						Count = amount
					};
				}
				else
				{
					this.m_queryDeadlocks.Remove(query);
				}
				this.QueryDeadlockEmulated = (this.m_queryDeadlocks.Count > 0);
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000082C8 File Offset: 0x000066C8
		public bool DebugDeadlockDecRetries(string query)
		{
			object queryDeadlocks = this.m_queryDeadlocks;
			bool result;
			lock (queryDeadlocks)
			{
				DALConfig.DeadlockCounter deadlockCounter;
				if (this.m_queryDeadlocks.TryGetValue(query, out deadlockCounter))
				{
					if (deadlockCounter.CurrentRetries > 0)
					{
						this.m_queryDeadlocks[query] = new DALConfig.DeadlockCounter
						{
							TotalRetries = deadlockCounter.TotalRetries,
							CurrentRetries = deadlockCounter.CurrentRetries - 1,
							Count = deadlockCounter.Count
						};
					}
					else
					{
						this.m_queryDeadlocks[query] = new DALConfig.DeadlockCounter
						{
							TotalRetries = deadlockCounter.TotalRetries,
							CurrentRetries = deadlockCounter.TotalRetries,
							Count = deadlockCounter.Count
						};
					}
				}
				result = (deadlockCounter.CurrentRetries > 0);
			}
			return result;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000083BC File Offset: 0x000067BC
		public void DebugDeadlockDecCounts(string query)
		{
			object queryDeadlocks = this.m_queryDeadlocks;
			lock (queryDeadlocks)
			{
				DALConfig.DeadlockCounter deadlockCounter;
				if (this.m_queryDeadlocks.TryGetValue(query, out deadlockCounter))
				{
					if (deadlockCounter.Count > 0)
					{
						this.m_queryDeadlocks[query] = new DALConfig.DeadlockCounter
						{
							TotalRetries = deadlockCounter.TotalRetries,
							CurrentRetries = deadlockCounter.CurrentRetries,
							Count = deadlockCounter.Count - 1
						};
					}
					else
					{
						this.m_queryDeadlocks.Remove(query);
						this.QueryDeadlockEmulated = (this.m_queryDeadlocks.Count > 0);
					}
				}
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00008480 File Offset: 0x00006880
		public void DebugDeadlockReset(string query)
		{
			object queryDeadlocks = this.m_queryDeadlocks;
			lock (queryDeadlocks)
			{
				DALConfig.DeadlockCounter deadlockCounter;
				if (this.m_queryDeadlocks.TryGetValue(query, out deadlockCounter))
				{
					this.m_queryDeadlocks[query] = new DALConfig.DeadlockCounter
					{
						TotalRetries = deadlockCounter.TotalRetries,
						CurrentRetries = deadlockCounter.TotalRetries
					};
				}
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00008504 File Offset: 0x00006904
		public void DebugDeadlockDump()
		{
			object queryDeadlocks = this.m_queryDeadlocks;
			lock (queryDeadlocks)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<string, DALConfig.DeadlockCounter> keyValuePair in this.m_queryDeadlocks)
				{
					stringBuilder.AppendFormat("Query: {0}, Deadlocks: {1}, Current: {2}\n", keyValuePair.Key, keyValuePair.Value.TotalRetries, keyValuePair.Value.CurrentRetries);
				}
				Log.Info(stringBuilder.ToString());
			}
		}

		// Token: 0x04000078 RID: 120
		private readonly Dictionary<string, DALConfig.DeadlockCounter> m_queryDeadlocks;

		// Token: 0x0200003B RID: 59
		private struct DeadlockCounter
		{
			// Token: 0x0400007C RID: 124
			public int Count;

			// Token: 0x0400007D RID: 125
			public int TotalRetries;

			// Token: 0x0400007E RID: 126
			public int CurrentRetries;
		}
	}
}
