using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.Telemetry.Metrics;
using OLAPHypervisor;

namespace MasterServer.GameLogic.PlayerStats
{
	// Token: 0x02000536 RID: 1334
	internal class PlayerStatsUpdateProcessingQueue : MasterServer.Core.ProcessingQueue<List<Measure>>
	{
		// Token: 0x06001D02 RID: 7426 RVA: 0x00075230 File Offset: 0x00073630
		public PlayerStatsUpdateProcessingQueue(int limit, IPlayerStatsService playerStatsService, IProcessingQueueMetricsTracker processingQueueMetricsTracker) : base("PlayerStatsUpdateProcessingQueue", processingQueueMetricsTracker, true)
		{
			base.QueueLimit = limit;
			this.m_playerStats = playerStatsService;
		}

		// Token: 0x06001D03 RID: 7427 RVA: 0x0007524D File Offset: 0x0007364D
		public override void Process(List<Measure> data)
		{
			this.m_playerStats.FlushPlayerStats(data);
		}

		// Token: 0x04000DCE RID: 3534
		private readonly IPlayerStatsService m_playerStats;
	}
}
