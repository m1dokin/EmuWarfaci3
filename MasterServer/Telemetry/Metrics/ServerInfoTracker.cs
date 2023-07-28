using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HK2Net;
using LibMetricsNet;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Services.Metrics;
using MasterServer.Core.Timers;
using MasterServer.ServerInfo;
using Ninject;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006D5 RID: 1749
	[Service]
	[Singleton]
	internal class ServerInfoTracker : BaseTracker, IServerInfoTracker
	{
		// Token: 0x060024C7 RID: 9415 RVA: 0x00099E9B File Offset: 0x0009829B
		public ServerInfoTracker(IServerInfo serverInfo, [Named("NonReentrant")] ITimerFactory timerFactory, IConfigProvider<ServerInfoTrackerConfig> config)
		{
			this.m_serverInfo = serverInfo;
			this.m_timerFactory = timerFactory;
			this.m_config = config;
		}

		// Token: 0x060024C8 RID: 9416 RVA: 0x00099EC4 File Offset: 0x000982C4
		public override void Init(IMetricsService service)
		{
			base.Init(service);
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_serverMetrics = new ServerInfoTracker.ServerMetrics(this.m_metricsService.MetricsConfig);
			}
			this.m_config.Changed += this.OnConfigChanged;
			this.m_timer = this.m_timerFactory.CreateTimer(new TimerCallback(this.OnTimeTick), null, this.m_config.Get().ReportTime);
		}

		// Token: 0x060024C9 RID: 9417 RVA: 0x00099F64 File Offset: 0x00098364
		public override void Dispose()
		{
			if (this.m_timer != null)
			{
				this.m_timer.Dispose();
				this.m_timer = null;
			}
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_serverMetrics != null)
				{
					this.m_serverMetrics.Dispose();
					this.m_serverMetrics = null;
				}
			}
		}

		// Token: 0x060024CA RID: 9418 RVA: 0x00099FDC File Offset: 0x000983DC
		public override IEnumerable<Metric> GetMetrics()
		{
			object @lock = this.m_lock;
			IEnumerable<Metric> result;
			lock (@lock)
			{
				result = ((this.m_serverMetrics == null) ? Enumerable.Empty<Metric>() : this.m_serverMetrics.Metrics.GetMetrics().ToArray<Metric>());
			}
			return result;
		}

		// Token: 0x060024CB RID: 9419 RVA: 0x0009A048 File Offset: 0x00098448
		public void ReportQueueSize(int size)
		{
			if (this.m_serverMetrics != null && this.m_metricsService.Enabled)
			{
				this.m_serverMetrics.Metrics.Get<Gauge>(ServerInfoTracker.server_metrics.ms_server_queue_size).Track(DateTime.Now, (long)size);
			}
		}

		// Token: 0x060024CC RID: 9420 RVA: 0x0009A087 File Offset: 0x00098487
		public void ReportAskServerFailed()
		{
			if (this.m_serverMetrics != null && this.m_metricsService.Enabled)
			{
				this.m_serverMetrics.Metrics.Get<Meter>(ServerInfoTracker.server_metrics.ms_server_ask_failed).Inc(DateTime.Now);
			}
		}

		// Token: 0x060024CD RID: 9421 RVA: 0x0009A0C4 File Offset: 0x000984C4
		private void OnConfigChanged(ServerInfoTrackerConfig config)
		{
			this.m_timer.Change(config.ReportTime, config.ReportTime);
		}

		// Token: 0x060024CE RID: 9422 RVA: 0x0009A0E0 File Offset: 0x000984E0
		private void OnTimeTick(object state)
		{
			if (this.m_serverMetrics != null && this.m_metricsService.Enabled)
			{
				int count = this.m_serverInfo.GetBoundServers(false).Count;
				this.m_serverMetrics.Metrics.Get<Gauge>(ServerInfoTracker.server_metrics.ms_server_binded).Track(DateTime.Now, (long)count);
			}
		}

		// Token: 0x04001292 RID: 4754
		private readonly IConfigProvider<ServerInfoTrackerConfig> m_config;

		// Token: 0x04001293 RID: 4755
		private readonly ITimerFactory m_timerFactory;

		// Token: 0x04001294 RID: 4756
		private readonly IServerInfo m_serverInfo;

		// Token: 0x04001295 RID: 4757
		private readonly object m_lock = new object();

		// Token: 0x04001296 RID: 4758
		private ITimer m_timer;

		// Token: 0x04001297 RID: 4759
		private ServerInfoTracker.ServerMetrics m_serverMetrics;

		// Token: 0x020006D6 RID: 1750
		private enum server_metrics
		{
			// Token: 0x04001299 RID: 4761
			ms_server_binded,
			// Token: 0x0400129A RID: 4762
			ms_server_ask_failed,
			// Token: 0x0400129B RID: 4763
			ms_server_queue_size
		}

		// Token: 0x020006D7 RID: 1751
		private class ServerMetrics : IDisposable
		{
			// Token: 0x060024CF RID: 9423 RVA: 0x0009A13C File Offset: 0x0009853C
			public ServerMetrics(MetricsConfig metricsConfig)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(ServerInfoTracker.server_metrics), metricsConfig);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName
				};
				this.Metrics.InitGauge(ServerInfoTracker.server_metrics.ms_server_queue_size, tags);
				this.Metrics.InitGauge(ServerInfoTracker.server_metrics.ms_server_binded, tags);
				this.Metrics.InitMeter(ServerInfoTracker.server_metrics.ms_server_ask_failed, tags);
			}

			// Token: 0x17000393 RID: 915
			// (get) Token: 0x060024D0 RID: 9424 RVA: 0x0009A1B2 File Offset: 0x000985B2
			// (set) Token: 0x060024D1 RID: 9425 RVA: 0x0009A1BA File Offset: 0x000985BA
			public EnumBasedMetricsContainer Metrics { get; private set; }

			// Token: 0x060024D2 RID: 9426 RVA: 0x0009A1C3 File Offset: 0x000985C3
			public void Dispose()
			{
				if (this.Metrics != null)
				{
					this.Metrics.Dispose();
					this.Metrics = null;
				}
			}
		}
	}
}
