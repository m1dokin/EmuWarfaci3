using System;
using System.Linq;
using System.Text;
using MasterServer.Core;

namespace MasterServer.XMPP
{
	// Token: 0x0200080D RID: 2061
	[ConsoleCmdAttributes(CmdName = "get_presence", Help = "dump last calculated presence")]
	internal class GetPresenceCmd : IConsoleCmd
	{
		// Token: 0x06002A39 RID: 10809 RVA: 0x000B6285 File Offset: 0x000B4685
		public GetPresenceCmd(IServerPresenceNotifier serverPresenceNotifier)
		{
			this.m_serverPresenceNotifier = serverPresenceNotifier;
		}

		// Token: 0x06002A3A RID: 10810 RVA: 0x000B6294 File Offset: 0x000B4694
		public void ExecuteCmd(string[] args)
		{
			ServerLoadStats loadStats = this.m_serverPresenceNotifier.LoadStats;
			StringBuilder stringBuilder = new StringBuilder("server presence\n");
			stringBuilder.AppendFormat("online_users: {0}\n", loadStats.Online);
			stringBuilder.AppendFormat("total_load: {0}\n", loadStats.Load);
			stringBuilder.AppendFormat("load_stats_types: {0}\n", string.Join(",", from stat in loadStats.LoadStats
			select stat.Item1));
			stringBuilder.AppendFormat("load_stats: {0}\n", string.Join<float>(",", from stat in loadStats.LoadStats
			select stat.Item2));
			stringBuilder.AppendFormat("online_users_regions: {0}\n", string.Join(",", from stat in loadStats.OnlineStats
			select stat.Item1));
			stringBuilder.AppendFormat("online_users_count: {0}\n", string.Join<int>(",", from stat in loadStats.OnlineStats
			select stat.Item2));
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x04001682 RID: 5762
		private readonly IServerPresenceNotifier m_serverPresenceNotifier;
	}
}
