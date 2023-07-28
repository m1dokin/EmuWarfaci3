using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MasterServer.Core.Diagnostics.Threading;
using MasterServer.Telemetry.Metrics;
using Util.Common;

namespace MasterServer.Core
{
	// Token: 0x02000147 RID: 327
	public abstract class ProcessingQueue<T> : IProcessingQueue<T>, IDisposable
	{
		// Token: 0x060005AA RID: 1450 RVA: 0x000116F0 File Offset: 0x0000FAF0
		protected ProcessingQueue(string queueName, IProcessingQueueMetricsTracker processingQueueMetricsTracker, bool autoStart = true)
		{
			this.m_processingQueueMetricsTracker = processingQueueMetricsTracker;
			this.m_queueName = queueName;
			this.m_lock = new object();
			this.m_jobQueue = new List<T>();
			this.m_processingQueue = new List<T>();
			this.QueueLimit = 0;
			this.m_wakeupEvent = new AutoResetEvent(false);
			this.m_updatesDone = new AutoResetEvent(false);
			this.m_shutdownRequested = false;
			this.m_thread = new Thread(new ThreadStart(this.ThreadFunc));
			this.m_thread.Name = this.m_queueName;
			if (autoStart)
			{
				this.Start();
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060005AB RID: 1451 RVA: 0x0001178C File Offset: 0x0000FB8C
		// (set) Token: 0x060005AC RID: 1452 RVA: 0x00011794 File Offset: 0x0000FB94
		public int QueueLimit { get; set; }

		// Token: 0x060005AD RID: 1453 RVA: 0x0001179D File Offset: 0x0000FB9D
		public void Dispose()
		{
			this.Stop();
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x000117A5 File Offset: 0x0000FBA5
		public void Start()
		{
			this.m_thread.Start();
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x000117B4 File Offset: 0x0000FBB4
		public void Add(T item)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.QueueLimit > 0 && this.m_jobQueue.Count >= this.QueueLimit)
				{
					this.m_processingQueueMetricsTracker.ReportSkipped(this.m_queueName);
					Log.Warning<string, int>("Processing queue {0} is full, limit {1}", this.m_queueName, this.QueueLimit);
					return;
				}
				this.m_jobQueue.Add(item);
			}
			this.m_wakeupEvent.Set();
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x00011858 File Offset: 0x0000FC58
		public void Add(IEnumerable<T> items)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_jobQueue.AddRange(items);
			}
			this.m_wakeupEvent.Set();
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x000118B0 File Offset: 0x0000FCB0
		private void ThreadFunc()
		{
			CultureHelpers.SetNeutralThreadCulture();
			while (!this.m_shutdownRequested)
			{
				this.m_wakeupEvent.WaitOne();
				using (new ThreadTracker.Tracker(string.Format("ProcessingQueue: {0}", this.m_thread.Name)))
				{
					this.SwapQueues();
					this.ProcessItems();
					this.m_updatesDone.Set();
				}
			}
			this.m_updatesDone.Set();
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x00011940 File Offset: 0x0000FD40
		protected void SwapQueues()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				List<T> processingQueue = this.m_processingQueue;
				this.m_processingQueue = this.m_jobQueue;
				this.m_jobQueue = processingQueue;
			}
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x00011998 File Offset: 0x0000FD98
		public void Stop()
		{
			this.m_shutdownRequested = true;
			this.m_updatesDone.Reset();
			this.m_wakeupEvent.Set();
			if (!this.m_updatesDone.WaitOne(2000, false))
			{
				this.m_thread.Abort();
			}
			this.m_thread = null;
			this.m_processingQueue.Clear();
			this.m_jobQueue.Clear();
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x00011A04 File Offset: 0x0000FE04
		protected void ProcessItems()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (T item in this.m_processingQueue)
			{
				stopwatch.Restart();
				try
				{
					this.Process(item);
					stopwatch.Stop();
				}
				catch (Exception e)
				{
					stopwatch.Stop();
					Log.Error<string>("Processing queue {0} thread error", this.m_thread.Name);
					Log.Error(e);
				}
				this.m_processingQueueMetricsTracker.ReportConsumed(this.m_queueName);
				this.m_processingQueueMetricsTracker.ReportProcessingTime(this.m_queueName, stopwatch.Elapsed);
			}
			this.m_processingQueue.Clear();
		}

		// Token: 0x060005B5 RID: 1461
		public abstract void Process(T item);

		// Token: 0x040003AE RID: 942
		private readonly IProcessingQueueMetricsTracker m_processingQueueMetricsTracker;

		// Token: 0x040003AF RID: 943
		private readonly string m_queueName;

		// Token: 0x040003B0 RID: 944
		private object m_lock;

		// Token: 0x040003B1 RID: 945
		private Thread m_thread;

		// Token: 0x040003B2 RID: 946
		private AutoResetEvent m_wakeupEvent;

		// Token: 0x040003B3 RID: 947
		private AutoResetEvent m_updatesDone;

		// Token: 0x040003B4 RID: 948
		private List<T> m_jobQueue;

		// Token: 0x040003B5 RID: 949
		private List<T> m_processingQueue;

		// Token: 0x040003B6 RID: 950
		private bool m_shutdownRequested;
	}
}
