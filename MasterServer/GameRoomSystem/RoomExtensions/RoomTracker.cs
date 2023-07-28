using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.Telemetry;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000606 RID: 1542
	[RoomExtension]
	internal class RoomTracker : RoomExtensionBase
	{
		// Token: 0x060020F3 RID: 8435 RVA: 0x000874FC File Offset: 0x000858FC
		public RoomTracker(ITelemetryService telemetryService, IRankSystem rankSystem)
		{
			this.m_telemetryService = telemetryService;
			this.m_rankSystem = rankSystem;
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x00087514 File Offset: 0x00085914
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			this.m_createTime = DateTime.UtcNow.Ticks / 10000L;
			this.m_sessionEndTime = this.m_createTime;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.SessionStarted += this.RoomSessionStarted;
			extension.SessionEnded += this.RoomSessionEnded;
			base.Room.PlayerRemoved += this.OnPlayerRemoved;
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x00087598 File Offset: 0x00085998
		public override void Close()
		{
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.SessionStarted -= this.RoomSessionStarted;
			extension.SessionEnded -= this.RoomSessionEnded;
			base.Room.PlayerRemoved -= this.OnPlayerRemoved;
			base.Close();
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x000875F4 File Offset: 0x000859F4
		private void RoomSessionStarted(IGameRoom room, string sessionId)
		{
			DateTime utcNow = DateTime.UtcNow;
			string text = utcNow.ToString("yyyy-MM-dd");
			long num = utcNow.Ticks / 10000L;
			long num2 = Interlocked.Read(ref this.m_createTime);
			this.m_telemetryService.AddMeasure((num - num2) / 100L, new object[]
			{
				"stat",
				"room_setup_time",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname,
				"date",
				text
			});
			if (room.IsClanWarMode())
			{
				this.m_telemetryService.AddMeasure(1L, new object[]
				{
					"stat",
					"clan_war_started",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname,
					"date",
					text
				});
			}
			List<int> list = null;
			List<KeyValuePair<string, TimeSpan>> list2 = null;
			List<TimeSpan> list3 = null;
			list = (from x in room.Players
			select x.Rank).ToList<int>();
			list2 = new List<KeyValuePair<string, TimeSpan>>(list.Count);
			list3 = new List<TimeSpan>(list.Count);
			foreach (RoomPlayer roomPlayer in room.Players)
			{
				if (roomPlayer.SessionsPlayedInRoom == 0)
				{
					list2.Add((!(roomPlayer.QuickPlaySearchTime == TimeSpan.Zero)) ? new KeyValuePair<string, TimeSpan>(GameRoomPlayerAddReason.Matchmaking.ToString(), roomPlayer.QuickPlaySearchTime) : new KeyValuePair<string, TimeSpan>(GameRoomPlayerAddReason.RoomBrowser.ToString(), utcNow - roomPlayer.JoinTime));
				}
				else
				{
					list3.Add(utcNow - roomPlayer.LastSessionEndTime);
				}
			}
			int num3 = (from x in list
			select this.m_rankSystem.ClassifyRankGroup(x)).Distinct<Resources.ChannelRankGroup>().Count<Resources.ChannelRankGroup>();
			string humanReadablePlayerCount = this.GetHumanReadablePlayerCount(list.Count);
			string softChannelName = this.GetSoftChannelName();
			this.m_telemetryService.AddMeasure(1L, new object[]
			{
				"stat",
				"room_sessions_started",
				"channel",
				Resources.ChannelName,
				"bucket1",
				humanReadablePlayerCount,
				"bucket2",
				softChannelName,
				"bucket3",
				room.Type.ToString(),
				"date",
				text
			});
			if (num3 > 1)
			{
				this.m_telemetryService.AddMeasure(1L, new object[]
				{
					"stat",
					"room_sessions_started_mixed",
					"channel",
					Resources.ChannelName,
					"bucket1",
					humanReadablePlayerCount,
					"bucket2",
					softChannelName,
					"date",
					text
				});
			}
			foreach (KeyValuePair<string, TimeSpan> keyValuePair in list2)
			{
				this.m_telemetryService.AddMeasure(1L, new object[]
				{
					"stat",
					"room_join_to_start_time",
					"channel",
					Resources.ChannelName,
					"bucket1",
					this.GetTimeRange(keyValuePair.Value),
					"bucket2",
					softChannelName,
					"bucket3",
					keyValuePair.Key,
					"date",
					text
				});
			}
			foreach (TimeSpan time in list3)
			{
				this.m_telemetryService.AddMeasure(1L, new object[]
				{
					"stat",
					"room_end_to_start_time",
					"channel",
					Resources.ChannelName,
					"bucket1",
					this.GetTimeRange(time),
					"bucket2",
					softChannelName,
					"date",
					text
				});
			}
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x00087A78 File Offset: 0x00085E78
		private void RoomSessionEnded(IGameRoom room, string sessionId, bool abnormal)
		{
			if (abnormal)
			{
				return;
			}
			Interlocked.Exchange(ref this.m_sessionEndTime, DateTime.UtcNow.Ticks / 10000L);
			string text = DateTime.UtcNow.ToString("yyyy-MM-dd");
			if (room.IsClanWarMode())
			{
				string playersCount = string.Empty;
				room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					playersCount = r.PlayerCount.ToString();
				});
				this.m_telemetryService.AddMeasure(1L, new object[]
				{
					"stat",
					"clan_war_players",
					"server",
					Resources.ServerName,
					"host",
					Resources.Hostname,
					"date",
					text,
					"players",
					playersCount
				});
			}
			string softChannelName = this.GetSoftChannelName();
			bool autobalance = room.Autobalance;
			int playerCount = room.PlayerCount;
			this.m_telemetryService.AddMeasure((!autobalance) ? 0L : 1L, new object[]
			{
				"stat",
				"room_team_balance",
				"date",
				text
			});
			this.m_telemetryService.AddMeasure(1L, new object[]
			{
				"stat",
				"room_sessions_finished",
				"channel",
				Resources.ChannelName,
				"bucket1",
				this.GetHumanReadablePlayerCount(playerCount),
				"bucket2",
				softChannelName,
				"bucket3",
				room.Type.ToString(),
				"date",
				text
			});
		}

		// Token: 0x060020F8 RID: 8440 RVA: 0x00087C30 File Offset: 0x00086030
		private void OnPlayerRemoved(IGameRoom room, RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			string text = DateTime.UtcNow.ToString("yyyy-MM-dd");
			this.m_telemetryService.AddMeasure(1L, new object[]
			{
				"stat",
				"room_consequent_sessions_played",
				"channel",
				Resources.ChannelName,
				"bucket1",
				this.GetHumanReadableSessionCount(player.SessionsPlayedInRoom),
				"bucket2",
				this.GetSoftChannelName(),
				"date",
				text
			});
			if (reason == GameRoomPlayerRemoveReason.Left && player.SessionsPlayedInRoom == 0)
			{
				this.m_telemetryService.AddMeasure(1L, new object[]
				{
					"stat",
					"room_leave_before_start",
					"channel",
					Resources.ChannelName,
					"bucket2",
					this.GetSoftChannelName(),
					"bucket3",
					room.Type.ToString(),
					"date",
					text
				});
			}
		}

		// Token: 0x060020F9 RID: 8441 RVA: 0x00087D3C File Offset: 0x0008613C
		protected override void OnDisposing()
		{
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.SessionStarted -= this.RoomSessionStarted;
			extension.SessionEnded -= this.RoomSessionEnded;
			base.Room.PlayerRemoved -= this.OnPlayerRemoved;
			DateTime utcNow = DateTime.UtcNow;
			string text = utcNow.ToString("yyyy-MM-dd");
			long num = utcNow.Ticks / 10000L;
			long num2 = Interlocked.Read(ref this.m_createTime);
			this.m_telemetryService.AddMeasure((num - num2) / 100L, new object[]
			{
				"stat",
				"room_lifetime",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname,
				"date",
				text
			});
			base.OnDisposing();
		}

		// Token: 0x060020FA RID: 8442 RVA: 0x00087E1A File Offset: 0x0008621A
		private string GetHumanReadablePlayerCount(int numPlayers)
		{
			return string.Format("{0} player(s)", numPlayers);
		}

		// Token: 0x060020FB RID: 8443 RVA: 0x00087E2C File Offset: 0x0008622C
		private string GetHumanReadableSessionCount(int numSessions)
		{
			return string.Format("{0} session(s)", numSessions);
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x00087E40 File Offset: 0x00086240
		private string GetSoftChannelName()
		{
			Resources.ChannelRankGroup channelRankGroup = this.m_rankSystem.ChannelRankGroup;
			if (channelRankGroup == Resources.ChannelRankGroup.Newbie)
			{
				return "newbie";
			}
			if (channelRankGroup != Resources.ChannelRankGroup.Skilled)
			{
				return null;
			}
			return "skilled";
		}

		// Token: 0x060020FD RID: 8445 RVA: 0x00087E7C File Offset: 0x0008627C
		private string GetTimeRange(TimeSpan time)
		{
			int num = RoomTracker.RoomStartTimeRanges.IntervalIndex(time);
			if (num < 0)
			{
				return string.Format("{0} - INF", RoomTracker.RoomStartTimeRanges.Max.ToString());
			}
			KeyValuePair<TimeSpan, TimeSpan> keyValuePair = RoomTracker.RoomStartTimeRanges.Interval(num);
			return string.Format("{0} - {1}", keyValuePair.Key, keyValuePair.Value);
		}

		// Token: 0x04001013 RID: 4115
		private const string TIME_FMT = "yyyy-MM-dd";

		// Token: 0x04001014 RID: 4116
		private long m_sessionEndTime;

		// Token: 0x04001015 RID: 4117
		private long m_createTime;

		// Token: 0x04001016 RID: 4118
		private static readonly Range<TimeSpan> RoomStartTimeRanges = new Range<TimeSpan>(new TimeSpan[]
		{
			TimeSpan.Zero,
			TimeSpan.FromSeconds(15.0),
			TimeSpan.FromSeconds(30.0),
			TimeSpan.FromSeconds(45.0),
			TimeSpan.FromMinutes(1.0),
			TimeSpan.FromMinutes(1.5),
			TimeSpan.FromMinutes(2.0),
			TimeSpan.FromMinutes(3.0),
			TimeSpan.FromMinutes(5.0),
			TimeSpan.FromMinutes(10.0)
		});

		// Token: 0x04001017 RID: 4119
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x04001018 RID: 4120
		private readonly IRankSystem m_rankSystem;
	}
}
