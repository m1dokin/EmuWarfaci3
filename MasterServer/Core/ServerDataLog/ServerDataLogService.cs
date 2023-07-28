using System;
using System.Collections.Generic;
using HK2Net;
using HK2Net.Kernel;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;

namespace MasterServer.Core.ServerDataLog
{
	// Token: 0x02000130 RID: 304
	[OrphanService]
	[Singleton]
	internal class ServerDataLogService : ServiceModule
	{
		// Token: 0x06000500 RID: 1280 RVA: 0x00015788 File Offset: 0x00013B88
		public ServerDataLogService(IContainer container)
		{
			this.m_extensions = new List<IServerDataLogExtension>();
			this.m_container = container;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x000157A2 File Offset: 0x00013BA2
		public override void Init()
		{
			base.Init();
			ServicesManager.OnExecutionPhaseChanged += this.OnExecutionPhaseChanged;
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x000157BC File Offset: 0x00013BBC
		private void OnExecutionPhaseChanged(ExecutionPhase executionPhase)
		{
			if (executionPhase == ExecutionPhase.Started)
			{
				ConfigSection section = Resources.ModuleSettings.GetSection("LogServerData");
				section.OnConfigChanged += this.SecOnOnConfigChanged;
				bool flag = int.Parse(section.Get("enable")) > 0;
				if (Resources.DBUpdaterPermission)
				{
					IEnumerable<IServerDataLogExtension> collection = this.m_container.CreateSetWithParams<IServerDataLogExtension>(new Dictionary<string, object>
					{
						{
							"isEnabled",
							flag
						}
					});
					this.m_extensions.AddRange(collection);
					foreach (IServerDataLogExtension serverDataLogExtension in this.m_extensions)
					{
						serverDataLogExtension.Start();
					}
				}
			}
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00015894 File Offset: 0x00013C94
		private void SecOnOnConfigChanged(ConfigEventArgs args)
		{
			if (string.Compare(args.Name, "enable", true) == 0)
			{
				bool enable = args.iValue > 0;
				foreach (IServerDataLogExtension serverDataLogExtension in this.m_extensions)
				{
					serverDataLogExtension.Enable(enable);
				}
			}
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x00015910 File Offset: 0x00013D10
		public override void Stop()
		{
			base.Stop();
			foreach (IServerDataLogExtension serverDataLogExtension in this.m_extensions)
			{
				serverDataLogExtension.Dispose();
			}
		}

		// Token: 0x04000213 RID: 531
		private readonly List<IServerDataLogExtension> m_extensions;

		// Token: 0x04000214 RID: 532
		private readonly IContainer m_container;
	}
}
