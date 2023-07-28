using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using LibMetricsNet;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services.Metrics;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameRoomSystem;
using Util.Common;

namespace MasterServer.Telemetry.Metrics
{
	// Token: 0x02000721 RID: 1825
	[OrphanService]
	[Singleton]
	internal class RoomsMetricsTracker : BaseTracker
	{
		// Token: 0x060025EF RID: 9711 RVA: 0x0009F2A5 File Offset: 0x0009D6A5
		public RoomsMetricsTracker(IGameRoomManager gameRoomManager, IGameModesSystem gameModesSystem)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x060025F0 RID: 9712 RVA: 0x0009F2C8 File Offset: 0x0009D6C8
		public override void Init(IMetricsService service)
		{
			base.Init(service);
			IEnumerator enumerator = Enum.GetValues(typeof(GameRoomType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					GameRoomType gameRoomType = (GameRoomType)obj;
					this.m_metricsMap.Add(gameRoomType, new RoomsMetricsTracker.RoomsMetrics(gameRoomType, this.m_metricsService.MetricsConfig));
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
			this.m_gameRoomManager.SessionStarted += this.OnSessionStarted;
			this.m_gameRoomManager.RoomClosed += this.OnRoomClosed;
		}

		// Token: 0x060025F1 RID: 9713 RVA: 0x0009F384 File Offset: 0x0009D784
		public override IEnumerable<Metric> GetMetrics()
		{
			return this.m_metricsMap.Values.SelectMany((RoomsMetricsTracker.RoomsMetrics m) => m.Metrics.GetMetrics());
		}

		// Token: 0x060025F2 RID: 9714 RVA: 0x0009F3B3 File Offset: 0x0009D7B3
		public override void Dispose()
		{
			this.m_gameRoomManager.SessionStarted -= this.OnSessionStarted;
			this.m_gameRoomManager.RoomClosed -= this.OnRoomClosed;
		}

		// Token: 0x060025F3 RID: 9715 RVA: 0x0009F3E4 File Offset: 0x0009D7E4
		private void WriteMetrics(Action<DateTime> writer)
		{
			if (!this.m_metricsService.Enabled)
			{
				return;
			}
			using (this.m_metricsService.Synchronizer.BatchWrite())
			{
				DateTime now = DateTime.Now;
				writer(now);
			}
		}

		// Token: 0x060025F4 RID: 9716 RVA: 0x0009F444 File Offset: 0x0009D844
		private void OnRoomClosed(IGameRoom room)
		{
			this.WriteMetrics(delegate(DateTime now)
			{
				EnumBasedMetricsContainer metrics = this.m_metricsMap[room.Type].Metrics;
				metrics.Get<Meter>(RoomsMetricsTracker.rooms_metrics.ms_rooms_closed).Inc(now);
			});
		}

		// Token: 0x060025F5 RID: 9717 RVA: 0x0009F478 File Offset: 0x0009D878
		private void OnSessionStarted(IGameRoom room, string session_id)
		{
			this.WriteMetrics(delegate(DateTime now)
			{
				double num = 0.0;
				var source = (from p in room.Players
				group p by p.TeamID into g
				select new
				{
					TeamId = g.Key,
					PlayersSkills = (from p in g
					select p.Skill.Value).ToList<double>()
				}).ToList();
				double num2 = (double)(room.MaxPlayers - room.Players.Count<RoomPlayer>());
				double num3 = source.SelectMany(e => e.PlayersSkills).StdDev();
				if (!room.NoTeamsMode)
				{
					num = Math.Abs(source.First().PlayersSkills.Average() - source.Last().PlayersSkills.Average());
				}
				EnumBasedMetricsContainer metrics = this.m_metricsMap[room.Type].Metrics;
				metrics.Get<Gauge>(RoomsMetricsTracker.rooms_metrics.ms_rooms_occupancy_rate).Track(now, (long)(num2 * 1000.0));
				metrics.Get<Gauge>(RoomsMetricsTracker.rooms_metrics.ms_rooms_skill_deviation).Track(now, (long)(num3 * 1000.0));
				metrics.Get<Meter>(RoomsMetricsTracker.rooms_metrics.ms_rooms_session_started).Inc(now);
				if (num > 0.0)
				{
					metrics.Get<Gauge>(RoomsMetricsTracker.rooms_metrics.ms_rooms_teams_skill_difference).Track(now, (long)(num * 1000.0));
				}
				GameModeSetting gameModeSetting = this.m_gameModesSystem.GetGameModeSetting(room.MissionType.Name);
				string first;
				if (gameModeSetting != null && gameModeSetting.GetSetting(GameRoomType.PvE_AutoStart, ERoomSetting.CLASS_PATTERN, out first))
				{
					IEnumerable<ProfileProgressionInfo.PlayerClass> source2 = from p in room.Players
					select ProfileProgressionInfo.PlayerClassFromClassId(p.ClassID);
					RoomsMetricsTracker.rooms_metrics rooms_metrics = (!first.BagDifference(from c in source2
					select ProfileProgressionInfo.ClassToClassChar[c]).Any<char>()) ? RoomsMetricsTracker.rooms_metrics.ms_rooms_class_pattern_succeded : RoomsMetricsTracker.rooms_metrics.ms_rooms_class_pattern_violated;
					metrics.Get<Meter>(rooms_metrics).Inc(now);
				}
				string roomRegionId = room.RegionId;
				int num4 = room.Players.Count((RoomPlayer p) => p.RegionId != roomRegionId);
				metrics.Get<Gauge>(RoomsMetricsTracker.rooms_metrics.ms_rooms_region_id_violation_rate).Track(now, (long)((double)num4 / (double)room.Players.Count<RoomPlayer>() * 1000.0));
				IEnumerable<double> enumerable = from player in room.Players
				where player.QuickPlaySearchTime > TimeSpan.Zero
				select player.QuickPlaySearchTime.TotalSeconds;
				double num5 = (!enumerable.Any<double>()) ? 0.0 : enumerable.Max();
				metrics.Get<Gauge>(RoomsMetricsTracker.rooms_metrics.ms_room_quickplay_search_time).Track(now, (long)(enumerable.Median() * 1000.0));
				metrics.Get<Gauge>(RoomsMetricsTracker.rooms_metrics.ms_room_quickplay_search_time_peak).Track(now, (long)(num5 * 1000.0));
			});
		}

		// Token: 0x04001357 RID: 4951
		private const double ScaleRate = 1000.0;

		// Token: 0x04001358 RID: 4952
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04001359 RID: 4953
		private readonly IGameModesSystem m_gameModesSystem;

		// Token: 0x0400135A RID: 4954
		private readonly Dictionary<GameRoomType, RoomsMetricsTracker.RoomsMetrics> m_metricsMap = new Dictionary<GameRoomType, RoomsMetricsTracker.RoomsMetrics>();

		// Token: 0x02000722 RID: 1826
		private enum rooms_metrics
		{
			// Token: 0x0400135D RID: 4957
			ms_rooms_occupancy_rate,
			// Token: 0x0400135E RID: 4958
			ms_rooms_skill_deviation,
			// Token: 0x0400135F RID: 4959
			ms_rooms_teams_skill_difference,
			// Token: 0x04001360 RID: 4960
			ms_rooms_class_pattern_succeded,
			// Token: 0x04001361 RID: 4961
			ms_rooms_class_pattern_violated,
			// Token: 0x04001362 RID: 4962
			ms_rooms_region_id_violation_rate,
			// Token: 0x04001363 RID: 4963
			ms_rooms_session_started,
			// Token: 0x04001364 RID: 4964
			ms_rooms_closed,
			// Token: 0x04001365 RID: 4965
			ms_room_quickplay_search_time,
			// Token: 0x04001366 RID: 4966
			ms_room_quickplay_search_time_peak
		}

		// Token: 0x02000723 RID: 1827
		private class RoomsMetrics
		{
			// Token: 0x060025F7 RID: 9719 RVA: 0x0009F4B8 File Offset: 0x0009D8B8
			public RoomsMetrics(GameRoomType roomType, MetricsConfig metricsConfig)
			{
				this.Metrics = new EnumBasedMetricsContainer(typeof(RoomsMetricsTracker.rooms_metrics), metricsConfig);
				string[] tags = new string[]
				{
					"server",
					Resources.ServerName,
					"room_type",
					roomType.ToString().ToLower()
				};
				this.Metrics.InitGauge(RoomsMetricsTracker.rooms_metrics.ms_rooms_occupancy_rate, tags);
				this.Metrics.InitGauge(RoomsMetricsTracker.rooms_metrics.ms_rooms_skill_deviation, tags);
				this.Metrics.InitGauge(RoomsMetricsTracker.rooms_metrics.ms_rooms_teams_skill_difference, tags);
				this.Metrics.InitGauge(RoomsMetricsTracker.rooms_metrics.ms_rooms_region_id_violation_rate, tags);
				this.Metrics.InitGauge(RoomsMetricsTracker.rooms_metrics.ms_room_quickplay_search_time, tags);
				this.Metrics.InitGauge(RoomsMetricsTracker.rooms_metrics.ms_room_quickplay_search_time_peak, tags);
				this.Metrics.InitMeter(RoomsMetricsTracker.rooms_metrics.ms_rooms_class_pattern_succeded, tags);
				this.Metrics.InitMeter(RoomsMetricsTracker.rooms_metrics.ms_rooms_class_pattern_violated, tags);
				this.Metrics.InitMeter(RoomsMetricsTracker.rooms_metrics.ms_rooms_session_started, tags);
				this.Metrics.InitMeter(RoomsMetricsTracker.rooms_metrics.ms_rooms_closed, tags);
			}

			// Token: 0x1700039B RID: 923
			// (get) Token: 0x060025F8 RID: 9720 RVA: 0x0009F5CA File Offset: 0x0009D9CA
			// (set) Token: 0x060025F9 RID: 9721 RVA: 0x0009F5D2 File Offset: 0x0009D9D2
			public EnumBasedMetricsContainer Metrics { get; private set; }
		}
	}
}
