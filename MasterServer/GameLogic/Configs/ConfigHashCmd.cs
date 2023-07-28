using System;
using MasterServer.Core;

namespace MasterServer.GameLogic.Configs
{
	// Token: 0x0200028C RID: 652
	[ConsoleCmdAttributes(CmdName = "config_hash", ArgsSize = 1, Help = "Dump Configs hash")]
	internal class ConfigHashCmd : IConsoleCmd
	{
		// Token: 0x06000E1F RID: 3615 RVA: 0x00038BFA File Offset: 0x00036FFA
		public ConfigHashCmd(IConfigsService configsService, IDebugConfigService debugConfigService)
		{
			this.m_configsService = configsService;
			this.m_debugConfigService = debugConfigService;
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x00038C10 File Offset: 0x00037010
		public void ExecuteCmd(string[] args)
		{
			Log.Info<int>("Config service hash: {0}", this.m_configsService.GetHash());
			foreach (IConfigProvider configProvider in this.m_debugConfigService.GetConfigProviders())
			{
				Log.Info<string, int>("Provider {0} hash: {1}", configProvider.GetType().Name, configProvider.GetHash());
			}
		}

		// Token: 0x0400067A RID: 1658
		private readonly IConfigsService m_configsService;

		// Token: 0x0400067B RID: 1659
		private readonly IDebugConfigService m_debugConfigService;
	}
}
