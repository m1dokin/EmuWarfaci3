using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Configuration;
using Util.Common;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200000E RID: 14
	[Service]
	[Singleton]
	internal class GroupSizeConfigProvider : ServiceModule, IConfigProvider<GroupSizeConfig>
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00004F01 File Offset: 0x00003301
		public GroupSizeConfigProvider(IConfigurationService configurationService, IConfigValidator<GroupSizeConfig> validator)
		{
			this.m_configurationService = configurationService;
			this.m_configValidator = validator;
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000039 RID: 57 RVA: 0x00004F18 File Offset: 0x00003318
		// (remove) Token: 0x0600003A RID: 58 RVA: 0x00004F50 File Offset: 0x00003350
		public event Action<GroupSizeConfig> Changed;

		// Token: 0x0600003B RID: 59 RVA: 0x00004F88 File Offset: 0x00003388
		public override void Init()
		{
			base.Init();
			ConfigSection section = this.GetSection();
			this.InitConfig(section);
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00004FBC File Offset: 0x000033BC
		public override void Stop()
		{
			ConfigSection section = this.GetSection();
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004FE8 File Offset: 0x000033E8
		public GroupSizeConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004FF0 File Offset: 0x000033F0
		private void InitConfig(ConfigSection section)
		{
			Dictionary<GameRoomType, int> dictionary = new Dictionary<GameRoomType, int>();
			IEnumerator enumerator = Enum.GetValues(typeof(GameRoomType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					GameRoomType key = (GameRoomType)obj;
					int value;
					if (section.TryGet(key.ToString(), out value, 0))
					{
						dictionary.Add(key, value);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			GroupSizeConfig groupSizeConfig = new GroupSizeConfig(dictionary);
			this.m_configValidator.Validate(groupSizeConfig);
			Interlocked.Exchange<GroupSizeConfig>(ref this.m_config, groupSizeConfig);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000050A8 File Offset: 0x000034A8
		private void OnConfigChanged(ConfigEventArgs args)
		{
			this.InitConfig(args.Section);
			this.Changed.SafeInvoke(this.m_config);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000050C7 File Offset: 0x000034C7
		private ConfigSection GetSection()
		{
			return this.m_configurationService.GetConfig(ConfigInfo.ModuleConfiguration).GetSection("GameRoom.GroupSize");
		}

		// Token: 0x0400001C RID: 28
		private readonly IConfigurationService m_configurationService;

		// Token: 0x0400001D RID: 29
		private readonly IConfigValidator<GroupSizeConfig> m_configValidator;

		// Token: 0x0400001E RID: 30
		private GroupSizeConfig m_config;
	}
}
