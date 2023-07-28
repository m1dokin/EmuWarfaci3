using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Configuration;
using Util.Common;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006D9 RID: 1753
	[Service]
	[Singleton]
	internal class ServerInfoTrackerConfigProvider : ServiceModule, IConfigProvider<ServerInfoTrackerConfig>
	{
		// Token: 0x060024D6 RID: 9430 RVA: 0x0009A204 File Offset: 0x00098604
		public ServerInfoTrackerConfigProvider(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			ConfigSection section = config.GetSection("Metrics");
			section.OnConfigChanged += this.OnConfigChanged;
			this.ReadConfig();
		}

		// Token: 0x1400009D RID: 157
		// (add) Token: 0x060024D7 RID: 9431 RVA: 0x0009A254 File Offset: 0x00098654
		// (remove) Token: 0x060024D8 RID: 9432 RVA: 0x0009A28C File Offset: 0x0009868C
		public event Action<ServerInfoTrackerConfig> Changed;

		// Token: 0x060024D9 RID: 9433 RVA: 0x0009A2C4 File Offset: 0x000986C4
		public override void Stop()
		{
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			ConfigSection section = config.GetSection("Metrics");
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x060024DA RID: 9434 RVA: 0x0009A306 File Offset: 0x00098706
		public ServerInfoTrackerConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x060024DB RID: 9435 RVA: 0x0009A310 File Offset: 0x00098710
		private void OnConfigChanged(ConfigEventArgs args)
		{
			TimeSpan reportTime = this.m_config.ReportTime;
			this.ReadConfig();
			if (reportTime != this.m_config.ReportTime)
			{
				this.Changed.SafeInvoke(this.m_config);
			}
		}

		// Token: 0x060024DC RID: 9436 RVA: 0x0009A358 File Offset: 0x00098758
		private void ReadConfig()
		{
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			ConfigSection section = config.GetSection("Metrics");
			TimeSpan reportTime;
			if (!section.TryGet("ServerMetricsUpdateTime_sec", out reportTime, default(TimeSpan)))
			{
				throw new KeyNotFoundException("Cannot find section ServerMetricsUpdateTime_sec in 'modules_configuration.xml'");
			}
			this.m_config = new ServerInfoTrackerConfig(reportTime);
		}

		// Token: 0x0400129E RID: 4766
		private readonly IConfigurationService m_configurationService;

		// Token: 0x0400129F RID: 4767
		private ServerInfoTrackerConfig m_config;
	}
}
