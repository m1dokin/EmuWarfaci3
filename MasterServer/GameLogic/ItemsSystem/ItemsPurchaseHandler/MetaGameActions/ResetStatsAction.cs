using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.GameLogic.PlayerStats;
using OLAPHypervisor;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler.MetaGameActions
{
	// Token: 0x0200031D RID: 797
	[Service]
	internal class ResetStatsAction : IMetaGameAction
	{
		// Token: 0x06001221 RID: 4641 RVA: 0x000480C4 File Offset: 0x000464C4
		public ResetStatsAction(IPlayerStatsService playerStats, IPlayerStatsFactory playerStatsFactory)
		{
			this.m_playerStats = playerStats;
			this.m_playerStatsFactory = playerStatsFactory;
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06001222 RID: 4642 RVA: 0x000480DA File Offset: 0x000464DA
		public string Name
		{
			get
			{
				return "on_activate.action";
			}
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x000480E4 File Offset: 0x000464E4
		public void Execute(ulong profileId, string action)
		{
			if (action == "pvp_kd_reset")
			{
				List<Measure> data = this.m_playerStatsFactory.PvPKillDeathRatioReset(profileId);
				this.m_playerStats.FlushPlayerStats(data);
			}
			else if (action == "pvp_wl_reset")
			{
				List<Measure> data2 = this.m_playerStatsFactory.PvPWinLoseRatioReset(profileId);
				this.m_playerStats.FlushPlayerStats(data2);
			}
		}

		// Token: 0x04000852 RID: 2130
		private readonly IPlayerStatsService m_playerStats;

		// Token: 0x04000853 RID: 2131
		private readonly IPlayerStatsFactory m_playerStatsFactory;
	}
}
