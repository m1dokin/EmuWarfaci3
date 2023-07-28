using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;
using MasterServer.GameRoomSystem;
using MasterServer.Matchmaking;
using MasterServer.Matchmaking.Data;
using Util.Common;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x020006FD RID: 1789
	[Service]
	[Singleton]
	internal class MatchMakingTraker : BaseTracker, IMatchMakingTracker
	{
		// Token: 0x0600256F RID: 9583 RVA: 0x0009CD34 File Offset: 0x0009B134
		public MatchMakingTraker(IMatchmakingPerformer matchmakingPerformer, IMatchmakingSystem matchmakingSystem)
		{
			this.m_matchmakingPerformer = matchmakingPerformer;
			this.m_matchmakingSystem = matchmakingSystem;
			this.m_matchmakingPerformer.OnMatchmakingPerformerStats += this.OnMatchmakingPerformerStats;
			this.m_matchmakingSystem.OnEntitiesFailed += this.OnEntitiesFailed;
			this.m_matchmakingSystem.OnEntitiesSucceded += this.OnEntitiesSucceded;
			this.m_matchmakingSystem.OnUnQueueEntity += this.OnUnQueueEntity;
		}

		// Token: 0x06002570 RID: 9584 RVA: 0x0009CDB4 File Offset: 0x0009B1B4
		public override void Init(IMetricsService service)
		{
			base.Init(service);
			this.m_metrics = new EnumBasedMetricsContainer(typeof(MatchMakingTraker.ms_metric), this.m_metricsService.MetricsConfig);
			string[] tags = new string[]
			{
				"server",
				Resources.ServerName
			};
			this.m_metrics.InitMeter(MatchMakingTraker.ms_metric.ms_matchmk_runs, tags);
			this.m_metrics.InitMeter(MatchMakingTraker.ms_metric.ms_matchmk_players_served, tags);
			this.m_metrics.InitMeter(MatchMakingTraker.ms_metric.ms_matchmk_players_succeded, tags);
			this.m_metrics.InitMeter(MatchMakingTraker.ms_metric.ms_matchmk_players_failed, tags);
			this.m_metrics.InitTimer(MatchMakingTraker.ms_metric.ms_matchmk_run_time, tags);
			this.m_metrics.InitMeter(MatchMakingTraker.ms_metric.ms_matchmaking_players_succeded, tags);
			this.m_metrics.InitMeter(MatchMakingTraker.ms_metric.ms_matchmaking_players_failed, tags);
			this.m_metrics.InitMeter(MatchMakingTraker.ms_metric.ms_matchmaking_players_canceled, tags);
			this.m_metrics.InitTimer(MatchMakingTraker.ms_metric.ms_matchmaking_success_time, tags);
			this.m_metrics.InitTimer(MatchMakingTraker.ms_metric.ms_matchmaking_cancel_time, tags);
			this.m_metrics.InitMeterDynamic(MatchMakingTraker.ms_metric.ms_matchmaking_region_violation, tags);
		}

		// Token: 0x06002571 RID: 9585 RVA: 0x0009CEC7 File Offset: 0x0009B2C7
		public override IEnumerable<Metric> GetMetrics()
		{
			return this.m_metrics.GetMetrics();
		}

		// Token: 0x06002572 RID: 9586 RVA: 0x0009CED4 File Offset: 0x0009B2D4
		public override void Dispose()
		{
			this.m_matchmakingPerformer.OnMatchmakingPerformerStats -= this.OnMatchmakingPerformerStats;
			this.m_matchmakingSystem.OnEntitiesFailed -= this.OnEntitiesFailed;
			this.m_matchmakingSystem.OnEntitiesSucceded -= this.OnEntitiesSucceded;
			this.m_matchmakingSystem.OnUnQueueEntity -= this.OnUnQueueEntity;
		}

		// Token: 0x06002573 RID: 9587 RVA: 0x0009CF40 File Offset: 0x0009B340
		public void TrackRegionViolation(string regionId)
		{
			this.WriteMetrics(delegate(DateTime now)
			{
				this.m_metrics.Get<Meter>(MatchMakingTraker.ms_metric.ms_matchmaking_region_violation, new string[]
				{
					"region_id",
					regionId
				}).Inc(now);
			});
		}

		// Token: 0x06002574 RID: 9588 RVA: 0x0009CF74 File Offset: 0x0009B374
		private void WriteMetrics(Action<DateTime> writer)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			using (this.m_metricsService.Synchronizer.BatchWrite())
			{
				writer(DateTime.Now);
			}
		}

		// Token: 0x06002575 RID: 9589 RVA: 0x0009CFD0 File Offset: 0x0009B3D0
		private void OnMatchmakingPerformerStats(MatchmakingPerformerStats stats)
		{
			this.WriteMetrics(delegate(DateTime now)
			{
				this.m_metrics.Get<Meter>(MatchMakingTraker.ms_metric.ms_matchmk_runs).Inc(now);
				this.m_metrics.Get<Meter>(MatchMakingTraker.ms_metric.ms_matchmk_players_served).Adjust(now, (long)stats.PlayersTotal);
				this.m_metrics.Get<Meter>(MatchMakingTraker.ms_metric.ms_matchmk_players_succeded).Adjust(now, (long)stats.PlayersSuccess);
				this.m_metrics.Get<Meter>(MatchMakingTraker.ms_metric.ms_matchmk_players_failed).Adjust(now, (long)stats.PlayersFailed);
				this.m_metrics.Get<Timer>(MatchMakingTraker.ms_metric.ms_matchmk_run_time).TimeMilliseconds(now, stats.TimeSpend);
			});
		}

		// Token: 0x06002576 RID: 9590 RVA: 0x0009D004 File Offset: 0x0009B404
		private void OnEntitiesFailed(IEnumerable<MMResultEntity> entities)
		{
			this.WriteMetrics(delegate(DateTime now)
			{
				this.m_metrics.Get<Meter>(MatchMakingTraker.ms_metric.ms_matchmaking_players_failed).Adjust(now, (long)(from e in entities
				select e.Players.Count).Sum());
			});
		}

		// Token: 0x06002577 RID: 9591 RVA: 0x0009D038 File Offset: 0x0009B438
		private void OnEntitiesSucceded(IEnumerable<MMResultEntity> entities, IGameRoom room)
		{
			string roomRegionId = null;
			try
			{
				room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					roomRegionId = room.RegionId;
				});
			}
			catch (RoomClosedException)
			{
				Log.Info<ulong>("Room {0} is closed, so tracking region misses is unavailable", room.ID);
			}
			this.WriteMetrics(delegate(DateTime now)
			{
				this.m_metrics.Get<Meter>(MatchMakingTraker.ms_metric.ms_matchmaking_players_succeded).Adjust(now, (long)(from e in entities
				select e.Players.Count).Sum());
				foreach (MMResultEntity mmresultEntity in entities)
				{
					this.m_metrics.Get<Timer>(MatchMakingTraker.ms_metric.ms_matchmaking_success_time).TimeMilliseconds(now, now.ToUniversalTime() - TimeUtils.UTCTimestampToUTCTime(mmresultEntity.StartTime));
				}
				if (!string.IsNullOrEmpty(roomRegionId))
				{
					IEnumerable<IGrouping<string, MMResultEntity>> enumerable = from e in entities
					where e.RegionId != roomRegionId
					group e by e.RegionId;
					foreach (IGrouping<string, MMResultEntity> grouping in enumerable)
					{
						this.m_metrics.Get<Meter>(MatchMakingTraker.ms_metric.ms_matchmaking_region_violation, new string[]
						{
							"region_id",
							grouping.Key
						}).Adjust(now, (long)grouping.Count<MMResultEntity>());
					}
				}
			});
		}

		// Token: 0x06002578 RID: 9592 RVA: 0x0009D0C4 File Offset: 0x0009B4C4
		private void OnUnQueueEntity(MMEntityInfo mmEntity, EUnQueueReason reason)
		{
			if (reason != EUnQueueReason.Cancelled)
			{
				return;
			}
			this.WriteMetrics(delegate(DateTime now)
			{
				this.m_metrics.Get<Meter>(MatchMakingTraker.ms_metric.ms_matchmaking_players_canceled).Inc(now);
				this.m_metrics.Get<Timer>(MatchMakingTraker.ms_metric.ms_matchmaking_cancel_time).TimeMilliseconds(now, now.ToUniversalTime() - mmEntity.Settings.StartTimeUtc);
			});
		}

		// Token: 0x040012F3 RID: 4851
		private readonly IMatchmakingPerformer m_matchmakingPerformer;

		// Token: 0x040012F4 RID: 4852
		private readonly IMatchmakingSystem m_matchmakingSystem;

		// Token: 0x040012F5 RID: 4853
		private EnumBasedMetricsContainer m_metrics;

		// Token: 0x020006FE RID: 1790
		private enum ms_metric
		{
			// Token: 0x040012F7 RID: 4855
			ms_matchmk_runs,
			// Token: 0x040012F8 RID: 4856
			ms_matchmk_players_served,
			// Token: 0x040012F9 RID: 4857
			ms_matchmk_players_succeded,
			// Token: 0x040012FA RID: 4858
			ms_matchmk_players_failed,
			// Token: 0x040012FB RID: 4859
			ms_matchmk_run_time,
			// Token: 0x040012FC RID: 4860
			ms_matchmaking_players_succeded,
			// Token: 0x040012FD RID: 4861
			ms_matchmaking_players_failed,
			// Token: 0x040012FE RID: 4862
			ms_matchmaking_players_canceled,
			// Token: 0x040012FF RID: 4863
			ms_matchmaking_success_time,
			// Token: 0x04001300 RID: 4864
			ms_matchmaking_cancel_time,
			// Token: 0x04001301 RID: 4865
			ms_matchmaking_region_violation
		}
	}
}
