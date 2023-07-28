using System;
using System.Collections.Generic;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SkillSystem;
using OLAPHypervisor;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000623 RID: 1571
	internal class SessionSummary
	{
		// Token: 0x060021BE RID: 8638 RVA: 0x0008AB74 File Offset: 0x00088F74
		public SessionSummary(string sessionId, ulong roomId, GameRoomType roomType, string roomName, MissionContext mission)
		{
			this.SessionId = sessionId;
			this.RoomId = roomId;
			this.RoomType = roomType;
			this.RoomName = roomName;
			this.Mission = mission;
			this.StartTime = DateTime.Now;
			this.EndTime = DateTime.MinValue;
		}

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x060021BF RID: 8639 RVA: 0x0008ABF9 File Offset: 0x00088FF9
		public TimeSpan Duration
		{
			get
			{
				return this.EndTime - this.StartTime;
			}
		}

		// Token: 0x060021C0 RID: 8640 RVA: 0x0008AC0C File Offset: 0x0008900C
		public SessionSummary.RoundData AddRound(int id)
		{
			SessionSummary.RoundData roundData = new SessionSummary.RoundData(id);
			this.Rounds.Add(roundData);
			return roundData;
		}

		// Token: 0x060021C1 RID: 8641 RVA: 0x0008AC30 File Offset: 0x00089030
		public SessionSummary.PlayerData GetPlayer(ulong profileId)
		{
			SessionSummary.PlayerData playerData;
			if (!this.Players.TryGetValue(profileId, out playerData))
			{
				playerData = new SessionSummary.PlayerData(profileId);
				this.Players.Add(profileId, playerData);
			}
			return playerData;
		}

		// Token: 0x060021C2 RID: 8642 RVA: 0x0008AC68 File Offset: 0x00089068
		public void AddPlayerStat(ulong profileId, string stat, string val)
		{
			SessionSummary.PlayerData player = this.GetPlayer(profileId);
			player.Stats[stat] = val;
		}

		// Token: 0x060021C3 RID: 8643 RVA: 0x0008AC8C File Offset: 0x0008908C
		public SessionSummary.ObserverData GetObserver(ulong profileId)
		{
			SessionSummary.ObserverData observerData;
			if (!this.Observers.TryGetValue(profileId, out observerData))
			{
				observerData = new SessionSummary.ObserverData(profileId);
				this.Observers.Add(profileId, observerData);
			}
			return observerData;
		}

		// Token: 0x060021C4 RID: 8644 RVA: 0x0008ACC4 File Offset: 0x000890C4
		public void AddObserverStat(ulong profileId, string stat, string val)
		{
			SessionSummary.ObserverData observer = this.GetObserver(profileId);
			observer.Stats[stat] = val;
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x0008ACE6 File Offset: 0x000890E6
		public void AddMeasure(Measure msr)
		{
			this.MeasuresList.Add(msr);
		}

		// Token: 0x060021C6 RID: 8646 RVA: 0x0008ACF4 File Offset: 0x000890F4
		public void AddMeasure(IEnumerable<Measure> msrs)
		{
			this.MeasuresList.AddRange(msrs);
		}

		// Token: 0x04001063 RID: 4195
		public const int SUMMARY_VERSION = 2;

		// Token: 0x04001064 RID: 4196
		public readonly string SessionId;

		// Token: 0x04001065 RID: 4197
		public string Host;

		// Token: 0x04001066 RID: 4198
		public string MasterServer;

		// Token: 0x04001067 RID: 4199
		public string SessionLog;

		// Token: 0x04001068 RID: 4200
		public readonly ulong RoomId;

		// Token: 0x04001069 RID: 4201
		public readonly GameRoomType RoomType;

		// Token: 0x0400106A RID: 4202
		public readonly string RoomName;

		// Token: 0x0400106B RID: 4203
		public readonly MissionContext Mission;

		// Token: 0x0400106C RID: 4204
		public DateTime StartTime;

		// Token: 0x0400106D RID: 4205
		public DateTime EndTime;

		// Token: 0x0400106E RID: 4206
		public int SessionTime;

		// Token: 0x0400106F RID: 4207
		public string Mode;

		// Token: 0x04001070 RID: 4208
		public string SubMode;

		// Token: 0x04001071 RID: 4209
		public bool ClanWar;

		// Token: 0x04001072 RID: 4210
		public string EndOutcome;

		// Token: 0x04001073 RID: 4211
		public string EndReason;

		// Token: 0x04001074 RID: 4212
		public string EndWinningTeam;

		// Token: 0x04001075 RID: 4213
		public bool IsAutobalanced;

		// Token: 0x04001076 RID: 4214
		public readonly Dictionary<ulong, SessionSummary.PlayerData> Players = new Dictionary<ulong, SessionSummary.PlayerData>();

		// Token: 0x04001077 RID: 4215
		public readonly Dictionary<ulong, SessionSummary.ObserverData> Observers = new Dictionary<ulong, SessionSummary.ObserverData>();

		// Token: 0x04001078 RID: 4216
		public readonly List<SessionSummary.RoundData> Rounds = new List<SessionSummary.RoundData>();

		// Token: 0x04001079 RID: 4217
		public readonly Dictionary<string, string> Leaderboard = new Dictionary<string, string>();

		// Token: 0x0400107A RID: 4218
		public readonly List<Measure> MeasuresList = new List<Measure>();

		// Token: 0x0400107B RID: 4219
		public bool TelemetryContributed;

		// Token: 0x0400107C RID: 4220
		public bool RewardsContributed;

		// Token: 0x02000624 RID: 1572
		public class PlayerPlaytime
		{
			// Token: 0x060021C7 RID: 8647 RVA: 0x0008AD02 File Offset: 0x00089102
			internal PlayerPlaytime(int team, string klass, int playtime)
			{
				this.TeamId = team;
				this.Class = klass;
				this.Playtime = playtime;
			}

			// Token: 0x0400107D RID: 4221
			public int TeamId;

			// Token: 0x0400107E RID: 4222
			public string Class;

			// Token: 0x0400107F RID: 4223
			public int Playtime;
		}

		// Token: 0x02000625 RID: 1573
		public class PlayerReward
		{
			// Token: 0x060021C8 RID: 8648 RVA: 0x0008AD20 File Offset: 0x00089120
			public PlayerReward(string name, string value, params string[] attrs)
			{
				this.Name = name;
				this.Value = value;
				for (int i = 0; i < attrs.Length; i += 2)
				{
					this.Attrs[attrs[i]] = attrs[i + 1];
				}
			}

			// Token: 0x04001080 RID: 4224
			public readonly string Name;

			// Token: 0x04001081 RID: 4225
			public readonly Dictionary<string, string> Attrs = new Dictionary<string, string>();

			// Token: 0x04001082 RID: 4226
			public readonly string Value;
		}

		// Token: 0x02000626 RID: 1574
		public class PlayerData
		{
			// Token: 0x060021C9 RID: 8649 RVA: 0x0008AD74 File Offset: 0x00089174
			internal PlayerData(ulong profileId)
			{
				this.ProfileId = profileId;
				this.OverallResult = SessionOutcome.DNF;
			}

			// Token: 0x060021CA RID: 8650 RVA: 0x0008ADC4 File Offset: 0x000891C4
			public void AddPlaytime(int team, string klass, int playtime)
			{
				foreach (SessionSummary.PlayerPlaytime playerPlaytime in this.Playtimes)
				{
					if (playerPlaytime.TeamId == team && playerPlaytime.Class == klass)
					{
						playerPlaytime.Playtime += playtime;
						return;
					}
				}
				this.Playtimes.Add(new SessionSummary.PlayerPlaytime(team, klass, playtime));
			}

			// Token: 0x060021CB RID: 8651 RVA: 0x0008AE5C File Offset: 0x0008925C
			public void AddReward(string name, string value, params string[] attrs)
			{
				this.Rewards.Add(new SessionSummary.PlayerReward(name, value, attrs));
			}

			// Token: 0x04001083 RID: 4227
			public readonly ulong ProfileId;

			// Token: 0x04001084 RID: 4228
			public string Nickname;

			// Token: 0x04001085 RID: 4229
			public int Rank;

			// Token: 0x04001086 RID: 4230
			public SessionOutcome OverallResult;

			// Token: 0x04001087 RID: 4231
			public bool FirstWin;

			// Token: 0x04001088 RID: 4232
			public Skill Skill = Skill.Empty;

			// Token: 0x04001089 RID: 4233
			public readonly List<SessionSummary.PlayerPlaytime> Playtimes = new List<SessionSummary.PlayerPlaytime>();

			// Token: 0x0400108A RID: 4234
			public readonly Dictionary<string, string> Stats = new Dictionary<string, string>();

			// Token: 0x0400108B RID: 4235
			public readonly List<SessionSummary.PlayerReward> Rewards = new List<SessionSummary.PlayerReward>();
		}

		// Token: 0x02000627 RID: 1575
		public class ObserverData
		{
			// Token: 0x060021CC RID: 8652 RVA: 0x0008AE71 File Offset: 0x00089271
			internal ObserverData(ulong profileId)
			{
				this.ProfileId = profileId;
				this.Playtime = 0;
				this.Rank = 0;
			}

			// Token: 0x060021CD RID: 8653 RVA: 0x0008AE99 File Offset: 0x00089299
			public void AddPlaytime(int playtime)
			{
				this.Playtime += playtime;
			}

			// Token: 0x0400108C RID: 4236
			public readonly ulong ProfileId;

			// Token: 0x0400108D RID: 4237
			public string Nickname;

			// Token: 0x0400108E RID: 4238
			public int Rank;

			// Token: 0x0400108F RID: 4239
			public int Playtime;

			// Token: 0x04001090 RID: 4240
			public readonly Dictionary<string, string> Stats = new Dictionary<string, string>();
		}

		// Token: 0x02000628 RID: 1576
		public class RoundData
		{
			// Token: 0x060021CE RID: 8654 RVA: 0x0008AEA9 File Offset: 0x000892A9
			public RoundData(int id)
			{
				this.Id = id;
			}

			// Token: 0x17000367 RID: 871
			// (get) Token: 0x060021CF RID: 8655 RVA: 0x0008AEB8 File Offset: 0x000892B8
			public int Duration
			{
				get
				{
					return (int)(this.EndTime - this.BeginTime).TotalSeconds;
				}
			}

			// Token: 0x04001091 RID: 4241
			public readonly int Id;

			// Token: 0x04001092 RID: 4242
			public DateTime BeginTime;

			// Token: 0x04001093 RID: 4243
			public DateTime EndTime;

			// Token: 0x04001094 RID: 4244
			public int WinningTeamId;
		}
	}
}
