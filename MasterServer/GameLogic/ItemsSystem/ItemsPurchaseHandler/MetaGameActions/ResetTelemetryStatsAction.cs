using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.Telemetry;
using OLAPHypervisor;

namespace MasterServer.GameLogic.ItemsSystem.ItemsPurchaseHandler.MetaGameActions
{
	// Token: 0x0200031B RID: 795
	[Service]
	internal class ResetTelemetryStatsAction : IMetaGameAction
	{
		// Token: 0x0600121C RID: 4636 RVA: 0x0004801C File Offset: 0x0004641C
		public ResetTelemetryStatsAction(ITelemetryService telemetryService, IPlayerStatsFactory playerStatsFactory)
		{
			this.m_telemetryService = telemetryService;
			this.m_playerStatsFactory = playerStatsFactory;
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x0600121D RID: 4637 RVA: 0x00048032 File Offset: 0x00046432
		public string Name
		{
			get
			{
				return "on_activate.telemetry_action";
			}
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x0004803C File Offset: 0x0004643C
		public void Execute(ulong profileId, string action)
		{
			try
			{
				if (action == "pvp_kd_reset")
				{
					List<Measure> msrs = this.m_playerStatsFactory.PvPKillDeathRatioReset(profileId);
					this.m_telemetryService.AddMeasure(msrs);
				}
				else if (action == "pvp_wl_reset")
				{
					List<Measure> msrs2 = this.m_playerStatsFactory.PvPWinLoseRatioReset(profileId);
					this.m_telemetryService.AddMeasure(msrs2);
				}
			}
			catch (Exception e)
			{
				MasterServer.Core.Log.Error(e);
			}
		}

		// Token: 0x04000850 RID: 2128
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x04000851 RID: 2129
		private readonly IPlayerStatsFactory m_playerStatsFactory;
	}
}
