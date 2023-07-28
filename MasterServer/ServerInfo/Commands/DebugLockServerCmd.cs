using System;
using System.Threading.Tasks;
using DedicatedPoolServer.Model;
using MasterServer.Core;

namespace MasterServer.ServerInfo.Commands
{
	// Token: 0x020006BA RID: 1722
	[ConsoleCmdAttributes(CmdName = "debug_lock_server")]
	internal class DebugLockServerCmd : ConsoleCommand<DebugLockServerCmdParams>
	{
		// Token: 0x0600241B RID: 9243 RVA: 0x00097099 File Offset: 0x00095499
		public DebugLockServerCmd(IServerInfo serverInfo)
		{
			this.m_serverInfo = serverInfo;
		}

		// Token: 0x0600241C RID: 9244 RVA: 0x000970A8 File Offset: 0x000954A8
		protected override void Execute(DebugLockServerCmdParams param)
		{
			if (!this.m_serverInfo.IsGlobalLbsEnabled)
			{
				Log.Error("To execute this command you need to enable global lbs flow first in module_configuration.xml");
				return;
			}
			string mode = param.Mode;
			if (mode != null)
			{
				DedicatedMode mode2;
				if (!(mode == "pvp"))
				{
					if (!(mode == "pve"))
					{
						goto IL_5B;
					}
					mode2 = DedicatedMode.PVP_PVE;
				}
				else
				{
					mode2 = DedicatedMode.PurePVP;
				}
				try
				{
					Task<DedicatedInfo> task = this.m_serverInfo.RequestServer(mode2, "--" + param.BuildType.ToLower(), param.RegionId);
					task.Wait();
					if (task.Result == null)
					{
						Log.Error("debug_lock_server failed");
					}
					else
					{
						Log.Info<string>("debug_lock_server succeeded. Server id = '{0}'", task.Result.DedicatedId);
					}
				}
				catch (AggregateException e)
				{
					Log.Error("debug_lock_server failed");
					Log.Error(e);
				}
				return;
			}
			IL_5B:
			throw new ArgumentException("Invalid dedicated server mode specified");
		}

		// Token: 0x0400121B RID: 4635
		private readonly IServerInfo m_serverInfo;
	}
}
