using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001A8 RID: 424
	[Service]
	[Singleton]
	internal class QoSQueue : ServiceModule, IQoSQueue
	{
		// Token: 0x060007E0 RID: 2016 RVA: 0x0001DFB8 File Offset: 0x0001C3B8
		public QoSQueue()
		{
			this.m_shapers.Add(new CongestionShaper());
			this.m_shapers.Add(new OnlineUsersShaper());
			this.m_shapers.Add(new QueryClassShaper());
			this.m_shapers.Add(new UserShaper());
			this.m_shapers.Add(new ThreadPoolProxyShaper());
			this.m_shapers.Add(new TimeoutShaper());
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x0001E055 File Offset: 0x0001C455
		public IQoSShaper[] GetShapers()
		{
			return this.m_shapers.ToArray();
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x0001E062 File Offset: 0x0001C462
		public bool QueueWorkItem(TShapingInfo shaping_info, WaitCallback callback)
		{
			return this.QueueWorkItem(shaping_info, callback, null);
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x0001E070 File Offset: 0x0001C470
		public bool QueueWorkItem(TShapingInfo shaping_info, WaitCallback callback, object state)
		{
			return this.QueueWorkItem(new WorkItem(shaping_info, delegate(object s)
			{
				callback(s);
				return null;
			}, state));
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x0001E0A3 File Offset: 0x0001C4A3
		public bool QueueAsyncWorkItem(TShapingInfo shaping_info, Func<object, Task> async_callback)
		{
			return this.QueueAsyncWorkItem(shaping_info, async_callback, null);
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x0001E0AE File Offset: 0x0001C4AE
		public bool QueueAsyncWorkItem(TShapingInfo shaping_info, Func<object, Task> async_callback, object state)
		{
			return this.QueueWorkItem(new WorkItem(shaping_info, async_callback, state));
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x0001E0BE File Offset: 0x0001C4BE
		private bool QueueWorkItem(WorkItem item)
		{
			return this.QueueWorkItem(item, 0);
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x0001E0C8 File Offset: 0x0001C4C8
		private bool QueueWorkItem(WorkItem item, int first_shaper)
		{
			object shapers = this.m_shapers;
			bool result;
			lock (shapers)
			{
				for (int i = first_shaper; i < this.m_shapers.Count; i++)
				{
					IQoSShaper qoSShaper = this.m_shapers[i];
					ShaperDecision shaperDecision;
					for (shaperDecision = qoSShaper.IncomingWorkItem(item); shaperDecision == ShaperDecision.Block; shaperDecision = qoSShaper.IncomingWorkItem(item))
					{
						this.TryPrintQueues();
						Monitor.Wait(this.m_shapers);
					}
					if (shaperDecision == ShaperDecision.Discard)
					{
						this.DiscardWorkItem(item, i);
						return false;
					}
					if (shaperDecision == ShaperDecision.Queued)
					{
						return true;
					}
				}
				this.StartWorkItem(item);
				result = true;
			}
			return result;
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x0001E194 File Offset: 0x0001C594
		private void DiscardWorkItem(WorkItem item, int by_shaper)
		{
			for (int i = by_shaper - 1; i >= 0; i--)
			{
				IQoSShaper qoSShaper = this.m_shapers[i];
				qoSShaper.WorkItemFinished(item);
			}
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x0001E1C9 File Offset: 0x0001C5C9
		private void StartWorkItem(WorkItem item)
		{
			ThreadPoolProxy.QueueUserWorkItem(new WaitCallback(this.Dispatch), item, false);
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x0001E1E0 File Offset: 0x0001C5E0
		private void Dispatch(object wi)
		{
			WorkItem item = (WorkItem)wi;
			Task task = null;
			try
			{
				task = item.callback(item.state);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
			if (task == null)
			{
				this.OnWorkItemFinished(item);
			}
			else
			{
				task.ContinueWith(delegate(Task x)
				{
					if (x.IsFaulted)
					{
						Log.Error(x.Exception.InnerException);
					}
					this.OnWorkItemFinished(item);
				}, TaskContinuationOptions.ExecuteSynchronously);
			}
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0001E274 File Offset: 0x0001C674
		private void OnWorkItemFinished(WorkItem item)
		{
			object shapers = this.m_shapers;
			lock (shapers)
			{
				int num = 0;
				foreach (IQoSShaper qoSShaper in this.m_shapers)
				{
					qoSShaper.WorkItemFinished(item);
					num++;
					WorkItem workItem = qoSShaper.DequeueWorkItem(item);
					if (workItem != null)
					{
						this.QueueWorkItem(workItem, num);
					}
				}
				Monitor.PulseAll(this.m_shapers);
			}
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x0001E32C File Offset: 0x0001C72C
		private void TryPrintQueues()
		{
			if (DateTime.UtcNow > this.m_dumpQueuesNextTime)
			{
				StringBuilder stringBuilder = new StringBuilder(10240);
				stringBuilder.AppendLine();
				foreach (IQoSShaper qoSShaper in this.m_shapers)
				{
					stringBuilder.AppendLine(qoSShaper.ToString());
				}
				Log.Warning(stringBuilder.ToString());
				this.m_dumpQueuesNextTime = DateTime.UtcNow + this.m_dumpQueuesPeriod;
			}
		}

		// Token: 0x040004A5 RID: 1189
		private TimeSpan m_dumpQueuesPeriod = TimeSpan.FromMinutes(5.0);

		// Token: 0x040004A6 RID: 1190
		private DateTime m_dumpQueuesNextTime = DateTime.UtcNow;

		// Token: 0x040004A7 RID: 1191
		private List<IQoSShaper> m_shapers = new List<IQoSShaper>();
	}
}
