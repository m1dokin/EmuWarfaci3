using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Configuration;
using Util.Common;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004C1 RID: 1217
	[Service]
	[Singleton]
	internal class RatingRoomConfigProvider : ServiceModule, IConfigProvider<RatingRoomConfig>
	{
		// Token: 0x06001A50 RID: 6736 RVA: 0x0006C3D4 File Offset: 0x0006A7D4
		public RatingRoomConfigProvider(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			ConfigSection section = config.GetSection("GameRoom");
			section.OnConfigChanged += this.OnConfigChanged;
			this.ReadConfig();
		}

		// Token: 0x14000063 RID: 99
		// (add) Token: 0x06001A51 RID: 6737 RVA: 0x0006C424 File Offset: 0x0006A824
		// (remove) Token: 0x06001A52 RID: 6738 RVA: 0x0006C45C File Offset: 0x0006A85C
		public event Action<RatingRoomConfig> Changed;

		// Token: 0x06001A53 RID: 6739 RVA: 0x0006C494 File Offset: 0x0006A894
		public override void Stop()
		{
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			ConfigSection section = config.GetSection("GameRoom");
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x0006C4D6 File Offset: 0x0006A8D6
		public RatingRoomConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x06001A55 RID: 6741 RVA: 0x0006C4E0 File Offset: 0x0006A8E0
		private void OnConfigChanged(ConfigEventArgs args)
		{
			TimeSpan playersCheckTimeout = this.m_config.PlayersCheckTimeout;
			this.ReadConfig();
			if (playersCheckTimeout != this.m_config.PlayersCheckTimeout)
			{
				this.Changed.SafeInvoke(this.m_config);
			}
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x0006C528 File Offset: 0x0006A928
		private void ReadConfig()
		{
			Config config = this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration);
			ConfigSection section = config.GetSection("GameRoom");
			TimeSpan playersCheckTimeout;
			if (!section.TryGet("RatingCheckPlayersTimeout_sec", out playersCheckTimeout, default(TimeSpan)))
			{
				throw new KeyNotFoundException("Cannot find section RatingCheckPlayersTimeout_sec in 'modules_configuration.xml'");
			}
			this.m_config = new RatingRoomConfig(playersCheckTimeout);
		}

		// Token: 0x04000C99 RID: 3225
		private readonly IConfigurationService m_configurationService;

		// Token: 0x04000C9A RID: 3226
		private RatingRoomConfig m_config;
	}
}
