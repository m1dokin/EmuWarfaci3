using System;
using System.Collections.Generic;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020007B5 RID: 1973
	public class RewardInputData
	{
		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x0600288F RID: 10383 RVA: 0x000AE606 File Offset: 0x000ACA06
		public bool IsClanWar
		{
			get
			{
				return this.roomType == GameRoomType.PvP_ClanWar;
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06002890 RID: 10384 RVA: 0x000AE611 File Offset: 0x000ACA11
		public bool IsPvE
		{
			get
			{
				return GameRoomType.PvE.HasFlag(this.roomType);
			}
		}

		// Token: 0x04001546 RID: 5446
		public string missionId;

		// Token: 0x04001547 RID: 5447
		public byte difficulty;

		// Token: 0x04001548 RID: 5448
		public GameRoomType roomType;

		// Token: 0x04001549 RID: 5449
		public bool incompleteSession;

		// Token: 0x0400154A RID: 5450
		public float sessionTime;

		// Token: 0x0400154B RID: 5451
		public uint sessionKillCount;

		// Token: 0x0400154C RID: 5452
		public int winnerTeamId;

		// Token: 0x0400154D RID: 5453
		public int maxSessionScore;

		// Token: 0x0400154E RID: 5454
		public int maxRoundLimit;

		// Token: 0x0400154F RID: 5455
		public uint passedSubLevelsCount;

		// Token: 0x04001550 RID: 5456
		public uint passedCheckpointsCount;

		// Token: 0x04001551 RID: 5457
		public byte secondaryObjectivesCompleted;

		// Token: 0x04001552 RID: 5458
		public Dictionary<uint, uint> playersPerformances = new Dictionary<uint, uint>();

		// Token: 0x04001553 RID: 5459
		public Dictionary<byte, RewardInputData.Team> teams = new Dictionary<byte, RewardInputData.Team>();

		// Token: 0x04001554 RID: 5460
		public bool leaversPunished;

		// Token: 0x020007B6 RID: 1974
		public class Team
		{
			// Token: 0x04001555 RID: 5461
			public List<RewardInputData.Team.Player> playerScores = new List<RewardInputData.Team.Player>();

			// Token: 0x020007B7 RID: 1975
			public class Player
			{
				// Token: 0x04001556 RID: 5462
				public const int UndefinedPosition = -1;

				// Token: 0x04001557 RID: 5463
				public ulong profileId;

				// Token: 0x04001558 RID: 5464
				public byte teamId;

				// Token: 0x04001559 RID: 5465
				public int position = -1;

				// Token: 0x0400155A RID: 5466
				public int score;

				// Token: 0x0400155B RID: 5467
				public bool inSessionFromStart = true;

				// Token: 0x0400155C RID: 5468
				public TimeSpan sessionTime;

				// Token: 0x0400155D RID: 5469
				public float xp_boost;

				// Token: 0x0400155E RID: 5470
				public float vp_boost;

				// Token: 0x0400155F RID: 5471
				public float gm_boost;

				// Token: 0x04001560 RID: 5472
				public uint firstCheckpoint;

				// Token: 0x04001561 RID: 5473
				public uint lastCheckpoint;

				// Token: 0x04001562 RID: 5474
				public string groupId;

				// Token: 0x04001563 RID: 5475
				public bool isVip;

				// Token: 0x04001564 RID: 5476
				public SRewardMultiplier dynamicMultiplier;
			}
		}
	}
}
