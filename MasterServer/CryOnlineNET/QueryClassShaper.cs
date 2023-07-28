using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001AB RID: 427
	internal class QueryClassShaper : IQoSShaper
	{
		// Token: 0x060007FE RID: 2046 RVA: 0x0001E854 File Offset: 0x0001CC54
		public QueryClassShaper()
		{
			List<ConfigSection> sections = Resources.QoSSettings.GetSection("query_shaper").GetSections("class");
			foreach (ConfigSection configSection in sections)
			{
				string text = configSection.Get("qos");
				int num = int.Parse(configSection.Get("queue"));
				int num2 = int.Parse(configSection.Get("concurrency"));
				if (num < 0 || num2 <= 0)
				{
					Log.Warning<string>("Invalid QoS params for class '{0}'", text);
				}
				else
				{
					this.m_qosData.Add(text, new QueryClassShaper.QoSParams
					{
						QueueLimit = num,
						ConcurrencyLimit = num2
					});
					this.m_qosQueue.Add(text, new QueryClassShaper.QueryClassInfo());
				}
			}
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x0001E964 File Offset: 0x0001CD64
		public QueryClassShaper.QoSParams GetParams(string query_class)
		{
			return this.m_qosData[query_class];
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x0001E974 File Offset: 0x0001CD74
		public void SetParams(string query_class, QueryClassShaper.QoSParams prms)
		{
			Dictionary<string, QueryClassShaper.QoSParams> dictionary = new Dictionary<string, QueryClassShaper.QoSParams>(this.m_qosData);
			dictionary[query_class] = prms;
			this.m_qosData = dictionary;
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x0001E99C File Offset: 0x0001CD9C
		public ShaperDecision IncomingWorkItem(WorkItem item)
		{
			if (string.IsNullOrEmpty(item.shaping_info.query_class))
			{
				return ShaperDecision.Execute;
			}
			QueryClassShaper.QoSParams qoSParams;
			if (!this.m_qosData.TryGetValue(item.shaping_info.query_class, out qoSParams))
			{
				Log.Warning<string>("Not configured qos class '{0}'", item.shaping_info.query_class);
				return ShaperDecision.Execute;
			}
			QueryClassShaper.QueryClassInfo queryClassInfo = this.m_qosQueue[item.shaping_info.query_class];
			if (queryClassInfo.tasks_in_progress < qoSParams.ConcurrencyLimit)
			{
				queryClassInfo.tasks_in_progress++;
				return ShaperDecision.Execute;
			}
			if (queryClassInfo.tasks_queue.Count < qoSParams.QueueLimit)
			{
				queryClassInfo.tasks_queue.Enqueue(item);
				return ShaperDecision.Queued;
			}
			Log.Warning<string, int>("Query class '{0}' QoS queue limit of {1} reached", item.shaping_info.query_class, qoSParams.QueueLimit);
			return ShaperDecision.Discard;
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x0001EA70 File Offset: 0x0001CE70
		public void WorkItemFinished(WorkItem finished)
		{
			if (string.IsNullOrEmpty(finished.shaping_info.query_class))
			{
				return;
			}
			QueryClassShaper.QueryClassInfo queryClassInfo;
			if (this.m_qosQueue.TryGetValue(finished.shaping_info.query_class, out queryClassInfo))
			{
				queryClassInfo.tasks_in_progress--;
			}
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x0001EAC0 File Offset: 0x0001CEC0
		public WorkItem DequeueWorkItem(WorkItem finished)
		{
			if (string.IsNullOrEmpty(finished.shaping_info.query_class))
			{
				return null;
			}
			QueryClassShaper.QueryClassInfo queryClassInfo;
			if (this.m_qosQueue.TryGetValue(finished.shaping_info.query_class, out queryClassInfo) && queryClassInfo.tasks_queue.Count > 0)
			{
				queryClassInfo.tasks_in_progress++;
				return queryClassInfo.tasks_queue.Dequeue();
			}
			return null;
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x0001EB30 File Offset: 0x0001CF30
		public void FillMemoryUsageInfo(StringBuilder stringBuidler)
		{
			foreach (QueryClassShaper.QueryClassInfo queryClassInfo in this.m_qosQueue.Values)
			{
				stringBuidler.AppendFormat("Task in progress: {0} Queue size: {1}\n", queryClassInfo.tasks_in_progress, queryClassInfo.tasks_queue.Count);
			}
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x0001EBB4 File Offset: 0x0001CFB4
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[QueryClassShaper]:");
			foreach (KeyValuePair<string, QueryClassShaper.QueryClassInfo> keyValuePair in this.m_qosQueue)
			{
				stringBuilder.AppendLine(string.Format("\t{0}: count {1}", keyValuePair.Key, keyValuePair.Value.tasks_in_progress));
				foreach (WorkItem arg in keyValuePair.Value.tasks_queue)
				{
					stringBuilder.AppendLine(string.Format("\t\t{0}:", arg));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040004AF RID: 1199
		private Dictionary<string, QueryClassShaper.QoSParams> m_qosData = new Dictionary<string, QueryClassShaper.QoSParams>();

		// Token: 0x040004B0 RID: 1200
		private Dictionary<string, QueryClassShaper.QueryClassInfo> m_qosQueue = new Dictionary<string, QueryClassShaper.QueryClassInfo>();

		// Token: 0x020001AC RID: 428
		private class QueryClassInfo
		{
			// Token: 0x040004B1 RID: 1201
			public int tasks_in_progress;

			// Token: 0x040004B2 RID: 1202
			public Queue<WorkItem> tasks_queue = new Queue<WorkItem>();
		}

		// Token: 0x020001AD RID: 429
		public struct QoSParams
		{
			// Token: 0x040004B3 RID: 1203
			public int QueueLimit;

			// Token: 0x040004B4 RID: 1204
			public int ConcurrencyLimit;
		}
	}
}
