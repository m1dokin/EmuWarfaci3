using System;
using System.Collections.Generic;
using System.Threading;
using HK2Net;
using MasterServer.Core.Services;
using Util.Common;

namespace MasterServer.Core
{
	// Token: 0x02000141 RID: 321
	[Service]
	[Singleton]
	internal class LogService : ServiceModule, ILogService
	{
		// Token: 0x0600058E RID: 1422 RVA: 0x0001691C File Offset: 0x00014D1C
		public LogService(ILogServiceBuilder logServiceBuilder)
		{
			this.m_logGroupType = logServiceBuilder.Build(typeof(LogGroup), new Type[]
			{
				typeof(int),
				typeof(IEnumerable<ILogSync>)
			}, typeof(ILogGroup));
		}

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x0600058F RID: 1423 RVA: 0x0001697C File Offset: 0x00014D7C
		// (remove) Token: 0x06000590 RID: 1424 RVA: 0x000169B4 File Offset: 0x00014DB4
		public event Action<string> OnEvent;

		// Token: 0x06000591 RID: 1425 RVA: 0x000169EC File Offset: 0x00014DEC
		public override void Init()
		{
			try
			{
				this.m_sync.Add(new LogProxySync());
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
			this.m_sync.Add(new CNullSync());
			this.m_mainGroup = (LogGroup)Activator.CreateInstance(this.m_logGroupType, new object[]
			{
				0,
				this.m_sync
			});
			this.m_mainGroup.OnEvent += this.OnEventImpl;
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x00016A80 File Offset: 0x00014E80
		public override void Stop()
		{
			base.Stop();
			foreach (ILogSync logSync in this.m_sync)
			{
				logSync.Dispose();
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x00016AE4 File Offset: 0x00014EE4
		public ILogGroup Event
		{
			get
			{
				return (ILogGroup)this.m_mainGroup;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000594 RID: 1428 RVA: 0x00016AF1 File Offset: 0x00014EF1
		public long EventCount
		{
			get
			{
				return Interlocked.Read(ref this.m_eventCount);
			}
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x00016B00 File Offset: 0x00014F00
		public ILogGroup CreateGroup()
		{
			int num;
			do
			{
				num = Interlocked.Increment(ref LogService.m_logGroupId);
			}
			while (num == 0);
			LogGroup logGroup = (LogGroup)Activator.CreateInstance(this.m_logGroupType, new object[]
			{
				num,
				this.m_sync
			});
			logGroup.OnEvent += this.OnEventImpl;
			return (ILogGroup)logGroup;
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00016B61 File Offset: 0x00014F61
		private void OnEventImpl(string eventCategory)
		{
			this.OnEvent.SafeInvoke(eventCategory);
			Interlocked.Increment(ref this.m_eventCount);
		}

		// Token: 0x040003A8 RID: 936
		private long m_eventCount;

		// Token: 0x040003A9 RID: 937
		private static int m_logGroupId;

		// Token: 0x040003AA RID: 938
		private readonly Type m_logGroupType;

		// Token: 0x040003AB RID: 939
		private readonly List<ILogSync> m_sync = new List<ILogSync>();

		// Token: 0x040003AC RID: 940
		private LogGroup m_mainGroup;
	}
}
