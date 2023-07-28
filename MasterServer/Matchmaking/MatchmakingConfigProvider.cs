using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000505 RID: 1285
	[Service]
	[Singleton]
	internal class MatchmakingConfigProvider : ServiceModule, IMatchmakingConfigProvider
	{
		// Token: 0x06001BC2 RID: 7106 RVA: 0x00070708 File Offset: 0x0006EB08
		public MatchmakingConfigProvider()
		{
			this.m_matchmakingConfig = new Lazy<MatchmakingConfig>(new Func<MatchmakingConfig>(this.ParseMatchmakingConfig));
		}

		// Token: 0x1400006C RID: 108
		// (add) Token: 0x06001BC3 RID: 7107 RVA: 0x00070734 File Offset: 0x0006EB34
		// (remove) Token: 0x06001BC4 RID: 7108 RVA: 0x0007076C File Offset: 0x0006EB6C
		public event Action<MatchmakingConfig> OnConfigChanged;

		// Token: 0x06001BC5 RID: 7109 RVA: 0x000707A2 File Offset: 0x0006EBA2
		public override void Start()
		{
			base.Start();
			Resources.ModuleSettings.GetSection("Matchmaking").OnConfigChanged += this.OnConfigChangedImpl;
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x000707CA File Offset: 0x0006EBCA
		public override void Stop()
		{
			Resources.ModuleSettings.GetSection("Matchmaking").OnConfigChanged -= this.OnConfigChangedImpl;
			base.Stop();
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x000707F4 File Offset: 0x0006EBF4
		public MatchmakingConfig Get()
		{
			object @lock = this.m_lock;
			MatchmakingConfig value;
			lock (@lock)
			{
				value = this.m_matchmakingConfig.Value;
			}
			return value;
		}

		// Token: 0x06001BC8 RID: 7112 RVA: 0x00070840 File Offset: 0x0006EC40
		private MatchmakingConfig ParseMatchmakingConfig()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Matchmaking");
			bool isAutostartEnabled;
			section.Get("AutostartEnabled", out isAutostartEnabled);
			bool isPveAutostartEnabled;
			section.Get("PveAutostartEnabled", out isPveAutostartEnabled);
			return new MatchmakingConfig
			{
				QueueInterval = TimeSpan.FromMilliseconds((double)int.Parse(section.Get("QueueInterval"))),
				TimeToMapsResetNotification = TimeSpan.FromSeconds((double)int.Parse(section.Get("TimeToMapsResetNotification"))),
				IsAutostartEnabled = isAutostartEnabled,
				IsPveAutostartEnabled = isPveAutostartEnabled
			};
		}

		// Token: 0x06001BC9 RID: 7113 RVA: 0x000708CC File Offset: 0x0006ECCC
		private void OnConfigChangedImpl(ConfigEventArgs args)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_matchmakingConfig = new Lazy<MatchmakingConfig>(new Func<MatchmakingConfig>(this.ParseMatchmakingConfig));
			}
			if (this.OnConfigChanged != null)
			{
				MatchmakingConfig obj = this.Get();
				this.OnConfigChanged(obj);
			}
		}

		// Token: 0x04000D4A RID: 3402
		private readonly object m_lock = new object();

		// Token: 0x04000D4B RID: 3403
		private Lazy<MatchmakingConfig> m_matchmakingConfig;
	}
}
