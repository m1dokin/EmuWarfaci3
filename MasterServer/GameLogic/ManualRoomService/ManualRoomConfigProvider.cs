using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using Util.Common;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x02000066 RID: 102
	[Service]
	[Singleton]
	internal class ManualRoomConfigProvider : ServiceModule, IConfigProvider<ManualRoomConfig>
	{
		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000185 RID: 389 RVA: 0x0000A2C8 File Offset: 0x000086C8
		// (remove) Token: 0x06000186 RID: 390 RVA: 0x0000A300 File Offset: 0x00008700
		public event Action<ManualRoomConfig> Changed;

		// Token: 0x06000187 RID: 391 RVA: 0x0000A338 File Offset: 0x00008738
		public override void Init()
		{
			base.Init();
			this.InitConfig();
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000A374 File Offset: 0x00008774
		public override void Stop()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000A3A9 File Offset: 0x000087A9
		public ManualRoomConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000A3B1 File Offset: 0x000087B1
		private void OnConfigChanged(ConfigEventArgs args)
		{
			this.InitConfig();
			this.Changed.SafeInvokeEach(this.m_config);
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000A3CC File Offset: 0x000087CC
		private void InitConfig()
		{
			TimeSpan timeout;
			if (!Resources.ModuleSettings.GetSection("GameRoom").TryGet("ManualRoomWaitTimeout_sec", out timeout, default(TimeSpan)))
			{
				throw new ArgumentException("Unable to retrieve ManualRoomWaitTimeout_sec value from module_configuration.xml");
			}
			this.m_config = new ManualRoomConfig(timeout);
		}

		// Token: 0x040000B5 RID: 181
		private ManualRoomConfig m_config;
	}
}
