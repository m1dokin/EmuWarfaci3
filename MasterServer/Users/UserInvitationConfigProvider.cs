using System;
using System.Threading;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Configuration;
using Util.Common;

namespace MasterServer.Users
{
	// Token: 0x02000800 RID: 2048
	[Service]
	[Singleton]
	internal class UserInvitationConfigProvider : ServiceModule, IConfigProvider<UserInvitationConfig>
	{
		// Token: 0x060029FC RID: 10748 RVA: 0x000B58AF File Offset: 0x000B3CAF
		public UserInvitationConfigProvider(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x140000B3 RID: 179
		// (add) Token: 0x060029FD RID: 10749 RVA: 0x000B58C0 File Offset: 0x000B3CC0
		// (remove) Token: 0x060029FE RID: 10750 RVA: 0x000B58F8 File Offset: 0x000B3CF8
		public event Action<UserInvitationConfig> Changed;

		// Token: 0x060029FF RID: 10751 RVA: 0x000B5930 File Offset: 0x000B3D30
		public override void Init()
		{
			base.Init();
			ConfigSection section = this.GetSection();
			this.InitConfig(section);
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06002A00 RID: 10752 RVA: 0x000B5964 File Offset: 0x000B3D64
		public override void Stop()
		{
			ConfigSection section = this.GetSection();
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x06002A01 RID: 10753 RVA: 0x000B5990 File Offset: 0x000B3D90
		public UserInvitationConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x06002A02 RID: 10754 RVA: 0x000B5998 File Offset: 0x000B3D98
		private void InitConfig(ConfigSection section)
		{
			bool useGroups;
			section.TryGet("use_group", out useGroups, true);
			UserInvitationConfig value = new UserInvitationConfig(useGroups);
			Interlocked.Exchange<UserInvitationConfig>(ref this.m_config, value);
		}

		// Token: 0x06002A03 RID: 10755 RVA: 0x000B59C8 File Offset: 0x000B3DC8
		private void OnConfigChanged(ConfigEventArgs args)
		{
			this.InitConfig(args.Section);
			this.Changed.SafeInvoke(this.m_config);
		}

		// Token: 0x06002A04 RID: 10756 RVA: 0x000B59E7 File Offset: 0x000B3DE7
		private ConfigSection GetSection()
		{
			return this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration).GetSection("GameRoom.UserInvitation");
		}

		// Token: 0x0400165C RID: 5724
		private readonly IConfigurationService m_configurationService;

		// Token: 0x0400165D RID: 5725
		private UserInvitationConfig m_config;
	}
}
